using CloudOnce.Internal;

namespace CloudOnce.CloudPrefs
{
	public sealed class CloudCurrencyFloat : PersistentCurrency
	{
		public CloudCurrencyFloat(string key, float defaultValue = 0f, bool allowNegative = false)
			: base(key, defaultValue, allowNegative)
		{
			DataManager.InitializeCurrency(key);
			Load();
		}
	}
}
