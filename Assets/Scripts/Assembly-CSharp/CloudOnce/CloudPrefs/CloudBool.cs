using CloudOnce.Internal;

namespace CloudOnce.CloudPrefs
{
	public sealed class CloudBool : PersistentValue<bool>
	{
		public CloudBool(string key, PersistenceType persistenceType, bool value = false)
			: base(key, persistenceType, value, value, (ValueLoaderDelegate)DataManager.GetBool, (ValueSetterDelegate)DataManager.SetBool)
		{
			DataManager.InitializeBool(key, persistenceType, value);
			Load();
		}

		public CloudBool(string key, PersistenceType persistenceType, bool value, bool defaultValue)
			: base(key, persistenceType, value, defaultValue, (ValueLoaderDelegate)DataManager.GetBool, (ValueSetterDelegate)DataManager.SetBool)
		{
			DataManager.InitializeBool(key, persistenceType, value);
			Load();
		}
	}
}
