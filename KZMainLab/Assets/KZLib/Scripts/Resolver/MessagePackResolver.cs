using UnityEngine;
using System;
using MessagePack;
using MessagePack.Formatters;
using MessagePack.Resolvers;

namespace KZLib.KZResolver
{
	/// <summary>
	/// example
	/// <br/> MessagePackSerializer.Serialize(data,MessagePackSerializerOptions.Standard.WithResolver(MessagePackResolver.In));
	/// <br/> MessagePackSerializer.Deserialize(data,MessagePackSerializerOptions.Standard.WithResolver(MessagePackResolver.In));
	/// <br/> support Color, Color32, Vector2, Vector2Int, Vector3, Vector3Int, Vector4, Quaternion, Rect, RectInt
	/// </summary>
	public class MessagePackResolver : IFormatterResolver
	{
		public static readonly IFormatterResolver In = new MessagePackResolver();

		public IMessagePackFormatter<T> GetFormatter<T>()
		{
			return typeof(T) switch
			{
				var type when type == typeof(Color)			=> new ColorFormatter()			as IMessagePackFormatter<T>,
				var type when type == typeof(Color32)		=> new Color32Formatter()		as IMessagePackFormatter<T>,

				var type when type == typeof(Vector2)		=> new Vector2Formatter()		as IMessagePackFormatter<T>,
				var type when type == typeof(Vector2Int)	=> new Vector2IntFormatter()	as IMessagePackFormatter<T>,
				var type when type == typeof(Vector3)		=> new Vector3Formatter()		as IMessagePackFormatter<T>,
				var type when type == typeof(Vector3Int)	=> new Vector3IntFormatter()	as IMessagePackFormatter<T>,
				var type when type == typeof(Vector4)		=> new Vector4Formatter()		as IMessagePackFormatter<T>,

				var type when type == typeof(Quaternion)	=> new QuaternionFormatter()	as IMessagePackFormatter<T>,

				var type when type == typeof(Rect)			=> new RectFormatter()			as IMessagePackFormatter<T>,
				var type when type == typeof(RectInt)		=> new RectIntFormatter()		as IMessagePackFormatter<T>,
				_ => StandardResolver.Instance.GetFormatter<T>()
			};
		}

		#region Color
		private class ColorFormatter : IMessagePackFormatter<Color>
		{
			public void Serialize(ref MessagePackWriter _writer,Color _color,MessagePackSerializerOptions _options)
			{
				_writer.WriteArrayHeader(4);

				_writer.Write(_color.r);
				_writer.Write(_color.g);
				_writer.Write(_color.b);
				_writer.Write(_color.a);
			}

			public Color Deserialize(ref MessagePackReader _reader,MessagePackSerializerOptions _options)
			{
				if(!_reader.TryReadArrayHeader(out int length) || length != 4)
				{
					throw new InvalidOperationException($"reader error. or {length} != 4");
				}

				return new Color(_reader.ReadSingle(),_reader.ReadSingle(),_reader.ReadSingle(),_reader.ReadSingle());
			}
		}
		#endregion Color

		#region Color32
		private class Color32Formatter : IMessagePackFormatter<Color32>
		{
			public void Serialize(ref MessagePackWriter _writer,Color32 _color,MessagePackSerializerOptions _options)
			{
				_writer.WriteArrayHeader(4);

				_writer.Write(_color.r);
				_writer.Write(_color.g);
				_writer.Write(_color.b);
				_writer.Write(_color.a);
			}

			public Color32 Deserialize(ref MessagePackReader _reader,MessagePackSerializerOptions _options)
			{
				if(!_reader.TryReadArrayHeader(out int length) || length != 4)
				{
					throw new InvalidOperationException($"reader error. or {length} != 4");
				}

				return new Color32(_reader.ReadByte(),_reader.ReadByte(),_reader.ReadByte(),_reader.ReadByte());
			}
		}
		#endregion Color32

		#region Vector2
		private class Vector2Formatter : IMessagePackFormatter<Vector2>
		{
			public void Serialize(ref MessagePackWriter _writer,Vector2 _vector,MessagePackSerializerOptions _options)
			{
				_writer.WriteArrayHeader(2);

				_writer.Write(_vector.x);
				_writer.Write(_vector.y);
			}

			public Vector2 Deserialize(ref MessagePackReader _reader,MessagePackSerializerOptions _options)
			{
				if(!_reader.TryReadArrayHeader(out int length) || length != 2)
				{
					throw new InvalidOperationException($"reader error. or {length} != 2");
				}

				return new Vector2(_reader.ReadSingle(),_reader.ReadSingle());
			}
		}
		#endregion Vector2

		#region Vector2Int
		private class Vector2IntFormatter : IMessagePackFormatter<Vector2Int>
		{
			public void Serialize(ref MessagePackWriter _writer,Vector2Int _vector,MessagePackSerializerOptions _options)
			{
				_writer.WriteArrayHeader(2);

				_writer.Write(_vector.x);
				_writer.Write(_vector.y);
			}

			public Vector2Int Deserialize(ref MessagePackReader _reader,MessagePackSerializerOptions _options)
			{
				if(!_reader.TryReadArrayHeader(out int length) || length != 2)
				{
					throw new InvalidOperationException($"reader error. or {length} != 2");
				}

				return new Vector2Int(_reader.ReadInt32(),_reader.ReadInt32());
			}
		}
		#endregion Vector2Int

		#region Vector3
		private class Vector3Formatter : IMessagePackFormatter<Vector3>
		{
			public void Serialize(ref MessagePackWriter _writer,Vector3 _vector,MessagePackSerializerOptions _options)
			{
				_writer.WriteArrayHeader(3);

				_writer.Write(_vector.x);
				_writer.Write(_vector.y);
				_writer.Write(_vector.z);
			}

			public Vector3 Deserialize(ref MessagePackReader _reader,MessagePackSerializerOptions _options)
			{
				if(!_reader.TryReadArrayHeader(out int length) || length != 3)
				{
					throw new InvalidOperationException($"reader error. or {length} != 3");
				}

				return new Vector3(_reader.ReadSingle(),_reader.ReadSingle(),_reader.ReadSingle());
			}
		}
		#endregion Vector3

		#region Vector3Int
		private class Vector3IntFormatter : IMessagePackFormatter<Vector3Int>
		{
			public void Serialize(ref MessagePackWriter _writer,Vector3Int _vector,MessagePackSerializerOptions _options)
			{
				_writer.WriteArrayHeader(3);

				_writer.Write(_vector.x);
				_writer.Write(_vector.y);
				_writer.Write(_vector.z);
			}

			public Vector3Int Deserialize(ref MessagePackReader _reader,MessagePackSerializerOptions _options)
			{
				if(!_reader.TryReadArrayHeader(out int length) || length != 3)
				{
					throw new InvalidOperationException($"reader error. or {length} != 3");
				}

				return new Vector3Int(_reader.ReadInt32(),_reader.ReadInt32(),_reader.ReadInt32());
			}
		}
		#endregion Vector3Int

		#region Vector4
		private class Vector4Formatter : IMessagePackFormatter<Vector4>
		{
			public void Serialize(ref MessagePackWriter _writer,Vector4 _vector,MessagePackSerializerOptions _options)
			{
				_writer.WriteArrayHeader(4);

				_writer.Write(_vector.x);
				_writer.Write(_vector.y);
				_writer.Write(_vector.z);
				_writer.Write(_vector.w);
			}

			public Vector4 Deserialize(ref MessagePackReader _reader,MessagePackSerializerOptions _options)
			{
				if(!_reader.TryReadArrayHeader(out int length) || length != 4)
				{
					throw new InvalidOperationException($"reader error. or {length} != 4");
				}

				return new Vector4(_reader.ReadSingle(),_reader.ReadSingle(),_reader.ReadSingle(),_reader.ReadSingle());
			}
		}
		#endregion Vector4

		#region Quaternion
		private class QuaternionFormatter : IMessagePackFormatter<Quaternion>
		{
			public void Serialize(ref MessagePackWriter _writer,Quaternion _quaternion,MessagePackSerializerOptions _options)
			{
				_writer.WriteArrayHeader(4);

				_writer.Write(_quaternion.x);
				_writer.Write(_quaternion.y);
				_writer.Write(_quaternion.z);
				_writer.Write(_quaternion.w);
			}

			public Quaternion Deserialize(ref MessagePackReader _reader,MessagePackSerializerOptions _options)
			{
				if(!_reader.TryReadArrayHeader(out int length) || length != 4)
				{
					throw new InvalidOperationException($"reader error. or {length} != 4");
				}

				return new Quaternion(_reader.ReadSingle(),_reader.ReadSingle(),_reader.ReadSingle(),_reader.ReadSingle());
			}
		}
		#endregion Quaternion

		#region Rect
		private class RectFormatter : IMessagePackFormatter<Rect>
		{
			public void Serialize(ref MessagePackWriter _writer,Rect _rect,MessagePackSerializerOptions _options)
			{
				_writer.WriteArrayHeader(4);

				_writer.Write(_rect.x);
				_writer.Write(_rect.y);
				_writer.Write(_rect.width);
				_writer.Write(_rect.height);
			}

			public Rect Deserialize(ref MessagePackReader _reader,MessagePackSerializerOptions _options)
			{
				if(!_reader.TryReadArrayHeader(out int length) || length != 4)
				{
					throw new InvalidOperationException($"reader error. or {length} != 4");
				}

				return new Rect(_reader.ReadSingle(),_reader.ReadSingle(),_reader.ReadSingle(),_reader.ReadSingle());
			}
		}
		#endregion Rect

		#region RectInt
		private class RectIntFormatter : IMessagePackFormatter<RectInt>
		{
			public void Serialize(ref MessagePackWriter _writer,RectInt _rect,MessagePackSerializerOptions _options)
			{
				_writer.WriteArrayHeader(4);

				_writer.Write(_rect.x);
				_writer.Write(_rect.y);
				_writer.Write(_rect.width);
				_writer.Write(_rect.height);
			}

			public RectInt Deserialize(ref MessagePackReader _reader,MessagePackSerializerOptions _options)
			{
				if(!_reader.TryReadArrayHeader(out int length) || length != 4)
				{
					throw new InvalidOperationException($"reader error. or {length} != 4");
				}

				return new RectInt(_reader.ReadInt32(),_reader.ReadInt32(),_reader.ReadInt32(),_reader.ReadInt32());
			}
		}
		#endregion RectInt
	}
}