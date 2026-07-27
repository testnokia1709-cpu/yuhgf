using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Parse.Internal
{
	internal class ParseRelationOperation : IParseFieldOperation
	{
		private readonly IList<string> adds;

		private readonly IList<string> removes;

		private readonly string targetClassName;

		public string TargetClassName
		{
			get
			{
				return targetClassName;
			}
		}

		private ParseRelationOperation(IEnumerable<string> adds, IEnumerable<string> removes, string targetClassName)
		{
			this.targetClassName = targetClassName;
			this.adds = new ReadOnlyCollection<string>(adds.ToList());
			this.removes = new ReadOnlyCollection<string>(removes.ToList());
		}

		public ParseRelationOperation(IEnumerable<ParseObject> adds, IEnumerable<ParseObject> removes)
		{
			adds = adds ?? new ParseObject[0];
			removes = removes ?? new ParseObject[0];
			targetClassName = (from o in adds.Concat(removes)
				select o.ClassName).FirstOrDefault();
			this.adds = new ReadOnlyCollection<string>(IdsFromObjects(adds).ToList());
			this.removes = new ReadOnlyCollection<string>(IdsFromObjects(removes).ToList());
		}

		public object Encode()
		{
			List<object> list = adds.Select((string id) => PointerOrLocalIdEncoder.Instance.Encode(ParseObject.CreateWithoutData(targetClassName, id))).ToList();
			List<object> list2 = removes.Select((string id) => PointerOrLocalIdEncoder.Instance.Encode(ParseObject.CreateWithoutData(targetClassName, id))).ToList();
			Dictionary<string, object> dictionary = ((list.Count == 0) ? null : new Dictionary<string, object>
			{
				{ "__op", "AddRelation" },
				{ "objects", list }
			});
			Dictionary<string, object> dictionary2 = ((list2.Count == 0) ? null : new Dictionary<string, object>
			{
				{ "__op", "RemoveRelation" },
				{ "objects", list2 }
			});
			if (dictionary != null && dictionary2 != null)
			{
				Dictionary<string, object> dictionary3 = new Dictionary<string, object>();
				dictionary3.Add("__op", "Batch");
				dictionary3.Add("ops", new Dictionary<string, object>[2] { dictionary, dictionary2 });
				return dictionary3;
			}
			return dictionary ?? dictionary2;
		}

		public IParseFieldOperation MergeWithPrevious(IParseFieldOperation previous)
		{
			if (previous == null)
			{
				return this;
			}
			if (previous is ParseDeleteOperation)
			{
				throw new InvalidOperationException("You can't modify a relation after deleting it.");
			}
			ParseRelationOperation parseRelationOperation = previous as ParseRelationOperation;
			if (parseRelationOperation != null)
			{
				if (parseRelationOperation.TargetClassName != TargetClassName)
				{
					throw new InvalidOperationException(string.Format("Related object must be of class {0}, but {1} was passed in.", parseRelationOperation.TargetClassName, TargetClassName));
				}
				List<string> list = adds.Union(parseRelationOperation.adds.Except(removes)).ToList();
				List<string> list2 = removes.Union(parseRelationOperation.removes.Except(adds)).ToList();
				return new ParseRelationOperation(list, list2, TargetClassName);
			}
			throw new InvalidOperationException("Operation is invalid after previous operation.");
		}

		public object Apply(object oldValue, string key)
		{
			if (adds.Count == 0 && removes.Count == 0)
			{
				return null;
			}
			if (oldValue == null)
			{
				return ParseRelationBase.CreateRelation(null, key, targetClassName);
			}
			if (oldValue is ParseRelationBase)
			{
				ParseRelationBase parseRelationBase = (ParseRelationBase)oldValue;
				string text = parseRelationBase.TargetClassName;
				if (text != null && text != targetClassName)
				{
					throw new InvalidOperationException("Related object must be a " + text + ", but a " + targetClassName + " was passed in.");
				}
				parseRelationBase.TargetClassName = targetClassName;
				return parseRelationBase;
			}
			throw new InvalidOperationException("Operation is invalid after previous operation.");
		}

		private IEnumerable<string> IdsFromObjects(IEnumerable<ParseObject> objects)
		{
			foreach (ParseObject @object in objects)
			{
				if (@object.ObjectId == null)
				{
					throw new ArgumentException("You can't add an unsaved ParseObject to a relation.");
				}
				if (@object.ClassName != targetClassName)
				{
					throw new ArgumentException(string.Format("Tried to create a ParseRelation with 2 different types: {0} and {1}", targetClassName, @object.ClassName));
				}
			}
			return objects.Select((ParseObject o) => o.ObjectId).Distinct();
		}
	}
}
