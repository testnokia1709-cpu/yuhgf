using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Parse.Internal
{
	internal static class MissingExtensions
	{
		public static Type GetTypeInfo(this Type t)
		{
			return t;
		}

		public static bool HasFlag(this Enum enumValue, Enum flag)
		{
			long num = Convert.ToInt64(enumValue);
			long num2 = Convert.ToInt64(flag);
			return (num & num2) == num2;
		}

		internal static T GetCustomAttribute<T>(this PropertyInfo prop, bool inherit) where T : Attribute
		{
			return (T)prop.GetCustomAttributes(typeof(T), inherit).FirstOrDefault();
		}

		internal static T GetCustomAttribute<T>(this PropertyInfo prop) where T : Attribute
		{
			return prop.GetCustomAttribute<T>(true);
		}

		internal static T GetCustomAttribute<T>(this Type type, bool inherit) where T : Attribute
		{
			return (T)type.GetCustomAttributes(typeof(T), inherit).FirstOrDefault();
		}

		internal static T GetCustomAttribute<T>(this Type type) where T : Attribute
		{
			return type.GetCustomAttribute<T>(true);
		}

		internal static Task<string> ReadToEndAsync(this StreamReader reader)
		{
			return Task.Run(() => reader.ReadToEnd());
		}

		internal static Task CopyToAsync(this Stream stream, Stream destination)
		{
			return stream.CopyToAsync(destination, 2048, CancellationToken.None);
		}

		internal static Task CopyToAsync(this Stream stream, Stream destination, int bufferSize, CancellationToken cancellationToken)
		{
			byte[] buffer = new byte[bufferSize];
			int bytesRead = 0;
			return InternalExtensions.WhileAsync(() => stream.ReadAsync(buffer, 0, bufferSize, cancellationToken).OnSuccess(delegate(Task<int> readTask)
			{
				bytesRead = readTask.Result;
				return bytesRead > 0;
			}), delegate
			{
				cancellationToken.ThrowIfCancellationRequested();
				return destination.WriteAsync(buffer, 0, bytesRead, cancellationToken).OnSuccess(delegate
				{
					cancellationToken.ThrowIfCancellationRequested();
				});
			});
		}

		internal static Task<int> ReadAsync(this Stream stream, byte[] buffer, int offset, int count, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				TaskCompletionSource<int> taskCompletionSource = new TaskCompletionSource<int>();
				taskCompletionSource.SetCanceled();
				return taskCompletionSource.Task;
			}
			return Task.Factory.FromAsync((Func<byte[], int, int, AsyncCallback, object, IAsyncResult>)stream.BeginRead, (Func<IAsyncResult, int>)stream.EndRead, buffer, offset, count, (object)null);
		}

		internal static Task WriteAsync(this Stream stream, byte[] buffer, int offset, int count, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				TaskCompletionSource<object> taskCompletionSource = new TaskCompletionSource<object>();
				taskCompletionSource.SetCanceled();
				return taskCompletionSource.Task;
			}
			return Task.Factory.FromAsync(stream.BeginWrite, stream.EndWrite, buffer, offset, count, null);
		}

		internal static IEnumerable<TResult> Zip<T1, T2, TResult>(this IEnumerable<T1> list1, IEnumerable<T2> list2, Func<T1, T2, TResult> zipper)
		{
			IEnumerator<T1> e1 = list1.GetEnumerator();
			IEnumerator<T2> e2 = list2.GetEnumerator();
			while (e1.MoveNext() && e2.MoveNext())
			{
				yield return zipper(e1.Current, e2.Current);
			}
		}
	}
}
