using CloudOnce.Internal;

namespace CloudOnce.CloudPrefs
{
	public sealed class CloudDouble : PersistentValue<double>
	{
		public CloudDouble(string key, PersistenceType persistenceType, double value = 0.0)
			: base(key, persistenceType, value, value, (ValueLoaderDelegate)DataManager.GetDouble, (ValueSetterDelegate)DataManager.SetDouble)
		{
			DataManager.InitializeDouble(key, persistenceType, value);
			Load();
		}

		public CloudDouble(string key, PersistenceType persistenceType, double value, double defaultValue)
			: base(key, persistenceType, value, defaultValue, (ValueLoaderDelegate)DataManager.GetDouble, (ValueSetterDelegate)DataManager.SetDouble)
		{
			DataManager.InitializeDouble(key, persistenceType, value);
			Load();
		}
	}
}
