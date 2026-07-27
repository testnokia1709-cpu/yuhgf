using System;

namespace Parse
{
	internal class ParseDownloadProgressEventArgs : EventArgs
	{
		public double Progress { get; internal set; }

		internal ParseDownloadProgressEventArgs()
		{
		}
	}
}
