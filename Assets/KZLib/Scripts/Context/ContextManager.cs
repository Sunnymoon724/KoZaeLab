using System;
using System.Collections.Generic;
using KZLib.KZUtility;

namespace KZLib
{
	public interface IContext { }

	public class ContextManager : Singleton<ContextManager>
	{
		private bool m_disposed = false;

		private readonly Dictionary<string,Type> m_contextTypeDict = new();
		private readonly Dictionary<string,IContext> m_contextDict = new();

		protected override void Initialize()
		{
			base.Initialize();

			var contextType = typeof(IContext);

			foreach(var type in CommonUtility.FindDerivedTypeGroup(contextType))
			{
				if(type.IsAbstract || type.IsInterface || type.IsGenericTypeDefinition || type.IsNested)
				{
					continue;
				}

				var interfaceArray = type.GetInterfaces();
				var flag = false;

				foreach(var interfaceType in interfaceArray)
				{
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

		protected override void Release(bool disposing)
		{
			if(m_disposed)
			{
				return;
			}

			if(disposing)
			{
				m_contextDict.Clear();
			}

			m_disposed = true;

			base.Release(disposing);
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
					LogSvc.System.E($"{key}context is not exist.");

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
				return null;
			}

			return Activator.CreateInstance(contextType) as IContext;
		} 
	}
}