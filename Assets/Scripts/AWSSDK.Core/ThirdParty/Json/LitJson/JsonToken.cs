namespace ThirdParty.Json.LitJson
{
	public enum JsonToken
	{
		None = 0,
		ObjectStart = 1,
		PropertyName = 2,
		ObjectEnd = 3,
		ArrayStart = 4,
		ArrayEnd = 5,
		Int = 6,
		UInt = 7,
		Long = 8,
		ULong = 9,
		Double = 10,
		String = 11,
		Boolean = 12,
		Null = 13
	}
}
