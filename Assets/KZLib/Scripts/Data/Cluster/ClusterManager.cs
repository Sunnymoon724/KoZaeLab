using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using KZLib.Utilities;

namespace KZLib.Data
{
	/// <summary>
	/// Lazy cache of Proto-composed <see cref="ICluster"/> snapshots (first-pass composed data).
	/// Combines multiple protos and derived fields once per key; repeated fetches return the same instance.
	/// Key = cluster type + ID (<see cref="ValueTuple"/>, int, key record, etc.).
	/// </summary>
	/// <remarks>
	/// Cluster ctors must throw when proto lookup or composition fails. Never return early with uninitialized fields —
	/// a successfully constructed instance is cached and reused.
	/// <see cref="_Release"/> clears the registry cache only (immutable snapshots; no per-cluster teardown).
	/// </remarks>
	public class ClusterManager : Singleton<ClusterManager>
	{
		/// <summary>Sentinel key for parameterless cluster ctors. Used only by <see cref="Fetch{TCluster}()"/>.</summary>
		private readonly struct ParameterlessKey : IEquatable<ParameterlessKey>
		{
			public bool Equals(ParameterlessKey other) => true;

			public override int GetHashCode() => 0;
		}

		/// <summary>Dictionary key: cluster runtime type + ID key with typed equality.</summary>
		private readonly struct ClusterKey : IEquatable<ClusterKey>
		{
			private readonly int m_hashCode;
			private readonly IEqualityComparer m_keyComparer;

			public Type ClusterType { get; init; }
			public object Key { get; init; }

			private ClusterKey(Type cltType,object key,IEqualityComparer keyComparer,int hashCode)
			{
				ClusterType = cltType;
				Key = key;
				m_keyComparer = keyComparer;
				m_hashCode = hashCode;
			}

			public static ClusterKey Create<TKey>(Type cltType,TKey key) where TKey : IEquatable<TKey>
			{
				var comparer = EqualityComparer<TKey>.Default;
				var hashCode = new HashCode();

				hashCode.Add(cltType);
				hashCode.Add(key,comparer);

				return new ClusterKey(cltType,key,comparer,hashCode.ToHashCode());
			}

			public bool Equals(ClusterKey other)
			{
				if(m_hashCode != other.m_hashCode)
				{
					return false;
				}

				if(ClusterType != other.ClusterType)
				{
					return false;
				}

				return m_keyComparer.Equals(Key,other.Key);
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

		/// <summary>Clears all cached clusters. Does not run per-instance cleanup — clusters are treated as immutable snapshots.</summary>
		protected override void _Release(bool disposing)
		{
			if(disposing)
			{
				m_registry.Release();
			}

			base._Release(disposing);
		}

		/// <summary>Returns a cached parameterless cluster, creating it on first access.</summary>
		public TCluster Fetch<TCluster>() where TCluster : class,ICluster
		{
			return Fetch<TCluster,ParameterlessKey>(default);
		}

		/// <summary>
		/// Returns a cached cluster for the given ID key, creating and composing it on first access.
		/// ex) FetchCluster&lt;AnimalCluster,(int,int)&gt;((animalNum,colorNum))
		/// ex) FetchCluster&lt;HeroCluster,int&gt;(heroNum)
		/// </summary>
		/// <typeparam name="TCluster">Concrete <see cref="ICluster"/> (typically an immutable record).</typeparam>
		/// <typeparam name="TKey">
		/// Stable ID for this cluster. <c>TKey</c> shape must match the cluster ctor:
		/// single value → one ctor arg; ValueTuple → unpacked ctor args (e.g. <c>(int,int)</c> → <c>Cluster(int,int)</c>).
		/// </typeparam>
		public TCluster Fetch<TCluster,TKey>(TKey key) where TCluster : class,ICluster where TKey : IEquatable<TKey>
		{
			if(!TryFetch<TCluster,TKey>(key,out var clt))
			{
				throw new InvalidOperationException($"Failed to create cluster [{typeof(TCluster).Name}].");
			}

			return clt;
		}

		/// <summary>Same as <see cref="Fetch{TCluster,TKey}"/> but returns false when creation fails instead of throwing.</summary>
		public bool TryFetch<TCluster,TKey>(TKey key,out TCluster clt) where TCluster : class,ICluster where TKey : IEquatable<TKey>
		{
			clt = null;

			if(!TryFetch(typeof(TCluster),key,out var result))
			{
				return false;
			}

			if(result is TCluster newClt)
			{
				clt = newClt;

				return true;
			}

			LogChannel.Data.E($"Created cluster type [{result.GetType().Name}] does not match requested type [{typeof(TCluster).Name}].");

			return false;
		}

		/// <summary>Returns a cached parameterless cluster, creating it on first access.</summary>
		public bool TryFetch<TCluster>(out TCluster clt) where TCluster : class,ICluster
		{
			return TryFetch<TCluster,ParameterlessKey>(default,out clt);
		}

		/// <summary>Returns a cached cluster for the given type and ID key, creating it on first access.</summary>
		public ICluster Fetch<TKey>(Type type,TKey key) where TKey : IEquatable<TKey>
		{
			if(!TryFetch(type,key,out var clt))
			{
				throw new InvalidOperationException($"Failed to create cluster [{type.Name}].");
			}

			return clt;
		}

		/// <summary>Same as <see cref="Fetch{TKey}(Type,TKey)"/> but returns false when creation fails instead of throwing.</summary>
		public bool TryFetch<TKey>(Type type,TKey key,out ICluster clt) where TKey : IEquatable<TKey>
		{
			clt = null;

			_ValidateType(type);

			var cltKey = ClusterKey.Create(type,key);
			clt = m_registry.Fetch(cltKey,_TryCreate);

			return clt != null;
		}

		private static void _ValidateType(Type type)
		{
			if(type == null)
			{
				throw new ArgumentNullException(nameof(type));
			}

			if(type.IsAbstract || type.IsInterface)
			{
				throw new ArgumentException($"Type [{type.Name}] must be a concrete cluster class.",nameof(type));
			}

			if(!typeof(ICluster).IsAssignableFrom(type))
			{
				throw new ArgumentException($"Type [{type.Name}] must implement {nameof(ICluster)}.",nameof(type));
			}
		}

		private bool _TryCreate(ClusterKey key,out ICluster clt)
		{
			try
			{
				clt = _Create(key.ClusterType,key.Key);

				if(clt == null)
				{
					LogChannel.Data.E($"Created instance of [{key.ClusterType.Name}] is not {nameof(ICluster)}.");
				}

				return clt != null;
			}
			catch(Exception exception)
			{
				LogChannel.Data.E($"Failed to create cluster [{key.ClusterType.Name}] [{exception.Message}]");

				clt = null;

				return false;
			}
		}

		/// <summary>
		/// Instantiates the cluster ctor matching the key shape.
		/// ParameterlessKey → parameterless ctor; single value → one arg; ValueTuple → unpacked args.
		/// </summary>
		private static ICluster _Create(Type type,object key)
		{
			const BindingFlags bindingFlag = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.CreateInstance;

			if(key is ParameterlessKey)
			{
				return Activator.CreateInstance(type,true) as ICluster;
			}

			if(key is ITuple tuple)
			{
				if(tuple.Length == 0)
				{
					return Activator.CreateInstance(type,true) as ICluster;
				}

				var argumentArray = new object[tuple.Length];

				for(var i=0;i<tuple.Length;i++)
				{
					argumentArray[i] = tuple[i];
				}

				return Activator.CreateInstance(type,bindingFlag,null,argumentArray,null) as ICluster;
			}

			return Activator.CreateInstance(type,bindingFlag,null,new[] { key },null) as ICluster;
		}
	}
}
