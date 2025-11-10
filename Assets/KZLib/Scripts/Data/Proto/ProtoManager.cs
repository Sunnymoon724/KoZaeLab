using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Cysharp.Threading.Tasks;
using MessagePack;
using UnityEngine;
using KZLib.KZUtility;

namespace KZLib.KZData
{
	public class ProtoManager : Singleton<ProtoManager>
	{
		private const double c_frameTime = 1.0/30.0d; // 30 fps (0.0333s)
		private const int c_delayTime = 1; // 1ms
		private const int c_InvalidNumber = 0;

		private bool m_disposed = false;
		private bool m_isLoaded = false;

		//? Type / Num / Proto
		private readonly Dictionary<Type,Dictionary<int,IProto>> m_protoDict = new();

		private readonly Dictionary<int,Vector4[]> m_colorVectorDict = new();

		protected override void Release(bool disposing)
		{
			if(m_disposed)
			{
				return;
			}

			if(disposing)
			{
				_ClearProto();
			}

			m_disposed = true;

			base.Release(disposing);
		}

		private void _ClearProto()
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

			m_protoDict.Clear();

			var start = DateTime.Now;

			if(!_TryGetTextAsset(out var textAssetArray))
			{
				return false;
			}

			var accumulatedTime = 0.0d;
			var stopwatch = new Stopwatch();

			LogSvc.System.I("Proto Load Start");

			for(var i=0;i<textAssetArray.Length;i++)
			{
				var textAsset = textAssetArray[i];

				if(textAsset == null)
				{
					LogSvc.System.W($"TextAsset is null in {i}");

					continue;
				}

				stopwatch.Restart();

				if(!_TryLoadProto(textAsset))
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

			LogSvc.System.I($"Proto Load Complete [Count : {textAssetArray.Length} / Duration : {(DateTime.Now-start).TotalSeconds}]");

			m_isLoaded = true;

			return true;
		}

		public bool TryGetProto<TProto>(int num,out TProto proto) where TProto : class,IProto
		{
			if(TryGetProto(num,typeof(TProto),out var result))
			{
				proto = result as TProto;

				return true;
			}

			proto = default;

			return false;
		}

		public bool TryGetProto(int num,Type protoType,out IProto proto)
		{
			proto = GetProto(num,protoType);

			if(proto == null)
			{
				LogSvc.System.W($"Proto not found for {protoType.Name} with num {num}.");
			}

			return proto != null;
		}

		public TProto GetProto<TProto>(int num) where TProto : class,IProto
		{
			return GetProto(num,typeof(TProto)) as TProto;
		}

		public IProto GetProto(int num,Type protoType)
		{
			if(num <= c_InvalidNumber)
			{
				LogSvc.System.E($"{num} is not valid. [type : {protoType}]");

				return null;
			}

			if(!m_protoDict.TryGetValue(protoType, out var protoDict))
			{
				LogSvc.System.E($"{protoType.Name} is not exist.");

				return null;
			}

			if(!protoDict.TryGetValue(num, out var proto))
			{
				LogSvc.System.E($"{protoType.Name} is not include {num}.");

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

		public bool IsValid<TProto>(int num) where TProto : class,IProto
		{
			return num > 0 && m_protoDict.TryGetValue(typeof(TProto),out var protoDict) && protoDict.ContainsKey(num);
		}

		public int GetCount<TProto>() where TProto : class,IProto
		{
			return m_protoDict.TryGetValue(typeof(TProto),out var protoDict) ? protoDict.Count : 0;
		}

		private bool _TryLoadProto(TextAsset textAsset)
		{
			try
			{
				var protoName = textAsset.name;

				if(textAsset.bytes == null)
				{
					throw new InvalidCastException($"{protoName} is empty.");
				}

				var protoTypeName = $"KZLib.KZData.{protoName}Proto";
				var protoType = CommonUtility.FindType(protoTypeName) ?? throw new InvalidOperationException($"{protoTypeName} is not exist.");
				var deserialize = MessagePackSerializer.Deserialize(protoType.MakeArrayType(),textAsset.bytes,MessagePackSerializerOptions.Standard.WithResolver(MessagePackResolver.Instance));

				if(deserialize is not object[] resultArray)
				{
					throw new InvalidOperationException($"{protoName} is not array.");
				}

				var protoDict = new Dictionary<int,IProto>();

				foreach(var result in resultArray)
				{
					var proto = result as IProto ?? throw new InvalidOperationException($"{protoTypeName} is not exist.");

					if(proto.Num == c_InvalidNumber)
					{
						throw new ArgumentException($"Num is zero in {proto}.");
					}

					if(protoDict.ContainsKey(proto.Num))
					{
						throw new ArgumentException($"{proto.Num} is already added in {proto}.");
					}

					protoDict.Add(proto.Num,proto);
				}

				m_protoDict.Add(protoType,protoDict);

				return true;
			}
			catch(Exception exception)
			{
				LogSvc.System.E($"Load failed. [Exception : {exception.Message}]");

				return false;
			}
		}

#if UNITY_EDITOR
		public void Reload()
		{
			_ClearProto();

			LoadInEditor();
		}

		public void LoadInEditor()
		{
			if(m_isLoaded)
			{
				return;
			}

			m_protoDict.Clear();

			if(!_TryGetTextAsset(out var textAssetArray))
			{
				return;
			}

			for(var i=0;i<textAssetArray.Length;i++)
			{
				var textAsset = textAssetArray[i];

				if(textAsset == null)
				{
					LogSvc.System.W($"TextAsset is null in {i}");

					continue;
				}

				if(!_TryLoadProto(textAsset))
				{
					return;
				}
			}

			m_isLoaded = true;
		}
#endif
		private bool _TryGetTextAsset(out TextAsset[] textAssetArray)
		{
			var gameCfg = ConfigManager.In.Access<GameConfig>();

			textAssetArray = ResourceManager.In.GetTextAssetArray(gameCfg.ProtoFolderPath);

			if(textAssetArray.IsNullOrEmpty())
			{
				LogSvc.System.E("Load failed, textAsset is null.");

				return false;
			}

			return true;
		}

		public Vector4[] GetColorVectorArray(int num)
		{
			if(!m_colorVectorDict.TryGetValue(num,out Vector4[] colorVectorArray))
			{
				var colorPrt = GetProto<ColorProto>(num);

				if(colorPrt == null)
				{
					return null;
				}

				var hexCodeArray = colorPrt.ColorArray;

				colorVectorArray = new Vector4[hexCodeArray.Length];

				colorVectorArray[0] = hexCodeArray[0].IsEmpty() ? Color.clear : hexCodeArray[0].ToColor();

				for(var i=1;i<hexCodeArray.Length;i++)
				{
					colorVectorArray[i] = hexCodeArray[i].IsEmpty() ? colorVectorArray[0] : hexCodeArray[i].ToColor();
				}

				m_colorVectorDict.Add(num,colorVectorArray);
			}

			return colorVectorArray;
		}
	}
}