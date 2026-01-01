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

		for(var i=0;i<rendererArray.Length;i++)
		{
			var renderer = rendererArray[i];
			var materialArray = renderer.materials;

			for(var j=0;j<materialArray.Length;j++)
			{
				materialArray[j] = material;
			}

			renderer.materials = materialArray;
		}
	}

	private static bool _IsValid(Material material)
	{
		if(!material)
		{
			LogSvc.System.E("Material is null");

			return false;
		}

		return true;
	}
}