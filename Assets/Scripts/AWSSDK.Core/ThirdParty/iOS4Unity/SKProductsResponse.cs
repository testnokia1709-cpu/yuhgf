using System;

namespace ThirdParty.iOS4Unity
{
	public sealed class SKProductsResponse : NSObject
	{
		private static readonly IntPtr _classHandle;

		public override IntPtr ClassHandle
		{
			get
			{
				return _classHandle;
			}
		}

		public string[] InvalidProducts
		{
			get
			{
				return ObjC.FromNSArray(ObjC.MessageSendIntPtr(Handle, Selector.GetHandle("invalidProductIdentifiers")));
			}
		}

		public SKProduct[] Products
		{
			get
			{
				return ObjC.FromNSArray<SKProduct>(ObjC.MessageSendIntPtr(Handle, Selector.GetHandle("products")));
			}
		}

		static SKProductsResponse()
		{
			_classHandle = ObjC.GetClass("SKProductsResponse");
		}

		internal SKProductsResponse(IntPtr handle)
			: base(handle)
		{
		}
	}
}
