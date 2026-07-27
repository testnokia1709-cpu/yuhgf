using System.Collections.Generic;
using Amazon.Runtime;

namespace Amazon.CognitoIdentity.Model
{
	public class GetIdentityPoolRolesResponse : AmazonWebServiceResponse
	{
		private string _identityPoolId;

		private Dictionary<string, RoleMapping> _roleMappings = new Dictionary<string, RoleMapping>();

		private Dictionary<string, string> _roles = new Dictionary<string, string>();

		public string IdentityPoolId
		{
			get
			{
				return _identityPoolId;
			}
			set
			{
				_identityPoolId = value;
			}
		}

		public Dictionary<string, RoleMapping> RoleMappings
		{
			get
			{
				return _roleMappings;
			}
			set
			{
				_roleMappings = value;
			}
		}

		public Dictionary<string, string> Roles
		{
			get
			{
				return _roles;
			}
			set
			{
				_roles = value;
			}
		}

		internal bool IsSetIdentityPoolId()
		{
			return _identityPoolId != null;
		}

		internal bool IsSetRoleMappings()
		{
			if (_roleMappings != null)
			{
				return _roleMappings.Count > 0;
			}
			return false;
		}

		internal bool IsSetRoles()
		{
			if (_roles != null)
			{
				return _roles.Count > 0;
			}
			return false;
		}
	}
}
