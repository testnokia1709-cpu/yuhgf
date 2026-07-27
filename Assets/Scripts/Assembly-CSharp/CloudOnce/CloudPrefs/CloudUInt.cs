using CloudOnce.Internal;

namespace CloudOnce.CloudPrefs
{
	public sealed class CloudUInt : PersistentValue<uint>
	{
		public CloudUInt(string key, PersistenceType persistenceType, uint value = 0u)
			: base(key, persistenceType, value, value, (ValueLoaderDelegate)DataManager.GetUInt, (ValueSetterDelegate)DataManager.SetUInt)
		{
			DataManager.InitializeUInt(key, persistenceType, value);
			Load();
		}

		public CloudUInt(string key, PersistenceType persistenceType, uint value, uint defaultValue)
			: base(key, persistenceType, value, defaultValue, (ValueLoaderDelegate)DataManager.GetUInt, (ValueSetterDelegate)DataManager.SetUInt)
		{
			DataManager.InitializeUInt(key, persistenceType, value);
			Load();
		}
	}
}
