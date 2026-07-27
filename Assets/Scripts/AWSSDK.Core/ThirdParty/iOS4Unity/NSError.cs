using System;

namespace ThirdParty.iOS4Unity
{
	public class NSError : NSObject
	{
		private static readonly IntPtr _classHandle;

		public override IntPtr ClassHandle
		{
			get
			{
				return _classHandle;
			}
		}

		public int Code
		{
			get
			{
				return ObjC.MessageSendInt(Handle, Selector.GetHandle("code"));
			}
		}

		public string Domain
		{
			get
			{
				return ObjC.MessageSendString(Handle, Selector.GetHandle("domain"));
			}
		}

		public string LocalizedDescription
		{
			get
			{
				return ObjC.MessageSendString(Handle, Selector.GetHandle("localizedDescription"));
			}
		}

		static NSError()
		{
			_classHandle = ObjC.GetClass("NSError");
		}

		public NSError(string domain, int code)
		{
			ObjC.MessageSend(Handle, Selector.GetHandle("initWithDomain:code:userInfo:"), domain, code, IntPtr.Zero);
		}

		internal NSError(IntPtr handle)
			: base(handle)
		{
		}

		public override string ToString()
		{
			return LocalizedDescription;
		}
	}
}
