using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using Parse.Internal;

namespace Parse
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public abstract class ParseRelationBase : IJsonConvertible
	{
		private ParseObject parent;

		private string key;

		private string targetClassName;

		internal static IObjectSubclassingController SubclassingController
		{
			get
			{
				return ParseCorePlugins.Instance.SubclassingController;
			}
		}

		internal string TargetClassName
		{
			get
			{
				return targetClassName;
			}
			set
			{
				targetClassName = value;
			}
		}

		internal ParseRelationBase(ParseObject parent, string key)
		{
			EnsureParentAndKey(parent, key);
		}

		internal ParseRelationBase(ParseObject parent, string key, string targetClassName)
			: this(parent, key)
		{
			this.targetClassName = targetClassName;
		}

		internal void EnsureParentAndKey(ParseObject parent, string key)
		{
			this.parent = this.parent ?? parent;
			this.key = this.key ?? key;
		}

		internal void Add(ParseObject obj)
		{
			ParseRelationOperation parseRelationOperation = new ParseRelationOperation(new ParseObject[1] { obj }, null);
			parent.PerformOperation(key, parseRelationOperation);
			targetClassName = parseRelationOperation.TargetClassName;
		}

		internal void Remove(ParseObject obj)
		{
			ParseRelationOperation parseRelationOperation = new ParseRelationOperation(null, new ParseObject[1] { obj });
			parent.PerformOperation(key, parseRelationOperation);
			targetClassName = parseRelationOperation.TargetClassName;
		}

		IDictionary<string, object> IJsonConvertible.ToJSON()
		{
			return new Dictionary<string, object>
			{
				{ "__type", "Relation" },
				{ "className", targetClassName }
			};
		}

		internal ParseQuery<T> GetQuery<T>() where T : ParseObject
		{
			if (targetClassName != null)
			{
				return new ParseQuery<T>(targetClassName).WhereRelatedTo(parent, key);
			}
			return new ParseQuery<T>(parent.ClassName).RedirectClassName(key).WhereRelatedTo(parent, key);
		}

		internal static ParseRelationBase CreateRelation(ParseObject parent, string key, string targetClassName)
		{
			if (PlatformHooks.IsCompiledByIL2CPP)
			{
				return CreateRelation<ParseObject>(parent, key, targetClassName);
			}
			Type type = SubclassingController.GetType(targetClassName) ?? typeof(ParseObject);
			return (ParseRelationBase)((MethodCallExpression)((Expression<Func<ParseRelation<ParseObject>>>)(() => CreateRelation<ParseObject>(parent, key, targetClassName))).Body).Method.GetGenericMethodDefinition().MakeGenericMethod(type).Invoke(null, new object[3] { parent, key, targetClassName });
		}

		private static ParseRelation<T> CreateRelation<T>(ParseObject parent, string key, string targetClassName) where T : ParseObject
		{
			return new ParseRelation<T>(parent, key, targetClassName);
		}
	}
}
