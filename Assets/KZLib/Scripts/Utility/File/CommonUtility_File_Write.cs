#if UNITY_EDITOR
using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

public static partial class CommonUtility
{
	private const int c_wav_header_size = 44;

	/// <summary>
	/// Create folder. (path is file ? create parent folder. : create folder)
	/// </summary>
	/// <param name="path">The absolute path of the file or folder.</param>
	public static void CreateFolder(string path)
	{
		if(!IsPathExist(path,true))
		{
			return;
		}

		// Path is file ? Get parent path. : Get path
		var fullPath = IsFilePath(path) ? GetParentPath(path) : path;

		if(!IsFolderExist(fullPath))
		{
			Directory.CreateDirectory(fullPath);
		}
	}

	/// <param name="filePath">The absolute path of the file.</param>
	public static void CreateFile(string filePath)
	{
		if(!IsPathExist(filePath,true))
		{
			return;
		}

		if(!IsFileExist(filePath))
		{
			File.Create(filePath).Close();
		}
	}

	/// <param name="filePath">The absolute path of the file.</param>
	public static void WriteByteToFile(string filePath,byte[] _bytes)
	{
		if(!IsPathExist(filePath,true))
		{
			return;
		}

		CreateFolder(filePath);

		File.WriteAllBytes(filePath,_bytes);
	}

	/// <param name="filePath">The absolute path of the file.</param>
	public static void WriteTextToFile(string filePath,string _text)
	{
		if(!IsPathExist(filePath,true))
		{
			return;
		}

		CreateFolder(filePath);

		File.WriteAllText(filePath,_text);
	}

	/// <param name="filePath">The absolute path of the file.</param>
	public static void WriteJsonToFile<TObject>(string filePath,TObject _object)
	{
		if(!IsPathExist(filePath,true))
		{
			return;
		}

		WriteTextToFile(filePath,JsonConvert.SerializeObject(_object));
	}

	/// <param name="filePath">The absolute path of the file.</param>
	public static void WriteTextureToFile(string filePath,Texture2D _texture)
	{
		if(!IsPathExist(filePath,true))
		{
			return;
		}

		WriteByteToFile(filePath,_texture.EncodeToPNG());
	}

	/// <param name="filePath">The absolute path of the file.</param>
	public static void WriteAudioClipToWav(string filePath,AudioClip _clip)
	{
		if(!IsPathExist(filePath,true))
		{
			return;
		}

		using var stream = CreateEmpty(filePath,c_wav_header_size);

		ConvertAndWrite(stream,_clip);
		WriteHeader(stream,_clip);
	}

	private static FileStream CreateEmpty(string filePath,int size)
	{
		var stream = new FileStream(filePath,FileMode.Create);
		var empty = new byte();

		for(var i=0;i<size;i++)
		{
			stream.WriteByte(empty);
		}

		return stream;
	}

	private static void ConvertAndWrite(FileStream fileStream,AudioClip audioClip)
	{
		var sampleArray = new float[audioClip.samples];

		audioClip.GetData(sampleArray,0);

		var shortArray = new short[sampleArray.Length];
		var byteArray = new byte[sampleArray.Length*2];
		var factor = 32767; //to convert float to Int16

		for (int i=0;i<sampleArray.Length;i++)
		{
			shortArray[i] = (short) (sampleArray[i]*factor);
			var dataArray = BitConverter.GetBytes(shortArray[i]);
			dataArray.CopyTo(byteArray,i*2);
		}

		fileStream.Write(byteArray,0,byteArray.Length);
	}

	private static void WriteHeader(FileStream fileStream,AudioClip audioClip)
	{
		fileStream.Seek(0,SeekOrigin.Begin);

		fileStream.Write(Encoding.UTF8.GetBytes("RIFF"),0,4);
		fileStream.Write(BitConverter.GetBytes(fileStream.Length-8),0,4);

		fileStream.Write(Encoding.UTF8.GetBytes("WAVE"),0,4);

		fileStream.Write(Encoding.UTF8.GetBytes("fmt "),0,4);
		fileStream.Write(BitConverter.GetBytes(16),0,4);
		fileStream.Write(BitConverter.GetBytes(1),0,2);
		fileStream.Write(BitConverter.GetBytes(audioClip.channels),0,2);
		fileStream.Write(BitConverter.GetBytes(audioClip.frequency),0,4);
		fileStream.Write(BitConverter.GetBytes(audioClip.frequency*audioClip.channels*2),0,4);
		fileStream.Write(BitConverter.GetBytes((ushort)(audioClip.channels*2)),0,2);
		fileStream.Write(BitConverter.GetBytes(16),0,2);

		fileStream.Write(Encoding.UTF8.GetBytes("data"),0,4);
		fileStream.Write(BitConverter.GetBytes(audioClip.samples*audioClip.channels*2),0,4);
	}

	/// <param name="folderPath">The absolute path of the folder.</param>
	public static void AddOrUpdateTemplateText(string folderPath,string templateName,string scriptName,string newData,Func<string,string> onUpdate)
	{
		var absolutePath = GetAbsolutePath(PathCombine(folderPath,scriptName),true);
		var templateText = IsFolderExist(absolutePath) ? ReadFileToText(absolutePath) : FindTemplateText(templateName);

		if(templateText.IsEmpty() || templateText.Contains(newData))
		{
			return;
		}

		templateText = onUpdate(templateText);

		WriteTextToFile(absolutePath,templateText);

		AssetDatabase.Refresh();
	}

	/// <param name="_filePath">The absolute path of the file.</param>
	public static string FindTemplateText(string filePath)
	{
		if(!IsPathExist(filePath,true))
		{
			return null;
		}

		return ReadFileToText(FindTemplateFileAbsolutePath(filePath));
	}

	/// <param name="sourcePath">The absolute path of the file.</param>
	/// <param name="_destinationPath">The absolute path of the folder.</param>
	public static void CopyFile(string sourceFilePath,string destinationFolderPath,bool isOverride)
	{
		if(!IsFileExist(sourceFilePath,true))
		{
			return;
		}

		var fileName = GetFileName(sourceFilePath);
		var destinationFilePath = PathCombine(destinationFolderPath,fileName);

		if(IsFileExist(destinationFilePath) && !isOverride)
		{
			return;
		}

		WriteByteToFile(destinationFilePath,ReadFileToBytes(sourceFilePath));
	}

	/// <param name="sourcePath">The absolute path of the folder.</param>
	/// <param name="_destinationPath">The absolute path of the folder.</param>
	public static void CopyFolder(string sourceFolderPath,string destinationFolderPath,bool isOverride)
	{
		if(!IsFolderExist(sourceFolderPath,true))
		{
			return;
		}

		CreateFolder(destinationFolderPath);

		foreach(var filePath in GetFilePathArray(sourceFolderPath,"*.*"))
		{
			CopyFile(filePath,destinationFolderPath,isOverride);
		}
	}
}
#endif