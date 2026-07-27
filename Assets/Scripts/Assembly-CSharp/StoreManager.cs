using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Purchasing;

public class StoreManager : MonoBehaviour, IStoreListener
{
	public static StoreManager Instance;

	public const string PRODUCT_1_NO_ADS = "product_1_no_ads";

	public const string PRODUCT_2_PUZZLE_PACK_2 = "product_2_puzzle_pack";

	public const string PRODUCT_3_PUZZLE_PACK_3 = "product_3_puzzle_pack";

	public const string PRODUCT_4_PUZZLE_PACK_4 = "product_4_puzzle_pack";

	public const string PRODUCT_5_PUZZLE_PACK_5 = "product_5_puzzle_pack";

	public const string PRODUCT_6_FULL_GAME = "product_6_full_game";

	public const string PRODUCT_7_FULL_GAME_DISCOUNT_25 = "product_7_full_game_discount_25";

	public const string PRODUCT_8_FULL_GAME_DISCOUNT_50 = "product_8_full_game_discount_50";

	public const string PRODUCT_9_FULL_GAME_DISCOUNT_75 = "product_9_full_game_discount_75";

	public const string PRODUCT_10_PUZZLE_PACK_6 = "product_10_puzzle_pack";

	public const string PRODUCT_11_PUZZLE_PACK_7 = "product_11_puzzle_pack";

	public const string PRODUCT_12_PUZZLE_PACK_8 = "product_12_puzzle_pack";

	public const string PRODUCT_13_PUZZLE_PACK_9 = "product_13_puzzle_pack";

	public const string PRODUCT_14_PUZZLE_PACK_10 = "product_14_puzzle_pack";

	public static List<string> NONCONSUMABLE_LIST = new List<string>
	{
		"product_1_no_ads", "product_2_puzzle_pack", "product_3_puzzle_pack", "product_4_puzzle_pack", "product_5_puzzle_pack", "product_6_full_game", "product_7_full_game_discount_25", "product_8_full_game_discount_50", "product_9_full_game_discount_75", "product_10_puzzle_pack",
		"product_11_puzzle_pack", "product_12_puzzle_pack", "product_13_puzzle_pack", "product_14_puzzle_pack"
	};

	public static Dictionary<string, string> LegacyItemIdToProductId = new Dictionary<string, string>
	{
		{ "item_1_no_ads", "product_1_no_ads" },
		{ "item_2_puzzle_pack", "product_2_puzzle_pack" },
		{ "item_3_puzzle_pack", "product_3_puzzle_pack" },
		{ "item_4_puzzle_pack", "product_4_puzzle_pack" },
		{ "item_5_puzzle_pack", "product_5_puzzle_pack" },
		{ "item_6_full_game", "product_6_full_game" },
		{ "item_7_full_game_discount_25", "product_7_full_game_discount_25" },
		{ "item_8_full_game_discount_50", "product_8_full_game_discount_50" },
		{ "item_9_full_game_discount_75", "product_9_full_game_discount_75" },
		{ "item_10_puzzle_pack", "product_10_puzzle_pack" },
		{ "item_11_puzzle_pack", "product_11_puzzle_pack" },
		{ "item_12_puzzle_pack", "product_12_puzzle_pack" },
		{ "item_13_puzzle_pack", "product_13_puzzle_pack" },
		{ "item_14_puzzle_pack", "product_14_puzzle_pack" }
	};

	public Action OnInitializedComplete;

	public bool InEditorGameOwned;

	private IStoreController m_controller;

	private IExtensionProvider m_extensions;

	private Action<bool, bool, PurchasableItem, int> m_purchaseCallback;

	public void Awake()
	{
		if (Instance != null)
		{
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		Instance = this;
		UnityEngine.Object.DontDestroyOnLoad(this);
	}

	public void Start()
	{
		Initialize();
		legacyConversion();
	}

	public void Initialize()
	{
		ConfigurationBuilder configurationBuilder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
		foreach (string item in NONCONSUMABLE_LIST)
		{
			configurationBuilder.AddProduct(item, ProductType.NonConsumable, new IDs
			{
				{ item, "GooglePlay" },
				{ item, "MacAppStore" },
				{ item, "AmazonApps" }
			});
		}
		UnityPurchasing.Initialize(this, configurationBuilder);
		Debug.Log("Current Purchase Status:");
		foreach (KeyValuePair<string, bool> purchase in DataStore.Instance.Purchases)
		{
			Debug.Log("Saved Purchase: " + purchase.Key);
		}
		foreach (KeyValuePair<string, bool> freeItem in DataStore.Instance.FreeItems)
		{
			Debug.Log("Gifted Purchase: " + freeItem.Key);
		}
	}

	public bool IsInitialized()
	{
		return m_controller != null && m_extensions != null;
	}

	public string GetProductId(PurchasableItem item)
	{
		switch (item)
		{
		case PurchasableItem.NO_ADS:
			return "product_1_no_ads";
		case PurchasableItem.PACK_2:
			return "product_2_puzzle_pack";
		case PurchasableItem.PACK_3:
			return "product_3_puzzle_pack";
		case PurchasableItem.PACK_4:
			return "product_4_puzzle_pack";
		case PurchasableItem.PACK_5:
			return "product_5_puzzle_pack";
		case PurchasableItem.PACK_6:
			return "product_10_puzzle_pack";
		case PurchasableItem.PACK_7:
			return "product_11_puzzle_pack";
		case PurchasableItem.PACK_8:
			return "product_12_puzzle_pack";
		case PurchasableItem.PACK_9:
			return "product_13_puzzle_pack";
		case PurchasableItem.PACK_10:
			return "product_14_puzzle_pack";
		case PurchasableItem.FULL_GAME_25:
			return "product_7_full_game_discount_25";
		case PurchasableItem.FULL_GAME_50:
			return "product_8_full_game_discount_50";
		case PurchasableItem.FULL_GAME_75:
			return "product_9_full_game_discount_75";
		case PurchasableItem.FULL_GAME_100:
			return "product_6_full_game";
		default:
			return string.Empty;
		}
	}

	public PurchasableItem GetPurchasableItem(string productId)
	{
		switch (productId)
		{
		case "product_1_no_ads":
			return PurchasableItem.NO_ADS;
		case "product_2_puzzle_pack":
			return PurchasableItem.PACK_2;
		case "product_3_puzzle_pack":
			return PurchasableItem.PACK_3;
		case "product_4_puzzle_pack":
			return PurchasableItem.PACK_4;
		case "product_5_puzzle_pack":
			return PurchasableItem.PACK_5;
		case "product_10_puzzle_pack":
			return PurchasableItem.PACK_6;
		case "product_11_puzzle_pack":
			return PurchasableItem.PACK_7;
		case "product_12_puzzle_pack":
			return PurchasableItem.PACK_8;
		case "product_13_puzzle_pack":
			return PurchasableItem.PACK_9;
		case "product_14_puzzle_pack":
			return PurchasableItem.PACK_10;
		case "product_7_full_game_discount_25":
			return PurchasableItem.FULL_GAME_25;
		case "product_8_full_game_discount_50":
			return PurchasableItem.FULL_GAME_50;
		case "product_9_full_game_discount_75":
			return PurchasableItem.FULL_GAME_75;
		case "product_6_full_game":
			return PurchasableItem.FULL_GAME_100;
		default:
			return PurchasableItem.INVALID;
		}
	}

	public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
	{
		m_controller = controller;
		m_extensions = extensions;
		Debug.Log("StoreManager.OnInitialized");
		Product[] all = m_controller.products.all;
		foreach (Product product in all)
		{
			if (product.hasReceipt)
			{
				Debug.Log("Restoring purchase: " + product.definition.id);
				recordPurchase(product);
			}
		}
		if (OnInitializedComplete != null)
		{
			OnInitializedComplete();
		}
	}

	public void OnInitializeFailed(InitializationFailureReason error)
	{
		Debug.Log("Store Initialization Failed: " + error);
	}

	public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
	{
		Product purchasedProduct = e.purchasedProduct;
		Debug.Log("StoreManager.ProcessProcessingResult: " + purchasedProduct.definition.id);
		recordPurchase(purchasedProduct);
		if (m_purchaseCallback != null)
		{
			PurchasableItem purchasableItem = GetPurchasableItem(purchasedProduct.definition.id);
			m_purchaseCallback(true, false, purchasableItem, -1);
		}
		return PurchaseProcessingResult.Complete;
	}

	public void OnPurchaseFailed(Product i, PurchaseFailureReason p)
	{
		Debug.Log("StoreManager.OnPurchaseFailed: " + p);
		if (m_purchaseCallback != null)
		{
			PurchasableItem purchasableItem = GetPurchasableItem(i.definition.id);
			bool arg = false;
			if (p == PurchaseFailureReason.UserCancelled)
			{
				arg = true;
			}
			m_purchaseCallback(false, arg, purchasableItem, (int)p);
		}
	}

	public void Purchase(PurchasableItem item, Action<bool, bool, PurchasableItem, int> callback)
	{
		string productId = GetProductId(item);
		if (IsInitialized())
		{
			m_purchaseCallback = callback;
			Product product = m_controller.products.WithID(productId);
			if (product.hasReceipt)
			{
				Debug.LogError("Product is already owned, returning success.");
				recordPurchase(product);
				if (m_purchaseCallback != null)
				{
					m_purchaseCallback(true, false, item, -1);
				}
			}
			else if (product.availableToPurchase)
			{
				Debug.Log("Purchasing: " + product.definition.id);
				m_controller.InitiatePurchase(productId);
			}
			else
			{
				Debug.LogError("Product is invalid or not available for purchase.");
			}
		}
		else
		{
			Debug.LogError("Trying to make a purchase when not initialized!");
		}
	}

	public void RestorePurchases(Action<bool> callback)
	{
		m_purchaseCallback = null;
		if (!IsInitialized())
		{
			Debug.Log("Trying to restore when store is not initialized.");
			if (callback != null)
			{
				callback(false);
			}
		}
		else if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.OSXPlayer)
		{
			m_extensions.GetExtension<IAppleExtensions>().RestoreTransactions(delegate(bool result)
			{
				if (!result)
				{
					Debug.LogError("Failed to restore purchases.");
				}
				if (callback != null)
				{
					callback(result);
				}
			});
		}
		else if (callback != null)
		{
			callback(true);
		}
	}

	public bool HasPurchaseBeenMade()
	{
		Debug.Log("HasPurchaseBeenMade(): " + DataStore.Instance.Purchases.Count);
		return DataStore.Instance.Purchases.Count > 0;
	}

	public bool CheckIfPurchased(PurchasableItem item)
	{
		string productId = GetProductId(item);
		return DataStore.Instance.Purchases.ContainsKey(productId);
	}

	public bool CheckIfOwned(PurchasableItem item)
	{
		string productId = GetProductId(item);
		return DataStore.Instance.FreeItems.ContainsKey(productId) || DataStore.Instance.Purchases.ContainsKey(productId);
	}

	public bool IsGameOwned()
	{
		return CheckIfPurchased(PurchasableItem.FULL_GAME_100) || CheckIfPurchased(PurchasableItem.FULL_GAME_25) || CheckIfPurchased(PurchasableItem.FULL_GAME_50) || CheckIfPurchased(PurchasableItem.FULL_GAME_75);
	}

	public void Gift(PurchasableItem item)
	{
		string productId = GetProductId(item);
		Debug.Log("Gifting product: " + productId);
		recordGift(productId);
		AnalyticsManager.LogEvent("Store", "Item_Gifted", productId, 1L);
	}

	public string GetProductName(PurchasableItem item)
	{
		string productId = GetProductId(item);
		string text = string.Empty;
		if (string.IsNullOrEmpty(productId))
		{
			text = LevelManager.GetPack(item).Name;
		}
		else if (m_controller != null)
		{
			text = m_controller.products.WithID(productId).metadata.localizedTitle;
			if (text.Contains("("))
			{
				int length = text.IndexOf("(");
				text = text.Substring(0, length).Trim();
			}
		}
		return text;
	}

	private void legacyConversion()
	{
		int num = 0;
		Debug.Log("Legacy Purchase conversion...");
		List<string> list = new List<string>();
		List<string> list2 = new List<string>();
		foreach (KeyValuePair<string, bool> purchase in DataStore.Instance.Purchases)
		{
			if (LegacyItemIdToProductId.ContainsKey(purchase.Key))
			{
				list.Add(LegacyItemIdToProductId[purchase.Key]);
				list2.Add(purchase.Key);
			}
		}
		num += list2.Count();
		foreach (string item in list2)
		{
			Debug.Log("Removing: " + item);
			DataStore.Instance.Purchases.Remove(item);
		}
		foreach (string item2 in list)
		{
			Debug.Log("Adding: " + item2);
			if (!DataStore.Instance.Purchases.ContainsKey(item2))
			{
				DataStore.Instance.Purchases.Add(item2, true);
			}
			else
			{
				DataStore.Instance.Purchases[item2] = true;
			}
		}
		Debug.Log("Legacy FreeItem conversion...");
		list = new List<string>();
		list2 = new List<string>();
		foreach (KeyValuePair<string, bool> freeItem in DataStore.Instance.FreeItems)
		{
			if (LegacyItemIdToProductId.ContainsKey(freeItem.Key))
			{
				list.Add(LegacyItemIdToProductId[freeItem.Key]);
				list2.Add(freeItem.Key);
			}
		}
		num += list2.Count();
		foreach (string item3 in list2)
		{
			Debug.Log("Removing: " + item3);
			DataStore.Instance.FreeItems.Remove(item3);
		}
		foreach (string item4 in list)
		{
			Debug.Log("Adding: " + item4);
			if (!DataStore.Instance.FreeItems.ContainsKey(item4))
			{
				DataStore.Instance.FreeItems.Add(item4, true);
			}
			else
			{
				DataStore.Instance.FreeItems[item4] = true;
			}
		}
		if (num > 0)
		{
			DataStore.Save();
		}
	}

	private void recordPurchase(Product product)
	{
		if (!DataStore.Instance.Purchases.ContainsKey(product.definition.id))
		{
			DataStore.Instance.Purchases.Add(product.definition.id, true);
		}
		else
		{
			DataStore.Instance.Purchases[product.definition.id] = true;
		}
	}

	private void recordGift(string productId)
	{
		if (!DataStore.Instance.FreeItems.ContainsKey(productId))
		{
			DataStore.Instance.FreeItems.Add(productId, true);
		}
		else
		{
			DataStore.Instance.FreeItems[productId] = true;
		}
	}
}
