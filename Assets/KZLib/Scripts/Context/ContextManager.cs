using System;
using System.Collections.Generic;
using KZLib.KZUtility;

namespace KZLib
{
	public interface IContext { }

	public class ContextManager : Singleton<ContextManager>
	{
		private readonly Dictionary<string,Type> m_contextTypeDict = new();
		private readonly Dictionary<string,IContext> m_contextDict = new();

		private ContextManager() { }

		protected override void _Initialize()
		{
			base._Initialize();

			var contextType = typeof(IContext);

			foreach(var type in CommonUtility.FindDerivedTypeGroup(contextType))
			{
				if(type.IsAbstract || type.IsInterface || type.IsGenericTypeDefinition || type.IsNested)
				{
					continue;
				}

				var interfaceTypeArray = type.GetInterfaces();
				var flag = false;

				for(var i=0;i<interfaceTypeArray.Length;i++)
				{
					var interfaceType = interfaceTypeArray[i];
	
					if(interfaceType == contextType || !contextType.IsAssignableFrom(interfaceType))
					{
						continue;
					}

					var key = interfaceType.Name;

					if(flag)
					{
						throw new ArgumentException($"Value : {type} is already exists.");
					}

					if(!m_contextTypeDict.ContainsKey(key))
					{
						m_contextTypeDict.Add(key,type);
						flag = true;
					}
					else
					{
						throw new ArgumentException($"Key : {key} is already exists.");
					}
				}
			}
		}

		protected override void _Release(bool disposing)
		{
			if(disposing)
			{
				m_contextDict.Clear();
			}

			base._Release(disposing);
		}

		/// <summary>
		/// If config is not exist, create new config.
		/// </summary>
		public TContext Access<TContext>() where TContext : class,IContext
		{
			var key = typeof(TContext).Name;

			if(!m_contextDict.TryGetValue(key,out var context))
			{
				context = _Create(key);

				if(context == null)
				{
					LogChannel.System.E($"{key}context is not exist.");

					return null;
				}

				m_contextDict.Add(key,context);
			}

			return context as TContext;
		}

		private IContext _Create(string key)
		{
			var contextType = m_contextTypeDict.TryGetValue(key,out var type) ? type : null;
			
			if(contextType != null)
			{
				return Activator.CreateInstance(contextType) as IContext;
			}

			return null;
		} 
	}
}