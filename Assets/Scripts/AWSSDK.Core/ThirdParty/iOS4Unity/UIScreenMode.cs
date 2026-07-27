using System;

namespace ThirdParty.iOS4Unity
{
	public class UIScreenMode : NSObject
	{
		private static readonly IntPtr _classHandle;

		public override IntPtr ClassHandle
		{
			get
			{
				return _classHandle;
			}
		}

		public float PixelAspectRatio
		{
			get
			{
				return ObjC.MessageSendFloat(Handle, Selector.GetHandle("pixelAspectRatio"));
			}
		}

		public CGSize Size
		{
			get
			{
				return ObjC.MessageSendCGSize(Handle, "size");
			}
		}

		static UIScreenMode()
		{
			_classHandle = ObjC.GetClass("UIScreenMode");
		}

		internal UIScreenMode(IntPtr handle)
			: base(handle)
		{
		}
	}
}
