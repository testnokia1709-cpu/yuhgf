using System;
using System.Reflection;

namespace Amazon.Runtime.Internal
{
	public class DownloadHandlerBufferWrapper : IDisposable
	{
		private static Type downloadHandlerBufferType;

		private static PropertyInfo[] downloadHandlerBufferProperties;

		private static MethodInfo[] downloadHandlerBufferMethods;

		private static PropertyInfo dataProperty;

		private static MethodInfo dataGetMethod;

		private bool disposedValue;

		public object Instance { get; private set; }

		public byte[] Data
		{
			get
			{
				return (byte[])dataGetMethod.Invoke(Instance, null);
			}
		}

		static DownloadHandlerBufferWrapper()
		{
			downloadHandlerBufferType = Type.GetType("UnityEngine.Networking.DownloadHandlerBuffer, UnityEngine");
			if (downloadHandlerBufferType == null)
			{
				downloadHandlerBufferType = Type.GetType("UnityEngine.Experimental.Networking.DownloadHandlerBuffer, UnityEngine");
			}
			downloadHandlerBufferMethods = downloadHandlerBufferType.GetMethods();
			downloadHandlerBufferProperties = downloadHandlerBufferType.GetProperties();
			dataProperty = downloadHandlerBufferType.GetProperty("data");
			dataGetMethod = dataProperty.GetGetMethod();
		}

		public DownloadHandlerBufferWrapper()
		{
			Instance = Activator.CreateInstance(downloadHandlerBufferType);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposedValue)
			{
				return;
			}
			if (disposing)
			{
				IDisposable disposable = Instance as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
			Instance = null;
			disposedValue = true;
		}

		public void Dispose()
		{
			Dispose(true);
		}
	}
}
