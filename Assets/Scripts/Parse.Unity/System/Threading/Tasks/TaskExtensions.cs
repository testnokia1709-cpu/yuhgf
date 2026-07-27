namespace System.Threading.Tasks
{
	public static class TaskExtensions
	{
		public static Task Unwrap(this Task<Task> task)
		{
			TaskCompletionSource<int> tcs = new TaskCompletionSource<int>();
			task.ContinueWith(delegate(Task<Task> t)
			{
				if (t.IsFaulted)
				{
					tcs.TrySetException(t.Exception);
				}
				else if (t.IsCanceled)
				{
					tcs.TrySetCanceled();
				}
				else
				{
					task.Result.ContinueWith(delegate(Task inner)
					{
						if (inner.IsFaulted)
						{
							tcs.TrySetException(inner.Exception);
						}
						else if (inner.IsCanceled)
						{
							tcs.TrySetCanceled();
						}
						else
						{
							tcs.TrySetResult(0);
						}
					});
				}
			});
			return tcs.Task;
		}

		public static Task<T> Unwrap<T>(this Task<Task<T>> task)
		{
			TaskCompletionSource<T> tcs = new TaskCompletionSource<T>();
			task.ContinueWith(delegate(Task<Task<T>> t)
			{
				if (t.IsFaulted)
				{
					tcs.TrySetException(t.Exception);
				}
				else if (t.IsCanceled)
				{
					tcs.TrySetCanceled();
				}
				else
				{
					t.Result.ContinueWith(delegate(Task<T> inner)
					{
						if (inner.IsFaulted)
						{
							tcs.TrySetException(inner.Exception);
						}
						else if (inner.IsCanceled)
						{
							tcs.TrySetCanceled();
						}
						else
						{
							tcs.TrySetResult(inner.Result);
						}
					});
				}
			});
			return tcs.Task;
		}
	}
}
