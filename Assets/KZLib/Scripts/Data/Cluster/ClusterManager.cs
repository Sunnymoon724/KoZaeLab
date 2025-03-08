using System;
using System.Collections.Generic;
using KZLib.KZUtility;
using KZLib.KZData;

namespace KZLib
{
	public class ClusterManager : DataSingleton<ClusterManager>
	{
		// Type / Cluster
		private readonly Dictionary<Type,ICluster> m_clusterDict = new();

		protected override void Clear()
		{
			m_clusterDict.Clear();
		}

		public TCluster GetOrCreateCluster<TCluster>(IClusterParam param) where TCluster : class,ICluster
		{
			return GetOrCreateCluster(typeof(TCluster),param) as TCluster;
		}

		public ICluster GetOrCreateCluster(Type clusterType,IClusterParam param)
		{
			if(!m_clusterDict.TryGetValue(clusterType,out var cluster))
			{
				cluster = Activator.CreateInstance(clusterType,param) as ICluster;

				m_clusterDict.Add(clusterType,cluster);
			}

			return cluster;
		}
	}
}