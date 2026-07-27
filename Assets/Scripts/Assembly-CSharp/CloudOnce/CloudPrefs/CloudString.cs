using CloudOnce.Internal;

namespace CloudOnce.CloudPrefs
{
	public sealed class CloudString : PersistentValue<string>
	{
		public CloudString(string key, PersistenceType persistenceType = PersistenceType.Latest, string value = "")
			: base(key, persistenceType, value, value, (ValueLoaderDelegate)DataManager.GetString, (ValueSetterDelegate)DataManager.SetString)
		{
			DataManager.InitializeString(key, persistenceType, value);
			Load();
		}

		public CloudString(string key, PersistenceType persistenceType, string value, string defaultValue)
			: base(key, persistenceType, value, defaultValue, (ValueLoaderDelegate)DataManager.GetString, (ValueSetterDelegate)DataManager.SetString)
		{
			DataManager.InitializeString(key, persistenceType, value);
			Load();
		}
	}
}
