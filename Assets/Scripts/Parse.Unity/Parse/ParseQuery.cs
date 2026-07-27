using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Parse.Internal;

namespace Parse
{
	public class ParseQuery<T> where T : ParseObject
	{
		private readonly string className;

		private readonly Dictionary<string, object> where;

		private readonly ReadOnlyCollection<string> orderBy;

		private readonly ReadOnlyCollection<string> includes;

		private readonly ReadOnlyCollection<string> selectedKeys;

		private readonly string redirectClassNameForKey;

		private readonly int? skip;

		private readonly int? limit;

		internal string ClassName
		{
			get
			{
				return className;
			}
		}

		internal static IParseQueryController QueryController
		{
			get
			{
				return ParseCorePlugins.Instance.QueryController;
			}
		}

		internal static IObjectSubclassingController SubclassingController
		{
			get
			{
				return ParseCorePlugins.Instance.SubclassingController;
			}
		}

		private ParseQuery(ParseQuery<T> source, IDictionary<string, object> where = null, IEnumerable<string> replacementOrderBy = null, IEnumerable<string> thenBy = null, int? skip = null, int? limit = null, IEnumerable<string> includes = null, IEnumerable<string> selectedKeys = null, string redirectClassNameForKey = null)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			className = source.className;
			this.where = source.where;
			orderBy = source.orderBy;
			this.skip = source.skip;
			this.limit = source.limit;
			this.includes = source.includes;
			this.selectedKeys = source.selectedKeys;
			this.redirectClassNameForKey = source.redirectClassNameForKey;
			if (where != null)
			{
				IDictionary<string, object> dictionary = MergeWhereClauses(where);
				this.where = new Dictionary<string, object>(dictionary);
			}
			if (replacementOrderBy != null)
			{
				orderBy = new ReadOnlyCollection<string>(replacementOrderBy.ToList());
			}
			if (thenBy != null)
			{
				if (orderBy == null)
				{
					throw new ArgumentException("You must call OrderBy before calling ThenBy.");
				}
				List<string> list = new List<string>(orderBy);
				list.AddRange(thenBy);
				orderBy = new ReadOnlyCollection<string>(list);
			}
			if (orderBy != null)
			{
				HashSet<string> source2 = new HashSet<string>(orderBy);
				orderBy = new ReadOnlyCollection<string>(source2.ToList());
			}
			if (skip.HasValue)
			{
				this.skip = (this.skip ?? 0) + skip;
			}
			if (limit.HasValue)
			{
				this.limit = limit;
			}
			if (includes != null)
			{
				HashSet<string> source3 = MergeIncludes(includes);
				this.includes = new ReadOnlyCollection<string>(source3.ToList());
			}
			if (selectedKeys != null)
			{
				HashSet<string> source4 = MergeSelectedKeys(selectedKeys);
				this.selectedKeys = new ReadOnlyCollection<string>(source4.ToList());
			}
			if (redirectClassNameForKey != null)
			{
				this.redirectClassNameForKey = redirectClassNameForKey;
			}
		}

		private HashSet<string> MergeIncludes(IEnumerable<string> includes)
		{
			if (this.includes == null)
			{
				return new HashSet<string>(includes);
			}
			HashSet<string> hashSet = new HashSet<string>(this.includes);
			foreach (string include in includes)
			{
				hashSet.Add(include);
			}
			return hashSet;
		}

		private HashSet<string> MergeSelectedKeys(IEnumerable<string> selectedKeys)
		{
			if (this.selectedKeys == null)
			{
				return new HashSet<string>(selectedKeys);
			}
			HashSet<string> hashSet = new HashSet<string>(this.selectedKeys);
			foreach (string selectedKey in selectedKeys)
			{
				hashSet.Add(selectedKey);
			}
			return hashSet;
		}

		private IDictionary<string, object> MergeWhereClauses(IDictionary<string, object> where)
		{
			if (this.where == null)
			{
				return where;
			}
			Dictionary<string, object> dictionary = new Dictionary<string, object>(this.where);
			foreach (KeyValuePair<string, object> item in where)
			{
				IDictionary<string, object> dictionary2 = item.Value as IDictionary<string, object>;
				if (dictionary.ContainsKey(item.Key))
				{
					IDictionary<string, object> obj = dictionary[item.Key] as IDictionary<string, object>;
					if (obj == null || dictionary2 == null)
					{
						throw new ArgumentException("More than one where clause for the given key provided.");
					}
					Dictionary<string, object> dictionary3 = new Dictionary<string, object>(obj);
					foreach (KeyValuePair<string, object> item2 in dictionary2)
					{
						if (dictionary3.ContainsKey(item2.Key))
						{
							throw new ArgumentException("More than one condition for the given key provided.");
						}
						dictionary3[item2.Key] = item2.Value;
					}
					dictionary[item.Key] = dictionary3;
				}
				else
				{
					dictionary[item.Key] = item.Value;
				}
			}
			return dictionary;
		}

		public ParseQuery()
			: this(SubclassingController.GetClassName(typeof(T)))
		{
		}

		public ParseQuery(string className)
		{
			if (className == null)
			{
				throw new ArgumentNullException("className", "Must specify a ParseObject class name when creating a ParseQuery.");
			}
			this.className = className;
		}

		public static ParseQuery<T> Or(IEnumerable<ParseQuery<T>> queries)
		{
			string text = null;
			List<IDictionary<string, object>> list = new List<IDictionary<string, object>>();
			foreach (ParseQuery<T> item in (IEnumerable)queries)
			{
				if (text != null && item.className != text)
				{
					throw new ArgumentException("All of the queries in an or query must be on the same class.");
				}
				text = item.className;
				IDictionary<string, object> dictionary = item.BuildParameters();
				if (dictionary.Count != 0)
				{
					object value;
					if (!dictionary.TryGetValue("where", out value) || dictionary.Count > 1)
					{
						throw new ArgumentException("None of the queries in an or query can have non-filtering clauses");
					}
					list.Add(value as IDictionary<string, object>);
				}
			}
			return new ParseQuery<T>(new ParseQuery<T>(text), new Dictionary<string, object> { { "$or", list } });
		}

		public ParseQuery<T> OrderBy(string key)
		{
			return new ParseQuery<T>(this, null, new List<string> { key });
		}

		public ParseQuery<T> OrderByDescending(string key)
		{
			return new ParseQuery<T>(this, null, new List<string> { "-" + key });
		}

		public ParseQuery<T> ThenBy(string key)
		{
			return new ParseQuery<T>(this, null, null, new List<string> { key });
		}

		public ParseQuery<T> ThenByDescending(string key)
		{
			return new ParseQuery<T>(this, null, null, new List<string> { "-" + key });
		}

		public ParseQuery<T> Include(string key)
		{
			return new ParseQuery<T>(this, null, null, null, null, null, new List<string> { key });
		}

		public ParseQuery<T> Select(string key)
		{
			return new ParseQuery<T>(this, null, null, null, null, null, null, new List<string> { key });
		}

		public ParseQuery<T> Skip(int count)
		{
			return new ParseQuery<T>(this, null, null, null, count);
		}

		public ParseQuery<T> Limit(int count)
		{
			return new ParseQuery<T>(this, null, null, null, null, count);
		}

		internal ParseQuery<T> RedirectClassName(string key)
		{
			return new ParseQuery<T>(this, null, null, null, null, null, null, null, key);
		}

		public ParseQuery<T> WhereContainedIn<TIn>(string key, IEnumerable<TIn> values)
		{
			return new ParseQuery<T>(this, new Dictionary<string, object> { 
			{
				key,
				new Dictionary<string, object> { 
				{
					"$in",
					values.ToList()
				} }
			} });
		}

		public ParseQuery<T> WhereContainsAll<TIn>(string key, IEnumerable<TIn> values)
		{
			return new ParseQuery<T>(this, new Dictionary<string, object> { 
			{
				key,
				new Dictionary<string, object> { 
				{
					"$all",
					values.ToList()
				} }
			} });
		}

		public ParseQuery<T> WhereContains(string key, string substring)
		{
			return new ParseQuery<T>(this, new Dictionary<string, object> { 
			{
				key,
				new Dictionary<string, object> { 
				{
					"$regex",
					RegexQuote(substring)
				} }
			} });
		}

		public ParseQuery<T> WhereDoesNotExist(string key)
		{
			return new ParseQuery<T>(this, new Dictionary<string, object> { 
			{
				key,
				new Dictionary<string, object> { { "$exists", false } }
			} });
		}

		public ParseQuery<T> WhereDoesNotMatchQuery<TOther>(string key, ParseQuery<TOther> query) where TOther : ParseObject
		{
			return new ParseQuery<T>(this, new Dictionary<string, object> { 
			{
				key,
				new Dictionary<string, object> { 
				{
					"$notInQuery",
					query.BuildParameters(true)
				} }
			} });
		}

		public ParseQuery<T> WhereEndsWith(string key, string suffix)
		{
			return new ParseQuery<T>(this, new Dictionary<string, object> { 
			{
				key,
				new Dictionary<string, object> { 
				{
					"$regex",
					RegexQuote(suffix) + "$"
				} }
			} });
		}

		public ParseQuery<T> WhereEqualTo(string key, object value)
		{
			return new ParseQuery<T>(this, new Dictionary<string, object> { { key, value } });
		}

		public ParseQuery<T> WhereExists(string key)
		{
			return new ParseQuery<T>(this, new Dictionary<string, object> { 
			{
				key,
				new Dictionary<string, object> { { "$exists", true } }
			} });
		}

		public ParseQuery<T> WhereGreaterThan(string key, object value)
		{
			return new ParseQuery<T>(this, new Dictionary<string, object> { 
			{
				key,
				new Dictionary<string, object> { { "$gt", value } }
			} });
		}

		public ParseQuery<T> WhereGreaterThanOrEqualTo(string key, object value)
		{
			return new ParseQuery<T>(this, new Dictionary<string, object> { 
			{
				key,
				new Dictionary<string, object> { { "$gte", value } }
			} });
		}

		public ParseQuery<T> WhereLessThan(string key, object value)
		{
			return new ParseQuery<T>(this, new Dictionary<string, object> { 
			{
				key,
				new Dictionary<string, object> { { "$lt", value } }
			} });
		}

		public ParseQuery<T> WhereLessThanOrEqualTo(string key, object value)
		{
			return new ParseQuery<T>(this, new Dictionary<string, object> { 
			{
				key,
				new Dictionary<string, object> { { "$lte", value } }
			} });
		}

		public ParseQuery<T> WhereMatches(string key, Regex regex, string modifiers)
		{
			if (!regex.Options.HasFlag(RegexOptions.ECMAScript))
			{
				throw new ArgumentException("Only ECMAScript-compatible regexes are supported. Please use the ECMAScript RegexOptions flag when creating your regex.");
			}
			return new ParseQuery<T>(this, new Dictionary<string, object> { 
			{
				key,
				EncodeRegex(regex, modifiers)
			} });
		}

		public ParseQuery<T> WhereMatches(string key, Regex regex)
		{
			return WhereMatches(key, regex, null);
		}

		public ParseQuery<T> WhereMatches(string key, string pattern, string modifiers = null)
		{
			return WhereMatches(key, new Regex(pattern, RegexOptions.ECMAScript), modifiers);
		}

		public ParseQuery<T> WhereMatches(string key, string pattern)
		{
			return WhereMatches(key, pattern, null);
		}

		public ParseQuery<T> WhereMatchesKeyInQuery<TOther>(string key, string keyInQuery, ParseQuery<TOther> query) where TOther : ParseObject
		{
			Dictionary<string, object> value = new Dictionary<string, object>
			{
				{
					"query",
					query.BuildParameters(true)
				},
				{ "key", keyInQuery }
			};
			return new ParseQuery<T>(this, new Dictionary<string, object> { 
			{
				key,
				new Dictionary<string, object> { { "$select", value } }
			} });
		}

		public ParseQuery<T> WhereDoesNotMatchesKeyInQuery<TOther>(string key, string keyInQuery, ParseQuery<TOther> query) where TOther : ParseObject
		{
			Dictionary<string, object> value = new Dictionary<string, object>
			{
				{
					"query",
					query.BuildParameters(true)
				},
				{ "key", keyInQuery }
			};
			return new ParseQuery<T>(this, new Dictionary<string, object> { 
			{
				key,
				new Dictionary<string, object> { { "$dontSelect", value } }
			} });
		}

		public ParseQuery<T> WhereMatchesQuery<TOther>(string key, ParseQuery<TOther> query) where TOther : ParseObject
		{
			return new ParseQuery<T>(this, new Dictionary<string, object> { 
			{
				key,
				new Dictionary<string, object> { 
				{
					"$inQuery",
					query.BuildParameters(true)
				} }
			} });
		}

		public ParseQuery<T> WhereNear(string key, ParseGeoPoint point)
		{
			return new ParseQuery<T>(this, new Dictionary<string, object> { 
			{
				key,
				new Dictionary<string, object> { { "$nearSphere", point } }
			} });
		}

		public ParseQuery<T> WhereNotContainedIn<TIn>(string key, IEnumerable<TIn> values)
		{
			return new ParseQuery<T>(this, new Dictionary<string, object> { 
			{
				key,
				new Dictionary<string, object> { 
				{
					"$nin",
					values.ToList()
				} }
			} });
		}

		public ParseQuery<T> WhereNotEqualTo(string key, object value)
		{
			return new ParseQuery<T>(this, new Dictionary<string, object> { 
			{
				key,
				new Dictionary<string, object> { { "$ne", value } }
			} });
		}

		public ParseQuery<T> WhereStartsWith(string key, string suffix)
		{
			return new ParseQuery<T>(this, new Dictionary<string, object> { 
			{
				key,
				new Dictionary<string, object> { 
				{
					"$regex",
					"^" + RegexQuote(suffix)
				} }
			} });
		}

		public ParseQuery<T> WhereWithinGeoBox(string key, ParseGeoPoint southwest, ParseGeoPoint northeast)
		{
			return new ParseQuery<T>(this, new Dictionary<string, object> { 
			{
				key,
				new Dictionary<string, object> { 
				{
					"$within",
					new Dictionary<string, object> { 
					{
						"$box",
						new ParseGeoPoint[2] { southwest, northeast }
					} }
				} }
			} });
		}

		public ParseQuery<T> WhereWithinDistance(string key, ParseGeoPoint point, ParseGeoDistance maxDistance)
		{
			return new ParseQuery<T>(WhereNear(key, point), new Dictionary<string, object> { 
			{
				key,
				new Dictionary<string, object> { { "$maxDistance", maxDistance.Radians } }
			} });
		}

		internal ParseQuery<T> WhereRelatedTo(ParseObject parent, string key)
		{
			return new ParseQuery<T>(this, new Dictionary<string, object> { 
			{
				"$relatedTo",
				new Dictionary<string, object>
				{
					{ "object", parent },
					{ "key", key }
				}
			} });
		}

		public Task<IEnumerable<T>> FindAsync()
		{
			return FindAsync(CancellationToken.None);
		}

		public Task<IEnumerable<T>> FindAsync(CancellationToken cancellationToken)
		{
			EnsureNotInstallationQuery();
			return QueryController.FindAsync(this, ParseUser.CurrentUser, cancellationToken).OnSuccess((Task<IEnumerable<IObjectState>> t) => t.Result.Select((IObjectState state) => ParseObject.FromState<T>(state, ClassName)));
		}

		public Task<T> FirstOrDefaultAsync()
		{
			return FirstOrDefaultAsync(CancellationToken.None);
		}

		public Task<T> FirstOrDefaultAsync(CancellationToken cancellationToken)
		{
			EnsureNotInstallationQuery();
			return QueryController.FirstAsync(this, ParseUser.CurrentUser, cancellationToken).OnSuccess(delegate(Task<IObjectState> t)
			{
				IObjectState result = t.Result;
				return (result != null) ? ParseObject.FromState<T>(result, ClassName) : null;
			});
		}

		public Task<T> FirstAsync()
		{
			return FirstAsync(CancellationToken.None);
		}

		public Task<T> FirstAsync(CancellationToken cancellationToken)
		{
			return FirstOrDefaultAsync(cancellationToken).OnSuccess(delegate(Task<T> t)
			{
				if (t.Result == null)
				{
					throw new ParseException(ParseException.ErrorCode.ObjectNotFound, "No results matched the query.");
				}
				return t.Result;
			});
		}

		public Task<int> CountAsync()
		{
			return CountAsync(CancellationToken.None);
		}

		public Task<int> CountAsync(CancellationToken cancellationToken)
		{
			EnsureNotInstallationQuery();
			return QueryController.CountAsync(this, ParseUser.CurrentUser, cancellationToken);
		}

		public Task<T> GetAsync(string objectId)
		{
			return GetAsync(objectId, CancellationToken.None);
		}

		public Task<T> GetAsync(string objectId, CancellationToken cancellationToken)
		{
			return new ParseQuery<T>(new ParseQuery<T>(className).WhereEqualTo("objectId", objectId), null, null, null, null, includes: includes, selectedKeys: selectedKeys, limit: 1).FindAsync(cancellationToken).OnSuccess(delegate(Task<IEnumerable<T>> t)
			{
				T val = t.Result.FirstOrDefault();
				if (val == null)
				{
					throw new ParseException(ParseException.ErrorCode.ObjectNotFound, "Object with the given objectId not found.");
				}
				return val;
			});
		}

		internal object GetConstraint(string key)
		{
			if (where != null)
			{
				return where.GetOrDefault(key, null);
			}
			return null;
		}

		internal IDictionary<string, object> BuildParameters(bool includeClassName = false)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			if (where != null)
			{
				dictionary["where"] = PointerOrLocalIdEncoder.Instance.Encode(where);
			}
			if (orderBy != null)
			{
				dictionary["order"] = string.Join(",", orderBy.ToArray());
			}
			if (skip.HasValue)
			{
				dictionary["skip"] = skip.Value;
			}
			if (limit.HasValue)
			{
				dictionary["limit"] = limit.Value;
			}
			if (includes != null)
			{
				dictionary["include"] = string.Join(",", includes.ToArray());
			}
			if (selectedKeys != null)
			{
				dictionary["keys"] = string.Join(",", selectedKeys.ToArray());
			}
			if (includeClassName)
			{
				dictionary["className"] = className;
			}
			if (redirectClassNameForKey != null)
			{
				dictionary["redirectClassNameForKey"] = redirectClassNameForKey;
			}
			return dictionary;
		}

		private string RegexQuote(string input)
		{
			return "\\Q" + input.Replace("\\E", "\\E\\\\E\\Q") + "\\E";
		}

		private string GetRegexOptions(Regex regex, string modifiers)
		{
			string text = modifiers ?? "";
			if (regex.Options.HasFlag(RegexOptions.IgnoreCase) && !modifiers.Contains("i"))
			{
				text += "i";
			}
			if (regex.Options.HasFlag(RegexOptions.Multiline) && !modifiers.Contains("m"))
			{
				text += "m";
			}
			return text;
		}

		private IDictionary<string, object> EncodeRegex(Regex regex, string modifiers)
		{
			string regexOptions = GetRegexOptions(regex, modifiers);
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary["$regex"] = regex.ToString();
			if (!string.IsNullOrEmpty(regexOptions))
			{
				dictionary["$options"] = regexOptions;
			}
			return dictionary;
		}

		private void EnsureNotInstallationQuery()
		{
			if (className.Equals("_Installation"))
			{
				throw new InvalidOperationException("Cannot directly query the Installation class.");
			}
		}

		public override bool Equals(object obj)
		{
			if (obj == null || !(obj is ParseQuery<T>))
			{
				return false;
			}
			ParseQuery<T> parseQuery = obj as ParseQuery<T>;
			if (object.Equals(className, parseQuery.ClassName) && where.CollectionsEqual(parseQuery.where) && orderBy.CollectionsEqual(parseQuery.orderBy) && includes.CollectionsEqual(parseQuery.includes) && selectedKeys.CollectionsEqual(parseQuery.selectedKeys) && object.Equals(skip, parseQuery.skip))
			{
				return object.Equals(limit, parseQuery.limit);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return 0;
		}
	}
}
