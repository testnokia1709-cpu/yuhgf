using System;

namespace Parse
{
	public class ParseUploadProgressEventArgs : EventArgs
	{
		public double Progress { get; internal set; }

		internal ParseUploadProgressEventArgs()
		{
		}
	}
}
