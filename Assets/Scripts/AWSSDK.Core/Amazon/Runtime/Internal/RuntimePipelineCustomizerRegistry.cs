using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Amazon.Runtime.Internal.Util;

namespace Amazon.Runtime.Internal
{
	public class RuntimePipelineCustomizerRegistry : IDisposable
	{
		[CompilerGenerated]
		private static readonly RuntimePipelineCustomizerRegistry _003CInstance_003Ek__BackingField = new RuntimePipelineCustomizerRegistry();

		private Logger _logger = Logger.GetLogger(typeof(RuntimePipelineCustomizerRegistry));

		private ReaderWriterLockSlim _rwlock = new ReaderWriterLockSlim();

		private IList<IRuntimePipelineCustomizer> _customizers = new List<IRuntimePipelineCustomizer>();

		public static RuntimePipelineCustomizerRegistry Instance
		{
			[CompilerGenerated]
			get
			{
				return _003CInstance_003Ek__BackingField;
			}
		}

		private RuntimePipelineCustomizerRegistry()
		{
		}

		public void Register(IRuntimePipelineCustomizer customizer)
		{
			_rwlock.EnterWriteLock();
			try
			{
				if (_customizers.FirstOrDefault((IRuntimePipelineCustomizer x) => string.Equals(x.UniqueName, customizer.UniqueName)) != null)
				{
					_logger.InfoFormat("Skipping registration because runtime pipeline customizer {0} already registered", customizer.UniqueName);
				}
				else
				{
					_logger.InfoFormat("Registering runtime pipeline customizer {0}", customizer.UniqueName);
					_customizers.Add(customizer);
				}
			}
			finally
			{
				_rwlock.ExitWriteLock();
			}
		}

		public void Deregister(IRuntimePipelineCustomizer customizer)
		{
			Deregister(customizer.UniqueName);
		}

		public void Deregister(string uniqueName)
		{
			_rwlock.EnterWriteLock();
			try
			{
				int num = -1;
				for (int i = 0; i < _customizers.Count; i++)
				{
					if (string.Equals(uniqueName, _customizers[i].UniqueName, StringComparison.Ordinal))
					{
						num = i;
						break;
					}
				}
				if (num != -1)
				{
					_logger.InfoFormat("Deregistering runtime pipeline customizer {0}", uniqueName);
					_customizers.RemoveAt(num);
				}
				else
				{
					_logger.InfoFormat("Runtime pipeline customizer {0} not found to deregister", uniqueName);
				}
			}
			finally
			{
				_rwlock.ExitWriteLock();
			}
		}

		internal void ApplyCustomizations(Type type, RuntimePipeline pipeline)
		{
			_rwlock.EnterReadLock();
			try
			{
				foreach (IRuntimePipelineCustomizer customizer in _customizers)
				{
					_logger.InfoFormat("Applying runtime pipeline customization {0}", customizer.UniqueName);
					customizer.Customize(type, pipeline);
				}
			}
			finally
			{
				_rwlock.ExitReadLock();
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing && _rwlock != null)
			{
				_rwlock.Dispose();
				_rwlock = null;
			}
		}
	}
}
