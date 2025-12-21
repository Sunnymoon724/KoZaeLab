using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Text;

namespace KZLib.KZDevelop
{
	public class MeshNameShower : MonoBehaviour
	{
		[Button("Show Mesh Name",ButtonSizes.Large)]
		protected void _OnShowMeshName()
		{
			var textListDict = new Dictionary<string,List<string>>();
			var meshFilterArray = GetComponentsInChildren<MeshFilter>();

			for(int i=0;i<meshFilterArray.Length;i++)
			{
				var meshFilter = meshFilterArray[i];

				if(meshFilter == null)
				{
					continue;
				}

				textListDict.AddOrCreate($"{meshFilter.sharedMesh.name}",meshFilter.transform.FindHierarchy());
			}

			if(textListDict.Count == 0)
			{
				LogSvc.Editor.I("MeshFilter is not found.");
			}
			else
			{
				LogSvc.Editor.I("MeshFilter list");

				var builder = new StringBuilder();

				foreach(var pair in textListDict)
				{
					builder.Clear();

					builder.Append($"{pair.Key}\n");

					foreach(var path in pair.Value)
					{
						builder.Append($" -[{path}]\n");
					}

					builder.AppendLine();

					LogSvc.Editor.I(builder.ToString());
				}
			}
		}
	}
}