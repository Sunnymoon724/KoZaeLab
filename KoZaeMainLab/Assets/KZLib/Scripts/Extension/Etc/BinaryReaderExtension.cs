using System.IO;

public static class BinaryReaderExtension
{
	public static string ReadTextAddColon(this BinaryReader _reader,int _length)
	{
		var bytes = new byte[_length];
		_reader.Read(bytes,0,_length);

		return string.Join(":",bytes);
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