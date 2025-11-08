using System;
using System.Collections.Generic;
using KZLib.KZUtility;

namespace KZLib.KZData
{
	public class ClusterManager : Singleton<ClusterManager>
	{
        private class ClusterKey : IEquatable<ClusterKey>
		{
			private readonly Type m_type;
			private readonly object[] m_paramArray;

			public ClusterKey(Type type,object[] paramArray)
			{
				m_type = type;
				m_paramArray = paramArray;
			}

			public bool Equals(ClusterKey other)
			{
				if(other == null || m_type != other.m_type || m_paramArray.Length != other.m_paramArray.Length)
				{
					return false;
				}

				for(var i=0;i<m_paramArray.Length;i++)
				{
					if(!Equals(m_paramArray[i],other.m_paramArray[i]))
					{
						return false;
					}
				}

				return true;
			}

			public override bool Equals(object obj) => Equals(obj as ClusterKey);

			public override int GetHashCode()
			{
				int hashCode = m_type.GetHashCode();

				foreach(var param in m_paramArray)
				{
					hashCode = HashCode.Combine(hashCode,param?.GetHashCode() ?? 0);
				}

				return hashCode;
			}
		}

		private bool m_disposed = false;

		//? Type / Cluster
		private readonly Dictionary<ClusterKey,ICluster> m_clusterDict = new();

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

		public TCluster GetOrCreateCluster<TCluster>(params object[] paramArray) where TCluster : class,ICluster
		{
			return GetOrCreateCluster(typeof(TCluster),paramArray) as TCluster;
		}

		public ICluster GetOrCreateCluster(Type type,params object[] paramArray)
		{
			var key = new ClusterKey(type,paramArray);

			if(!m_clusterDict.TryGetValue(key,out var cluster))
			{
				cluster = Activator.CreateInstance(type,paramArray) as ICluster;

				if(cluster == null)
				{
					throw new InvalidCastException($"Failed to create cluster of type {type}");
				}

				m_clusterDict.Add(key,cluster);
			}

			return cluster;
		}
	}
}