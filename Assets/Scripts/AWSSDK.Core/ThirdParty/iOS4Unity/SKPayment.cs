using System;

namespace ThirdParty.iOS4Unity
{
	public class SKPayment : NSObject
	{
		private static readonly IntPtr _classHandle;

		public override IntPtr ClassHandle
		{
			get
			{
				return _classHandle;
			}
		}

		public string ApplicationUsername
		{
			get
			{
				return ObjC.MessageSendString(Handle, Selector.GetHandle("applicationUsername"));
			}
		}

		public string ProductIdentifier
		{
			get
			{
				return ObjC.MessageSendString(Handle, Selector.GetHandle("productIdentifier"));
			}
		}

		public int Quantity
		{
			get
			{
				return ObjC.MessageSendInt(Handle, Selector.GetHandle("quantity"));
			}
		}

		static SKPayment()
		{
			_classHandle = ObjC.GetClass("SKPayment");
		}

		internal SKPayment(IntPtr handle)
			: base(handle)
		{
		}

		public static SKPayment PaymentWithProduct(SKProduct product)
		{
			return Runtime.GetNSObject<SKPayment>(ObjC.MessageSendIntPtr(_classHandle, Selector.GetHandle("paymentWithProduct:"), product.Handle));
		}
	}
}
