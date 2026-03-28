using System;
using KZLib.Utilities;

namespace KZLib.Data
{
	public class ClusterManager : Singleton<ClusterManager>
	{
		private readonly struct ClusterKey : IEquatable<ClusterKey>
		{
			private readonly int m_hashCode;

			public Type Type { get; init; }
			public object[] ParamArray { get; init; }

			public ClusterKey(Type type,object[] paramArray)
			{
				Type = type;
				ParamArray = paramArray;

				var hashCode = new HashCode();

				hashCode.Add(Type);

				if(paramArray != null)
				{
					for(var i=0;i<ParamArray.Length;i++)
					{
						hashCode.Add(ParamArray[i]);
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

				if(Type != other.Type)
				{
					return false;
				}

				if(ParamArray.Length != other.ParamArray.Length)
				{
					return false;
				}

				for(var i=0;i<ParamArray.Length;i++)
				{
					if(!Equals(ParamArray[i],other.ParamArray[i]))
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

		private readonly LazyRegistry<ClusterKey,ICluster> m_registry = new();

		protected override void _Release(bool disposing)
		{
			if(disposing)
			{
				m_registry.Release();
			}

			base._Release(disposing);
		}

		public TCluster FetchCluster<TCluster>(params object[] paramArray) where TCluster : class,ICluster
		{
			return FetchCluster(typeof(TCluster),paramArray) as TCluster;
		}

		public ICluster FetchCluster(Type type,params object[] paramArray)
		{
			var key = new ClusterKey(type,paramArray);

			return m_registry.Fetch(key,_TryCreateCluster);
		}

		private bool _TryCreateCluster(ClusterKey key,out ICluster cluster)
		{
			cluster = Activator.CreateInstance(key.Type,key.ParamArray) as ICluster;

			return cluster != null;
		}
	}
}