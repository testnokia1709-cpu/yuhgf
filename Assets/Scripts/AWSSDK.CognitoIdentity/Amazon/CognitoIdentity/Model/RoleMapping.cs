namespace Amazon.CognitoIdentity.Model
{
	public class RoleMapping
	{
		private AmbiguousRoleResolutionType _ambiguousRoleResolution;

		private RulesConfigurationType _rulesConfiguration;

		private RoleMappingType _type;

		public AmbiguousRoleResolutionType AmbiguousRoleResolution
		{
			get
			{
				return _ambiguousRoleResolution;
			}
			set
			{
				_ambiguousRoleResolution = value;
			}
		}

		public RulesConfigurationType RulesConfiguration
		{
			get
			{
				return _rulesConfiguration;
			}
			set
			{
				_rulesConfiguration = value;
			}
		}

		public RoleMappingType Type
		{
			get
			{
				return _type;
			}
			set
			{
				_type = value;
			}
		}

		internal bool IsSetAmbiguousRoleResolution()
		{
			return _ambiguousRoleResolution != null;
		}

		internal bool IsSetRulesConfiguration()
		{
			return _rulesConfiguration != null;
		}

		internal bool IsSetType()
		{
			return _type != null;
		}
	}
}
