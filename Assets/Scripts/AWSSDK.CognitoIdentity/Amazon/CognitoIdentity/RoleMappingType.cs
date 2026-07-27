using Amazon.Runtime;

namespace Amazon.CognitoIdentity
{
	public class RoleMappingType : ConstantClass
	{
		public static readonly RoleMappingType Rules = new RoleMappingType("Rules");

		public static readonly RoleMappingType Token = new RoleMappingType("Token");

		public RoleMappingType(string value)
			: base(value)
		{
		}

		public static RoleMappingType FindValue(string value)
		{
			return ConstantClass.FindValue<RoleMappingType>(value);
		}

		public static implicit operator RoleMappingType(string value)
		{
			return FindValue(value);
		}
	}
}
