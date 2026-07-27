namespace Amazon.Runtime
{
	public class StringParameterValue : ParameterValue
	{
		public string Value { get; set; }

		public StringParameterValue(string value)
		{
			Value = value;
		}

		internal StringParameterValue()
		{
		}
	}
}
