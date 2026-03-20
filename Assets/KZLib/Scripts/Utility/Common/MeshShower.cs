using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Text;

namespace KZLib.Utilities
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

				if(!meshFilter || !meshFilter.sharedMesh)
				{
					continue;
				}

				textListDict.AddOrCreate($"{meshFilter.sharedMesh.name}",meshFilter.transform.FindHierarchy());
			}

			if(textListDict.Count == 0)
			{
				LogChannel.None.I("MeshFilter is not found.");

				return;
			}

			var builder = new StringBuilder();
			builder.AppendLine("MeshFilter list");

			foreach(var pair in textListDict)
			{
				builder.Append($"{pair.Key}\n");

				var textList = pair.Value;

				for(var i=0;i<textList.Count;i++)
				{
					builder.AppendLine($" -[{textList[i]}]");
				}

				builder.AppendLine();
			}

			LogChannel.None.I(builder.ToString());
		}
	}
}