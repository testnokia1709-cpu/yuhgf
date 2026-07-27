using Amazon.Runtime;

namespace Amazon.CognitoIdentity
{
	public class MappingRuleMatchType : ConstantClass
	{
		public static readonly MappingRuleMatchType Contains = new MappingRuleMatchType("Contains");

		public new static readonly MappingRuleMatchType Equals = new MappingRuleMatchType("Equals");

		public static readonly MappingRuleMatchType NotEqual = new MappingRuleMatchType("NotEqual");

		public static readonly MappingRuleMatchType StartsWith = new MappingRuleMatchType("StartsWith");

		public MappingRuleMatchType(string value)
			: base(value)
		{
		}

		public static MappingRuleMatchType FindValue(string value)
		{
			return ConstantClass.FindValue<MappingRuleMatchType>(value);
		}

		public static implicit operator MappingRuleMatchType(string value)
		{
			return FindValue(value);
		}
	}
}
