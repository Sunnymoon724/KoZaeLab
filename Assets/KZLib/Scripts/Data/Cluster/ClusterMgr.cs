using System;
using System.Collections.Generic;
using KZLib.KZUtility;
using KZLib.KZData;

namespace KZLib
{
	public class ClusterMgr : Singleton<ClusterMgr>
	{
		private bool m_disposed = false;

		//? Type / Cluster
		private readonly Dictionary<string,ICluster> m_clusterDict = new();

		protected override void Release(bool disposing)
		{
			if(m_disposed)
			{
				return;
			}

			if(disposing)
			{
				m_clusterDict.Clear();
			}

			m_disposed = true;

			base.Release(disposing);
		}

		public TCluster GetOrCreateCluster<TCluster>(object param) where TCluster : class,ICluster
		{
			return GetOrCreateCluster(typeof(TCluster),param) as TCluster;
		}

		public ICluster GetOrCreateCluster(Type type,object param)
		{
			var key = $"{type.Name}_{param}";

			if(!m_clusterDict.TryGetValue(key,out var cluster))
			{
				cluster = Activator.CreateInstance(type,param) as ICluster;

				if(cluster == null)
				{
					LogTag.System.E($"Failed to create cluster of type {type}");

					return null;
				}

				m_clusterDict.Add(key,cluster);
			}

			return cluster;
		}
	}
}