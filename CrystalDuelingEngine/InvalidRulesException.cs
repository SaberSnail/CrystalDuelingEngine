using System;

namespace CrystalDuelingEngine
{
	public class InvalidRulesException : Exception
	{
		public InvalidRulesException(string message)
			: base(message)
		{
		}
	}
}
