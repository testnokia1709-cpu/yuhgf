using System;

namespace Amazon.Runtime.Internal.Util
{
	public static class GuidUtils
	{
		public static bool TryParseNullableGuid(string value, out Guid? result)
		{
			Guid result2;
			if (TryParseGuid(value, out result2))
			{
				result = result2;
				return true;
			}
			result = null;
			return false;
		}

		public static bool TryParseGuid(string value, out Guid result)
		{
			try
			{
				result = new Guid(value);
				return true;
			}
			catch (Exception)
			{
				result = Guid.Empty;
				return false;
			}
		}
	}
}
