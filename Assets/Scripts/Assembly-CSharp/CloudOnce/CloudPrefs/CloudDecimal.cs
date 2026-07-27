using CloudOnce.Internal;

namespace CloudOnce.CloudPrefs
{
	public sealed class CloudDecimal : PersistentValue<decimal>
	{
		public CloudDecimal(string key, PersistenceType persistenceType, decimal value = 0m)
			: base(key, persistenceType, value, value, (ValueLoaderDelegate)DataManager.GetDecimal, (ValueSetterDelegate)DataManager.SetDecimal)
		{
			DataManager.InitializeDecimal(key, persistenceType, value);
			Load();
		}

		public CloudDecimal(string key, PersistenceType persistenceType, decimal value, decimal defaultValue)
			: base(key, persistenceType, value, defaultValue, (ValueLoaderDelegate)DataManager.GetDecimal, (ValueSetterDelegate)DataManager.SetDecimal)
		{
			DataManager.InitializeDecimal(key, persistenceType, value);
			Load();
		}
	}
}
