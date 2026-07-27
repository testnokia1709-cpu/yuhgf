namespace CloudOnce.Internal.Providers
{
	public interface ICloudStorageProvider
	{
		void Save();

		void Load();

		void Synchronize();

		bool DeleteVariable(string key);

		string[] ClearUnusedVariables();

		void DeleteAll();
	}
}
