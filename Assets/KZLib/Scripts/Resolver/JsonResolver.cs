using UnityEngine;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace KZLib.KZResolver
{
	/// <summary>
	/// example
	/// <br/> JsonConvert.SerializeObject(m_Queue,JsonResolver.In);
	/// <br/> support Color, Color32, Vector2, Vector2Int, Vector3, Vector3Int, Vector4, Quaternion, Rect, RectInt
	/// </summary>
	public class JsonResolver
	{
		public static readonly JsonConverter[] In = new JsonConverter[] { new ColorConverter(), new Color32Converter(), new Vector2Converter(), new Vector2IntConverter(), new Vector3Converter(), new Vector3IntConverter(), new Vector4Converter(), new QuaternionConverter(), new RectConverter(), new RectIntConverter() };

		#region Color
		private class ColorConverter : JsonConverter
		{
			public override bool CanConvert(Type _type)
			{
				return _type == typeof(Color);
			}

			public override object ReadJson(JsonReader _reader,Type _type,object _object,JsonSerializer _serializer)
			{
				var data = JObject.Load(_reader);

				return new Color(Convert.ToSingle(data["r"]),Convert.ToSingle(data["g"]),Convert.ToSingle(data["b"]),Convert.ToSingle(data["a"]));
			}

			public override void WriteJson(JsonWriter _writer,object _object,JsonSerializer _serializer)
			{
				var color = (Color) _object;

				_writer.WriteStartObject();

				_writer.WritePropertyName("r");
				_writer.WriteValue(color.r);
				_writer.WritePropertyName("g");
				_writer.WriteValue(color.g);
				_writer.WritePropertyName("b");
				_writer.WriteValue(color.b);
				_writer.WritePropertyName("a");
				_writer.WriteValue(color.a);

				_writer.WriteEndObject();
			}
		}
		#endregion Color

		#region Color32
		private class Color32Converter : JsonConverter
		{
			public override bool CanConvert(Type _type)
			{
				return _type == typeof(Color32);
			}

			public override object ReadJson(JsonReader _reader,Type _type,object _object,JsonSerializer _serializer)
			{
				var data = JObject.Load(_reader);

				return new Color32(Convert.ToByte(data["r"]),Convert.ToByte(data["g"]),Convert.ToByte(data["b"]),Convert.ToByte(data["a"]));
			}

			public override void WriteJson(JsonWriter _writer,object _object,JsonSerializer _serializer)
			{
				var color = (Color32) _object;

				_writer.WriteStartObject();

				_writer.WritePropertyName("r");
				_writer.WriteValue(color.r);
				_writer.WritePropertyName("g");
				_writer.WriteValue(color.g);
				_writer.WritePropertyName("b");
				_writer.WriteValue(color.b);
				_writer.WritePropertyName("a");
				_writer.WriteValue(color.a);

				_writer.WriteEndObject();
			}
		}
		#endregion Color32

		#region Vector2
		private class Vector2Converter : JsonConverter
		{
			public override bool CanConvert(Type _type)
			{
				return _type == typeof(Vector2);
			}

			public override object ReadJson(JsonReader _reader,Type _type,object _object,JsonSerializer _serializer)
			{
				var data = JObject.Load(_reader);

				return new Vector2(Convert.ToSingle(data["x"]),Convert.ToSingle(data["y"]));
			}

			public override void WriteJson(JsonWriter _writer,object _object,JsonSerializer _serializer)
			{
				var vector = (Vector2) _object;

				_writer.WriteStartObject();

				_writer.WritePropertyName("x");
				_writer.WriteValue(vector.x);
				_writer.WritePropertyName("y");
				_writer.WriteValue(vector.y);

				_writer.WriteEndObject();
			}
		}
		#endregion Vector2

		#region Vector2Int
		private class Vector2IntConverter : JsonConverter
		{
			public override bool CanConvert(Type _type)
			{
				return _type == typeof(Vector2Int);
			}

			public override object ReadJson(JsonReader _reader,Type _type,object _object,JsonSerializer _serializer)
			{
				var data = JObject.Load(_reader);

				return new Vector2Int(Convert.ToInt32(data["x"]),Convert.ToInt32(data["y"]));
			}

			public override void WriteJson(JsonWriter _writer,object _object,JsonSerializer _serializer)
			{
				var vector = (Vector2Int) _object;

				_writer.WriteStartObject();

				_writer.WritePropertyName("x");
				_writer.WriteValue(vector.x);
				_writer.WritePropertyName("y");
				_writer.WriteValue(vector.y);

				_writer.WriteEndObject();
			}
		}
		#endregion Vector2Int

		#region Vector3
		private class Vector3Converter : JsonConverter
		{
			public override bool CanConvert(Type _type)
			{
				return _type == typeof(Vector3);
			}

			public override object ReadJson(JsonReader _reader,Type _type,object _object,JsonSerializer _serializer)
			{
				var data = JObject.Load(_reader);

				return new Vector3(Convert.ToSingle(data["x"]),Convert.ToSingle(data["y"]),Convert.ToSingle(data["z"]));
			}

			public override void WriteJson(JsonWriter _writer,object _object,JsonSerializer _serializer)
			{
				var vector = (Vector3) _object;

				_writer.WriteStartObject();

				_writer.WritePropertyName("x");
				_writer.WriteValue(vector.x);
				_writer.WritePropertyName("y");
				_writer.WriteValue(vector.y);
				_writer.WritePropertyName("z");
				_writer.WriteValue(vector.z);

				_writer.WriteEndObject();
			}
		}
		#endregion Vector3

		#region Vector3Int
		private class Vector3IntConverter : JsonConverter
		{
			public override bool CanConvert(Type _type)
			{
				return _type == typeof(Vector3Int);
			}

			public override object ReadJson(JsonReader _reader,Type _type,object _object,JsonSerializer _serializer)
			{
				var data = JObject.Load(_reader);

				return new Vector3Int(Convert.ToInt32(data["x"]),Convert.ToInt32(data["y"]),Convert.ToInt32(data["z"]));
			}

			public override void WriteJson(JsonWriter _writer,object _object,JsonSerializer _serializer)
			{
				var vector = (Vector3Int) _object;

				_writer.WriteStartObject();

				_writer.WritePropertyName("x");
				_writer.WriteValue(vector.x);
				_writer.WritePropertyName("y");
				_writer.WriteValue(vector.y);
				_writer.WritePropertyName("z");
				_writer.WriteValue(vector.z);

				_writer.WriteEndObject();
			}
		}
		#endregion Vector3Int

		#region Vector4
		private class Vector4Converter : JsonConverter
		{
			public override bool CanConvert(Type _type)
			{
				return _type == typeof(Vector4);
			}

			public override object ReadJson(JsonReader _reader,Type _type,object _object,JsonSerializer _serializer)
			{
				var data = JObject.Load(_reader);

				return new Vector4(Convert.ToSingle(data["x"]),Convert.ToSingle(data["y"]),Convert.ToSingle(data["z"]),Convert.ToSingle(data["w"]));
			}

			public override void WriteJson(JsonWriter _writer,object _object,JsonSerializer _serializer)
			{
				var vector = (Vector4) _object;

				_writer.WriteStartObject();

				_writer.WritePropertyName("x");
				_writer.WriteValue(vector.x);
				_writer.WritePropertyName("y");
				_writer.WriteValue(vector.y);
				_writer.WritePropertyName("z");
				_writer.WriteValue(vector.z);
				_writer.WritePropertyName("w");
				_writer.WriteValue(vector.w);

				_writer.WriteEndObject();
			}
		}
		#endregion Vector4

		#region Quaternion
		private class QuaternionConverter : JsonConverter
		{
			public override bool CanConvert(Type _type)
			{
				return _type == typeof(Quaternion);
			}

			public override object ReadJson(JsonReader _reader,Type _type,object _object,JsonSerializer _serializer)
			{
				var data = JObject.Load(_reader);

				return new Quaternion(Convert.ToSingle(data["x"]),Convert.ToSingle(data["y"]),Convert.ToSingle(data["z"]),Convert.ToSingle(data["w"]));
			}

			public override void WriteJson(JsonWriter _writer,object _object,JsonSerializer _serializer)
			{
				var quaternion = (Quaternion) _object;

				_writer.WriteStartObject();

				_writer.WritePropertyName("x");
				_writer.WriteValue(quaternion.x);
				_writer.WritePropertyName("y");
				_writer.WriteValue(quaternion.y);
				_writer.WritePropertyName("z");
				_writer.WriteValue(quaternion.z);
				_writer.WritePropertyName("w");
				_writer.WriteValue(quaternion.w);

				_writer.WriteEndObject();
			}
		}
		#endregion Quaternion

		#region Rect
		private class RectConverter : JsonConverter
		{
			public override bool CanConvert(Type _type)
			{
				return _type == typeof(Rect);
			}

			public override object ReadJson(JsonReader _reader,Type _type,object _object,JsonSerializer _serializer)
			{
				var data = JObject.Load(_reader);

				return new Rect(Convert.ToSingle(data["x"]),Convert.ToSingle(data["y"]),Convert.ToSingle(data["width"]),Convert.ToSingle(data["height"]));
			}

			public override void WriteJson(JsonWriter _writer,object _object,JsonSerializer _serializer)
			{
				var rect = (Rect) _object;

				_writer.WriteStartObject();

				_writer.WritePropertyName("x");
				_writer.WriteValue(rect.x);
				_writer.WritePropertyName("y");
				_writer.WriteValue(rect.y);
				_writer.WritePropertyName("width");
				_writer.WriteValue(rect.width);
				_writer.WritePropertyName("height");
				_writer.WriteValue(rect.height);

				_writer.WriteEndObject();
			}
		}
		#endregion Rect

		#region RectInt
		private class RectIntConverter : JsonConverter
		{
			public override bool CanConvert(Type _type)
			{
				return _type == typeof(RectInt);
			}

			public override object ReadJson(JsonReader _reader,Type _type,object _object,JsonSerializer _serializer)
			{
				var data = JObject.Load(_reader);

				return new RectInt(Convert.ToInt32(data["x"]),Convert.ToInt32(data["y"]),Convert.ToInt32(data["width"]),Convert.ToInt32(data["height"]));
			}

			public override void WriteJson(JsonWriter _writer,object _object,JsonSerializer _serializer)
			{
				var rect = (RectInt) _object;

				_writer.WriteStartObject();

				_writer.WritePropertyName("x");
				_writer.WriteValue(rect.x);
				_writer.WritePropertyName("y");
				_writer.WriteValue(rect.y);
				_writer.WritePropertyName("width");
				_writer.WriteValue(rect.width);
				_writer.WritePropertyName("height");
				_writer.WriteValue(rect.height);

				_writer.WriteEndObject();
			}
		}
		#endregion RectInt
	}

	public class Vector4Converter : JsonConverter
	{
		public override bool CanConvert(Type _type)
		{
			return _type == typeof(Vector4);
		}

		public override object ReadJson(JsonReader _reader,Type _type,object _object,JsonSerializer _serializer)
		{
			var data = _serializer.Deserialize(_reader);

			return JsonConvert.DeserializeObject<Vector4>(data.ToString());
		}

		public override void WriteJson(JsonWriter _writer,object _object,JsonSerializer _serializer)
		{
			var data = (Vector4) _object;

			_writer.WriteStartObject();
			_writer.WritePropertyName("x");
			_writer.WriteValue(data.x);
			_writer.WritePropertyName("y");
			_writer.WriteValue(data.y);
			_writer.WritePropertyName("z");
			_writer.WriteValue(data.z);
			_writer.WritePropertyName("w");
			_writer.WriteValue(data.w);
			_writer.WriteEndObject();
		}
	}

	public class Vector3Converter : JsonConverter
	{
		public override bool CanConvert(Type _type)
		{
			return _type == typeof(Vector3);
		}

		public override object ReadJson(JsonReader _reader,Type _type,object _object,JsonSerializer _serializer)
		{
			var data = _serializer.Deserialize(_reader);

			return JsonConvert.DeserializeObject<Vector3>(data.ToString());
		}

		public override void WriteJson(JsonWriter _writer,object _object,JsonSerializer _serializer)
		{
			var data = (Vector3) _object;

			_writer.WriteStartObject();
			_writer.WritePropertyName("x");
			_writer.WriteValue(data.x);
			_writer.WritePropertyName("y");
			_writer.WriteValue(data.y);
			_writer.WritePropertyName("z");
			_writer.WriteValue(data.z);
			_writer.WriteEndObject();
		}
	}

	public class Vector3IntConverter : JsonConverter
	{
		public override bool CanConvert(Type _type)
		{
			return _type == typeof(Vector3Int);
		}

		public override object ReadJson(JsonReader _reader,Type _type,object _object,JsonSerializer _serializer)
		{
			var data = _serializer.Deserialize(_reader);

			return JsonConvert.DeserializeObject<Vector3Int>(data.ToString());
		}

		public override void WriteJson(JsonWriter _writer,object _object,JsonSerializer _serializer)
		{
			var data = (Vector3Int) _object;

			_writer.WriteStartObject();
			_writer.WritePropertyName("x");
			_writer.WriteValue(data.x);
			_writer.WritePropertyName("y");
			_writer.WriteValue(data.y);
			_writer.WritePropertyName("z");
			_writer.WriteValue(data.z);
			_writer.WriteEndObject();
		}
	}
}