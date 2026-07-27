using Amazon.Runtime;

namespace Amazon.S3
{
	public sealed class OwnerOverride : ConstantClass
	{
		public static readonly OwnerOverride Destination = new OwnerOverride("Destination");

		public OwnerOverride(string value)
			: base(value)
		{
		}

		public static OwnerOverride FindValue(string value)
		{
			return ConstantClass.FindValue<OwnerOverride>(value);
		}

		public static implicit operator OwnerOverride(string value)
		{
			return FindValue(value);
		}
	}
}
