namespace Parse.Internal
{
	internal interface IParseFieldOperation
	{
		object Encode();

		IParseFieldOperation MergeWithPrevious(IParseFieldOperation previous);

		object Apply(object oldValue, string key);
	}
}
