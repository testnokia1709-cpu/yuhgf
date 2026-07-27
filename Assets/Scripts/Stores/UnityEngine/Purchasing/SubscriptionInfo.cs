using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine.Purchasing.Security;

namespace UnityEngine.Purchasing
{
	public class SubscriptionInfo
	{
		private Result is_subscribed;

		private Result is_expired;

		private Result is_cancelled;

		private Result is_free_trial;

		private Result is_auto_renewing;

		private Result is_introductory_price_period;

		private string productId;

		private DateTime purchaseDate;

		private DateTime subscriptionExpireDate;

		private DateTime subscriptionCancelDate;

		private TimeSpan remainedTime;

		private string introductory_price;

		private TimeSpan introductory_price_period;

		private long introductory_price_cycles;

		private TimeSpan freeTrialPeriod;

		private TimeSpan subscriptionPeriod;

		private string free_trial_period_string;

		private string sku_details;

		public SubscriptionInfo(AppleInAppPurchaseReceipt r, string intro_json)
		{
			AppleStoreProductType appleStoreProductType = (AppleStoreProductType)Enum.Parse(typeof(AppleStoreProductType), r.productType.ToString());
			if (appleStoreProductType == AppleStoreProductType.Consumable || appleStoreProductType == AppleStoreProductType.NonConsumable)
			{
				throw new InvalidProductTypeException();
			}
			if (!string.IsNullOrEmpty(intro_json))
			{
				Dictionary<string, object> dic = (Dictionary<string, object>)MiniJson.JsonDecode(intro_json);
				int num = -1;
				SubscriptionPeriodUnit subscriptionPeriodUnit = SubscriptionPeriodUnit.NotAvailable;
				introductory_price = dic.TryGetString("introductoryPrice") + dic.TryGetString("introductoryPriceLocale");
				if (string.IsNullOrEmpty(introductory_price))
				{
					introductory_price = "not available";
				}
				else
				{
					try
					{
						introductory_price_cycles = Convert.ToInt64(dic.TryGetString("introductoryPriceNumberOfPeriods"));
						num = Convert.ToInt32(dic.TryGetString("numberOfUnits"));
						subscriptionPeriodUnit = (SubscriptionPeriodUnit)Convert.ToInt32(dic.TryGetString("unit"));
					}
					catch (Exception message)
					{
						Debug.unityLogger.Log("Unable to parse introductory period cycles and duration, this product does not have configuration of introductory price period", message);
						subscriptionPeriodUnit = SubscriptionPeriodUnit.NotAvailable;
					}
				}
				DateTime now = DateTime.Now;
				switch (subscriptionPeriodUnit)
				{
				case SubscriptionPeriodUnit.Day:
					introductory_price_period = TimeSpan.FromTicks(TimeSpan.FromDays(1.0).Ticks * num);
					break;
				case SubscriptionPeriodUnit.Month:
					introductory_price_period = TimeSpan.FromTicks((now.AddMonths(1) - now).Ticks * num);
					break;
				case SubscriptionPeriodUnit.Week:
					introductory_price_period = TimeSpan.FromTicks(TimeSpan.FromDays(7.0).Ticks * num);
					break;
				case SubscriptionPeriodUnit.Year:
					introductory_price_period = TimeSpan.FromTicks((now.AddYears(1) - now).Ticks * num);
					break;
				case SubscriptionPeriodUnit.NotAvailable:
					introductory_price_period = TimeSpan.Zero;
					introductory_price_cycles = 0L;
					break;
				}
			}
			else
			{
				introductory_price = "not available";
				introductory_price_period = TimeSpan.Zero;
				introductory_price_cycles = 0L;
			}
			DateTime utcNow = DateTime.UtcNow;
			purchaseDate = r.purchaseDate;
			productId = r.productID;
			subscriptionExpireDate = r.subscriptionExpirationDate;
			subscriptionCancelDate = r.cancellationDate;
			if (appleStoreProductType == AppleStoreProductType.NonRenewingSubscription)
			{
				is_subscribed = Result.Unsupported;
				is_expired = Result.Unsupported;
				is_cancelled = Result.Unsupported;
				is_free_trial = Result.Unsupported;
				is_auto_renewing = Result.Unsupported;
				is_introductory_price_period = Result.Unsupported;
			}
			else
			{
				is_cancelled = ((r.cancellationDate.Ticks <= 0 || r.cancellationDate.Ticks >= utcNow.Ticks) ? Result.False : Result.True);
				is_subscribed = ((r.subscriptionExpirationDate.Ticks < utcNow.Ticks) ? Result.False : Result.True);
				is_expired = ((r.subscriptionExpirationDate.Ticks <= 0 || r.subscriptionExpirationDate.Ticks >= utcNow.Ticks) ? Result.False : Result.True);
				is_free_trial = ((r.isFreeTrial != 1) ? Result.False : Result.True);
				is_auto_renewing = ((appleStoreProductType != AppleStoreProductType.AutoRenewingSubscription || is_cancelled != Result.False || is_expired != Result.False) ? Result.False : Result.True);
				is_introductory_price_period = ((r.isIntroductoryPricePeriod != 1) ? Result.False : Result.True);
			}
			if (is_subscribed == Result.True)
			{
				remainedTime = r.subscriptionExpirationDate.Subtract(utcNow);
			}
			else
			{
				remainedTime = TimeSpan.Zero;
			}
		}

		public SubscriptionInfo(string skuDetails, bool isAutoRenewing, DateTime purchaseDate, bool isFreeTrial, bool hasIntroductoryPriceTrial, bool purchaseHistorySupported)
		{
			Dictionary<string, object> dictionary = (Dictionary<string, object>)MiniJson.JsonDecode(skuDetails);
			if ((string)dictionary["type"] == "inapp")
			{
				throw new InvalidProductTypeException();
			}
			productId = (string)dictionary["productId"];
			this.purchaseDate = purchaseDate;
			is_subscribed = Result.True;
			is_auto_renewing = ((!isAutoRenewing) ? Result.False : Result.True);
			is_expired = Result.False;
			is_cancelled = (isAutoRenewing ? Result.False : Result.True);
			is_free_trial = ((!isFreeTrial) ? Result.False : Result.True);
			string text = null;
			if (dictionary.ContainsKey("subscriptionPeriod"))
			{
				text = (string)dictionary["subscriptionPeriod"];
			}
			string text2 = null;
			if (dictionary.ContainsKey("freeTrialPeriod"))
			{
				text2 = (string)dictionary["freeTrialPeriod"];
			}
			string text3 = null;
			if (dictionary.ContainsKey("introductoryPrice"))
			{
				text3 = (string)dictionary["introductoryPrice"];
			}
			string text4 = null;
			if (dictionary.ContainsKey("introductoryPricePeriod"))
			{
				text4 = (string)dictionary["introductoryPricePeriod"];
			}
			long num = 0L;
			if (dictionary.ContainsKey("introductoryPriceCycles"))
			{
				num = (long)dictionary["introductoryPriceCycles"];
			}
			free_trial_period_string = text2;
			try
			{
				subscriptionPeriod = XmlConvert.ToTimeSpan(text);
			}
			catch (Exception)
			{
				if (text == null || text.Length == 0)
				{
					subscriptionPeriod = TimeSpan.Zero;
				}
				else
				{
					subscriptionPeriod = new TimeSpan(7, 0, 0, 0);
				}
			}
			freeTrialPeriod = TimeSpan.Zero;
			if (isFreeTrial)
			{
				try
				{
					freeTrialPeriod = XmlConvert.ToTimeSpan(text2);
				}
				catch (Exception)
				{
					if (text2 == null || text2.Length == 0)
					{
						freeTrialPeriod = TimeSpan.Zero;
					}
					else
					{
						freeTrialPeriod = new TimeSpan(7, 0, 0, 0);
					}
				}
			}
			introductory_price = text3;
			introductory_price_cycles = num;
			introductory_price_period = TimeSpan.Zero;
			is_introductory_price_period = Result.False;
			if (hasIntroductoryPriceTrial)
			{
				try
				{
					introductory_price_period = XmlConvert.ToTimeSpan(text4);
				}
				catch (Exception)
				{
					if (text4 == null || text4.Length == 0)
					{
						introductory_price_period = TimeSpan.Zero;
					}
					else
					{
						introductory_price_period = new TimeSpan(7, 0, 0, 0);
					}
				}
			}
			DateTime utcNow = DateTime.UtcNow;
			TimeSpan timeSpan = utcNow.Subtract(purchaseDate);
			TimeSpan ts = TimeSpan.FromTicks(introductory_price_period.Ticks * introductory_price_cycles);
			if (timeSpan <= freeTrialPeriod)
			{
				subscriptionExpireDate = purchaseDate.Add(freeTrialPeriod);
			}
			else if (timeSpan < freeTrialPeriod.Add(ts))
			{
				is_introductory_price_period = Result.True;
				long num2 = utcNow.Subtract(purchaseDate.Add(freeTrialPeriod)).Ticks / introductory_price_period.Ticks + 1;
				subscriptionExpireDate = purchaseDate.Add(TimeSpan.FromTicks(freeTrialPeriod.Ticks + introductory_price_period.Ticks * num2));
			}
			else
			{
				long num3 = utcNow.Subtract(purchaseDate.Add(freeTrialPeriod.Add(ts))).Ticks / subscriptionPeriod.Ticks + 1;
				subscriptionExpireDate = purchaseDate.Add(TimeSpan.FromTicks(num3 * subscriptionPeriod.Ticks + freeTrialPeriod.Ticks + ts.Ticks));
			}
			remainedTime = subscriptionExpireDate.Subtract(utcNow);
			sku_details = skuDetails;
			try
			{
				introductory_price_period = XmlConvert.ToTimeSpan(text4);
			}
			catch (Exception)
			{
				if (text4 == null || text4.Length == 0)
				{
					introductory_price_period = TimeSpan.Zero;
				}
				else
				{
					introductory_price_period = new TimeSpan(7, 0, 0, 0);
				}
			}
			if (!purchaseHistorySupported)
			{
				is_free_trial = Result.Unsupported;
				subscriptionExpireDate = DateTime.MaxValue;
				remainedTime = TimeSpan.MaxValue;
				is_introductory_price_period = Result.Unsupported;
			}
		}

		public string getProductId()
		{
			return productId;
		}

		public DateTime getPurchaseDate()
		{
			return purchaseDate;
		}

		public Result isSubscribed()
		{
			return is_subscribed;
		}

		public Result isExpired()
		{
			return is_expired;
		}

		public Result isCancelled()
		{
			return is_cancelled;
		}

		public Result isFreeTrial()
		{
			return is_free_trial;
		}

		public Result isAutoRenewing()
		{
			return is_auto_renewing;
		}

		public TimeSpan getRemainingTime()
		{
			return remainedTime;
		}

		public Result isIntroductoryPricePeriod()
		{
			return is_introductory_price_period;
		}

		public TimeSpan getIntroductoryPricePeriod()
		{
			return introductory_price_period;
		}

		public string getIntroductoryPrice()
		{
			return string.IsNullOrEmpty(introductory_price) ? "not available" : introductory_price;
		}

		public long getIntroductoryPricePeriodCycles()
		{
			return introductory_price_cycles;
		}

		public DateTime getExpireDate()
		{
			return subscriptionExpireDate;
		}

		public DateTime getCancelDate()
		{
			return subscriptionCancelDate;
		}

		public TimeSpan getFreeTrialPeriod()
		{
			return freeTrialPeriod;
		}

		public TimeSpan getSubscriptionPeriod()
		{
			return subscriptionPeriod;
		}

		public string getFreeTrialPeriodString()
		{
			return free_trial_period_string;
		}

		public string getSkuDetails()
		{
			return sku_details;
		}
	}
}
