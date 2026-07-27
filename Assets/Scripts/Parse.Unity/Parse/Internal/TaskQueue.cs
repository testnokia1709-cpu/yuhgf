using System;
using System.Threading;
using System.Threading.Tasks;

namespace Parse.Internal
{
	internal class TaskQueue
	{
		private Task tail;

		private readonly object mutex = new object();

		public object Mutex
		{
			get
			{
				return mutex;
			}
		}

		private Task GetTaskToAwait(CancellationToken cancellationToken)
		{
			lock (mutex)
			{
				return (tail ?? Task.FromResult(true)).ContinueWith(delegate
				{
				}, cancellationToken);
			}
		}

		public T Enqueue<T>(Func<Task, T> taskStart, CancellationToken cancellationToken) where T : Task
		{
			lock (mutex)
			{
				Task task = tail ?? Task.FromResult(true);
				T val = taskStart(GetTaskToAwait(cancellationToken));
				tail = Task.WhenAll(task, val);
				return val;
			}
		}
	}
}
