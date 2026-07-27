using System.Collections.Generic;

namespace Parse.Internal
{
	internal interface IJsonConvertible
	{
		IDictionary<string, object> ToJSON();
	}
}
