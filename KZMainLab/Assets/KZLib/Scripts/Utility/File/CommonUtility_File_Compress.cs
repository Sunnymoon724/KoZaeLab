#if UNITY_EDITOR
using System.IO;
using System.IO.Compression;

public static partial class CommonUtility
{
	public static byte[] CompressBytes(byte[] _bytes)
	{
		if(_bytes == null || _bytes.Length == 0)
		{
			LogTag.System.E("Bytes is null or empty");

			return null;
		}

		using var memoryStream = new MemoryStream();

		using(var archive = new ZipArchive(memoryStream,ZipArchiveMode.Create,true))
		{
			var entry = archive.CreateEntry("data.bin",CompressionLevel.Optimal);

			using var entryStream = entry.Open();

			entryStream.Write(_bytes,0,_bytes.Length);
		}

		return memoryStream.ToArray();
	}

	/// <param name="_sourcePath">The absolute path of the file or folder.</param>
	public static byte[] CompressZip(string _sourcePath)
	{
		if(!IsPathExist(_sourcePath,true))
		{
			return null;
		}

		if(IsFilePath(_sourcePath))
		{
			return CompressBytes(ReadFileToBytes(_sourcePath));
		}
		else
		{
			using var memoryStream = new MemoryStream();

			using(var archive = new ZipArchive(memoryStream,ZipArchiveMode.Create,true))
			{
				foreach(var filePath in Directory.GetFiles(_sourcePath,"*.*",SearchOption.AllDirectories))
				{
					var relativePath = Path.GetRelativePath(_sourcePath,filePath);
					var entry = archive.CreateEntry(relativePath,CompressionLevel.Optimal);

					using var entryStream = entry.Open();
					using var fileStream = File.OpenRead(filePath);

					fileStream.CopyTo(entryStream);
				}
			}

			return memoryStream.ToArray();
		}
	}

	/// <param name="_sourcePath">The absolute path of the file or folder.</param>
	/// <param name="_destinationPath">The absolute path of the file or folder.</param>
	public static void CompressZip(string _sourcePath,string _destinationPath)
	{
		if(!IsPathExist(_destinationPath,true))
		{
			return;
		}

		var extension = GetExtension(_destinationPath);

		if(!extension.IsEqual(".zip"))
		{
			LogTag.System.E($"Not supported extension. [{_destinationPath}]");

			return;
		}

		if(extension.IsEmpty())
		{
			_destinationPath = $"{_destinationPath}.zip";
		}

		if(!IsPathExist(_sourcePath,true))
		{
			return;
		}

		var compress = CompressZip(_sourcePath);

		if(compress == null)
		{
			LogTag.System.E($"Compress is failed. {_sourcePath}");

			return;
		}

		//? destinationPath == unique file path
		var destinationPath = GetUniquePath(_destinationPath);

		WriteByteToFile(destinationPath,compress);
	}

	public static byte[] DecompressBytes(byte[] _bytes)
	{
		if(_bytes == null || _bytes.Length == 0)
		{
			LogTag.System.E("Bytes is null or empty");

			return null;
		}

		using var compressedStream = new MemoryStream(_bytes);
		using var archive = new ZipArchive(compressedStream,ZipArchiveMode.Read);

		var entry = archive.Entries[0];

		using var entryStream = entry.Open();
		using var memoryStream = new MemoryStream();

		entryStream.CopyTo(memoryStream);

		return memoryStream.ToArray();
	}

	/// <param name="_sourcePath">The absolute path of the file.</param>
	/// <param name="_destinationPath">The absolute path of the folder.</param>
	public static void DecompressZip(string _sourcePath,string _destinationPath)
	{
		if(!IsPathExist(_destinationPath,true))
		{
			return;
		}

		var destinationExtension = GetExtension(_destinationPath);

		if(!destinationExtension.IsEmpty())
		{
			LogTag.System.E($"{_destinationPath} is not folder path.");

			return;
		}

		if(!IsFileExist(_sourcePath,true))
		{
			return;
		}

		var sourceExtension = GetExtension(_sourcePath);

		if(!sourceExtension.IsEqual(".zip"))
		{
			LogTag.System.E($"{_sourcePath} is not zip file.");

			return;
		}

		//? destinationPath == unique folder path
		var destinationPath = GetUniquePath(_destinationPath);

		CreateFolder(destinationPath);

		using var zipStream = new FileStream(_sourcePath,FileMode.Open);
		using var archive = new ZipArchive(zipStream,ZipArchiveMode.Read);

		foreach(var entry in archive.Entries)
		{
			var fullPath = Path.Combine(destinationPath,entry.FullName);

			if(!entry.FullName.EndsWith("/"))
			{
				CreateFolder(fullPath);

				using var entryStream = entry.Open();
				using var fileStream = new FileStream(fullPath,FileMode.Create);

				entryStream.CopyTo(fileStream);
			}
		}
	}
}
#endif