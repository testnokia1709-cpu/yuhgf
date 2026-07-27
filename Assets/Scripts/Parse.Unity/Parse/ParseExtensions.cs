using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Parse.Internal;

namespace Parse
{
	public static class ParseExtensions
	{
		public static Task SaveAllAsync<T>(this IEnumerable<T> objects) where T : ParseObject
		{
			return ParseObject.SaveAllAsync(objects);
		}

		public static Task SaveAllAsync<T>(this IEnumerable<T> objects, CancellationToken cancellationToken) where T : ParseObject
		{
			return ParseObject.SaveAllAsync(objects, cancellationToken);
		}

		public static Task<IEnumerable<T>> FetchAllAsync<T>(this IEnumerable<T> objects) where T : ParseObject
		{
			return ParseObject.FetchAllAsync(objects);
		}

		public static Task<IEnumerable<T>> FetchAllAsync<T>(this IEnumerable<T> objects, CancellationToken cancellationToken) where T : ParseObject
		{
			return ParseObject.FetchAllAsync(objects, cancellationToken);
		}

		public static Task<IEnumerable<T>> FetchAllIfNeededAsync<T>(this IEnumerable<T> objects) where T : ParseObject
		{
			return ParseObject.FetchAllIfNeededAsync(objects);
		}

		public static Task<IEnumerable<T>> FetchAllIfNeededAsync<T>(this IEnumerable<T> objects, CancellationToken cancellationToken) where T : ParseObject
		{
			return ParseObject.FetchAllIfNeededAsync(objects, cancellationToken);
		}

		public static ParseQuery<T> Or<T>(this ParseQuery<T> source, params ParseQuery<T>[] queries) where T : ParseObject
		{
			return ParseQuery<T>.Or(queries.Concat(new ParseQuery<T>[1] { source }));
		}

		public static Task<T> FetchAsync<T>(this T obj) where T : ParseObject
		{
			return obj.FetchAsyncInternal(CancellationToken.None).OnSuccess((Task<ParseObject> t) => (T)t.Result);
		}

		public static Task<T> FetchAsync<T>(this T obj, CancellationToken cancellationToken) where T : ParseObject
		{
			return obj.FetchAsyncInternal(cancellationToken).OnSuccess((Task<ParseObject> t) => (T)t.Result);
		}

		public static Task<T> FetchIfNeededAsync<T>(this T obj) where T : ParseObject
		{
			return obj.FetchIfNeededAsyncInternal(CancellationToken.None).OnSuccess((Task<ParseObject> t) => (T)t.Result);
		}

		public static Task<T> FetchIfNeededAsync<T>(this T obj, CancellationToken cancellationToken) where T : ParseObject
		{
			return obj.FetchIfNeededAsyncInternal(cancellationToken).OnSuccess((Task<ParseObject> t) => (T)t.Result);
		}
	}
}
