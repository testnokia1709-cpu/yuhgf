using System;

namespace CloudOnce.Internal
{
	public abstract class PersistentValue<T> : IPersistent
	{
		protected delegate T ValueLoaderDelegate(string key);

		protected delegate void ValueSetterDelegate(string key, T value);

		private T value;

		public string Key { get; private set; }

		public T Value
		{
			get
			{
				return value;
			}
			set
			{
				if (IsValidSet(value))
				{
					this.value = value;
				}
			}
		}

		public PersistenceType PersistenceType { get; private set; }

		public T DefaultValue { get; private set; }

		private ValueLoaderDelegate ValueLoader { get; set; }

		private ValueSetterDelegate ValueSetter { get; set; }

		protected PersistentValue(string key, PersistenceType type, T value, T defaultValue, ValueLoaderDelegate valueLoader, ValueSetterDelegate valueSetter)
		{
			Key = key;
			Value = value;
			PersistenceType = type;
			DefaultValue = defaultValue;
			ValueLoader = valueLoader;
			ValueSetter = valueSetter;
			DataManager.CloudPrefs[key] = this;
			DataManager.InitDataManager();
		}

		public void Load()
		{
			if (ValueLoader != null)
			{
				Value = ValueLoader(Key);
			}
		}

		public void Flush()
		{
			if (ValueSetter != null)
			{
				ValueSetter(Key, Value);
			}
		}

		public void Reset()
		{
			value = DefaultValue;
			Flush();
		}

		private bool IsValidSet(T newValue)
		{
			if (PersistenceType == PersistenceType.Latest)
			{
				return true;
			}
			if (newValue is DateTime)
			{
				DateTime dateTime = (DateTime)(object)newValue;
				DateTime dateTime2 = (DateTime)(object)value;
				return (PersistenceType != PersistenceType.Highest) ? (dateTime < dateTime2) : (dateTime.Ticks > dateTime2.Ticks);
			}
			if (newValue is long)
			{
				long num = long.Parse(newValue.ToString());
				long num2 = long.Parse(value.ToString());
				return (PersistenceType != PersistenceType.Highest) ? (num < num2) : (num > num2);
			}
			if (newValue is decimal)
			{
				decimal num3 = decimal.Parse(newValue.ToString());
				decimal num4 = decimal.Parse(value.ToString());
				return (PersistenceType != PersistenceType.Highest) ? (num3 < num4) : (num3 > num4);
			}
			if (!(newValue is bool) && !(newValue is string))
			{
				double num5 = double.Parse(newValue.ToString());
				double num6 = double.Parse(value.ToString());
				return (PersistenceType != PersistenceType.Highest) ? (num5 < num6) : (num5 > num6);
			}
			if (!(newValue is string))
			{
				bool flag = bool.Parse(newValue.ToString());
				bool flag2 = bool.Parse(value.ToString());
				return (PersistenceType == PersistenceType.Highest) ? (flag && !flag2) : (!flag && flag2);
			}
			int length = newValue.ToString().Length;
			int length2 = value.ToString().Length;
			return (PersistenceType != PersistenceType.Highest) ? (length < length2) : (length > length2);
		}
	}
}
