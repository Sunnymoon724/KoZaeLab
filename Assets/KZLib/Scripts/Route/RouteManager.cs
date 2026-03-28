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
				var content = @"# resources folder
				defaultRes : Assets/Resources

				# addressable folder
				gameRes : Assets/GameResources

				# for indirect references
				workRes : Assets/WorkResources

				# config folder
				config : Text/Config

				# proto folder
				proto : Text/Proto

				# lingo folder
				lingo : ScriptableObject/Lingo

				# generated script folder
				generatedScript : Assets/Scripts/Generated";

				KZFileKit.WriteTextToFile(routeFilePath,content.Replace("\t",""));
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
			return m_registry.Fetch(path,_TryCreateRoute);
		}

		private bool _TryCreateRoute(string path,out Route route)
		{
			var pathArray = path.Split(":");

			if(pathArray.Length == 0)
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