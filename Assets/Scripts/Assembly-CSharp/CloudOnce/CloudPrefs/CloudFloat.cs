using CloudOnce.Internal;

namespace CloudOnce.CloudPrefs
{
	public sealed class CloudFloat : PersistentValue<float>
	{
		public CloudFloat(string key, PersistenceType persistenceType, float value = 0f)
			: base(key, persistenceType, value, value, (ValueLoaderDelegate)DataManager.GetFloat, (ValueSetterDelegate)DataManager.SetFloat)
		{
			DataManager.InitializeFloat(key, persistenceType, value);
			Load();
		}

		public CloudFloat(string key, PersistenceType persistenceType, float value, float defaultValue)
			: base(key, persistenceType, value, defaultValue, (ValueLoaderDelegate)DataManager.GetFloat, (ValueSetterDelegate)DataManager.SetFloat)
		{
			DataManager.InitializeFloat(key, persistenceType, value);
			Load();
		}
	}
}
