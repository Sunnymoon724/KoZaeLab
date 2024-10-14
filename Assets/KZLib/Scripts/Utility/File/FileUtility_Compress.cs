#if UNITY_EDITOR
using System;
using System.IO;
using System.IO.Compression;

public static partial class FileUtility
{
	public static byte[] CompressBytes(byte[] _bytes)
	{
		using var memoryStream = new MemoryStream();

		using(var archive = new ZipArchive(memoryStream,ZipArchiveMode.Create,true))
		{
			var entry = archive.CreateEntry("data.bin",CompressionLevel.Optimal);

			using var entryStream = entry.Open();

			entryStream.Write(_bytes,0,_bytes.Length);
		}

		return memoryStream.ToArray();
	}

	public static byte[] CompressZip(string _sourcePath)
	{
		var absolutePath = GetAbsolutePath(_sourcePath,false);

		if(!IsExist(absolutePath,true))
		{
			return null;
		}

		if(IsFilePath(absolutePath))
		{
			using var memoryStream = new MemoryStream();

			using(var archive = new ZipArchive(memoryStream,ZipArchiveMode.Create,true))
			{
				var entry = archive.CreateEntry(Path.GetFileName(absolutePath),CompressionLevel.Optimal);

				using var entryStream = entry.Open();
				using var fileStream = File.OpenRead(absolutePath);

				fileStream.CopyTo(entryStream);
			}

			return memoryStream.ToArray();
		}
		else
		{
			using var memoryStream = new MemoryStream();

			using(var archive = new ZipArchive(memoryStream,ZipArchiveMode.Create,true))
			{
				foreach(var filePath in Directory.GetFiles(absolutePath,"*.*",SearchOption.AllDirectories))
				{
					var relativePath = Path.GetRelativePath(absolutePath,filePath);
					var entry = archive.CreateEntry(relativePath,CompressionLevel.Optimal);

					using var entryStream = entry.Open();
					using var fileStream = File.OpenRead(filePath);

					fileStream.CopyTo(entryStream);
				}
			}

			return memoryStream.ToArray();
		}
	}

	public static void CompressZip(string _sourcePath,string _destinationPath)
	{
		var bytes = CompressZip(_sourcePath);

		if(bytes == null)
		{
			return;
		}

		var extension = GetExtension(_destinationPath);

		if(extension.IsEmpty())
		{
			_destinationPath = string.Format("{0}.zip",_destinationPath);
		}
		else if(!extension.IsEqual(".zip"))
		{
			throw new ArgumentException($"Not supported extension. [{_destinationPath}]");
		}

		var destinationPath = GetUniquePath(GetAbsolutePath(_destinationPath,false));

		WriteByteToFile(destinationPath,bytes);
	}

	public static byte[] DecompressBytes(byte[] _bytes)
	{
		using var compressedStream = new MemoryStream(_bytes);
		using var archive = new ZipArchive(compressedStream,ZipArchiveMode.Read);

		var entry = archive.Entries[0];

		using var entryStream = entry.Open();
		using var memoryStream = new MemoryStream();

		entryStream.CopyTo(memoryStream);

		return memoryStream.ToArray();
	}

	public static void DecompressZip(string _sourcePath,string _destinationPath)
	{
		var extension = GetExtension(_sourcePath);

		if(!extension.IsEqual(".zip"))
		{
			throw new ArgumentException($"Not supported extension. [{_destinationPath}]");
		}

		var sourcePath = GetAbsolutePath(_sourcePath,false);

		if(!IsExist(sourcePath,true))
		{
			return;
		}

		var destinationPath = GetUniquePath(GetAbsolutePath(_destinationPath,false));

		CreateFolder(destinationPath);

		using var zipStream = new FileStream(destinationPath,FileMode.Open);
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