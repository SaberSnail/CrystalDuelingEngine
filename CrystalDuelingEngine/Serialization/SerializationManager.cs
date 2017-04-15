using GoldenAnvil.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CrystalDuelingEngine.Serialization
{
	public static class SerializationManager
	{
		public static void RegisterSerializable(string name, Func<IDeserializer, ISerializable> deserializer)
		{
			lock (s_lock)
			{
				if (s_deserializers.ContainsKey(name))
					throw new InvalidOperationException($"Serializable class {name} has already been registered.");
				s_deserializers.Add(name, deserializer);
			}
		}

		public static void StartObjectSerialization(ISerializer serializer, ISerializable obj)
		{
			serializer.SerializeValue("$", obj.SerializationName);
		}

		public static ISerializable Deserialize(IDeserializer deserializer)
		{
			var name = deserializer.GetValue<string>("$");
			Func<IDeserializer, ISerializable> deserialize;
			lock (s_lock)
				deserialize = s_deserializers.GetValueOrDefault(name);
			if (deserialize == null)
				throw new SerializationFormatException($"Attempted to deserialize unknown type: {name}");
			return deserialize(deserializer);
		}

		static SerializationManager()
		{
			lock (s_lock)
				s_deserializers = new Dictionary<string, Func<IDeserializer, ISerializable>>();

			// force serializable registration
			var serializableType = typeof(ISerializable);
			var serializableTypes = Assembly.GetExecutingAssembly().GetTypes().Where(x => serializableType.IsAssignableFrom(x));
			foreach (var type in serializableTypes)
				System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(type.TypeHandle);
		}

		static readonly Dictionary<string, Func<IDeserializer, ISerializable>> s_deserializers;
		static readonly object s_lock = new object();
	}
}
