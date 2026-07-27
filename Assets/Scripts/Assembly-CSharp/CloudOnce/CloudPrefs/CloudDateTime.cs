using System;
using CloudOnce.Internal;

namespace CloudOnce.CloudPrefs
{
	public class CloudDateTime : PersistentValue<DateTime>
	{
		public CloudDateTime(string key, PersistenceType persistenceType, DateTime value = default(DateTime))
			: base(key, persistenceType, value, value, (ValueLoaderDelegate)DataManager.GetDateTime, (ValueSetterDelegate)DataManager.SetDateTime)
		{
			DataManager.InitializeDateTime(key, persistenceType, value);
			Load();
		}

		public CloudDateTime(string key, PersistenceType persistenceType, DateTime value, DateTime defaultValue)
			: base(key, persistenceType, value, defaultValue, (ValueLoaderDelegate)DataManager.GetDateTime, (ValueSetterDelegate)DataManager.SetDateTime)
		{
			DataManager.InitializeDateTime(key, persistenceType, value);
			Load();
		}
	}
}
