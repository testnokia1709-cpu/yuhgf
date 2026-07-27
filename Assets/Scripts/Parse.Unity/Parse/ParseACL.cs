using System;
using System.Collections.Generic;
using System.Linq;
using Parse.Internal;

namespace Parse
{
	public class ParseACL : IJsonConvertible
	{
		private enum AccessKind
		{
			Read = 0,
			Write = 1
		}

		private const string publicName = "*";

		private readonly ICollection<string> readers = new HashSet<string>();

		private readonly ICollection<string> writers = new HashSet<string>();

		public bool PublicReadAccess
		{
			get
			{
				return GetAccess(AccessKind.Read, "*");
			}
			set
			{
				SetAccess(AccessKind.Read, "*", value);
			}
		}

		public bool PublicWriteAccess
		{
			get
			{
				return GetAccess(AccessKind.Write, "*");
			}
			set
			{
				SetAccess(AccessKind.Write, "*", value);
			}
		}

		internal ParseACL(IDictionary<string, object> jsonObject)
		{
			readers = new HashSet<string>(from pair in jsonObject
				where ((IDictionary<string, object>)pair.Value).ContainsKey("read")
				select pair.Key);
			writers = new HashSet<string>(from pair in jsonObject
				where ((IDictionary<string, object>)pair.Value).ContainsKey("write")
				select pair.Key);
		}

		public ParseACL()
		{
		}

		public ParseACL(ParseUser owner)
		{
			SetReadAccess(owner, true);
			SetWriteAccess(owner, true);
		}

		IDictionary<string, object> IJsonConvertible.ToJSON()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			foreach (string item in readers.Union(writers))
			{
				Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
				if (readers.Contains(item))
				{
					dictionary2["read"] = true;
				}
				if (writers.Contains(item))
				{
					dictionary2["write"] = true;
				}
				dictionary[item] = dictionary2;
			}
			return dictionary;
		}

		private void SetAccess(AccessKind kind, string userId, bool allowed)
		{
			if (userId == null)
			{
				throw new ArgumentException("Cannot set access for an unsaved user or role.");
			}
			ICollection<string> collection = null;
			switch (kind)
			{
			case AccessKind.Read:
				collection = readers;
				break;
			case AccessKind.Write:
				collection = writers;
				break;
			default:
				throw new NotImplementedException("Unknown AccessKind");
			}
			if (allowed)
			{
				collection.Add(userId);
			}
			else
			{
				collection.Remove(userId);
			}
		}

		private bool GetAccess(AccessKind kind, string userId)
		{
			if (userId == null)
			{
				throw new ArgumentException("Cannot get access for an unsaved user or role.");
			}
			switch (kind)
			{
			case AccessKind.Read:
				return readers.Contains(userId);
			case AccessKind.Write:
				return writers.Contains(userId);
			default:
				throw new NotImplementedException("Unknown AccessKind");
			}
		}

		public void SetReadAccess(string userId, bool allowed)
		{
			SetAccess(AccessKind.Read, userId, allowed);
		}

		public void SetReadAccess(ParseUser user, bool allowed)
		{
			SetReadAccess(user.ObjectId, allowed);
		}

		public void SetWriteAccess(string userId, bool allowed)
		{
			SetAccess(AccessKind.Write, userId, allowed);
		}

		public void SetWriteAccess(ParseUser user, bool allowed)
		{
			SetWriteAccess(user.ObjectId, allowed);
		}

		public bool GetReadAccess(string userId)
		{
			return GetAccess(AccessKind.Read, userId);
		}

		public bool GetReadAccess(ParseUser user)
		{
			return GetReadAccess(user.ObjectId);
		}

		public bool GetWriteAccess(string userId)
		{
			return GetAccess(AccessKind.Write, userId);
		}

		public bool GetWriteAccess(ParseUser user)
		{
			return GetWriteAccess(user.ObjectId);
		}

		public void SetRoleReadAccess(string roleName, bool allowed)
		{
			SetAccess(AccessKind.Read, "role:" + roleName, allowed);
		}

		public void SetRoleReadAccess(ParseRole role, bool allowed)
		{
			SetRoleReadAccess(role.Name, allowed);
		}

		public bool GetRoleReadAccess(string roleName)
		{
			return GetAccess(AccessKind.Read, "role:" + roleName);
		}

		public bool GetRoleReadAccess(ParseRole role)
		{
			return GetRoleReadAccess(role.Name);
		}

		public void SetRoleWriteAccess(string roleName, bool allowed)
		{
			SetAccess(AccessKind.Write, "role:" + roleName, allowed);
		}

		public void SetRoleWriteAccess(ParseRole role, bool allowed)
		{
			SetRoleWriteAccess(role.Name, allowed);
		}

		public bool GetRoleWriteAccess(string roleName)
		{
			return GetAccess(AccessKind.Write, "role:" + roleName);
		}

		public bool GetRoleWriteAccess(ParseRole role)
		{
			return GetRoleWriteAccess(role.Name);
		}
	}
}
