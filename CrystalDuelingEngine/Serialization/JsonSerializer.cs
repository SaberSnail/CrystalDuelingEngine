using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace CrystalDuelingEngine.Serialization
{
	public class JsonSerializer : ISerializer
	{
		public static string Serialize(ISerializable serializable)
		{
			var options = new JsonWriterOptions
			{
				Indented = true,
				Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
			};

			using var stream = new MemoryStream();
			var writer = new Utf8JsonWriter(stream, options);

			var serializer = new JsonSerializer(writer);
			serializable.Serialize(serializer);

			writer.Flush();
			return Encoding.UTF8.GetString(stream.ToArray());
		}

		public void StartObject(ISerializable obj)
		{
			var isSameAsPreviousObject = IsObjectSameAsPreviousObject(obj);
			m_objectStack.Push(obj);
			if (!isSameAsPreviousObject)
			{
				if (m_nestedKey is null)
				{
					m_writer.WriteStartObject();
				}
				else
				{
					m_writer.WriteStartObject(m_nestedKey);
					m_nestedKey = null;
				}
				SerializationManager.StartObjectSerialization(this, obj);
			}
		}

		public void EndObject()
		{
			var obj = m_objectStack.Pop();
			if (!IsObjectSameAsPreviousObject(obj))
				m_writer.WriteEndObject();
		}

		private JsonSerializer(Utf8JsonWriter writer)
		{
			m_objectStack = new Stack<ISerializable>();
			m_writer = writer;
		}

		private bool IsObjectSameAsPreviousObject(ISerializable obj)
		{
			return m_objectStack.Count != 0 && ReferenceEquals(obj, m_objectStack.Peek());
		}

		public void SerializeValue(string key, object value)
		{
			var type = value?.GetType();
			if (value is null)
			{
				m_writer.WriteNull(key);
			}
			else if (value is ISerializable serializable)
			{
				m_nestedKey = key;
				serializable.Serialize(this);
			}
			else if (type.IsEnum)
			{
				m_writer.WriteString(key, value.ToString());
			}
			else if (value is bool boolValue)
			{
				m_writer.WriteBoolean(key, boolValue);
			}
			else if (value is DateTime dateTimeValue)
			{
				m_writer.WriteString(key, dateTimeValue);
			}
			else if (value is DateTimeOffset dateTimeOffsetValue)
			{
				m_writer.WriteString(key, dateTimeOffsetValue);
			}
			else if (value is Guid guidValue)
			{
				m_writer.WriteString(key, guidValue);
			}
			else if (value is string stringValue)
			{
				m_writer.WriteString(key, stringValue);
			}
			else if (value is IEnumerable enumerable)
			{
				m_writer.WriteStartArray(key);
				foreach (var singleValue in enumerable)
					SerializeValueInArray(singleValue);
				m_writer.WriteEndArray();
			}
			else
			{
				if (!WriteNumber(key, value))
					throw new SerializationFormatException($"Serialized object must be IEnumerable, ISerializable, or simple: {value.GetType().FullName}");
			}
		}

		private bool WriteNumber(string key, object value)
		{
			var writerType = m_writer.GetType();
			var argType = value.GetType();
			var numberMethod = writerType.GetMethod("WriteNumber", new[] { typeof(string), argType });
			if (numberMethod is not null)
			{
				numberMethod.Invoke(m_writer, new[] { key, value });
				return true;
			}
			return false;
		}

		private void SerializeValueInArray(object value)
		{
			var type = value?.GetType();
			if (value is null)
			{
				m_writer.WriteNullValue();
			}
			else if (value is ISerializable serializable)
			{
				m_nestedKey = null;
				serializable.Serialize(this);
			}
			else if (type.IsEnum)
			{
				m_writer.WriteStringValue(value.ToString());
			}
			else if (value is bool boolValue)
			{
				m_writer.WriteBooleanValue(boolValue);
			}
			else if (value is DateTime dateTimeValue)
			{
				m_writer.WriteStringValue(dateTimeValue);
			}
			else if (value is DateTimeOffset dateTimeOffsetValue)
			{
				m_writer.WriteStringValue(dateTimeOffsetValue);
			}
			else if (value is Guid guidValue)
			{
				m_writer.WriteStringValue(guidValue);
			}
			else if (value is string stringValue)
			{
				m_writer.WriteStringValue(stringValue);
			}
			else if (value is IEnumerable enumerable)
			{
				m_writer.WriteStartArray();
				foreach (var singleValue in enumerable)
					SerializeValueInArray(singleValue);
				m_writer.WriteEndArray();
			}
			else
			{
				if (!WriteNumberValue(value))
					throw new SerializationFormatException($"Serialized object must be IEnumerable, ISerializable, or simple: {value.GetType().FullName}");
			}
		}

		private bool WriteNumberValue(object value)
		{
			var writerType = m_writer.GetType();
			var argType = value.GetType();
			var numberMethod = writerType.GetMethod("WriteNumberValue", new[] { argType });
			if (numberMethod is not null)
			{
				numberMethod.Invoke(m_writer, new[] { value });
				return true;
			}
			return false;
		}

		string m_nestedKey;
		Stack<ISerializable> m_objectStack;
		Utf8JsonWriter m_writer;
	}
}
