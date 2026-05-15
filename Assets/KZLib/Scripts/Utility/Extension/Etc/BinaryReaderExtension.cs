using System.IO;

public static class BinaryReaderExtension
{
	public static string ReadTextAddColon(this BinaryReader binaryReader,int _length)
	{
		var bytes = new byte[_length];
		binaryReader.Read(bytes,0,_length);

		return string.Join(":",bytes);
	}

	public static short ReadInt16(this BinaryReader binaryReader)
	{
		return (short)((binaryReader.ReadByte() << 8) | binaryReader.ReadByte());
	}

	public static int ReadInt32(this BinaryReader binaryReader)
	{
		return (binaryReader.ReadByte() << 24) | (binaryReader.ReadByte() << 16) | (binaryReader.ReadByte() << 8) | binaryReader.ReadByte();
	}

	public static int ReadVariableLength(this BinaryReader binaryReader)
	{
		var value = 0;
		int next;

		do
		{
			next = binaryReader.ReadByte();
			value <<= 7;
			value |= next & 0x7F;

		}while((next & 0x80) == 0x80);

		return value;
	}
}