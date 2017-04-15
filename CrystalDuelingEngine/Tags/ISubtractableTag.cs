namespace CrystalDuelingEngine.Tags
{
	public interface ISubtractableTag : IRenderable
	{
		ISubtractableTag SubtractTag(ISubtractableTag tag);
	}
}
