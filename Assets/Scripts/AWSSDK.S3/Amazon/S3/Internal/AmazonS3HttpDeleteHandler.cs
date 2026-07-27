using System;
using System.Collections.Generic;
using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Util.Internal;

namespace Amazon.S3.Internal
{
	public class AmazonS3HttpDeleteHandler : PipelineHandler
	{
		public override void InvokeSync(IExecutionContext executionContext)
		{
			PreInvoke(executionContext);
			base.InvokeSync(executionContext);
		}

		public override IAsyncResult InvokeAsync(IAsyncExecutionContext executionContext)
		{
			PreInvoke(ExecutionContext.CreateFromAsyncContext(executionContext));
			return base.InvokeAsync(executionContext);
		}

		protected virtual void PreInvoke(IExecutionContext executionContext)
		{
			AmazonWebServiceRequest originalRequest = executionContext.RequestContext.OriginalRequest;
			ModifyDeleteRequest(executionContext);
		}

		internal static void ModifyDeleteRequest(IExecutionContext executionContext)
		{
			IRequest request = executionContext.RequestContext.Request;
			IDictionary<string, string> headers = request.Headers;
			IDictionary<string, string> parameters = request.Parameters;
			if (InternalSDKUtils.IsAndroid && request.HttpMethod == "DELETE" && !parameters.ContainsKey("ContentType") && !headers.ContainsKey("Content-Type"))
			{
				headers.Add("Content-Type", "application/x-www-form-urlencoded");
			}
		}
	}
}
