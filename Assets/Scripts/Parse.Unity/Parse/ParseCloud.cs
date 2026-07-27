using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Parse.Internal;

namespace Parse
{
	public static class ParseCloud
	{
		internal static IParseCloudCodeController CloudCodeController
		{
			get
			{
				return ParseCorePlugins.Instance.CloudCodeController;
			}
		}

		public static Task<T> CallFunctionAsync<T>(string name, IDictionary<string, object> parameters)
		{
			return CallFunctionAsync<T>(name, parameters, CancellationToken.None);
		}

		public static Task<T> CallFunctionAsync<T>(string name, IDictionary<string, object> parameters, CancellationToken cancellationToken)
		{
			return CloudCodeController.CallFunctionAsync<T>(name, parameters, ParseUser.CurrentSessionToken, cancellationToken);
		}
	}
}
