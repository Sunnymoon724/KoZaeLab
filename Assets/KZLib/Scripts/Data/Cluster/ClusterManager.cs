using System;
using System.Collections.Generic;
using KZLib.Utilities;

namespace KZLib.Data
{
	public class ClusterManager : Singleton<ClusterManager>
	{
		private readonly struct ClusterKey : IEquatable<ClusterKey>
		{
			private readonly Type m_type;
			private readonly object[] m_paramArray;
			private readonly int m_hashCode;

			public ClusterKey(Type type,object[] paramArray)
			{
				m_type = type;
				m_paramArray = paramArray;

				var hashCode = new HashCode();

				hashCode.Add(m_type);

				if(paramArray != null)
				{
					for(var i=0;i<m_paramArray.Length;i++)
					{
						hashCode.Add(m_paramArray[i]);
					}
				}

				m_hashCode = hashCode.ToHashCode();
			}

			public bool Equals(ClusterKey other)
			{
				if(m_hashCode != other.m_hashCode)
				{
					return false;
				}

				if(m_type != other.m_type)
				{
					return false;
				}

				if(m_paramArray.Length != other.m_paramArray.Length)
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
			
			public override bool Equals(object target)
			{
				return target is ClusterKey other && Equals(other);
			}

			public override int GetHashCode()
			{
				return m_hashCode;
			}
		}

		private readonly Dictionary<ClusterKey,ICluster> m_clusterDict = new();

		protected override void _Release(bool disposing)
		{
			if(disposing)
			{
				m_clusterDict.Clear();
			}

			base._Release(disposing);
		}

		public TCluster GetOrCreateCluster<TCluster>(params object[] paramArray) where TCluster : class,ICluster
		{
			return GetOrCreateCluster(typeof(TCluster),paramArray) as TCluster;
		}

		public ICluster GetOrCreateCluster(Type type,params object[] paramArray)
		{
			var key = new ClusterKey(type,paramArray);

			if(!m_clusterDict.TryGetValue(key,out var clt))
			{
				clt = Activator.CreateInstance(type,paramArray) as ICluster ?? throw new InvalidCastException($"Failed to create cluster of type {type}");

				m_clusterDict.Add(key,clt);
			}

			return clt;
		}
	}
}