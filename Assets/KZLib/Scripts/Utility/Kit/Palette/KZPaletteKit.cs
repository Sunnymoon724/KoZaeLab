using System;
using System.Collections.Generic;
using KZLib.Data;
using UnityEngine;

/// <summary>
/// Applies a palette (color array) to <see cref="Renderer"/> instances via <see cref="MaterialPropertyBlock"/>.
/// Writes the shader property <c>_PixelColorArray</c> without instantiating per-renderer materials.
/// </summary>
public static class KZPaletteKit
{
	/// <summary>Shader property name for the palette color array.</summary>
	private const string c_pixelColorArray = "_PixelColorArray";

	/// <summary>Required shader for palette rendering.</summary>
	private const string c_shaderName = "KZLib/ModifyPalette";

	private static MaterialPropertyBlock s_propertyBlock = null;

	/// <summary>Shared property block reused across all target renderers.</summary>
	private static MaterialPropertyBlock PropertyBlock => s_propertyBlock ??= new MaterialPropertyBlock();

	/// <summary>Resolves colors from <paramref name="colorPrt"/> and applies them.</summary>
	public static void SetPalette(Renderer renderer,IColorProto colorPrt)
	{
		SetPalette(renderer,ProtoManager.In.GetColorVectorArray(colorPrt));
	}

	/// <summary>Resolves colors by proto number and applies them.</summary>
	public static void SetPalette(Renderer renderer,int colorNum)
	{
		SetPalette(renderer,ProtoManager.In.GetColorVectorArray(colorNum));
	}

	/// <summary>
	/// Sets <c>_PixelColorArray</c> on <paramref name="renderer"/>.
	/// Throws when the renderer, colors, material count, or shader are invalid.
	/// </summary>
	public static void SetPalette(Renderer renderer,Vector4[] colorArray)
	{
		if(!renderer)
		{
			throw new ArgumentNullException(nameof(renderer));
		}

		if(colorArray.IsNullOrEmpty())
		{
			throw new InvalidOperationException("Color array is null or empty. Color data must be assigned.");
		}

		_ApplyPalette(renderer,colorArray);
	}

	/// <summary>Resolves colors from <paramref name="colorPrt"/> and applies them to every renderer.</summary>
	public static void SetPalette(IEnumerable<Renderer> rendererGroup,IColorProto colorPrt)
	{
		SetPalette(rendererGroup,ProtoManager.In.GetColorVectorArray(colorPrt));
	}

	/// <summary>Resolves colors by proto number and applies them to every renderer.</summary>
	public static void SetPalette(IEnumerable<Renderer> rendererGroup,int colorNum)
	{
		SetPalette(rendererGroup,ProtoManager.In.GetColorVectorArray(colorNum));
	}

	/// <summary>
	/// Sets <c>_PixelColorArray</c> on every entry in <paramref name="rendererGroup"/>.
	/// Throws when renderers, colors, material count, or shader are invalid.
	/// </summary>
	public static void SetPalette(IEnumerable<Renderer> rendererGroup,Vector4[] colorArray)
	{
		if(rendererGroup == null)
		{
			throw new ArgumentNullException(nameof(rendererGroup));
		}

		if(colorArray.IsNullOrEmpty())
		{
			throw new InvalidOperationException("Color array is null or empty. Color data must be assigned.");
		}

		var hasRenderer = false;

		foreach(var renderer in rendererGroup)
		{
			if(!renderer)
			{
				throw new NullReferenceException("Renderer does not exist. GameObject must be assigned.");
			}

			hasRenderer = true;

			_ApplyPalette(renderer,colorArray);
		}

		if(!hasRenderer)
		{
			throw new InvalidOperationException("Renderer group is empty. At least one renderer must be assigned.");
		}
	}

	private static void _ValidateRendererMaterial(Renderer renderer)
	{
		var materialArray = renderer.sharedMaterials;

		if(materialArray.Length != 1)
		{
			throw new InvalidOperationException($"{renderer.name} must have exactly one material. Material count is {materialArray.Length}.");
		}

		var material = materialArray[0];

		if(!material)
		{
			throw new InvalidOperationException($"{renderer.name} material is null. Material must be assigned.");
		}

		if(material.shader.name != c_shaderName)
		{
			throw new InvalidOperationException($"{renderer.name} must use {c_shaderName} shader. Current shader is {material.shader.name}.");
		}
	}

	private static void _ApplyPalette(Renderer renderer,Vector4[] colorArray)
	{
		_ValidateRendererMaterial(renderer);

		PropertyBlock.SetVectorArray(c_pixelColorArray,colorArray);
		renderer.SetPropertyBlock(PropertyBlock);
	}
}