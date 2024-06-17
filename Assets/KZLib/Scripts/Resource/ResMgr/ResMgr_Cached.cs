using System;
using System.Collections.Generic;
using UniRx;
using Object = UnityEngine.Object;

namespace KZLib
{
	public partial class ResMgr : Singleton<ResMgr>
	{
		private record CachedData
		{
			private const double DEFAULT_DELETE_TIME = 60.0d;	// 60초

			public Object[] DataArray { get; }
			private readonly CompositeDisposable m_Disposable = new();

			public CachedData(Object[] _dataArray,Action _onAction)
			{
				Observable.Timer(TimeSpan.FromSeconds(DEFAULT_DELETE_TIME)).Subscribe(_ => _onAction()).AddTo(m_Disposable);

				DataArray = _dataArray;
			}

			public void Release()
			{
				m_Disposable.Dispose();
			}
		}

		private readonly Dictionary<string,CachedData> m_CachedDataDict = new();

		private TObject GetData<TObject>(string _path) where TObject : Object
		{
			return m_CachedDataDict.TryGetValue(_path,out var data) ? data.DataArray[0] as TObject : null;
		}

		private TObject[] GetDataArray<TObject>(string _path) where TObject : Object
		{
			return m_CachedDataDict.TryGetValue(_path,out var data) ? data.DataArray as TObject[] : null;
		}

		private void PutData<TObject>(string _path,TObject _object) where TObject : Object
		{
			if(m_CachedDataDict.ContainsKey(_path))
			{
				return;
			}

			m_CachedDataDict.Add(_path,new CachedData(new TObject[] {_object},()=>
			{
				m_CachedDataDict[_path].Release();
				m_CachedDataDict.RemoveSafe(_path);
			}));
		}

		private void PutDataArray<TObject>(string _path,TObject[] _objectArray) where TObject : Object
		{
			if(m_CachedDataDict.ContainsKey(_path))
			{
				return;
			}

			m_CachedDataDict.Add(_path,new CachedData(_objectArray,()=>
			{
				m_CachedDataDict[_path].Release();
				m_CachedDataDict.RemoveSafe(_path);
			}));
		}
	}
}