using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using YamlDotNet.Serialization;

namespace KZLib.Utilities
{
	/// <summary>
	/// Project-wide path alias registry backed by <c>Resources/Text/Setting/Route.yaml</c>.
	/// </summary>
	/// <remarks>
	/// Shortens long paths via named aliases (e.g. <c>defaultRes:config:GameConfig.yaml</c>).
	/// Placed under <c>System/Route</c> as shared infrastructure used across Data, editor menus, and tools —
	/// same layer as <see cref="PlayerPrefsManager"/>, not a single Data domain.
	/// </remarks>
	public class RouteManager : Singleton<RouteManager>
	{
		private readonly LazyRegistry<string,Route> m_registry = new();
		private readonly Dictionary<string,string> m_definedPathDict = new();

		private readonly string m_routePath = Path.Combine("Text","Setting","Route");

		private string RouteFilePath => Path.Combine("Assets","Resources",$"{m_routePath}.yaml");
		public string RouteFileAbsolutePath => _GetRouteFileAbsolutePath();

		protected override void _Initialize()
		{
			base._Initialize();

			_CreateRouteFile();
			_LoadDefinedPathDict(_LoadRouteYamlText());
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
			var absolutePath = _GetRouteFileAbsolutePath();

			if(!KZFileKit.IsFileExist(absolutePath))
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

				KZFileKit.WriteTextToFile(absolutePath,content);
			}
		}

		private string _GetRouteFileAbsolutePath()
		{
			return KZFileKit.GetAbsolutePath(RouteFilePath,true);
		}

		private string _LoadRouteYamlText()
		{
			var absolutePath = _GetRouteFileAbsolutePath();

			if(KZFileKit.IsFileExist(absolutePath))
			{
				return KZFileKit.ReadTextFromFile(absolutePath);
			}

			var textAsset = Resources.Load<TextAsset>(m_routePath);

			if(textAsset != null)
			{
				return textAsset.text;
			}

			throw new FileNotFoundException($"Route file not found at '{RouteFilePath}'.");
		}

		/// <summary>
		/// Resolves a route from alias chains or raw paths.
		/// <br/> ex) definedPath
		/// <br/> ex) definedPath:path
		/// <br/> ex) definedPath:definedPath:definedPath:path
		/// <br/> ex) path
		/// <br/> Segments are split on ':' (alias chains only; not for absolute Windows paths).
		/// </summary>
		public Route Fetch(string path)
		{
			if(path.IsEmpty())
			{
				throw new ArgumentException($"{nameof(path)} is empty.");
			}

			return m_registry.Fetch(path,_TryCreate);
		}

		private bool _TryCreate(string path,out Route route)
		{
			var pathArray = path.Split(':');

			for(var i=0;i<pathArray.Length;i++)
			{
				if(pathArray[i].IsEmpty())
				{
					throw new ArgumentException($"Route segment at index {i} is empty in '{path}'.");
				}
			}

			if(pathArray.Length == 1)
			{
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

		private void _LoadDefinedPathDict(string yamlText)
		{
			if(yamlText.IsEmpty())
			{
				throw new InvalidDataException($"{nameof(yamlText)} is empty.");
			}

			var deserializer = new DeserializerBuilder().Build();
			var sourceDict = deserializer.Deserialize<Dictionary<string,string>>(yamlText) ?? throw new InvalidDataException($"Route YAML root must be a mapping in '{yamlText}'.");

			foreach(var pair in sourceDict)
			{
				if(pair.Key.IsEmpty())
				{
					throw new InvalidDataException($"Route key at index {pair.Key} is empty in '{yamlText}'.");
				}

				if(pair.Value.IsEmpty())
				{
					throw new InvalidDataException($"Route value for key '{pair.Key}' is empty in '{yamlText}'.");
				}

				if(!m_definedPathDict.TryAdd(pair.Key,pair.Value))
				{
					throw new InvalidDataException($"Duplicate route key '{pair.Key}' in '{yamlText}'.");
				}
			}
		}
	}
}