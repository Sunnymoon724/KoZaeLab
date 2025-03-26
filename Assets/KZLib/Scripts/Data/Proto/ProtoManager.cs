using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Cysharp.Threading.Tasks;
using MessagePack;
using UnityEngine;
using KZLib.KZUtility;
using KZLib.KZData;

namespace KZLib
{
	public class ProtoMgr : Singleton<ProtoMgr>
	{
		private bool m_disposed = false;

		private const double c_frameTime = 1.0/30.0d; // 30 fps (0.0333s)
		private const int c_delayTime = 1; // 1ms

		private bool m_isLoaded = false;

		//? Type / Num / Proto
		private readonly Dictionary<Type,Dictionary<int,IProto>> m_protoDict = new();

		protected override void Release(bool disposing)
		{
			if(m_disposed)
			{
				return;
			}

			if(disposing)
			{
				ClearProto();
			}

			m_disposed = true;

			base.Release(disposing);
		}

		private void ClearProto()
		{
			m_protoDict.Clear();

			m_isLoaded = false;
		}

		public async UniTask<bool> TryLoadAsync(CancellationToken token)
		{
			if(m_isLoaded)
			{
				return true;
			}

			var start = DateTime.Now;

			if(!TryGetTextAsset(out var textAssetArray))
			{
				return false;
			}

			var accumulatedTime = 0.0d;
			var stopwatch = new Stopwatch();

			LogTag.System.I("Proto Load Start");

			for(var i=0;i<textAssetArray.Length;i++)
			{
				var textAsset = textAssetArray[i];

				if(textAsset == null)
				{
					LogTag.System.W($"TextAsset is null in {i}");

					continue;
				}

				stopwatch.Restart();

				if(!TryLoadProto(textAsset))
				{
					return false;
				}

				accumulatedTime += stopwatch.Elapsed.TotalSeconds;

				if(accumulatedTime >= c_frameTime)
				{
					accumulatedTime = 0.0d;

					await UniTask.Delay(c_delayTime,cancellationToken : token);
				}
			}

			stopwatch.Stop();

			LogTag.System.I($"Proto Load Complete [Count : {textAssetArray.Length} / Duration : {(DateTime.Now-start).TotalSeconds}]");

			m_isLoaded = true;

			return true;
		}

		public bool TryGetProto<TProto>(int num,out IProto proto) where TProto : class,IProto
		{
			return TryGetProto(num,typeof(TProto),out proto);
		}

		public bool TryGetProto(int num,Type protoType,out IProto proto)
		{
			proto = GetProto(num,protoType);

			if(proto == null)
			{
				LogTag.System.W($"Proto not found for {protoType.Name} with num {num}.");
			}

			return proto != null;
		}

		public TProto GetProto<TProto>(int num) where TProto : class,IProto
		{
			return GetProto(num,typeof(TProto)) as TProto;
		}

		public IProto GetProto(int num,Type protoType)
		{
			if(num <= Global.INVALID_NUM)
			{
				LogTag.System.E($"{num} is not valid. [type : {protoType}]");

				return null;
			}

			if(!m_protoDict.TryGetValue(protoType, out var protoDict))
			{
				LogTag.System.E($"{protoType.Name} is not exist.");

				return null;
			}

			if(!protoDict.TryGetValue(num, out var proto))
			{
				LogTag.System.E($"{protoType.Name} is not include {num}.");

				return null;
			}

			return proto;
		}

		public IEnumerable<TProto> GetProtoGroup<TProto>() where TProto : class,IProto
		{
			foreach(var proto in GetProtoGroup(typeof(TProto)))
			{
				yield return proto as TProto;
			}
		}

		public IEnumerable<IProto> GetProtoGroup(Type protoType)
		{
			if(m_protoDict.TryGetValue(protoType,out var protoDict))
			{
				foreach(var proto in protoDict.Values)
				{
					yield return proto;
				}
			}
		}

		public IEnumerable<Type> GetProtoTypeGroup()
		{
			foreach(var protoType in m_protoDict.Keys)
			{
				yield return protoType;
			}
		}

		public bool IsValidate<TProto>(int num) where TProto : class,IProto
		{
			return num > 0 && m_protoDict.TryGetValue(typeof(TProto),out var protoDict) && protoDict.ContainsKey(num);
		}

		public int GetCount<TProto>() where TProto : class,IProto
		{
			return m_protoDict.TryGetValue(typeof(TProto),out var protoDict) ? protoDict.Count : 0;
		}

		private bool TryLoadProto(TextAsset textAsset)
		{
			try
			{
				var protoName = textAsset.name;

				if(textAsset.bytes == null)
				{
					LogTag.System.E($"{protoName} is empty.");
					return false;
				}

				var protoTypeName = $"KZLib.KZData.{protoName}Proto";
				var protoType = CommonUtility.FindType(protoTypeName);

				if(protoType == null)
				{
					LogTag.System.E($"{protoTypeName} is not exist.");

					return false;
				}

				var deserialize = MessagePackSerializer.Deserialize(protoType.MakeArrayType(),textAsset.bytes);

				if(deserialize is not object[] resultArray)
				{
					LogTag.System.E($"{protoName} is not array.");

					return false;
				}

				var protoDict = new Dictionary<int,IProto>();

				foreach(var result in resultArray)
				{
					var proto = result as IProto;

					if(proto == null)
					{
						LogTag.System.E($"{proto} is not exist.");

						return false;
					}

					if(proto.Num == Global.INVALID_NUM)
					{
						LogTag.System.E($"Num is zero in {proto}.");

						return false;
					}

					if(protoDict.ContainsKey(proto.Num))
					{
						LogTag.System.E($"{proto.Num} is already added in {proto}.");

						return false;
					}

					protoDict.Add(proto.Num,proto);
				}

				m_protoDict.Add(protoType,protoDict);

				return true;
			}
			catch(Exception exception)
			{
				LogTag.System.E($"Load failed. [Exception : {exception.Message}]");
			}

			return false;
		}

#if UNITY_EDITOR
		public void Reload()
		{
			ClearProto();

			LoadInEditor();
		}

		public void LoadInEditor()
		{
			if(m_isLoaded)
			{
				return;
			}

			if(!TryGetTextAsset(out var textAssetArray))
			{
				return;
			}

			for(var i=0;i<textAssetArray.Length;i++)
			{
				var textAsset = textAssetArray[i];

				if(textAsset == null)
				{
					LogTag.System.W($"TextAsset is null in {i}");

					continue;
				}

				if(!TryLoadProto(textAsset))
				{
					return;
				}
			}

			m_isLoaded = true;
		}
#endif
		private bool TryGetTextAsset(out TextAsset[] textAssetArray)
		{
			var gameCfg = ConfigMgr.In.Access<ConfigData.GameConfig>();

			textAssetArray = ResMgr.In.GetTextAssetArray(gameCfg.ProtoFolderPath);

			if(textAssetArray.IsNullOrEmpty())
			{
				LogTag.System.E("Load failed, textAsset is null.");

				return false;
			}

			return true;
		}
	}
}