using System.Collections.Generic;
using System.IO;
using KZLib.KZData;
using KZLib.KZUtility;
using UnityEngine;
using YamlDotNet.Serialization;

namespace KZLib
{
	public class RouteMgr : Singleton<RouteMgr>
	{
		private readonly Dictionary<string,Route> m_routeDict = new();

		private readonly Dictionary<string,string> m_defaultPathDict = new();

		private bool m_disposed = false;

		private string m_routePath = "";

		protected override void Initialize()
		{
			base.Initialize();

			m_routePath = Path.Combine("Text","Setting","Route");

			_CreateRouteFile();

			var textAsset = Resources.Load<TextAsset>(m_routePath);

			var deserializer = new DeserializerBuilder().Build();

			foreach(var pair in deserializer.Deserialize<Dictionary<string,string>>(textAsset.text))
			{
				m_defaultPathDict.Add(pair.Key,pair.Value);
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
				m_defaultPathDict.Clear();
				m_routeDict.Clear();
			}

			m_disposed = true;

			base.Release(disposing);
		}

		private void _CreateRouteFile()
		{
			var routeFilePath = GetRouteFilePath();

			if(!FileUtility.IsFileExist(routeFilePath))
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

				FileUtility.WriteTextToFile(routeFilePath,content.Replace("\t",""));
			}
		}

		public string GetRouteFilePath()
		{
			return Path.Combine(Global.ASSETS_TEXT,Global.RESOURCES_TEXT,string.Format("{0}.yaml",m_routePath));
		}

		/// <summary>
		/// ex) defaultPath <br/>
		/// ex) defaultPath:path <br/>
		/// ex) defaultPath:defaultPath:defaultPath:path <br/>
		/// ex) path <br/>
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public Route GetOrCreateRoute(string path)
		{
			if(!m_routeDict.TryGetValue(path,out var route))
			{
				var pathArray = path.Split(":");

				if(pathArray.Length == 0)
				{
					// defaultPath or path
					route = new Route(_ConvertDefaultPath(path));
				}
				else
				{
					var count = pathArray.Length;
					var textArray = new string[count];

					for(var i=0;i<count;i++)
					{
						textArray[i] = _ConvertDefaultPath(pathArray[i]);
					}

					route = new Route(Path.Combine(textArray));
				}

				m_routeDict.Add(path,route);
			}

			return route;
		}

		private string _ConvertDefaultPath(string text)
		{
			return m_defaultPathDict.TryGetValue(text,out var path) ? path : text;
		}
	}
}