using UnityEngine;

/// <summary>
/// Extension methods for <see cref="Material"/> shader keywords and renderer assignment.
/// </summary>
public static class MaterialExtension
{
	/// <summary>
	/// Enables or disables a shader keyword and returns the material for chaining.
	/// </summary>
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

	/// <summary>
	/// Replaces every material slot on each renderer with this material.
	/// </summary>
	public static void SetMaterialToRendererArray(this Material material,params Renderer[] rendererArray)
	{
		if(!_IsValid(material))
		{
			return;
		}

		for(var i=0;i<rendererArray.Length;i++)
		{
			var renderer = rendererArray[i];

			if(!renderer)
			{
				continue;
			}

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
			LogChannel.Kit.E("Material is null. Material must be assigned.");

			return false;
		}

		return true;
	}
}