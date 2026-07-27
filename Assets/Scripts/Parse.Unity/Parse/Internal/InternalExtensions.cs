using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

namespace Parse.Internal
{
	internal static class InternalExtensions
	{
		internal delegate void PartialAccessor<T>(ref T arg);

		internal static Task<T> Safe<T>(this Task<T> task)
		{
			return task ?? Task.FromResult(default(T));
		}

		internal static Task Safe(this Task task)
		{
			return task ?? Task.FromResult<object>(null);
		}

		internal static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> self, TKey key, TValue defaultValue)
		{
			TValue value;
			if (self.TryGetValue(key, out value))
			{
				return value;
			}
			return defaultValue;
		}

		internal static bool CollectionsEqual<T>(this IEnumerable<T> a, IEnumerable<T> b)
		{
			if (!object.Equals(a, b))
			{
				if (a != null && b != null)
				{
					return a.SequenceEqual(b);
				}
				return false;
			}
			return true;
		}

		internal static T GetPartial<T>(this ParseObject self, PartialAccessor<T> action)
		{
			return GetPartial(action);
		}

		internal static T GetPartial<T>(PartialAccessor<T> action)
		{
			T arg = default(T);
			action(ref arg);
			return arg;
		}

		internal static Task<T> PartialAsync<T>(this object self, PartialAccessor<Task<T>> partial)
		{
			return PartialAsync(partial);
		}

		internal static Task<T> PartialAsync<T>(PartialAccessor<Task<T>> partial)
		{
			Task<T> arg = null;
			partial(ref arg);
			return arg.Safe();
		}

		internal static Task PartialAsync(this object self, PartialAccessor<Task> partial)
		{
			return PartialAsync(partial);
		}

		internal static Task PartialAsync(PartialAccessor<Task> partial)
		{
			Task arg = null;
			partial(ref arg);
			return arg.Safe();
		}

		internal static Task<TResult> OnSuccess<TIn, TResult>(this Task<TIn> task, Func<Task<TIn>, TResult> continuation)
		{
			return task.OnSuccess((Task t) => continuation((Task<TIn>)t));
		}

		internal static Task OnSuccess<TIn>(this Task<TIn> task, Action<Task<TIn>> continuation)
		{
			return task.OnSuccess(delegate(Task<TIn> t)
			{
				continuation(t);
				return (object)null;
			});
		}

		internal static Task<TResult> OnSuccess<TResult>(this Task task, Func<Task, TResult> continuation)
		{
			return task.ContinueWith(delegate(Task t)
			{
				if (t.IsFaulted)
				{
					AggregateException ex = t.Exception.Flatten();
					if (ex.InnerExceptions.Count == 1)
					{
						ExceptionDispatchInfo.Capture(ex.InnerExceptions[0]).Throw();
					}
					else
					{
						ExceptionDispatchInfo.Capture(ex).Throw();
					}
					return Task.FromResult(default(TResult));
				}
				if (t.IsCanceled)
				{
					TaskCompletionSource<TResult> taskCompletionSource = new TaskCompletionSource<TResult>();
					taskCompletionSource.SetCanceled();
					return taskCompletionSource.Task;
				}
				return Task.FromResult(continuation(t));
			}).Unwrap();
		}

		internal static Task OnSuccess(this Task task, Action<Task> continuation)
		{
			return task.OnSuccess(delegate(Task t)
			{
				continuation(t);
				return (object)null;
			});
		}

		internal static Task WhileAsync(Func<Task<bool>> predicate, Func<Task> body)
		{
			Func<Task> iterate = null;
			iterate = () => predicate().OnSuccess((Task<bool> t) => (!t.Result) ? Task.FromResult(0) : body().OnSuccess((Task _) => iterate()).Unwrap()).Unwrap();
			return iterate();
		}
	}
}
