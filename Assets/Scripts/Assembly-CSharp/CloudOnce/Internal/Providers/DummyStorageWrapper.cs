namespace CloudOnce.Internal.Providers
{
	public class DummyStorageWrapper : ICloudStorageProvider
	{
		private readonly CloudOnceEvents cloudOnceEvents;

		public DummyStorageWrapper(CloudOnceEvents events)
		{
			cloudOnceEvents = events;
		}

		public void Save()
		{
			DataManager.SaveToDisk();
			cloudOnceEvents.RaiseOnCloudSaveComplete(false);
		}

		public void Load()
		{
			cloudOnceEvents.RaiseOnCloudLoadComplete(false);
		}

		public void Synchronize()
		{
			Load();
			Save();
		}

		public bool DeleteVariable(string key)
		{
			return DataManager.DeleteCloudPref(key);
		}

		public string[] ClearUnusedVariables()
		{
			return DataManager.ClearStowawayVariablesFromGameData();
		}

		public void DeleteAll()
		{
			DataManager.DeleteAllCloudVariables();
		}
	}
}
