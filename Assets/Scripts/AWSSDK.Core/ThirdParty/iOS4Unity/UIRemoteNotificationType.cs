using System;

namespace ThirdParty.iOS4Unity
{
	[Flags]
	public enum UIRemoteNotificationType
	{
		None = 0,
		Badge = 1,
		Sound = 2,
		Alert = 4,
		NewsstandContentAvailability = 8
	}
}
