using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Parse.Internal
{
	internal class SynchronizedEventHandler<T>
	{
		private LinkedList<Tuple<Delegate, TaskFactory>> delegates = new LinkedList<Tuple<Delegate, TaskFactory>>();

		public void Add(Delegate del)
		{
			lock (delegates)
			{
				TaskFactory item = ((SynchronizationContext.Current == null) ? Task.Factory : new TaskFactory(CancellationToken.None, TaskCreationOptions.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.FromCurrentSynchronizationContext()));
				Delegate[] invocationList = del.GetInvocationList();
				foreach (Delegate item2 in invocationList)
				{
					delegates.AddLast(new Tuple<Delegate, TaskFactory>(item2, item));
				}
			}
		}

		public void Remove(Delegate del)
		{
			lock (delegates)
			{
				if (delegates.Count == 0)
				{
					return;
				}
				Delegate[] invocationList = del.GetInvocationList();
				foreach (Delegate obj in invocationList)
				{
					for (LinkedListNode<Tuple<Delegate, TaskFactory>> linkedListNode = delegates.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
					{
						if (linkedListNode.Value.Item1 == obj)
						{
							delegates.Remove(linkedListNode);
							break;
						}
					}
				}
			}
		}

		public Task Invoke(object sender, T args)
		{
			Task<int>[] toContinue = new Task<int>[1] { Task.FromResult(0) };
			IEnumerable<Tuple<Delegate, TaskFactory>> source;
			lock (delegates)
			{
				source = delegates.ToList();
			}
			return Task.WhenAll(source.Select((Tuple<Delegate, TaskFactory> p) => p.Item2.ContinueWhenAll(toContinue, delegate
			{
				p.Item1.DynamicInvoke(sender, args);
			})).ToList());
		}
	}
}
