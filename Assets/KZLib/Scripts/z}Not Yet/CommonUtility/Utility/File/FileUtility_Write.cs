#if UNITY_EDITOR
using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;

public static partial class FileUtility
{
	private const int WAV_HEADER_SIZE = 44;
	
	/// <summary>
	/// 파일 쓰기
	/// </summary>
	public static void WriteDataToFile(string _filePath,string _text)
	{
		CreateFolder(_filePath);

		using var writer = new StreamWriter(_filePath);
		writer.Write(_text);
	}

	/// <summary>
	/// 파일 쓰기
	/// </summary>
	public static void WriteJsonFile<TObject>(string _filePath,TObject _object)
	{
		WriteDataToFile(_filePath,JsonConvert.SerializeObject(_object));
	}

	/// <summary>
	/// byte[] 저장
	/// </summary>
	public static void WriteFile(string _filePath,Texture2D _texture)
	{
		WriteFile(_filePath,_texture.EncodeToPNG());
	}

	/// <summary>
	/// byte[] 저장
	/// </summary>
	public static void WriteFile(string _filePath,byte[] _dataArray)
	{
		CreateFolder(_filePath);

		File.WriteAllBytes(_filePath,_dataArray);
	}

	public static void WriteAudioClipToWAV(string _filePath,AudioClip _clip)
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
		_stream.Write(System.BitConverter.GetBytes(_stream.Length-8),0,4);

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
}
#endif