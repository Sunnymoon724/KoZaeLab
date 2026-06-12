#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Partial editor utility for locating and loading work resource template files.
/// </summary>
public static partial class KZEditorKit
{
	/// <summary>
	/// Reads and returns the text contents of the template file at the given absolute path.
	/// </summary>
	public static string FindTemplateText(string absoluteFilePath)
	{
		if(!KZFileKit.IsPathExist(absoluteFilePath))
		{
			return null;
		}

		return KZFileKit.ReadFileToText(FindTemplateFileAbsolutePath(absoluteFilePath));
	}

	/// <summary>
	/// Resolves a template file path, searching the package WorkResources folder first, then Assets/KZLib/WorkResources.
	/// </summary>
	public static string FindTemplateFilePath(string fileName)
	{
		var packagePath = Path.Combine("Packages","com.bsheepstudio.kzlib","WorkResources","Templates",$"{fileName}");

		if(KZFileKit.IsFileExist(Path.Combine(Global.ProjectPath,packagePath)))
		{
			return packagePath;
		}

		var assetPath = Path.Combine("Assets","KZLib","WorkResources","Templates",$"{fileName}");

		if(KZFileKit.IsFileExist(Path.GetFullPath(assetPath)))
		{
			return assetPath;
		}

		throw new NullReferenceException($"{fileName} is not exist in template folder.");
	}

	/// <summary>
	/// Loads the Ostrich.png template sprite and returns its PNG-encoded texture bytes.
	/// </summary>
	public static byte[] FindTestImage()
	{
		var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(FindTemplateFilePath("Ostrich.png"));

		return sprite != null ? sprite.texture.EncodeToPNG() : null;
	}

	/// <summary>
	/// Returns the absolute filesystem path for a template file resolved by FindTemplateFilePath.
	/// </summary>
	public static string FindTemplateFileAbsolutePath(string fileName)
	{
		var templateFilePath = FindTemplateFilePath(fileName);

		return templateFilePath.StartsWith("Assets") ? Path.GetFullPath(templateFilePath) : Path.Combine(Global.ProjectPath,templateFilePath);
	}
}
#endif
