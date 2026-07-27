using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Store;
using UnityEngine.UI;

[AddComponentMenu("Unity IAP/Demo")]
public class IAPDemo : MonoBehaviour, IStoreListener
{
	[Serializable]
	public class UnityChannelPurchaseError
	{
		public string error;

		public UnityChannelPurchaseInfo purchaseInfo;
	}

	[Serializable]
	public class UnityChannelPurchaseInfo
	{
		public string productCode;

		public string gameOrderId;

		public string orderQueryToken;
	}

	private class UnityChannelLoginHandler : ILoginListener
	{
		internal Action initializeSucceededAction;

		internal Action<string> initializeFailedAction;

		internal Action<UserInfo> loginSucceededAction;

		internal Action<string> loginFailedAction;

		public void OnInitialized()
		{
			initializeSucceededAction();
		}

		public void OnInitializeFailed(string message)
		{
			initializeFailedAction(message);
		}

		public void OnLogin(UserInfo userInfo)
		{
			loginSucceededAction(userInfo);
		}

		public void OnLoginFailed(string message)
		{
			loginFailedAction(message);
		}
	}

	private IStoreController m_Controller;

	private IAppleExtensions m_AppleExtensions;

	private IMoolahExtension m_MoolahExtensions;

	private ISamsungAppsExtensions m_SamsungExtensions;

	private IMicrosoftExtensions m_MicrosoftExtensions;

	private IUnityChannelExtensions m_UnityChannelExtensions;

	private ITransactionHistoryExtensions m_TransactionHistoryExtensions;

	private bool m_IsGooglePlayStoreSelected;

	private bool m_IsSamsungAppsStoreSelected;

	private bool m_IsCloudMoolahStoreSelected;

	private bool m_IsUnityChannelSelected;

	private string m_LastTransactionID;

	private bool m_IsLoggedIn;

	private UnityChannelLoginHandler unityChannelLoginHandler;

	private bool m_FetchReceiptPayloadOnPurchase;

	private bool m_PurchaseInProgress;

	private Dictionary<string, IAPDemoProductUI> m_ProductUIs = new Dictionary<string, IAPDemoProductUI>();

	public GameObject productUITemplate;

	public RectTransform contentRect;

	public Button restoreButton;

	public Button loginButton;

	public Button validateButton;

	public Text versionText;

	public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
	{
		m_Controller = controller;
		m_AppleExtensions = extensions.GetExtension<IAppleExtensions>();
		m_SamsungExtensions = extensions.GetExtension<ISamsungAppsExtensions>();
		m_MoolahExtensions = extensions.GetExtension<IMoolahExtension>();
		m_MicrosoftExtensions = extensions.GetExtension<IMicrosoftExtensions>();
		m_UnityChannelExtensions = extensions.GetExtension<IUnityChannelExtensions>();
		m_TransactionHistoryExtensions = extensions.GetExtension<ITransactionHistoryExtensions>();
		InitUI(controller.products.all);
		m_AppleExtensions.RegisterPurchaseDeferredListener(OnDeferred);
		Dictionary<string, string> introductoryPriceDictionary = m_AppleExtensions.GetIntroductoryPriceDictionary();
		Debug.Log("Available items:");
		Product[] all = controller.products.all;
		foreach (Product product in all)
		{
			if (!product.availableToPurchase)
			{
				continue;
			}
			Debug.Log(string.Join(" - ", new string[7]
			{
				product.metadata.localizedTitle,
				product.metadata.localizedDescription,
				product.metadata.isoCurrencyCode,
				product.metadata.localizedPrice.ToString(),
				product.metadata.localizedPriceString,
				product.transactionID,
				product.receipt
			}));
			if (product.receipt != null)
			{
				if (product.definition.type == ProductType.Subscription)
				{
					if (checkIfProductIsAvailableForSubscriptionManager(product.receipt))
					{
						string intro_json = ((introductoryPriceDictionary != null && introductoryPriceDictionary.ContainsKey(product.definition.storeSpecificId)) ? introductoryPriceDictionary[product.definition.storeSpecificId] : null);
						SubscriptionManager subscriptionManager = new SubscriptionManager(product, intro_json);
						SubscriptionInfo subscriptionInfo = subscriptionManager.getSubscriptionInfo();
						Debug.Log("product id is: " + subscriptionInfo.getProductId());
						Debug.Log("purchase date is: " + subscriptionInfo.getPurchaseDate());
						Debug.Log("subscription next billing date is: " + subscriptionInfo.getExpireDate());
						Debug.Log("is subscribed? " + subscriptionInfo.isSubscribed());
						Debug.Log("is expired? " + subscriptionInfo.isExpired());
						Debug.Log("is cancelled? " + subscriptionInfo.isCancelled());
						Debug.Log("product is in free trial peroid? " + subscriptionInfo.isFreeTrial());
						Debug.Log("product is auto renewing? " + subscriptionInfo.isAutoRenewing());
						Debug.Log("subscription remaining valid time until next billing date is: " + subscriptionInfo.getRemainingTime());
						Debug.Log("is this product in introductory price period? " + subscriptionInfo.isIntroductoryPricePeriod());
						Debug.Log("the product introductory localized price is: " + subscriptionInfo.getIntroductoryPrice());
						Debug.Log("the product introductory price period is: " + subscriptionInfo.getIntroductoryPricePeriod());
						Debug.Log("the number of product introductory price period cycles is: " + subscriptionInfo.getIntroductoryPricePeriodCycles());
					}
					else
					{
						Debug.Log("This product is not available for SubscriptionManager class, only products that are purchase by 1.19+ SDK can use this class.");
					}
				}
				else
				{
					Debug.Log("the product is not a subscription product");
				}
			}
			else
			{
				Debug.Log("the product should have a valid receipt");
			}
		}
		AddProductUIs(m_Controller.products.all);
		LogProductDefinitions();
	}

	private bool checkIfProductIsAvailableForSubscriptionManager(string receipt)
	{
		Dictionary<string, object> dictionary = (Dictionary<string, object>)MiniJson.JsonDecode(receipt);
		if (!dictionary.ContainsKey("Store") || !dictionary.ContainsKey("Payload"))
		{
			Debug.Log("The product receipt does not contain enough information");
			return false;
		}
		string text = (string)dictionary["Store"];
		string text2 = (string)dictionary["Payload"];
		if (text2 != null)
		{
			switch (text)
			{
			case "GooglePlay":
			{
				Dictionary<string, object> dictionary2 = (Dictionary<string, object>)MiniJson.JsonDecode(text2);
				if (!dictionary2.ContainsKey("json"))
				{
					Debug.Log("The product receipt does not contain enough information, the 'json' field is missing");
					return false;
				}
				Dictionary<string, object> dictionary3 = (Dictionary<string, object>)MiniJson.JsonDecode((string)dictionary2["json"]);
				if (dictionary3 != null && !dictionary3.ContainsKey("developerPayload"))
				{
					Debug.Log("The product receipt does not contain enough information, the 'developerPayload' field is missing");
					return false;
				}
				string json = (string)dictionary3["developerPayload"];
				Dictionary<string, object> dictionary4 = (Dictionary<string, object>)MiniJson.JsonDecode(json);
				if (dictionary4 == null || !dictionary4.ContainsKey("is_free_trial") || !dictionary4.ContainsKey("has_introductory_price_trial"))
				{
					Debug.Log("The product receipt does not contain enough information, the product is not purchased using 1.19 or later");
					return false;
				}
				return true;
			}
			case "AppleAppStore":
			case "MacAppStore":
				return true;
			default:
				return false;
			}
		}
		return false;
	}

	public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
	{
		Debug.Log("Purchase OK: " + e.purchasedProduct.definition.id);
		Debug.Log("Receipt: " + e.purchasedProduct.receipt);
		m_LastTransactionID = e.purchasedProduct.transactionID;
		m_PurchaseInProgress = false;
		if (m_IsUnityChannelSelected)
		{
			UnifiedReceipt unifiedReceipt = JsonUtility.FromJson<UnifiedReceipt>(e.purchasedProduct.receipt);
			if (unifiedReceipt != null && !string.IsNullOrEmpty(unifiedReceipt.Payload))
			{
				UnityChannelPurchaseReceipt unityChannelPurchaseReceipt = JsonUtility.FromJson<UnityChannelPurchaseReceipt>(unifiedReceipt.Payload);
				Debug.LogFormat("UnityChannel receipt: storeSpecificId = {0}, transactionId = {1}, orderQueryToken = {2}", unityChannelPurchaseReceipt.storeSpecificId, unityChannelPurchaseReceipt.transactionId, unityChannelPurchaseReceipt.orderQueryToken);
			}
		}
		UpdateProductUI(e.purchasedProduct);
		return PurchaseProcessingResult.Complete;
	}

	public void OnPurchaseFailed(Product item, PurchaseFailureReason r)
	{
		Debug.Log("Purchase failed: " + item.definition.id);
		Debug.Log(r);
		Debug.Log("Store specific error code: " + m_TransactionHistoryExtensions.GetLastStoreSpecificPurchaseErrorCode());
		if (m_TransactionHistoryExtensions.GetLastPurchaseFailureDescription() != null)
		{
			Debug.Log("Purchase failure description message: " + m_TransactionHistoryExtensions.GetLastPurchaseFailureDescription().message);
		}
		if (m_IsUnityChannelSelected)
		{
			string lastPurchaseError = m_UnityChannelExtensions.GetLastPurchaseError();
			UnityChannelPurchaseError unityChannelPurchaseError = JsonUtility.FromJson<UnityChannelPurchaseError>(lastPurchaseError);
			if (unityChannelPurchaseError != null && unityChannelPurchaseError.purchaseInfo != null)
			{
				UnityChannelPurchaseInfo purchaseInfo = unityChannelPurchaseError.purchaseInfo;
				Debug.LogFormat("UnityChannel purchaseInfo: productCode = {0}, gameOrderId = {1}, orderQueryToken = {2}", purchaseInfo.productCode, purchaseInfo.gameOrderId, purchaseInfo.orderQueryToken);
			}
			if (r == PurchaseFailureReason.DuplicateTransaction)
			{
				Debug.Log("Duplicate transaction detected, unlock this item");
			}
		}
		m_PurchaseInProgress = false;
	}

	public void OnInitializeFailed(InitializationFailureReason error)
	{
		Debug.Log("Billing failed to initialize!");
		switch (error)
		{
		case InitializationFailureReason.AppNotKnown:
			Debug.LogError("Is your App correctly uploaded on the relevant publisher console?");
			break;
		case InitializationFailureReason.PurchasingUnavailable:
			Debug.Log("Billing disabled!");
			break;
		case InitializationFailureReason.NoProductsAvailable:
			Debug.Log("No products available for purchase!");
			break;
		}
	}

	public void Awake()
	{
		StandardPurchasingModule standardPurchasingModule = StandardPurchasingModule.Instance();
		standardPurchasingModule.useFakeStoreUIMode = FakeStoreUIMode.StandardUser;
		ConfigurationBuilder builder = ConfigurationBuilder.Instance(standardPurchasingModule);
		builder.Configure<IMicrosoftConfiguration>().useMockBillingSystem = false;
		m_IsGooglePlayStoreSelected = Application.platform == RuntimePlatform.Android && standardPurchasingModule.appStore == AppStore.GooglePlay;
		builder.Configure<IMoolahConfiguration>().appKey = "d93f4564c41d463ed3d3cd207594ee1b";
		builder.Configure<IMoolahConfiguration>().hashKey = "cc";
		builder.Configure<IMoolahConfiguration>().SetMode(CloudMoolahMode.AlwaysSucceed);
		m_IsCloudMoolahStoreSelected = Application.platform == RuntimePlatform.Android && standardPurchasingModule.appStore == AppStore.CloudMoolah;
		m_IsUnityChannelSelected = Application.platform == RuntimePlatform.Android && standardPurchasingModule.appStore == AppStore.XiaomiMiPay;
		builder.Configure<IUnityChannelConfiguration>().fetchReceiptPayloadOnPurchase = m_FetchReceiptPayloadOnPurchase;
		ProductCatalog productCatalog = ProductCatalog.LoadDefaultCatalog();
		foreach (ProductCatalogItem allValidProduct in productCatalog.allValidProducts)
		{
			if (allValidProduct.allStoreIDs.Count > 0)
			{
				IDs ds = new IDs();
				foreach (StoreID allStoreID in allValidProduct.allStoreIDs)
				{
					ds.Add(allStoreID.id, allStoreID.store);
				}
				builder.AddProduct(allValidProduct.id, allValidProduct.type, ds);
			}
			else
			{
				builder.AddProduct(allValidProduct.id, allValidProduct.type);
			}
		}
		builder.AddProduct("100.gold.coins", ProductType.Consumable, new IDs
		{
			{ "100.gold.coins.mac", "MacAppStore" },
			{ "000000596586", "TizenStore" },
			{ "com.ff", "MoolahAppStore" }
		});
		builder.AddProduct("500.gold.coins", ProductType.Consumable, new IDs
		{
			{ "500.gold.coins.mac", "MacAppStore" },
			{ "000000596581", "TizenStore" },
			{ "com.ee", "MoolahAppStore" }
		});
		builder.AddProduct("sword", ProductType.NonConsumable, new IDs
		{
			{ "sword.mac", "MacAppStore" },
			{ "000000596583", "TizenStore" }
		});
		builder.AddProduct("sub9", ProductType.Subscription, new IDs { { "sub9", "MacAppStore" } });
		builder.AddProduct("sub4", ProductType.Subscription, new IDs { { "com.unity3d.unityiap.unityiapdemo.subscription.7", "MacAppStore" } });
		builder.Configure<IAmazonConfiguration>().WriteSandboxJSON(builder.products);
		builder.Configure<ISamsungAppsConfiguration>().SetMode(SamsungAppsMode.AlwaysSucceed);
		m_IsSamsungAppsStoreSelected = Application.platform == RuntimePlatform.Android && standardPurchasingModule.appStore == AppStore.SamsungApps;
		builder.Configure<ITizenStoreConfiguration>().SetGroupId("100000085616");
		Action initializeUnityIap = delegate
		{
			UnityPurchasing.Initialize(this, builder);
		};
		if (!m_IsUnityChannelSelected)
		{
			initializeUnityIap();
			return;
		}
		AppInfo appInfo = new AppInfo();
		appInfo.appId = "abc123appId";
		appInfo.appKey = "efg456appKey";
		appInfo.clientId = "hij789clientId";
		appInfo.clientKey = "klm012clientKey";
		appInfo.debug = false;
		unityChannelLoginHandler = new UnityChannelLoginHandler();
		unityChannelLoginHandler.initializeFailedAction = delegate(string message)
		{
			Debug.LogError("Failed to initialize and login to UnityChannel: " + message);
		};
		unityChannelLoginHandler.initializeSucceededAction = delegate
		{
			initializeUnityIap();
		};
		StoreService.Initialize(appInfo, unityChannelLoginHandler);
	}

	private void OnTransactionsRestored(bool success)
	{
		Debug.Log("Transactions restored.");
	}

	private void OnDeferred(Product item)
	{
		Debug.Log("Purchase deferred: " + item.definition.id);
	}

	private void InitUI(IEnumerable<Product> items)
	{
		restoreButton.gameObject.SetActive(NeedRestoreButton());
		loginButton.gameObject.SetActive(NeedLoginButton());
		validateButton.gameObject.SetActive(NeedValidateButton());
		ClearProductUIs();
		restoreButton.onClick.AddListener(RestoreButtonClick);
		loginButton.onClick.AddListener(LoginButtonClick);
		validateButton.onClick.AddListener(ValidateButtonClick);
		versionText.text = "Unity version: " + Application.unityVersion + "\nIAP version: 1.19.0";
	}

	public void PurchaseButtonClick(string productID)
	{
		if (m_PurchaseInProgress)
		{
			Debug.Log("Please wait, purchase in progress");
			return;
		}
		if (m_Controller == null)
		{
			Debug.LogError("Purchasing is not initialized");
			return;
		}
		if (m_Controller.products.WithID(productID) == null)
		{
			Debug.LogError("No product has id " + productID);
			return;
		}
		if (NeedLoginButton() && !m_IsLoggedIn)
		{
			Debug.LogWarning("Purchase notifications will not be forwarded server-to-server. Login incomplete.");
		}
		m_PurchaseInProgress = true;
		m_Controller.InitiatePurchase(m_Controller.products.WithID(productID), "aDemoDeveloperPayload");
	}

	public void RestoreButtonClick()
	{
		if (m_IsCloudMoolahStoreSelected)
		{
			if (!m_IsLoggedIn)
			{
				Debug.LogError("CloudMoolah purchase restoration aborted. Login incomplete.");
				return;
			}
			m_MoolahExtensions.RestoreTransactionID(delegate(RestoreTransactionIDState restoreTransactionIDState)
			{
				Debug.Log("restoreTransactionIDState = " + restoreTransactionIDState);
				bool success = restoreTransactionIDState != RestoreTransactionIDState.RestoreFailed && restoreTransactionIDState != RestoreTransactionIDState.NotKnown;
				OnTransactionsRestored(success);
			});
		}
		else if (m_IsSamsungAppsStoreSelected)
		{
			m_SamsungExtensions.RestoreTransactions(OnTransactionsRestored);
		}
		else if (Application.platform == RuntimePlatform.MetroPlayerX86 || Application.platform == RuntimePlatform.MetroPlayerX64 || Application.platform == RuntimePlatform.MetroPlayerARM)
		{
			m_MicrosoftExtensions.RestoreTransactions();
		}
		else
		{
			m_AppleExtensions.RestoreTransactions(OnTransactionsRestored);
		}
	}

	public void LoginButtonClick()
	{
		if (!m_IsUnityChannelSelected)
		{
			Debug.Log("Login is only required for the Xiaomi store");
			return;
		}
		unityChannelLoginHandler.loginSucceededAction = delegate(UserInfo userInfo)
		{
			m_IsLoggedIn = true;
			Debug.LogFormat("Succeeded logging into UnityChannel. channel {0}, userId {1}, userLoginToken {2} ", userInfo.channel, userInfo.userId, userInfo.userLoginToken);
		};
		unityChannelLoginHandler.loginFailedAction = delegate(string message)
		{
			m_IsLoggedIn = false;
			Debug.LogError("Failed logging into UnityChannel. " + message);
		};
		StoreService.Login(unityChannelLoginHandler);
	}

	public void ValidateButtonClick()
	{
		if (!m_IsUnityChannelSelected)
		{
			Debug.Log("Remote purchase validation is only supported for the Xiaomi store");
			return;
		}
		string txId = m_LastTransactionID;
		m_UnityChannelExtensions.ValidateReceipt(txId, delegate(bool success, string signData, string signature)
		{
			Debug.LogFormat("ValidateReceipt transactionId {0}, success {1}, signData {2}, signature {3}", txId, success, signData, signature);
		});
	}

	private void ClearProductUIs()
	{
		foreach (KeyValuePair<string, IAPDemoProductUI> productUI in m_ProductUIs)
		{
			UnityEngine.Object.Destroy(productUI.Value.gameObject);
		}
		m_ProductUIs.Clear();
	}

	private void AddProductUIs(Product[] products)
	{
		ClearProductUIs();
		RectTransform component = productUITemplate.GetComponent<RectTransform>();
		float height = component.rect.height;
		Vector3 localPosition = component.localPosition;
		contentRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (float)products.Length * height);
		foreach (Product product in products)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(productUITemplate.gameObject);
			gameObject.transform.SetParent(productUITemplate.transform.parent, false);
			RectTransform component2 = gameObject.GetComponent<RectTransform>();
			component2.localPosition = localPosition;
			localPosition += Vector3.down * height;
			gameObject.SetActive(true);
			IAPDemoProductUI component3 = gameObject.GetComponent<IAPDemoProductUI>();
			component3.SetProduct(product, PurchaseButtonClick);
			m_ProductUIs[product.definition.id] = component3;
		}
	}

	private void UpdateProductUI(Product p)
	{
		if (m_ProductUIs.ContainsKey(p.definition.id))
		{
			m_ProductUIs[p.definition.id].SetProduct(p, PurchaseButtonClick);
		}
	}

	private void UpdateProductPendingUI(Product p, int secondsRemaining)
	{
		if (m_ProductUIs.ContainsKey(p.definition.id))
		{
			m_ProductUIs[p.definition.id].SetPendingTime(secondsRemaining);
		}
	}

	private bool NeedRestoreButton()
	{
		return Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.tvOS || Application.platform == RuntimePlatform.MetroPlayerX86 || Application.platform == RuntimePlatform.MetroPlayerX64 || Application.platform == RuntimePlatform.MetroPlayerARM || m_IsSamsungAppsStoreSelected || m_IsCloudMoolahStoreSelected;
	}

	private bool NeedLoginButton()
	{
		return m_IsUnityChannelSelected;
	}

	private bool NeedValidateButton()
	{
		return m_IsUnityChannelSelected;
	}

	private void LogProductDefinitions()
	{
		Product[] all = m_Controller.products.all;
		Product[] array = all;
		foreach (Product product in array)
		{
			Debug.Log(string.Format("id: {0}\nstore-specific id: {1}\ntype: {2}\nenabled: {3}\n", product.definition.id, product.definition.storeSpecificId, product.definition.type.ToString(), (!product.definition.enabled) ? "disabled" : "enabled"));
		}
	}
}
