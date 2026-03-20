#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public static partial class KZEditorKit
{
	public static string FindTemplateText(string absoluteFilePath)
	{
		if(!KZFileKit.IsPathExist(absoluteFilePath))
		{
			return null;
		}

		return KZFileKit.ReadFileToText(FindTemplateFileAbsolutePath(absoluteFilePath));
	}

	public static string FindTemplateFilePath(string fileName)
	{
		var packagePath = Path.Combine("Packages","com.bsheepstudio.kzlib","WorkResources","Templates",$"{fileName}");

		if(KZFileKit.IsFileExist(Path.Combine(Global.PROJECT_PATH,packagePath)))
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

	public static byte[] FindTestImage()
	{
		var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(FindTemplateFilePath("Ostrich.png"));

		return sprite != null ? sprite.texture.EncodeToPNG() : null;
	}

	public static string FindTemplateFileAbsolutePath(string fileName)
	{
		var templateFilePath = FindTemplateFilePath(fileName);

		return templateFilePath.StartsWith("Assets") ? Path.GetFullPath(templateFilePath) : Path.Combine(Global.PROJECT_PATH,templateFilePath);
	}
}
#endif