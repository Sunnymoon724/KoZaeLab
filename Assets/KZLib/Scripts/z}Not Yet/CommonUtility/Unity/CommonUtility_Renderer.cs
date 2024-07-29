using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public static partial class CommonUtility
{
	/// <summary>
	/// 불러온 게임 오브젝트들의 쉐이더를 재지정해준다.
	/// </summary>
	public static void ReAssignShader(GameObject _object)
	{
		foreach(var graphic in _object.GetComponentsInChildren<TMPro.TMP_Text>(true))
		{
			if(!graphic.fontMaterial)
			{
				continue;
			}

			var shader = graphic.fontMaterial.shader;
		
			shader = Shader.Find(shader.name);
			graphic.fontMaterial.shader = shader;
		}

		foreach(var renderer in _object.GetComponentsInChildren<Renderer>(true))
		{
			if(renderer.materials == null)
			{
				continue;
			}

			for(var i=0;i<renderer.materials.Length;i++)
			{
				var shader = renderer.materials[i].shader;
		
				shader = Shader.Find(shader.name);
				renderer.materials[i].shader = shader;
			}
		}
	}

	


	

	#region Skin Mesh Flip
	/// <summary>
	/// 면이 뒤집히는 현상 수정
	/// </summary>
	public static void SkinMeshFlip(GameObject _object)
	{
		var mesh = _object.GetComponent<SkinnedMeshRenderer>().sharedMesh;

		for(var i=0;i<mesh.vertices.Length;i++)
		{
			mesh.vertices[i] = new Vector3(mesh.vertices[i].x+100f,mesh.vertices[i].y,mesh.vertices[i].z);
		}

		var verticesList = new List<Vector3>();
		var normalList = new List<Vector3>();
		var triangleList = new List<int>();
		var uvList = new List<Vector2>();
		var tangentList = new List<Vector4>();
		var boneWeightList = new List<BoneWeight>();
		var bindposeList = new List<Matrix4x4>();

		for(var i=0;i<mesh.vertices.Length;i++)
		{
			verticesList.Add(mesh.vertices[i]);
			normalList.Add(mesh.normals[i]);
			uvList.Add(mesh.uv[i]);
			tangentList.Add(mesh.tangents[i]);
		}

		for(var i=mesh.triangles.Length-1;i>=0;i--)
		{
			triangleList.Add(mesh.triangles[i]);
		}

		for(var i=0;i<mesh.boneWeights.Length;i++)
		{
			boneWeightList.Add(mesh.boneWeights[i]);
		}

		for(var i=0;i<mesh.bindposes.Length;i++)
		{
			bindposeList.Add(mesh.bindposes[i]);
		}

		mesh.Clear();

		mesh.vertices = verticesList.ToArray();
		mesh.triangles = triangleList.ToArray();
		mesh.uv = uvList.ToArray();
		mesh.normals = normalList.ToArray();
		mesh.tangents = tangentList.ToArray();
		mesh.boneWeights = boneWeightList.ToArray();
		mesh.bindposes = bindposeList.ToArray();
	}
	#endregion

	public static void BlitPixelPerfect(RenderTexture _source,RenderTexture _destination,Camera _camera = null)
	{
		var height = 0.0f;

		if(_destination)
		{
			height = _destination.height;
		}
		else
		{
			height = Screen.height;

			if(!_camera)
			{
				_camera = Camera.current;
			}

			if(_camera)
			{
				height *= _camera.rect.height;
			}
		}

		var scale = height/_source.height;
		var isZero = scale.ApproximatelyZero();

		if(scale >= 2.0f && !isZero)
		{
			var render = RenderTexture.GetTemporary(_source.width*(int)scale,_source.height*(int)scale,_source.depth,_source.format);

			try
			{
				_source.filterMode = FilterMode.Point;
				render.filterMode = FilterMode.Point;

				Graphics.Blit(_source,render);

				render.filterMode = FilterMode.Bilinear;

				Graphics.Blit(render,_destination);
			}
			finally
			{
				RenderTexture.ReleaseTemporary(render);
			}
		}
		else
		{
			_source.filterMode = isZero ? FilterMode.Point : FilterMode.Bilinear;

			Graphics.Blit(_source,_destination);
		}
	}
}