namespace CrystalDuelingEngine.Tags
{
	public interface IAddableTag : IRenderable
	{
		IAddableTag AddTag(IAddableTag tag);
	}
}
