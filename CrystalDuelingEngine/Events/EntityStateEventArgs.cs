using System;
using CrystalDuelingEngine.States;

namespace CrystalDuelingEngine.Events
{
	public class EntityStateEventArgs : EventArgs
	{
		public EntityStateEventArgs(EntityState entityState)
		{
			EntityState = entityState;
		}

		public EntityState EntityState { get; }
	}
}
