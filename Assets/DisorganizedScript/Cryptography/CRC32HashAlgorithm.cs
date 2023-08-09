// using System;
// using System.IO;
// using System.Security.Cryptography;

// public class CRC32HashAlgorithm : HashAlgorithm
// {
// 	private const UInt32 DEFAULT_POLYNOMIAL = 0xedb88320;
// 	private const UInt32 DEFAULT_SEED = 0xffffffff;

// 	private UInt32 m_Hash;
// 	private UInt32 m_Seed;
// 	private UInt32[] m_Table;
// 	private static UInt32[] s_DefaultTable;

// 	public CRC32HashAlgorithm()
// 	{
// 		m_Table = InitializeTable(DEFAULT_POLYNOMIAL);
// 		m_Seed = DEFAULT_SEED;

// 		Initialize();
// 	}

// 	public CRC32HashAlgorithm(UInt32 _polynomial,UInt32 _seed)
// 	{
// 		m_Table = InitializeTable(_polynomial);
// 		m_Seed = _seed;

// 		Initialize();
// 	}

// 	public override void Initialize()
// 	{
// 		m_Hash = m_Seed;
// 	}

// 	protected override void HashCore(byte[] _buffer,int _start,int _length)
// 	{
// 		m_Hash = CalculateHash(m_Table,m_Hash,_buffer,_start,_length);
// 	}

// 	protected override byte[] HashFinal()
// 	{
// 		HashValue = UInt32ToBigEndianBytes(~m_Hash);

// 		return HashValue;
// 	}

// 	public override int HashSize => 32;

// 	public static string Compute(FileStream _stream)
// 	{
// 		var buffer = new byte[_stream.Length];

// 		_stream.Read(buffer,0,(int)_stream.Length);
// 		_stream.Close();

// 		return Convert.ToString(Compute(buffer));
// 	}

// 	public static UInt32 Compute(byte[] _buffer)
// 	{
// 		return ~CalculateHash(InitializeTable(DEFAULT_POLYNOMIAL),DEFAULT_SEED,_buffer,0,_buffer.Length);
// 	}

// 	public static UInt32 Compute(UInt32 _seed,byte[] _buffer)
// 	{
// 		return ~CalculateHash(InitializeTable(DEFAULT_POLYNOMIAL),_seed,_buffer,0,_buffer.Length);
// 	}

// 	public static UInt32 Compute(UInt32 _polynomial,UInt32 _seed,byte[] _buffer)
// 	{
// 		return ~CalculateHash(InitializeTable(_polynomial),_seed,_buffer,0,_buffer.Length);
// 	}

// 	private static UInt32[] InitializeTable(UInt32 _polynomial)
// 	{
// 		if (_polynomial == DEFAULT_POLYNOMIAL && s_DefaultTable != null)
// 		{
// 			return s_DefaultTable;
// 		}

// 		var table = new UInt32[256];

// 		for(var i=0;i<256;i++)
// 		{
// 			var entry = (UInt32)i;

// 			for(var j=0;j<8;j++)
// 			{
// 				entry = ((entry&1) == 1) ? (entry>>1)^_polynomial :  entry >> 1;
// 			}
				
// 			table[i] = entry;
// 		}

// 		if (_polynomial == DEFAULT_POLYNOMIAL)
// 		{
// 			s_DefaultTable = table;
// 		}

// 		return table;
// 	}

// 	private static UInt32 CalculateHash(UInt32[] _table,UInt32 _seed,byte[] _buffer,int _start,int _size)
// 	{
// 		var crc = _seed;

// 		for (int i=_start;i<_size;i++)
// 		{
// 			unchecked
// 			{
// 				crc = (crc>>8)^_table[_buffer[i]^crc & 0xff];
// 			}
// 		}
			
// 		return crc;
// 	}

// 	private byte[] UInt32ToBigEndianBytes(UInt32 _num)
// 	{
// 		return new[]
// 		{
// 			(byte)((_num >> 24)	& 0xff),
// 			(byte)((_num >> 16)	& 0xff),
// 			(byte)((_num >> 8 )	& 0xff),
// 			(byte)( _num 		& 0xff),
// 		};
// 	}
// }