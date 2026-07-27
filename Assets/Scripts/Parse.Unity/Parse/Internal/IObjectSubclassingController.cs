using System;
using System.Collections.Generic;

namespace Parse.Internal
{
	internal interface IObjectSubclassingController
	{
		string GetClassName(Type type);

		Type GetType(string className);

		bool IsTypeValid(string className, Type type);

		void RegisterSubclass(Type t);

		void UnregisterSubclass(Type t);

		ParseObject Instantiate(string className);

		IDictionary<string, string> GetPropertyMappings(string className);
	}
}
