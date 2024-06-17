using UnityEngine;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace KZLib
{
	public partial class ResMgr : Singleton<ResMgr>
	{
		private Queue<LoadingData> m_LoadingQueue = new();
		
		private void AddLoadingQueue(string _path,bool _isFilePath,Transform _parent = null)
		{
			m_LoadingQueue.Enqueue(new LoadingData(_path,_isFilePath,_parent));

			if(m_LoadingQueue.Count == 1)
			{
				if(m_Source != null)
				{
					m_Source.Cancel();
					m_Source.Dispose();
				}

				m_Source = new();

				LoadingDataAsync(m_Source.Token).Forget();
			}
		}

		private async UniTaskVoid LoadingDataAsync(CancellationToken _token)
		{
			while(m_LoadingQueue.Count > 0)
			{
				await UniTask.WaitForSeconds(UPDATE_PERIOD,true,cancellationToken : _token);

				var data = m_LoadingQueue.Dequeue();

				GetObject(data.DataPath,data.Parent,true);
			}

			m_Source?.Dispose();
			m_Source = null;
		}

		private record LoadingData(string DataPath,bool IsFilePath,Transform Parent);
	}
}