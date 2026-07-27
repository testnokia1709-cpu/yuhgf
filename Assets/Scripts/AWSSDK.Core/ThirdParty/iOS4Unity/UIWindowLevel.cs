namespace ThirdParty.iOS4Unity
{
	public static class UIWindowLevel
	{
		public static float Alert
		{
			get
			{
				return ObjC.GetFloatConstant(ObjC.Libraries.UIKit, "UIWindowLevelAlert");
			}
		}

		public static float Normal
		{
			get
			{
				return ObjC.GetFloatConstant(ObjC.Libraries.UIKit, "UIWindowLevelNormal");
			}
		}

		public static float StatusBar
		{
			get
			{
				return ObjC.GetFloatConstant(ObjC.Libraries.UIKit, "UIWindowLevelStatusBar");
			}
		}
	}
}
