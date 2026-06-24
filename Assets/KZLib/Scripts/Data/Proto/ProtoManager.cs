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
	/// <summary>
	/// Loads MemoryPack proto tables from TextAssets and exposes row lookup by concrete proto class + Num.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Registry layout: <c>Dictionary&lt;concrete proto class, Dictionary&lt;Num, IProto&gt;&gt;</c>.
	/// Query with concrete classes (e.g. <c>BuffProto</c>), not marker interfaces (e.g. <c>IBuffProto</c>).
	/// </para>
	/// <para>
	/// Num is globally unique across all proto tables and must be a positive integer.
	/// </para>
	/// <para>
	/// <see cref="Get(int, Type)"/> throws on programmer error (not loaded, invalid Num, interface type, miss).
	/// <see cref="TryGet(int, Type, out IProto)"/> returns <c>false</c> for the same conditions without throwing.
	/// </para>
	/// <para>
	/// Distinct from <see cref="ClusterManager"/> (proto join + lazy cluster cache) and
	/// <see cref="ConfigManager"/> (YAML environment settings).
	/// </para>
	/// <para>
	/// TextAsset file name maps to type <c>KZLib.Data.{name}Proto</c> (e.g. <c>Buff</c> → <c>BuffProto</c>).
	/// </para>
	/// </remarks>
	public class ProtoManager : Singleton<ProtoManager>
	{
		/// <summary>Accumulated load work budget before yielding one frame (~30 fps).</summary>
		private const double c_frameTime = 1.0/30.0d;

		/// <summary>Delay after a frame budget is exhausted during async load.</summary>
		private const int c_delayTime = 1;

		/// <summary>Num values less than or equal to this are rejected.</summary>
		private const int c_invalidNumber = 0;

		private bool m_isLoaded = false;

		/// <summary>Loaded proto tables keyed by concrete proto class, then Num.</summary>
		private readonly Dictionary<Type,Dictionary<int,IProto>> m_protoDict = new();

		/// <summary>Cached palette <see cref="Vector4"/> arrays keyed by color proto Num.</summary>
		private readonly Dictionary<int,Vector4[]> m_colorVectorDict = new();

		/// <summary>True after a successful load via <see cref="TryLoadAsync"/> or editor <see cref="LoadInEditor"/>.</summary>
		public bool IsLoaded => m_isLoaded;

		protected override void _Release(bool disposing)
		{
			if(disposing)
			{
				_Clear();
			}

			base._Release(disposing);
		}

		/// <summary>Clears loaded tables, color cache, and the loaded flag.</summary>
		private void _Clear()
		{
			m_protoDict.Clear();
			m_colorVectorDict.Clear();

			m_isLoaded = false;
		}

		/// <summary>
		/// Loads all proto TextAssets on first call. Returns immediately when already loaded.
		/// Null TextAssets are skipped with a warning. Load error or cancellation clears the registry and returns <c>false</c>.
		/// </summary>
		public async UniTask<bool> TryLoadAsync(CancellationToken token)
		{
			if(m_isLoaded)
			{
				return true;
			}

			_Clear();

			if(!_TryGetTextAsset(out var textAssetArray))
			{
				return false;
			}

			var accumulatedTime = 0.0d;
			var stopwatch = new Stopwatch();
			var totalStopwatch = Stopwatch.StartNew();

			LogChannel.Data.I("Proto Load Start");

			try
			{
				for(var i=0;i<textAssetArray.Length;i++)
				{
					token.ThrowIfCancellationRequested();

					var textAsset = textAssetArray[i];

					if(textAsset == null)
					{
						LogChannel.Data.W($"TextAsset is null in {i}");

						continue;
					}

					stopwatch.Restart();

					if(!_TryLoad(textAsset))
					{
						_Clear();

						return false;
					}

					accumulatedTime += stopwatch.Elapsed.TotalSeconds;

					if(accumulatedTime >= c_frameTime)
					{
						accumulatedTime = 0.0d;

						if(await UniTask.Delay(c_delayTime,cancellationToken : token).SuppressCancellationThrow())
						{
							_Clear();

							return false;
						}
					}
				}
			}
			catch(OperationCanceledException)
			{
				_Clear();

				return false;
			}

			totalStopwatch.Stop();

			m_isLoaded = true;

			LogChannel.Data.I($"Proto Load Complete [Count : {textAssetArray.Length} / Duration : {totalStopwatch.Elapsed.TotalSeconds}]");

			return true;
		}

		/// <summary>
		/// Returns a proto row cast to <typeparamref name="TProto"/>.
		/// Returns <c>false</c> when not loaded, Num is invalid, type is null/interface, or row is missing.
		/// </summary>
		public bool TryGet<TProto>(int num,out TProto prt) where TProto : class,IProto
		{
			if(TryGet(num,typeof(TProto),out var result) && result is TProto newPrt)
			{
				prt = newPrt;

				return true;
			}

			prt = default;

			return false;
		}

		/// <summary>
		/// Returns a proto row cast to <typeparamref name="TProto"/>.
		/// Throws when not loaded, Num is invalid, <typeparamref name="TProto"/> is an interface, row is missing, or cast fails.
		/// </summary>
		public TProto Get<TProto>(int num) where TProto : class,IProto
		{
			var prt = Get(num,typeof(TProto));

			if(prt is TProto newPrt)
			{
				return newPrt;
			}

			throw new InvalidOperationException($"Proto [{typeof(TProto).Name}] with num {num} was not found.");
		}

		/// <summary>
		/// Returns a proto row for the given concrete proto class and Num.
		/// Throws when not loaded, Num is invalid, <paramref name="prtType"/> is an interface, or row is missing.
		/// </summary>
		public IProto Get(int num,Type prtType)
		{
			_ThrowIfNotLoaded();
			_ThrowIfInvalidNum(num);
			_ThrowIfInterfaceType(prtType);

			if(TryGet(num,prtType,out var prt))
			{
				return prt;
			}

			throw new InvalidOperationException($"Proto [{prtType.Name}] with num {num} was not found.");
		}

		/// <inheritdoc cref="TryGet(int, Type, out IProto)"/>
		public bool TryGet<TProto>(int num,out IProto prt)
		{
			return TryGet(num,typeof(TProto),out prt);
		}

		/// <summary>
		/// Returns a proto row without throwing.
		/// Returns <c>false</c> when not loaded, Num is invalid, <paramref name="prtType"/> is null or an interface,
		/// the proto class is not registered, or Num is missing.
		/// </summary>
		public bool TryGet(int num,Type prtType,out IProto prt)
		{
			prt = null;

			if(!m_isLoaded || num <= c_invalidNumber || prtType == null || prtType.IsInterface)
			{
				return false;
			}

			if(m_protoDict.TryGetValue(prtType,out var prtDict))
			{
				return prtDict.TryGetValue(num,out prt);
			}

			return false;
		}

		/// <summary>Yields all rows for a concrete proto class, cast to <typeparamref name="TProto"/>.</summary>
		public IEnumerable<TProto> FindGroup<TProto>() where TProto : class,IProto
		{
			foreach(var prt in FindGroup(typeof(TProto)))
			{
				if(prt is TProto newPrt)
				{
					yield return newPrt;
				}
			}
		}

		/// <summary>
		/// Yields all rows for a concrete proto class.
		/// Throws when not loaded or <paramref name="prtType"/> is an interface.
		/// Yields nothing when the proto class is not registered.
		/// </summary>
		public IEnumerable<IProto> FindGroup(Type prtType)
		{
			_ThrowIfNotLoaded();
			_ThrowIfInterfaceType(prtType);

			if(m_protoDict.TryGetValue(prtType,out var prtDict))
			{
				foreach(var pair in prtDict)
				{
					yield return pair.Value;
				}
			}
		}

		/// <summary>Yields every concrete proto class currently loaded in the registry.</summary>
		public IEnumerable<Type> FindTypeGroup()
		{
			foreach(var pair in m_protoDict)
			{
				yield return pair.Key;
			}
		}

		/// <summary>Yields all rows from the <c>ColorProto</c> table as <see cref="IColorProto"/>.</summary>
		public IEnumerable<IColorProto> FindColorGroup()
		{
			foreach(var prt in FindGroup(typeof(ColorProto)))
			{
				if(prt is IColorProto colorPrt)
				{
					yield return colorPrt;
				}
			}
		}

		/// <summary>Returns whether a row exists for the given concrete proto class and Num. Never throws.</summary>
		public bool IsValid<TProto>(int num) where TProto : class,IProto
		{
			return TryGet(num,typeof(TProto),out _);
		}

		/// <summary>
		/// Returns the row count for a concrete proto class.
		/// Throws when not loaded or <typeparamref name="TProto"/> is an interface.
		/// Returns <c>0</c> when the proto class is not registered.
		/// </summary>
		public int GetCount<TProto>() where TProto : class,IProto
		{
			_ThrowIfNotLoaded();
			_ThrowIfInterfaceType(typeof(TProto));

			return m_protoDict.TryGetValue(typeof(TProto),out var prtDict) ? prtDict.Count : 0;
		}

#if UNITY_EDITOR
		/// <summary>Clears the registry and synchronously reloads all proto TextAssets in the editor.</summary>
		public void Reload()
		{
			_Clear();

			LoadInEditor();
		}

		/// <summary>
		/// Synchronous load for editor tooling (<c>ProtoWindow</c>, <c>PaletteChanger</c>).
		/// No-op when already loaded — call <see cref="Reload"/> after proto assets change in the editor.
		/// </summary>
		public void LoadInEditor()
		{
			if(m_isLoaded)
			{
				return;
			}

			if(!_TryGetTextAsset(out var textAssetArray))
			{
				return;
			}

			if(!_LoadAll(textAssetArray))
			{
				_Clear();

				return;
			}

			m_isLoaded = true;
		}
#endif

		/// <summary>
		/// Returns cached palette colors for a color proto Num.
		/// Throws when not loaded, row is missing, or color data is invalid.
		/// </summary>
		public Vector4[] GetColorVectorArray(int num)
		{
			if(m_colorVectorDict.TryGetValue(num,out var colorVectorArray))
			{
				return colorVectorArray;
			}

			return GetColorVectorArray(Get<ColorProto>(num));
		}

		/// <summary>
		/// Builds and caches palette colors for a color proto row.
		/// Throws when <paramref name="colorPrt"/> is null or color data is invalid.
		/// </summary>
		public Vector4[] GetColorVectorArray(IColorProto colorPrt)
		{
			if(colorPrt == null)
			{
				throw new ArgumentNullException(nameof(colorPrt));
			}

			if(m_colorVectorDict.TryGetValue(colorPrt.Num,out Vector4[] colorVectorArray))
			{
				return colorVectorArray;
			}

			var hexCodeArray = colorPrt.ColorArray;

			if(hexCodeArray == null || hexCodeArray.Length < 2)
			{
				throw new InvalidOperationException($"Color proto [{colorPrt.Num}] requires at least 2 color entries.");
			}

			colorVectorArray = new Vector4[hexCodeArray.Length];

			colorVectorArray[0] = hexCodeArray[0].IsEmpty() ? Color.clear : hexCodeArray[0].ToColor();
			colorVectorArray[1] = hexCodeArray[1].IsEmpty() ? Color.clear : hexCodeArray[1].ToColor();

			for(var i=2;i<hexCodeArray.Length;i++)
			{
				colorVectorArray[i] = hexCodeArray[i].IsEmpty() ? colorVectorArray[1] : hexCodeArray[i].ToColor();
			}

			m_colorVectorDict.Add(colorPrt.Num,colorVectorArray);

			return colorVectorArray;
		}

		/// <summary>Loads all TextAssets synchronously. Null entries are skipped with a warning. Returns <c>false</c> on deserialize error.</summary>
		private bool _LoadAll(TextAsset[] textAssetArray)
		{
			for(var i=0;i<textAssetArray.Length;i++)
			{
				var textAsset = textAssetArray[i];

				if(textAsset == null)
				{
					LogChannel.Data.W($"TextAsset is null in {i}");

					continue;
				}

				if(!_TryLoad(textAsset))
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Deserializes one TextAsset into the registry.
		/// Validates <see cref="IProto.Num"/> (positive, unique within the table).
		/// </summary>
		private bool _TryLoad(TextAsset textAsset)
		{
			try
			{
				var protoName = textAsset.name;

				if(textAsset.bytes == null || textAsset.bytes.Length == 0)
				{
					throw new InvalidOperationException($"{protoName} bytes are empty.");
				}

				var protoTypeName = $"KZLib.Data.{protoName}Proto";
				var protoType = KZReflectionKit.FindType(protoTypeName) ?? throw new InvalidOperationException($"Type [{protoTypeName}] was not found.");

				var deserialize = MemoryPackSerializer.Deserialize(protoType.MakeArrayType(),textAsset.bytes);

				if(deserialize is not object[] resultArray)
				{
					throw new InvalidOperationException($"{protoName} did not deserialize to an array.");
				}

				var protoDict = new Dictionary<int,IProto>();

				for(var i=0;i<resultArray.Length;i++)
				{
					var proto = resultArray[i] as IProto ?? throw new InvalidOperationException($"Row in [{protoTypeName}] does not implement {nameof(IProto)}.");

					if(proto.Num <= c_invalidNumber)
					{
						throw new ArgumentException($"Invalid Num [{proto.Num}] at index {i} in [{protoTypeName}]. Num must be positive.");
					}

					if(protoDict.ContainsKey(proto.Num))
					{
						throw new ArgumentException($"Duplicate Num [{proto.Num}] in [{protoTypeName}].");
					}

					protoDict.Add(proto.Num,proto);
				}

				m_protoDict.Add(protoType,protoDict);

				return true;
			}
			catch(Exception exception)
			{
				LogChannel.Data.E($"Proto load failed [{textAsset.name}] [{exception.GetType().Name}] {exception.Message}");

				if(exception.InnerException != null)
				{
					LogChannel.Data.E($"Inner exception [{exception.InnerException.GetType().Name}] {exception.InnerException}");
				}

				LogChannel.Data.E(exception);

				return false;
			}
		}

		/// <summary>Resolves proto TextAssets from <see cref="GameConfig.ProtoFolderPath"/>.</summary>
		private bool _TryGetTextAsset(out TextAsset[] textAssetArray)
		{
			var gameCfg = ConfigManager.In.FetchConfig<GameConfig>();

			textAssetArray = ResourceManager.In.GetTextAssetArray(gameCfg.ProtoFolderPath);

			if(textAssetArray.IsNullOrEmpty())
			{
				LogChannel.Data.E("Proto load failed. No TextAssets found.");

				return false;
			}

			return true;
		}

		/// <summary>Throws when <paramref name="prtType"/> is null or an interface.</summary>
		private void _ThrowIfInterfaceType(Type prtType)
		{
			if(prtType == null)
			{
				throw new ArgumentNullException(nameof(prtType));
			}

			if(prtType.IsInterface)
			{
				throw new ArgumentException($"Proto type must be a concrete class, not an interface [{prtType.Name}].",nameof(prtType));
			}
		}

		/// <summary>Throws when proto tables have not been loaded yet.</summary>
		private void _ThrowIfNotLoaded()
		{
			if(!m_isLoaded)
			{
				throw new InvalidOperationException("ProtoManager is not loaded.");
			}
		}

		/// <summary>Throws when <paramref name="num"/> is zero or negative.</summary>
		private void _ThrowIfInvalidNum(int num)
		{
			if(num <= c_invalidNumber)
			{
				throw new ArgumentOutOfRangeException(nameof(num),num,"Proto Num must be positive.");
			}
		}
	}
}
