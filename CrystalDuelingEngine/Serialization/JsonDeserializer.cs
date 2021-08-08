using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;

namespace CrystalDuelingEngine.Serialization
{
	public class JsonDeserializer : IDeserializer
	{
		public static ISerializable Deserialize(string text)
		{
			using var document = JsonDocument.Parse(text);

			var deserializer = new JsonDeserializer(document.RootElement);
			return SerializationManager.Deserialize(deserializer);
		}

		public T GetValue<T>(string key)
		{
			if (!m_root.TryGetProperty(key, out var property))
				return default(T);
			return GetOrDeserializeValue<T>(property);
		}

		public IEnumerable<T> GetValues<T>(string key)
		{
			if (!m_root.TryGetProperty(key, out var property) || property.ValueKind == JsonValueKind.Null)
				return default(IEnumerable<T>);
			return property.EnumerateArray().Select(GetOrDeserializeValue<T>);
		}

		private JsonDeserializer(JsonElement root) => m_root = root;

		private T GetOrDeserializeValue<T>(JsonElement property)
		{
			if (property.ValueKind == JsonValueKind.Object)
			{
				var childDeserializer = new JsonDeserializer(property);
				return (T) SerializationManager.Deserialize(childDeserializer);
			}
			if (property.ValueKind == JsonValueKind.Array)
			{
				var argType = typeof(T).GetGenericArguments()[0];
				var getOrDeserializeValue = GetType().GetMethod(nameof(GetOrDeserializeValue), BindingFlags.NonPublic | BindingFlags.Instance).MakeGenericMethod(argType);
				var cast = typeof(Enumerable).GetMethod("Cast", BindingFlags.Public | BindingFlags.Static).MakeGenericMethod(argType);
				var toList = typeof(Enumerable).GetMethod("ToList", BindingFlags.Public | BindingFlags.Static).MakeGenericMethod(argType);
				return (T) toList.Invoke(null, new[] { cast.Invoke(null, new[] { property.EnumerateArray().Select(x => getOrDeserializeValue.Invoke(this, new object[] { x })) }) });
			}
			if (typeof(T).IsEnum)
				return (T) Enum.Parse(typeof(T), property.GetString());

			return System.Text.Json.JsonSerializer.Deserialize<T>(property.GetRawText());
		}

		JsonElement m_root;
	}
}
