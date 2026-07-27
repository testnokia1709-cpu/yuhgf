namespace CloudOnce.Internal
{
	public interface IPersistent
	{
		void Flush();

		void Load();

		void Reset();
	}
}
