using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

public static partial class CommonUtility
{	
	public static TObject CopyObject<TObject>(TObject _object,Transform _parent = null) where TObject : Object
	{
		var data = Object.Instantiate(_object,_parent);
		data.name = _object.name;

		return data;
	}

	public static void DestroyObject(Object _object)
	{
		if(!_object)
		{
			return;
		}

		if(Application.isEditor)
		{
			Object.DestroyImmediate(_object);
		}
		else
		{
			Object.Destroy(_object);
		}
	}

	public static byte[] GetBytesFromObject(Object _object)
	{
		if(!_object)
		{
			return null;
		}
		
		var binary = new BinaryFormatter();
		var stream = new MemoryStream();

		binary.Serialize(stream,_object);
		
		return stream.ToArray();
	}

	public static bool IsExistMeshFilter(GameObject _object)
	{
		var filterArray = _object.GetComponentsInChildren<MeshFilter>(true);

		for(var i=0;i<filterArray.Length;i++)
		{
			if(!filterArray[i].sharedMesh)
			{
				return false;
			}
		}

		return true;
	}

#if UNITY_EDITOR
	public static List<TComponent> GetAllComponentsInInUnity<TComponent>() where TComponent : Component
	{
		var componentList = GetAllComponentsInActiveScene<TComponent>();

		foreach(var prefab in LoadAssetGroup<GameObject>("t:prefab"))
		{
			componentList.AddRange(prefab.GetComponentsInChildren<TComponent>(true));
		}

		return componentList;
	}

	public static List<TComponent> GetAllComponentsInActiveScene<TComponent>() where TComponent : Component
	{
		var currScene = SceneManager.GetActiveScene();
		var componentList = new List<TComponent>();

		foreach(var data in currScene.GetRootGameObjects())
		{
			componentList.AddRange(data.GetComponentsInChildren<TComponent>(true));
		}

		return componentList;
	}
#endif

	//? 액션툴 만들때 쓸것
	// public static void ShowDebugColliderSphere(float _radius,Vector3 _position)
	// {
	// 	var diameter = _radius*2.0f;
	// 	var data = InstantResourceObj<GameObject>("common/DebugHitSphere");

	// 	data.transform.position = _position;
	// 	data.transform.localScale = Vector3.one*diameter;

	// 	GameObject.Destroy(obj, 1.0f);
	// }

	// static public void ShowDebugColliderCube(Vector3 vHalfExtents, Quaternion qRot, Vector3 vPos)
	// {
	// 	GameObject obj = InstantResourceObj<GameObject>("common/DebugHitCube");
	// 	obj.transform.position = vPos;
	// 	obj.transform.rotation = qRot;
	// 	obj.transform.localScale = vHalfExtents;

	// 	GameObject.Destroy(obj, 1.0f);
	// }
}