// using System.Collections.Generic;
// using System.IO;
// using KZLib.KZUtility;
// using UnityEngine;
// using YamlDotNet.Serialization;

// namespace KZLib.KZDevelop
// {
// 	public class RouteMgr : Singleton<RouteMgr>
// 	{
// 		private readonly Dictionary<string,Route> m_routeDict = new();

// 		private readonly Dictionary<string,string> m_pathDict = new();

// 		private bool m_disposed = false;

// 		protected override void Release(bool disposing)
// 		{
// 			if(m_disposed)
// 			{
// 				return;
// 			}

// 			if(disposing)
// 			{
// 				m_routeDict.Clear();
// 			}

// 			m_disposed = true;

// 			base.Release(disposing);
// 		}

// 		protected override void Initialize()
// 		{
// 			base.Initialize();

// 			var routeFile = Path.GetFullPath(Path.Combine(Application.dataPath,"../Route.yaml"));

// 			if(!File.Exists(routeFile))
// 			{
// 				// add default route
// 				var content = "# resources folder\ndefaultResource : Assets/Resources\n\n# addressable folder\ngameResource : Assets/GameResources\n\n# for indirect references\nworkResource : Assets/WorkResources";

// 				File.WriteAllText(routeFile,content.Trim());
// 			}

// 			var routeText = File.ReadAllText(routeFile);

// 			var deserializer = new DeserializerBuilder().Build();

// 			m_pathDict.AddRange(deserializer.Deserialize<Dictionary<string,string>>(routeText));
// 		}

// 		public Route GetOrCreateRoute(string path)
// 		{
// 			if(!m_routeDict.TryGetValue(path,out var route))
// 			{
// 				var pathArray = path.Split(":");
// 				var count = pathArray.Length-1;
// 				var headerArray = new string[count];

// 				for(var i=0;i<count;i++)
// 				{
// 					headerArray[i] = ConvertPath(pathArray[i]);
// 				}

// 				var header = Path.Combine(headerArray);
// 				var body = Path.GetFileNameWithoutExtension(pathArray[^1]);
// 				var extension = Path.GetExtension(body);

// 				route = new Route(header,body,extension);

// 				m_routeDict.Add(path,route);
// 			}

// 			return route;
// 		}

// 		private string ConvertPath(string text)
// 		{
// 			return m_pathDict.TryGetValue(text,out var path) ? path : text;
// 		}
// 	}
// }