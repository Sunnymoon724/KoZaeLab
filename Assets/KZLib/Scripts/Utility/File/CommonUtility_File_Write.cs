#if UNITY_EDITOR
using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

public static partial class CommonUtility
{
	private const int WAV_HEADER_SIZE = 44;

	/// <summary>
	/// Create folder. (path is file ? create parent folder. : create folder)
	/// </summary>
	/// <param name="_path">The absolute path of the file or folder.</param>
	public static void CreateFolder(string _path)
	{
		if(_path.IsEmpty())
		{
			throw new NullReferenceException("Path is null.");
		}

		// Path is file ? Get parent path. : Get path
		var fullPath = IsFilePath(_path) ? GetParentPath(_path) : _path;

		if(!IsFolderExist(fullPath))
		{
			Directory.CreateDirectory(fullPath);
		}
	}

	/// <param name="_filePath">The absolute path of the file.</param>
	public static void CreateFile(string _filePath)
	{
		if(!IsFileExist(_filePath))
		{
			File.Create(_filePath).Close();
		}
	}

	/// <param name="_filePath">The absolute path of the file.</param>
	public static void WriteByteToFile(string _filePath,byte[] _bytes)
	{
		CreateFolder(_filePath);

		File.WriteAllBytes(_filePath,_bytes);
	}

	/// <param name="_filePath">The absolute path of the file.</param>
	public static void WriteTextToFile(string _filePath,string _text)
	{
		CreateFolder(_filePath);

		File.WriteAllText(_filePath,_text);
	}

	/// <param name="_filePath">The absolute path of the file.</param>
	public static void WriteJsonToFile<TObject>(string _filePath,TObject _object)
	{
		WriteTextToFile(_filePath,JsonConvert.SerializeObject(_object));
	}

	/// <param name="_filePath">The absolute path of the file.</param>
	public static void WriteTextureToFile(string _filePath,Texture2D _texture)
	{
		WriteByteToFile(_filePath,_texture.EncodeToPNG());
	}

	/// <param name="_filePath">The absolute path of the file.</param>
	public static void WriteAudioClipToWav(string _filePath,AudioClip _clip)
	{
		using var stream = CreateEmpty(_filePath,WAV_HEADER_SIZE);

		ConvertAndWrite(stream,_clip);
		WriteHeader(stream,_clip);
	}

	private static FileStream CreateEmpty(string _filePath,int _size)
	{
		var stream = new FileStream(_filePath,FileMode.Create);
		var empty = new byte();

		for(var i=0;i<_size;i++)
		{
			stream.WriteByte(empty);
		}

		return stream;
	}

	private static void ConvertAndWrite(FileStream _stream,AudioClip _clip)
	{
		var sampleArray = new float[_clip.samples];

		_clip.GetData(sampleArray,0);

		var shortArray = new short[sampleArray.Length];
		var byteArray = new byte[sampleArray.Length*2];
		var factor = 32767; //to convert float to Int16

		for (int i=0;i<sampleArray.Length;i++)
		{
			shortArray[i] = (short) (sampleArray[i]*factor);
			var dataArray = BitConverter.GetBytes(shortArray[i]);
			dataArray.CopyTo(byteArray,i*2);
		}

		_stream.Write(byteArray,0,byteArray.Length);
	}

	private static void WriteHeader(FileStream _stream,AudioClip clip)
	{
		_stream.Seek(0,SeekOrigin.Begin);

		_stream.Write(Encoding.UTF8.GetBytes("RIFF"),0,4);
		_stream.Write(BitConverter.GetBytes(_stream.Length-8),0,4);

		_stream.Write(Encoding.UTF8.GetBytes("WAVE"),0,4);

		_stream.Write(Encoding.UTF8.GetBytes("fmt "),0,4);
		_stream.Write(BitConverter.GetBytes(16),0,4);
		_stream.Write(BitConverter.GetBytes(1),0,2);
		_stream.Write(BitConverter.GetBytes(clip.channels),0,2);
		_stream.Write(BitConverter.GetBytes(clip.frequency),0,4);
		_stream.Write(BitConverter.GetBytes(clip.frequency*clip.channels*2),0,4);
		_stream.Write(BitConverter.GetBytes((ushort)(clip.channels*2)),0,2);
		_stream.Write(BitConverter.GetBytes(16),0,2);

		_stream.Write(Encoding.UTF8.GetBytes("data"),0,4);
		_stream.Write(BitConverter.GetBytes(clip.samples*clip.channels*2),0,4);
	}

	/// <param name="_folderPath">The absolute path of the folder.</param>
	public static void AddOrUpdateTemplateText(string _folderPath,string _templateName,string _scriptName,string _newData,Func<string,string> _onUpdate)
	{
		var absolutePath = GetAbsolutePath(PathCombine(_folderPath,_scriptName),true);
		var templateText = IsFolderExist(absolutePath) ? ReadFileToText(absolutePath) : GetTemplateText(_templateName);

		if(templateText.IsEmpty() || templateText.Contains(_newData))
		{
			return;
		}

		templateText = _onUpdate(templateText);

		WriteTextToFile(absolutePath,templateText);

		AssetDatabase.Refresh();
	}

	/// <param name="_filePath">The absolute path of the file.</param>
	public static string GetTemplateText(string _filePath)
	{
		return ReadFileToText(GetTemplateFileAbsolutePath(_filePath));
	}

	/// <param name="_sourcePath">The absolute path of the file.</param>
	/// <param name="_destinationPath">The absolute path of the folder.</param>
	public static void CopyFile(string _sourcePath,string _destinationPath,bool _isOverride)
	{
		IsFileExist(_sourcePath,true);

		var fileName = GetFileName(_sourcePath);
		var destinationPath = PathCombine(_destinationPath,fileName);

		if(IsFileExist(destinationPath) && !_isOverride)
		{
			return;
		}

		WriteByteToFile(destinationPath,ReadFileToBytes(_sourcePath));
	}

	/// <param name="_sourcePath">The absolute path of the folder.</param>
	/// <param name="_destinationPath">The absolute path of the folder.</param>
	public static void CopyFolder(string _sourcePath,string _destinationPath,bool _isOverride)
	{
		IsFolderExist(_sourcePath,true);

		CreateFolder(_destinationPath);

		foreach(var filePath in GetFilePathArray(_sourcePath,"*.*"))
		{
			CopyFile(filePath,_destinationPath,_isOverride);
		}
	}
}
#endif