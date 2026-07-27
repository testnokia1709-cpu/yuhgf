using System;

namespace ThirdParty.iOS4Unity
{
	public class NSNotification : NSObject
	{
		private static readonly IntPtr _classHandle;

		public override IntPtr ClassHandle
		{
			get
			{
				return _classHandle;
			}
		}

		public string Name
		{
			get
			{
				return ObjC.MessageSendString(Handle, Selector.GetHandle("name"));
			}
		}

		public NSObject Object
		{
			get
			{
				return Runtime.GetNSObject<NSObject>(ObjC.MessageSendIntPtr(Handle, Selector.GetHandle("object")));
			}
		}

		static NSNotification()
		{
			_classHandle = ObjC.GetClass("NSNotification");
		}

		public static NSNotification FromName(string name, NSObject obj = null)
		{
			return Runtime.GetNSObject<NSNotification>(ObjC.MessageSendIntPtr(_classHandle, Selector.GetHandle("notificationWithName:object:"), name, (obj == null) ? IntPtr.Zero : obj.Handle));
		}

		internal NSNotification(IntPtr handle)
			: base(handle)
		{
		}
	}
}
