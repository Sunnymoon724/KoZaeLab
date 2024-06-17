using System;
using System.Collections.Generic;
using System.IO;

public static partial class CommonUtility
{
	private static readonly string[] UNIT_ARRAY = new string[] { "B","KB","MB","GB","TB" };

	public static short ReadInt16(BinaryReader _reader)
	{
		return (short)((_reader.ReadByte() << 8) | _reader.ReadByte());
	}

	public static int ReadInt32(BinaryReader _reader)
	{
		return (_reader.ReadByte() << 24) | (_reader.ReadByte() << 16) | (_reader.ReadByte() << 8) | _reader.ReadByte();
	}

	public static string ReadTextAddColon(BinaryReader _reader,int _length)
	{
		var byteList = new List<byte>();

		for(var i=0;i<_length;i++)
		{
			byteList.Add(_reader.ReadByte());
		}

		return string.Join(":",byteList);
	}

	public static long ByteScaleUpUnit(long _data)
	{
		return _data/1024L;
	}

	public static double ByteScaleUpUnitToDouble(long _data)
	{
		return _data/1024.0;
	}

	public static string ByteToString(long _byte)
	{
		ByteToString(_byte,out var size,out var unit);

		return string.Format("{0:N2} {1}",size,unit);
	}

	public static void ByteToString(long _byte,out double _size,out string _unit)
	{
		var index = 0;
		_size = _byte;

		while(_size >= 1024.0)
		{
			_size /= 1024.0;
			index++;
		}

		_unit = UNIT_ARRAY[index];
	}

	public static byte[] ByteCombine(byte[] _dataA,byte[] _dataB)
	{
		var data = new byte[_dataA.Length+_dataB.Length];

		Buffer.BlockCopy(_dataA,0,data,0,_dataA.Length);
		Buffer.BlockCopy(_dataB,0,data,_dataA.Length,_dataB.Length);

		return data;
	}
	
	public static string GetDownloadSpeed(long _tick)
	{
		var size = _tick/1024.0d;

		return size > 0.0d ? string.Format("{0:F2} MB/s",size/1024.0d) : string.Format("{0} B/s",_tick);
	}

	public static byte ConvertBoolArrayToByte(bool[] _sourceArray)
	{
		var result = (byte) 0x00;
		var index = 8-_sourceArray.Length;
		
		for(var i=0;i<_sourceArray.Length;i++)
		{
			if(_sourceArray[i])
			{
				result |= (byte) (1<<(7-index));
			}

			index++;
		}

		return result;
	}
	
	public static bool[] ConvertByteToBoolArray(byte _byte)
	{
		var resultArray = new bool[8];

		for(var i=0;i<8;i++)
		{
			resultArray[i] = (_byte&(1<<i)) != 0;
		}

		for(var i=0;i<resultArray.Length/2;i++)
		{
			(resultArray[resultArray.Length-i-1],resultArray[i]) = (resultArray[i],resultArray[resultArray.Length-i-1]);
		}

		return resultArray;
	}
}