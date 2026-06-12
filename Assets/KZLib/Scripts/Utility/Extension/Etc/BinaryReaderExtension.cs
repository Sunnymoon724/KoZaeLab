using System.IO;

/// <summary>
/// Extension methods for BinaryReader.
/// Provides big-endian integer reads, variable-length decoding, and byte-to-text helpers.
/// </summary>
public static class BinaryReaderExtension
{
	/// <summary>
	/// Reads the specified number of bytes and joins them as colon-separated decimal values.
	/// </summary>
	/// <param name="binaryReader">The reader to read from.</param>
	/// <param name="_length">Number of bytes to read.</param>
	/// <returns>A colon-separated string, or empty if the reader is null.</returns>
	public static string ReadTextAddColon(this BinaryReader binaryReader,int _length)
	{
		if(!_IsValid(binaryReader))
		{
			return string.Empty;
		}

		var bytes = new byte[_length];
		var read = binaryReader.Read(bytes,0,_length);

		if(read != _length)
		{
			LogChannel.Kit.W($"Expected {_length} bytes but read {read}.");
		}

		return string.Join(":",bytes);
	}

	/// <summary>
	/// Reads a 16-bit signed integer in big-endian byte order.
	/// </summary>
	public static short ReadInt16(this BinaryReader binaryReader)
	{
		if(!_IsValid(binaryReader))
		{
			return 0;
		}

		return (short)((binaryReader.ReadByte() << 8) | binaryReader.ReadByte());
	}

	/// <summary>
	/// Reads a 32-bit signed integer in big-endian byte order.
	/// </summary>
	public static int ReadInt32(this BinaryReader binaryReader)
	{
		if(!_IsValid(binaryReader))
		{
			return 0;
		}

		return (binaryReader.ReadByte() << 24) | (binaryReader.ReadByte() << 16) | (binaryReader.ReadByte() << 8) | binaryReader.ReadByte();
	}

	/// <summary>
	/// Reads a variable-length integer using 7-bit groups with a continuation high bit.
	/// </summary>
	/// <returns>The decoded value, or 0 if the reader is null or the stream ends unexpectedly.</returns>
	public static int ReadVariableLength(this BinaryReader binaryReader)
	{
		if(!_IsValid(binaryReader))
		{
			return 0;
		}

		var value = 0;
		int next;

		try
		{
			do
			{
				next = binaryReader.ReadByte();
				value <<= 7;
				value |= next & 0x7F;

			}while((next & 0x80) == 0x80);
		}
		catch(EndOfStreamException)
		{
			LogChannel.Kit.E("Unexpected end of stream while reading variable length.");
		}

		return value;
	}

	private static bool _IsValid(BinaryReader binaryReader)
	{
		if(binaryReader == null)
		{
			LogChannel.Kit.E("BinaryReader is null.");

			return false;
		}

		return true;
	}
}
