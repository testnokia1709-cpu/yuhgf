using Amazon.Runtime;

namespace Amazon.CognitoIdentity
{
	public class AmbiguousRoleResolutionType : ConstantClass
	{
		public static readonly AmbiguousRoleResolutionType AuthenticatedRole = new AmbiguousRoleResolutionType("AuthenticatedRole");

		public static readonly AmbiguousRoleResolutionType Deny = new AmbiguousRoleResolutionType("Deny");

		public AmbiguousRoleResolutionType(string value)
			: base(value)
		{
		}

		public static AmbiguousRoleResolutionType FindValue(string value)
		{
			return ConstantClass.FindValue<AmbiguousRoleResolutionType>(value);
		}

		public static implicit operator AmbiguousRoleResolutionType(string value)
		{
			return FindValue(value);
		}
	}
}
