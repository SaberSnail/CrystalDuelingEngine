using System;

namespace CrystalDuelingEngine
{
	[Flags]
	public enum TagScope
	{
		None = 0x000,
		Attacker = 0x001,
		Defender = 0x002,
		Battle = 0x004,
		AttackerAction = 0x008,
		DefenderAction = 0x010,
		AttackerResult = 0x020,
		DefenderResult = 0x040,
		TemporaryWorkspace = 0x080,
		CurrentAction = 0x100,
	}
}
