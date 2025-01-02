using UnityEngine;

public static class MaterialExtension
{
	public static Material SetAlpha(this Material material,float alpha)
	{
		if(!IsValid(material))
		{
			return null;
		}

		material.color = material.color.MaskAlpha(alpha);

		return material;
	}

	public static Material Color(this Material material,Color color)
	{
		if(!IsValid(material))
		{
			return null;
		}

		material.color = color;

		return material;
	}

	public static Material Color(this Material material,int nameId,Color color)
	{
		if(!IsValid(material))
		{
			return null;
		}

		material.SetColor(nameId,color);

		return material;
	}

	public static Material Color(this Material material,string name,Color color)
	{
		if(!IsValid(material))
		{
			return null;
		}

		material.SetColor(name,color);

		return material;
	}

	public static Material Float(this Material material,int nameId,float _value)
	{
		if(!IsValid(material))
		{
			return null;
		}

		material.SetFloat(nameId,_value);

		return material;
	}

	public static Material Float(this Material material,string name,float _value)
	{
		if(!IsValid(material))
		{
			return null;
		}

		material.SetFloat(name,_value);

		return material;
	}

	public static Material Keyword(this Material material,string keyword,bool enable)
	{
		if(!IsValid(material))
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

	private static bool IsValid(Material material)
	{
		if(!material)
		{
			LogTag.System.E("Material is null");

			return false;
		}

		return true;
	}
}