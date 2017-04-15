using System;

namespace CrystalDuelingEngine.Serialization
{
	public class SerializationFormatException : FormatException
	{
		public SerializationFormatException(string message)
			: base(message)
		{
		}
	}
}
