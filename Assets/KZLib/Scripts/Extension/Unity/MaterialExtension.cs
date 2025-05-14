using UnityEngine;

public static class MaterialExtension
{
	public static Material SetKeyword(this Material material,string keyword,bool enable)
	{
		if(!_IsValid(material))
		{
			return null;
		}

		if(enable)
		{
			material.EnableKeyword(keyword);
		}
		else
		{
			material.DisableKeyword(keyword);
		}

		return material;
	}

	public static void SetMaterialToRendererArray(this Material material,params Renderer[] rendererArray)
	{
		if(!_IsValid(material))
		{
			return;
		}

		foreach(var renderer in rendererArray)
		{
			var materialArray = renderer.materials;

			for(var i=0;i<materialArray.Length;i++ )
			{
				materialArray[i] = material;
			}

			renderer.materials = materialArray;
		}
	}

	private static bool _IsValid(Material material)
	{
		if(!material)
		{
			KZLogType.System.E("Material is null");

			return false;
		}

		return true;
	}
}