#if UNITY_EDITOR
using System.IO;

/// <summary>
/// Editor-only utility methods for resolving template files shipped with KZLib.
/// </summary>
public static partial class KZEditorKit
{
	/// <summary>
	/// Reads and returns the text contents of a template file resolved by file name.
	/// </summary>
	public static string ReadTemplateText(string fileName)
	{
		var absoluteFilePath = GetTemplateFileAbsolutePath(fileName);

		if(!KZFileKit.IsFileExist(absoluteFilePath))
		{
			return null;
		}

		return KZFileKit.ReadTextFromFile(absoluteFilePath);
	}

	/// <summary>
	/// Resolves a template file path in the UPM package first, then in the local Assets/KZLib copy.
	/// </summary>
	public static string ResolveTemplateFilePath(string fileName)
	{
		var packagePath = Path.Combine("Packages","com.bsheepstudio.kzlib","WorkResources","Template",$"{fileName}");

		if(KZFileKit.IsFileExist(Path.Combine(Global.ProjectPath,packagePath)))
		{
			return packagePath;
		}

		var assetPath = Path.Combine("Assets","KZLib","WorkResources","Template",$"{fileName}");

		if(KZFileKit.IsFileExist(Path.Combine(Global.ProjectPath,assetPath)))
		{
			return assetPath;
		}

		LogChannel.Kit.E($"{fileName} does not exist in template folder. FileName must be assigned.");

		return null;
	}

	/// <summary>
	/// Loads the built-in Ostrich.png template and returns its PNG byte data.
	/// </summary>
	public static byte[] ReadTemplateTestImage()
	{
		var absoluteFilePath = GetTemplateFileAbsolutePath("Ostrich.png");

		if(absoluteFilePath.IsEmpty() || !KZFileKit.IsFileExist(absoluteFilePath))
		{
			return null;
		}

		return KZFileKit.ReadBytesFromFile(absoluteFilePath);
	}

	/// <summary>
	/// Returns the absolute filesystem path for a template file resolved by file name.
	/// </summary>
	public static string GetTemplateFileAbsolutePath(string fileName)
	{
		var templateFilePath = ResolveTemplateFilePath(fileName);

		if(templateFilePath.IsEmpty())
		{
			return null;
		}

		return templateFilePath.StartsWith("Assets") ? Path.GetFullPath(templateFilePath) : Path.Combine(Global.ProjectPath,templateFilePath);
	}
}
#endif