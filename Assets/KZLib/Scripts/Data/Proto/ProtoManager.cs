using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using KZLib.Utilities;
using MemoryPack;

namespace KZLib.Data
{
	public class ProtoManager : Singleton<ProtoManager>
	{
		private const double c_frameTime = 1.0/30.0d; // 30 fps (0.0333s)
		private const int c_delayTime = 1; // 1ms
		private const int c_invalidNumber = 0;

		private bool m_isLoaded = false;

		//? Type / Num / Proto
		private readonly Dictionary<Type,Dictionary<int,IProto>> m_protoDict = new();

		private readonly Dictionary<int,Vector4[]> m_colorVectorDict = new();

		protected override void _Release(bool disposing)
		{
			if(disposing)
			{
				_ClearProto();
			}

			base._Release(disposing);
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

			var start = GameTimeManager.In.GetCurrentTime(true);

			if(!_TryGetTextAsset(out var textAssetArray))
			{
				return false;
			}

			var accumulatedTime = 0.0d;
			var stopwatch = new Stopwatch();

			LogChannel.System.I("Proto Load Start");

			for(var i=0;i<textAssetArray.Length;i++)
			{
				var textAsset = textAssetArray[i];

				if(textAsset == null)
				{
					LogChannel.System.W($"TextAsset is null in {i}");

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

					await UniTask.Delay(c_delayTime,cancellationToken : token).SuppressCancellationThrow();
				}
			}

			stopwatch.Stop();

			LogChannel.System.I($"Proto Load Complete [Count : {textAssetArray.Length} / Duration : {(GameTimeManager.In.GetCurrentTime(true)-start).TotalSeconds}]");

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
				LogChannel.System.W($"Proto not found for {protoType.Name} with num {num}.");
			}

			return proto != null;
		}

		public TProto GetProto<TProto>(int num) where TProto : class,IProto
		{
			return GetProto(num,typeof(TProto)) as TProto;
		}

		public IProto GetProto(int num,Type protoType)
		{
			if(num <= c_invalidNumber)
			{
				LogChannel.System.E($"{num} is not valid. [type : {protoType}]");

				return null;
			}

			if(!m_protoDict.TryGetValue(protoType, out var protoDict))
			{
				LogChannel.System.E($"{protoType.Name} is not exist.");

				return null;
			}

			if(!protoDict.TryGetValue(num, out var proto))
			{
				LogChannel.System.E($"{protoType.Name} is not include {num}.");

				return null;
			}

			return proto;
		}

		public IEnumerable<TProto> FindProtoGroup<TProto>() where TProto : class,IProto
		{
			foreach(var proto in FindProtoGroup(typeof(TProto)))
			{
				yield return proto as TProto;
			}
		}

		public IEnumerable<IProto> FindProtoGroup(Type protoType)
		{
			if(m_protoDict.TryGetValue(protoType,out var protoDict))
			{
				foreach(var pair in protoDict)
				{
					yield return pair.Value;
				}
			}
		}

		public IEnumerable<Type> FindProtoTypeGroup()
		{
			foreach(var pair in m_protoDict)
			{
				yield return pair.Key;
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

				var protoTypeName = $"KZLib.Data.{protoName}Proto";
				var protoType = CommonUtility.FindType(protoTypeName) ?? throw new InvalidOperationException($"{protoTypeName} is not exist.");
				var deserialize = MemoryPackSerializer.Deserialize(protoType.MakeArrayType(),textAsset.bytes);

				if(deserialize is not object[] resultArray)
				{
					throw new InvalidOperationException($"{protoName} is not array.");
				}

				var protoDict = new Dictionary<int,IProto>();

				for(var i=0;i<resultArray.Length;i++)
				{
					var proto = resultArray[i] as IProto ?? throw new InvalidOperationException($"{protoTypeName} is not exist.");

					if(proto.Num == c_invalidNumber)
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
				LogChannel.System.E($"Load failed. [Exception : {exception.Message}]");

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
					LogChannel.System.W($"TextAsset is null in {i}");

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
				LogChannel.System.E("Load failed, textAsset is null.");

				return false;
			}

			return true;
		}

		public Vector4[] GetColorVectorArray(int num)
		{
			if(!m_colorVectorDict.TryGetValue(num,out Vector4[] colorVectorArray))
			{
				var colorPrt = GetProto<IColorProto>(num);

				if(colorPrt == null)
				{
					return Array.Empty<Vector4>(); 
				}

				var hexCodeArray = colorPrt.ColorArray;
				
				if(hexCodeArray.Length < 2)
				{
					return Array.Empty<Vector4>(); 
				}

				colorVectorArray = new Vector4[hexCodeArray.Length];

				colorVectorArray[0] = hexCodeArray[0].IsEmpty() ? Color.clear : hexCodeArray[0].ToColor();
				colorVectorArray[1] = hexCodeArray[1].IsEmpty() ? Color.clear : hexCodeArray[1].ToColor();

				for(var i=2;i<hexCodeArray.Length;i++)
				{
					colorVectorArray[i] = hexCodeArray[i].IsEmpty() ? colorVectorArray[1] : hexCodeArray[i].ToColor();
				}

				m_colorVectorDict.Add(num,colorVectorArray);
			}

			return colorVectorArray;
		}
	}
}