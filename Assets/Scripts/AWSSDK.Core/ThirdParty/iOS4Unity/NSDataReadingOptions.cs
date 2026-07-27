using System;

namespace ThirdParty.iOS4Unity
{
	[Flags]
	[CLSCompliant(false)]
	public enum NSDataReadingOptions : uint
	{
		Mapped = 1u,
		Uncached = 2u,
		Coordinated = 4u,
		MappedAlways = 8u
	}
}
