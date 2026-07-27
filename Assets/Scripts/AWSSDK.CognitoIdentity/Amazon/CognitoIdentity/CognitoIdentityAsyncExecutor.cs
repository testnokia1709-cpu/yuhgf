using System;
using System.Threading;
using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Util;

namespace Amazon.CognitoIdentity
{
	internal class CognitoIdentityAsyncExecutor
	{
		private static Logger Logger = Logger.GetLogger(typeof(CognitoIdentityAsyncExecutor));

		public static void ExecuteAsync<T>(Func<T> function, AsyncOptions options, AmazonCognitoIdentityCallback<T> callback)
		{
			ThreadPool.QueueUserWorkItem(delegate
			{
				T result = default(T);
				Exception exception = null;
				try
				{
					result = function();
				}
				catch (Exception ex)
				{
					exception = ex;
				}
				if (callback != null)
				{
					if (!options.ExecuteCallbackOnMainThread)
					{
						try
						{
							callback(new AmazonCognitoIdentityResult<T>(result, exception, options.State));
							return;
						}
						catch (Exception exception2)
						{
							Logger.Error(exception2, "An unhandled exception was thrown from the callback method {0}.", callback.Method.Name);
							return;
						}
					}
					UnityRequestQueue.Instance.ExecuteOnMainThread(delegate
					{
						callback(new AmazonCognitoIdentityResult<T>(result, exception, options.State));
					});
				}
			});
		}
	}
}
