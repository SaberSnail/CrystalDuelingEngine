using System.Collections.Generic;

namespace CrystalDuelingEngine.Serialization
{
	public interface IDeserializer
	{
		T GetValue<T>(string key);
		IEnumerable<T> GetValues<T>(string key);
	}
}
