using System;
using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Util;
using Amazon.S3.Model;

namespace Amazon.S3.Internal
{
	public class AmazonS3ExceptionHandler : PipelineHandler
	{
		public override void InvokeSync(IExecutionContext executionContext)
		{
			try
			{
				base.InvokeSync(executionContext);
			}
			catch (Exception exception)
			{
				HandleException(executionContext, exception);
				throw;
			}
		}

		protected override void InvokeAsyncCallback(IAsyncExecutionContext executionContext)
		{
			Exception exception = executionContext.ResponseContext.AsyncResult.Exception;
			if (executionContext.ResponseContext.AsyncResult.Exception != null)
			{
				HandleException(ExecutionContext.CreateFromAsyncContext(executionContext), exception);
			}
			base.InvokeAsyncCallback(executionContext);
		}

		protected virtual void HandleException(IExecutionContext executionContext, Exception exception)
		{
			PutObjectRequest putObjectRequest = executionContext.RequestContext.OriginalRequest as PutObjectRequest;
			if (putObjectRequest != null)
			{
				HashStream hashStream = putObjectRequest.InputStream as HashStream;
				if (hashStream != null)
				{
					putObjectRequest.InputStream = hashStream.GetNonWrapperBaseStream();
				}
			}
			UploadPartRequest uploadPartRequest = executionContext.RequestContext.OriginalRequest as UploadPartRequest;
			if (uploadPartRequest != null)
			{
				HashStream hashStream2 = uploadPartRequest.InputStream as HashStream;
				if (hashStream2 != null)
				{
					uploadPartRequest.InputStream = hashStream2.GetNonWrapperBaseStream();
				}
			}
			AmazonS3Client.CleanupRequest(executionContext.RequestContext.OriginalRequest);
		}
	}
}
