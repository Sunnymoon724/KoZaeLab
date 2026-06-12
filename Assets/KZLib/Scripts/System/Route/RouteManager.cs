using System;
using System.Collections.Generic;
using System.IO;
using KZLib.Utilities;
using UnityEngine;
using YamlDotNet.Serialization;

namespace KZLib.Data
{
	public class RouteManager : Singleton<RouteManager>
	{
		private readonly LazyRegistry<string,Route> m_registry = new();
		private readonly Dictionary<string,string> m_definedPathDict = new();

		private string m_routePath = "";

		protected override void _Initialize()
		{
			base._Initialize();

			m_routePath = Path.Combine("Text","Setting","Route");

			_CreateRouteFile();

			var textAsset = Resources.Load<TextAsset>(m_routePath);

			if(textAsset == null)
			{
				throw new NullReferenceException($"Route file not found at '{m_routePath}'. RoutePath must be assigned.");
			}

			var deserializer = new DeserializerBuilder().Build();

			foreach(var pair in deserializer.Deserialize<Dictionary<string,string>>(textAsset.text))
			{
				m_definedPathDict.Add(pair.Key,pair.Value);
			}
		}

		protected override void _Release(bool disposing)
		{
			if(disposing)
			{
				m_definedPathDict.Clear();
				m_registry.Release();
			}

			base._Release(disposing);
		}

		private void _CreateRouteFile()
		{
			var routeFilePath = GetRouteFilePath();

			if(!KZFileKit.IsFileExist(routeFilePath))
			{
				// add default route
				var content =
								"# resources folder\n" +
								"defaultRes : Assets/Resources\n" +
								"\n" +
								"# addressable folder\n" +
								"gameRes : Assets/GameResources\n" +
								"\n" +
								"# for indirect references\n" +
								"workRes : Assets/WorkResources\n" +
								"\n" +
								"# config folder\n" +
								"config : Text/Config\n" +
								"\n" +
								"# proto folder\n" +
								"proto : Text/Proto\n" +
								"\n" +
								"# lingo folder\n" +
								"lingo : ScriptableObject/Lingo\n" +
								"\n" +
								"# generated script folder\n" +
								"generatedScript : Assets/Scripts/Generated";

				KZFileKit.WriteTextToFile(routeFilePath,content);
			}
		}

		public string GetRouteFilePath()
		{
			return Path.Combine("Assets","Resources",$"{m_routePath}.yaml");
		}

		/// <summary>
		/// ex) definedPath <br/>
		/// ex) definedPath:path <br/>
		/// ex) definedPath:definedPath:definedPath:path <br/>
		/// ex) path <br/>
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public Route FetchRoute(string path)
		{
			if(path.IsEmpty())
			{
				throw new NullReferenceException("Path cannot be null or empty.");
			}

			return m_registry.Fetch(path,_TryCreateRoute);
		}

		private bool _TryCreateRoute(string path,out Route route)
		{
			var pathArray = path.Split(":");

			if(pathArray.Length == 1)
			{
				// definedPath or path
				route = new Route(_ConvertDefinedPath(path));

				return true;
			}

			var count = pathArray.Length;
			var textArray = new string[count];

			for(var i=0;i<count;i++)
			{
				textArray[i] = _ConvertDefinedPath(pathArray[i]);
			}

			route = new Route(Path.Combine(textArray));

			return true;
		}

		private string _ConvertDefinedPath(string text)
		{
			return m_definedPathDict.TryGetValue(text,out var path) ? path : text;
		}
	}
}