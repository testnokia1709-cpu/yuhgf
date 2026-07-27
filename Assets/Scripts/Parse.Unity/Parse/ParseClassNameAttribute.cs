using System;

namespace Parse
{
	[AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
	public sealed class ParseClassNameAttribute : Attribute
	{
		public string ClassName { get; private set; }

		public ParseClassNameAttribute(string className)
		{
			ClassName = className;
		}
	}
}
