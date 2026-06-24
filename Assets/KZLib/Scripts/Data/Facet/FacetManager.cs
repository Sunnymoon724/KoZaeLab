using System;
using System.Collections.Generic;
using KZLib.Utilities;

namespace KZLib.Data
{
	/// <summary>
	/// Session cache of server-synced <see cref="IFacet"/> state. One cached instance per concrete facet type.
	/// <see cref="Networks.NetworkManager"/> pushes payloads via <see cref="Apply{TFacet}"/>; game code reads via Get/TryGet.
	/// </summary>
	/// <remarks>
	/// Does not persist to disk (use Tune for client settings). Facet implementations must throw when server data is invalid —
	/// a registered instance is reused via <see cref="IFacet.Apply"/>.
	/// Get/TryGet/Has/Revoke use <c>typeof(TFacet)</c> — pass a concrete facet type (e.g. <c>ProfileFacet</c>).
	/// </remarks>
	public class FacetManager : Singleton<FacetManager>
	{
		/// <summary>Concrete facet type → cached instance.</summary>
		private readonly Dictionary<Type,IFacet> m_facetDict = new();

		private FacetManager() { }

		/// <summary>Calls Release on every registered facet, then clears the registry.</summary>
		protected override void _Release(bool disposing)
		{
			if(disposing)
			{
				foreach(var pair in m_facetDict)
				{
					pair.Value.Release();
				}

				m_facetDict.Clear();
			}

			base._Release(disposing);
		}

		/// <summary>
		/// Registers a facet on first server payload, or merges the latest payload into the cached instance.
		/// Uses <see cref="object.GetType"/> as the registry key so calls through <see cref="IFacet"/> (Network) resolve to the concrete type.
		/// </summary>
		public void Apply<TFacet>(TFacet newFct) where TFacet : class,IFacet
		{
			if(newFct == null)
			{
				throw new ArgumentNullException(nameof(newFct));
			}

			var type = newFct.GetType();

			if(m_facetDict.TryGetValue(type,out var oldFct))
			{
				oldFct.Apply(newFct);
			}
			else
			{
				m_facetDict[type] = newFct;
			}
		}

		/// <summary>Returns the cached facet. Throws when not registered.</summary>
		public TFacet Get<TFacet>() where TFacet : class,IFacet
		{
			if(TryGet<TFacet>(out var fct))
			{
				return fct;
			}

			throw new InvalidOperationException($"Facet type [{typeof(TFacet).Name}] does not exist.");
		}

		/// <summary>Returns the cached facet, or false when not registered.</summary>
		public bool TryGet<TFacet>(out TFacet fct) where TFacet : class,IFacet
		{
			fct = m_facetDict.TryGetValue(typeof(TFacet),out var val) ? val as TFacet : null;

			return fct != null;
		}

		/// <summary>Returns whether a facet of the given type is registered.</summary>
		public bool Has<TFacet>() where TFacet : class,IFacet
		{
			return m_facetDict.ContainsKey(typeof(TFacet));
		}

		/// <summary>Calls Release on the facet and removes it from the registry.</summary>
		public void Revoke<TFacet>() where TFacet : class,IFacet
		{
			var type = typeof(TFacet);

			if(m_facetDict.TryGetValue(type,out var fct))
			{
				fct.Release();

				m_facetDict.Remove(type);
			}
		}
	}
}