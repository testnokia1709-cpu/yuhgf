using System.Collections.Generic;

namespace Amazon.CognitoIdentity.Model
{
	public class RulesConfigurationType
	{
		private List<MappingRule> _rules = new List<MappingRule>();

		public List<MappingRule> Rules
		{
			get
			{
				return _rules;
			}
			set
			{
				_rules = value;
			}
		}

		internal bool IsSetRules()
		{
			if (_rules != null)
			{
				return _rules.Count > 0;
			}
			return false;
		}
	}
}
