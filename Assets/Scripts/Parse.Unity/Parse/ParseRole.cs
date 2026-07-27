using System;
using System.Text.RegularExpressions;

namespace Parse
{
	[ParseClassName("_Role")]
	public class ParseRole : ParseObject
	{
		private static readonly Regex namePattern = new Regex("^[0-9a-zA-Z_\\- ]+$");

		[ParseFieldName("name")]
		public string Name
		{
			get
			{
				return GetProperty<string>("Name");
			}
			set
			{
				SetProperty(value, "Name");
			}
		}

		[ParseFieldName("users")]
		public ParseRelation<ParseUser> Users
		{
			get
			{
				return GetRelationProperty<ParseUser>("Users");
			}
		}

		[ParseFieldName("roles")]
		public ParseRelation<ParseRole> Roles
		{
			get
			{
				return GetRelationProperty<ParseRole>("Roles");
			}
		}

		public static ParseQuery<ParseRole> Query
		{
			get
			{
				return new ParseQuery<ParseRole>();
			}
		}

		public ParseRole()
		{
		}

		public ParseRole(string name, ParseACL acl)
			: this()
		{
			Name = name;
			base.ACL = acl;
		}

		internal override void OnSettingValue(ref string key, ref object value)
		{
			base.OnSettingValue(ref key, ref value);
			if (key == "name")
			{
				if (base.ObjectId != null)
				{
					throw new InvalidOperationException("A role's name can only be set before it has been saved.");
				}
				if (!(value is string))
				{
					throw new ArgumentException("A role's name must be a string.", "value");
				}
				if (!namePattern.IsMatch((string)value))
				{
					throw new ArgumentException("A role's name can only contain alphanumeric characters, _, -, and spaces.", "value");
				}
			}
		}
	}
}
