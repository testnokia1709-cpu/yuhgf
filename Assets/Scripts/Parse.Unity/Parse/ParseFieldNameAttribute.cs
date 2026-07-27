using System;

namespace Parse
{
	[AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
	public sealed class ParseFieldNameAttribute : Attribute
	{
		public string FieldName { get; private set; }

		public ParseFieldNameAttribute(string fieldName)
		{
			FieldName = fieldName;
		}
	}
}
