using CloudOnce.Internal;

namespace CloudOnce.CloudPrefs
{
	public sealed class CloudCurrencyInt : PersistentCurrency
	{
		public new int Additions
		{
			get
			{
				return (int)base.Additions;
			}
		}

		public new int Subtractions
		{
			get
			{
				return (int)base.Subtractions;
			}
		}

		public new int DefaultValue
		{
			get
			{
				return (int)base.DefaultValue;
			}
		}

		public new int Value
		{
			get
			{
				return (int)base.Value;
			}
			set
			{
				base.Value = value;
			}
		}

		public CloudCurrencyInt(string key, int defaultValue = 0, bool allowNegative = false)
			: base(key, defaultValue, allowNegative)
		{
			DataManager.InitializeCurrency(key);
			Load();
		}
	}
}
