namespace CloudOnce
{
	public class CloudRequestResult<T> where T : new()
	{
		public string Error { get; private set; }

		public T Result { get; private set; }

		public bool HasError
		{
			get
			{
				return !string.IsNullOrEmpty(Error);
			}
		}

		public CloudRequestResult(T result)
		{
			Error = string.Empty;
			Result = result;
		}

		public CloudRequestResult(T result, string error)
		{
			Error = error;
			Result = result;
		}
	}
}
