using System.Collections.Generic;
using System.IO;

public static class BinaryReaderExtension
{
	public static string ReadTextAddColon(this BinaryReader _reader,int _length)
	{
		var byteList = new List<byte>();

		for(var i=0;i<_length;i++)
		{
			byteList.Add(_reader.ReadByte());
		}

		return string.Join(":",byteList);
	}

	public static short ReadInt16(this BinaryReader _reader)
	{
		return (short)((_reader.ReadByte() << 8) | _reader.ReadByte());
	}

	public static int ReadInt32(this BinaryReader _reader)
	{
		return (_reader.ReadByte() << 24) | (_reader.ReadByte() << 16) | (_reader.ReadByte() << 8) | _reader.ReadByte();
	}

	public static int ReadVariableLength(this BinaryReader _reader)
	{
		var value = 0;
		int next;

		do
		{
			next = _reader.ReadByte();
			value <<= 7;
			value |= next & 0x7F;

		}while((next & 0x80) == 0x80);

		return value;
	}
}