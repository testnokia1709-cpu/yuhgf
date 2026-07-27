namespace Amazon.CognitoIdentity.Model
{
	public class MappingRule
	{
		private string _claim;

		private MappingRuleMatchType _matchType;

		private string _roleARN;

		private string _value;

		public string Claim
		{
			get
			{
				return _claim;
			}
			set
			{
				_claim = value;
			}
		}

		public MappingRuleMatchType MatchType
		{
			get
			{
				return _matchType;
			}
			set
			{
				_matchType = value;
			}
		}

		public string RoleARN
		{
			get
			{
				return _roleARN;
			}
			set
			{
				_roleARN = value;
			}
		}

		public string Value
		{
			get
			{
				return _value;
			}
			set
			{
				_value = value;
			}
		}

		internal bool IsSetClaim()
		{
			return _claim != null;
		}

		internal bool IsSetMatchType()
		{
			return _matchType != null;
		}

		internal bool IsSetRoleARN()
		{
			return _roleARN != null;
		}

		internal bool IsSetValue()
		{
			return _value != null;
		}
	}
}
