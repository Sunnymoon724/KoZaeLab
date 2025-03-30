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

		public TCluster GetOrCreateCluster<TCluster>(IClusterParam param) where TCluster : class,ICluster,new()
		{
			var clusterKey = param.Key;

			if(clusterKey.IsEmpty())
			{
				LogTag.System.E($"Cluster key must not be null or empty [{param}]");

				return null;
			}

			if(!m_clusterDict.TryGetValue(clusterKey,out var cluster))
			{
				cluster = new TCluster();

				if(cluster == null)
				{
					LogTag.System.E($"Failed to create cluster of type {typeof(TCluster)}");

					return null;
				}

				m_clusterDict.Add(clusterKey,cluster);
			}

			return cluster as TCluster;
		}
	}
}