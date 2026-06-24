#if UNITY_EDITOR
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace KZLib.EditorTools
{
	/// <summary>
	/// Strips <see cref="PathCreator"/> at build time; the path remains baked on <see cref="LineRenderer"/>.
	/// </summary>
	public class PathCreatorBuildProcessor : IProcessSceneWithReport
	{
		public int callbackOrder => 0;

		public void OnProcessScene(Scene scene,BuildReport report)
		{
			var pathCreatorArray = Object.FindObjectsByType<PathCreator>(FindObjectsInactive.Include,FindObjectsSortMode.None);

			for(var i=0;i<pathCreatorArray.Length;i++)
			{
				var pathCreator = pathCreatorArray[i];

				pathCreator.PrepareForBuild();

				Object.DestroyImmediate(pathCreator);
			}
		}
	}
}
#endif
