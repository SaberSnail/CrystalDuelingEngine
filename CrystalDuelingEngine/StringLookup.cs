using System;
using System.Collections.Generic;
using System.Linq;
using CrystalDuelingEngine.Serialization;
using GoldenAnvil.Utility;

namespace CrystalDuelingEngine
{
	public class StringLookup : ISerializable
	{
		public StringLookup()
			: this(Enumerable.Empty<Tuple<string, string>>())
		{
		}

		public StringLookup(IEnumerable<Tuple<string, string>> values)
		{
			m_lookup = values.EmptyIfNull().ToDictionary(x => x.Item1, x => x.Item2);
		}

		public string SerializationName => nameof(StringLookup);

		public string Lookup(string key)
		{
			return m_lookup[key];
		}

		public void AddLookup(string key, string value)
		{
			m_lookup.Add(key, value);
		}

		public void Serialize(ISerializer serializer)
		{
			serializer.StartObject(this);

			serializer.SerializeValue("Lookup", m_lookup.Select(x => new [] { x.Key, x.Value }));

			serializer.EndObject();
		}

		private StringLookup(IDeserializer deserializer)
		{
			m_lookup = deserializer.GetValues<List<string>>("Lookup").EmptyIfNull().ToDictionary(x => x[0], x => x[1]);
		}

		static StringLookup()
		{
			SerializationManager.RegisterSerializable(nameof(StringLookup), x => new StringLookup(x));
		}

		readonly Dictionary<string, string> m_lookup;
	}
}
