using System.Collections.Generic;

namespace Amazon.Runtime
{
	public class StringListParameterValue : ParameterValue
	{
		public List<string> Value { get; set; }

		public StringListParameterValue(List<string> values)
		{
			Value = values;
		}

		internal StringListParameterValue()
		{
		}
	}
}
