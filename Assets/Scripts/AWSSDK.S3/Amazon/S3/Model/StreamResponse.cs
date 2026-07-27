using System;
using System.IO;
using Amazon.Runtime;

namespace Amazon.S3.Model
{
	public abstract class StreamResponse : AmazonWebServiceResponse, IDisposable
	{
		private bool disposed;

		private Stream responseStream;

		public Stream ResponseStream
		{
			get
			{
				return responseStream;
			}
			set
			{
				responseStream = value;
			}
		}

		public void Dispose()
		{
			Dispose(true);
			if (!disposed)
			{
				GC.SuppressFinalize(this);
			}
		}

		private void Dispose(bool disposing)
		{
			if (!disposed)
			{
				if (disposing && responseStream != null)
				{
					responseStream.Dispose();
				}
				responseStream = null;
				disposed = true;
			}
		}

		internal bool IsSetResponseStream()
		{
			return responseStream != null;
		}
	}
}
