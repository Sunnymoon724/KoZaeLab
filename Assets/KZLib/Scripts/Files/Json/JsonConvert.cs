using UnityEngine;
using System;
using Newtonsoft.Json;

/// <summary>
/// example
/// new JsonSerializerSettings() { Converters = { new Vector3IntConverter() } }
/// </summary>
namespace KZLib.KZFiles.Converter
{
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

	public class Vector2Converter : JsonConverter
	{
		public override bool CanConvert(Type _type)
		{
			return _type == typeof(Vector2);
		}

		public override object ReadJson(JsonReader _reader,Type _type,object _object,JsonSerializer _serializer)
		{
			var data = _serializer.Deserialize(_reader);

			return JsonConvert.DeserializeObject<Vector2>(data.ToString());
		}

		public override void WriteJson(JsonWriter _writer,object _object,JsonSerializer _serializer)
		{
			var data = (Vector2) _object;

			_writer.WriteStartObject();
			_writer.WritePropertyName("x");
			_writer.WriteValue(data.x);
			_writer.WritePropertyName("y");
			_writer.WriteValue(data.y);
			_writer.WriteEndObject();
		}
	}

	public class ColorConverter : JsonConverter
	{
		public override bool CanConvert(Type _type)
		{
			return _type == typeof(Color);
		}

		public override object ReadJson(JsonReader _reader,Type _type,object _object,JsonSerializer _serializer)
		{
			var data = _serializer.Deserialize(_reader);

			return JsonConvert.DeserializeObject<Color>(data.ToString());
		}

		public override void WriteJson(JsonWriter _writer,object _object,JsonSerializer _serializer)
		{
			var data = (Color) _object;

			_writer.WriteStartObject();
			_writer.WritePropertyName("r");
			_writer.WriteValue(data.r);
			_writer.WritePropertyName("g");
			_writer.WriteValue(data.g);
			_writer.WritePropertyName("b");
			_writer.WriteValue(data.b);
			_writer.WritePropertyName("a");
			_writer.WriteValue(data.a);
			_writer.WriteEndObject();
		}
	}
}