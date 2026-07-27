using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Parse.Internal;
using Parse.Utilities;

namespace Parse
{
	public class ParseObject : IEnumerable<KeyValuePair<string, object>>, IEnumerable, INotifyPropertyChanged
	{
		private static readonly string AutoClassName = "_Automatic";

		internal readonly object mutex = new object();

		private readonly LinkedList<IDictionary<string, IParseFieldOperation>> operationSetQueue = new LinkedList<IDictionary<string, IParseFieldOperation>>();

		private readonly IDictionary<string, object> estimatedData = new Dictionary<string, object>();

		private static readonly ThreadLocal<bool> isCreatingPointer = new ThreadLocal<bool>(() => false);

		private bool hasBeenFetched;

		private bool dirty;

		internal TaskQueue taskQueue = new TaskQueue();

		private IObjectState state;

		private SynchronizedEventHandler<PropertyChangedEventArgs> propertyChanged = new SynchronizedEventHandler<PropertyChangedEventArgs>();

		internal IObjectState State
		{
			get
			{
				return state;
			}
		}

		internal static IParseObjectController ObjectController
		{
			get
			{
				return ParseCorePlugins.Instance.ObjectController;
			}
		}

		internal static IObjectSubclassingController SubclassingController
		{
			get
			{
				return ParseCorePlugins.Instance.SubclassingController;
			}
		}

		private bool HasDirtyChildren
		{
			get
			{
				lock (mutex)
				{
					return FindUnsavedChildren().FirstOrDefault() != null;
				}
			}
		}

		private bool CanBeSerialized
		{
			get
			{
				lock (mutex)
				{
					return CanBeSerializedAsValue(estimatedData);
				}
			}
		}

		public virtual object this[string key]
		{
			get
			{
				lock (mutex)
				{
					CheckGetAccess(key);
					object obj = estimatedData[key];
					ParseRelationBase parseRelationBase = obj as ParseRelationBase;
					if (parseRelationBase != null)
					{
						parseRelationBase.EnsureParentAndKey(this, key);
					}
					return obj;
				}
			}
			set
			{
				lock (mutex)
				{
					CheckKeyIsMutable(key);
					Set(key, value);
				}
			}
		}

		public bool IsDataAvailable
		{
			get
			{
				lock (mutex)
				{
					return hasBeenFetched;
				}
			}
		}

		internal IDictionary<string, IParseFieldOperation> CurrentOperations
		{
			get
			{
				lock (mutex)
				{
					return operationSetQueue.Last.Value;
				}
			}
		}

		public ICollection<string> Keys
		{
			get
			{
				lock (mutex)
				{
					return estimatedData.Keys;
				}
			}
		}

		[ParseFieldName("ACL")]
		public ParseACL ACL
		{
			get
			{
				return GetProperty<ParseACL>(null, "ACL");
			}
			set
			{
				SetProperty(value, "ACL");
			}
		}

		internal bool IsNew
		{
			get
			{
				return state.IsNew;
			}
			set
			{
				MutateState(delegate(MutableObjectState mutableClone)
				{
					mutableClone.IsNew = value;
				});
				OnPropertyChanged("IsNew");
			}
		}

		[ParseFieldName("updatedAt")]
		public DateTime? UpdatedAt
		{
			get
			{
				return state.UpdatedAt;
			}
		}

		[ParseFieldName("createdAt")]
		public DateTime? CreatedAt
		{
			get
			{
				return state.CreatedAt;
			}
		}

		public bool IsDirty
		{
			get
			{
				lock (mutex)
				{
					return CheckIsDirty(true);
				}
			}
			internal set
			{
				lock (mutex)
				{
					dirty = value;
					OnPropertyChanged("IsDirty");
				}
			}
		}

		[ParseFieldName("objectId")]
		public string ObjectId
		{
			get
			{
				return state.ObjectId;
			}
			set
			{
				IsDirty = true;
				SetObjectIdInternal(value);
			}
		}

		public string ClassName
		{
			get
			{
				return state.ClassName;
			}
		}

		public event PropertyChangedEventHandler PropertyChanged
		{
			add
			{
				propertyChanged.Add(value);
			}
			remove
			{
				propertyChanged.Remove(value);
			}
		}

		internal void MutateState(Action<MutableObjectState> func)
		{
			lock (mutex)
			{
				state = state.MutatedClone(func);
				RebuildEstimatedData();
			}
		}

		protected ParseObject()
			: this(AutoClassName)
		{
		}

		public ParseObject(string className)
		{
			bool value = isCreatingPointer.Value;
			isCreatingPointer.Value = false;
			if (className == null)
			{
				throw new ArgumentException("You must specify a Parse class name when creating a new ParseObject.");
			}
			if (AutoClassName.Equals(className))
			{
				className = SubclassingController.GetClassName(GetType());
			}
			if (!SubclassingController.IsTypeValid(className, GetType()))
			{
				throw new ArgumentException("You must create this type of ParseObject using ParseObject.Create() or the proper subclass.");
			}
			state = new MutableObjectState
			{
				ClassName = className
			};
			OnPropertyChanged("ClassName");
			operationSetQueue.AddLast(new Dictionary<string, IParseFieldOperation>());
			if (!value)
			{
				hasBeenFetched = true;
				IsDirty = true;
				SetDefaultValues();
			}
			else
			{
				IsDirty = false;
				hasBeenFetched = false;
			}
		}

		public static ParseObject Create(string className)
		{
			return SubclassingController.Instantiate(className);
		}

		public static ParseObject CreateWithoutData(string className, string objectId)
		{
			isCreatingPointer.Value = true;
			try
			{
				ParseObject parseObject = SubclassingController.Instantiate(className);
				parseObject.ObjectId = objectId;
				parseObject.IsDirty = false;
				if (parseObject.IsDirty)
				{
					throw new InvalidOperationException("A ParseObject subclass default constructor must not make changes to the object that cause it to be dirty.");
				}
				return parseObject;
			}
			finally
			{
				isCreatingPointer.Value = false;
			}
		}

		public static T Create<T>() where T : ParseObject
		{
			return (T)SubclassingController.Instantiate(SubclassingController.GetClassName(typeof(T)));
		}

		public static T CreateWithoutData<T>(string objectId) where T : ParseObject
		{
			return (T)CreateWithoutData(SubclassingController.GetClassName(typeof(T)), objectId);
		}

		internal static T FromState<T>(IObjectState state, string defaultClassName) where T : ParseObject
		{
			T obj = (T)CreateWithoutData(state.ClassName ?? defaultClassName, state.ObjectId);
			obj.HandleFetchResult(state);
			return obj;
		}

		private static string GetFieldForPropertyName(string className, string propertyName)
		{
			string value = null;
			SubclassingController.GetPropertyMappings(className).TryGetValue(propertyName, out value);
			return value;
		}

		protected void SetProperty<T>(T value, string propertyName)
		{
			this[GetFieldForPropertyName(ClassName, propertyName)] = value;
		}

		protected ParseRelation<T> GetRelationProperty<T>(string propertyName) where T : ParseObject
		{
			return GetRelation<T>(GetFieldForPropertyName(ClassName, propertyName));
		}

		protected T GetProperty<T>(string propertyName)
		{
			return GetProperty(default(T), propertyName);
		}

		protected T GetProperty<T>(T defaultValue, string propertyName)
		{
			T result;
			if (TryGetValue<T>(GetFieldForPropertyName(ClassName, propertyName), out result))
			{
				return result;
			}
			return defaultValue;
		}

		internal virtual void SetDefaultValues()
		{
		}

		public static void RegisterSubclass<T>() where T : ParseObject, new()
		{
			SubclassingController.RegisterSubclass(typeof(T));
		}

		internal static void UnregisterSubclass<T>() where T : ParseObject, new()
		{
			SubclassingController.UnregisterSubclass(typeof(T));
		}

		public void Revert()
		{
			lock (mutex)
			{
				if (CurrentOperations.Count > 0)
				{
					CurrentOperations.Clear();
					RebuildEstimatedData();
					OnPropertyChanged("IsDirty");
				}
			}
		}

		internal virtual void HandleFetchResult(IObjectState serverState)
		{
			lock (mutex)
			{
				MergeFromServer(serverState);
			}
		}

		internal void HandleFailedSave(IDictionary<string, IParseFieldOperation> operationsBeforeSave)
		{
			lock (mutex)
			{
				LinkedListNode<IDictionary<string, IParseFieldOperation>> linkedListNode = operationSetQueue.Find(operationsBeforeSave);
				IDictionary<string, IParseFieldOperation> value = linkedListNode.Next.Value;
				bool flag = value.Count > 0;
				operationSetQueue.Remove(linkedListNode);
				foreach (KeyValuePair<string, IParseFieldOperation> item in operationsBeforeSave)
				{
					IParseFieldOperation value2 = item.Value;
					IParseFieldOperation value3 = null;
					value.TryGetValue(item.Key, out value3);
					value3 = ((value3 == null) ? value2 : value3.MergeWithPrevious(value2));
					value[item.Key] = value3;
				}
				if (!flag && value == CurrentOperations && operationsBeforeSave.Count > 0)
				{
					OnPropertyChanged("IsDirty");
				}
			}
		}

		internal virtual void HandleSave(IObjectState serverState)
		{
			lock (mutex)
			{
				IDictionary<string, IParseFieldOperation> operationsBeforeSave = operationSetQueue.First.Value;
				operationSetQueue.RemoveFirst();
				MutateState(delegate(MutableObjectState mutableClone)
				{
					mutableClone.Apply(operationsBeforeSave);
				});
				MergeFromServer(serverState);
			}
		}

		internal virtual void MergeFromServer(IObjectState serverState)
		{
			Dictionary<string, object> newServerData = serverState.ToDictionary((KeyValuePair<string, object> t) => t.Key, (KeyValuePair<string, object> t) => t.Value);
			lock (mutex)
			{
				if (serverState.ObjectId != null)
				{
					hasBeenFetched = true;
					OnPropertyChanged("IsDataAvailable");
				}
				if (serverState.UpdatedAt.HasValue)
				{
					OnPropertyChanged("UpdatedAt");
				}
				if (serverState.CreatedAt.HasValue)
				{
					OnPropertyChanged("CreatedAt");
				}
				IDictionary<string, ParseObject> dictionary = CollectFetchedObjects();
				foreach (KeyValuePair<string, object> item in serverState)
				{
					object obj = item.Value;
					if (obj is ParseObject)
					{
						ParseObject parseObject = obj as ParseObject;
						if (dictionary.ContainsKey(parseObject.ObjectId))
						{
							obj = dictionary[parseObject.ObjectId];
						}
					}
					newServerData[item.Key] = obj;
				}
				IsDirty = false;
				serverState = serverState.MutatedClone(delegate(MutableObjectState mutableClone)
				{
					mutableClone.ServerData = newServerData;
				});
				MutateState(delegate(MutableObjectState mutableClone)
				{
					mutableClone.Apply(serverState);
				});
			}
		}

		internal void MergeFromObject(ParseObject other)
		{
			lock (mutex)
			{
				if (this == other)
				{
					return;
				}
			}
			if (operationSetQueue.Count != 1)
			{
				throw new InvalidOperationException("Attempt to MergeFromObject during save.");
			}
			operationSetQueue.Clear();
			foreach (IDictionary<string, IParseFieldOperation> item in other.operationSetQueue)
			{
				operationSetQueue.AddLast(item.ToDictionary((KeyValuePair<string, IParseFieldOperation> entry) => entry.Key, (KeyValuePair<string, IParseFieldOperation> entry) => entry.Value));
			}
			lock (mutex)
			{
				state = other.State;
			}
			RebuildEstimatedData();
		}

		internal static IEnumerable<object> DeepTraversal(object root, bool traverseParseObjects = false, bool yieldRoot = false)
		{
			IEnumerable<object> enumerable = DeepTraversalInternal(root, traverseParseObjects, new HashSet<object>(new IdentityEqualityComparer<object>()));
			if (yieldRoot)
			{
				return new object[1] { root }.Concat(enumerable);
			}
			return enumerable;
		}

		private static IEnumerable<object> DeepTraversalInternal(object root, bool traverseParseObjects, ICollection<object> seen)
		{
			seen.Add(root);
			IEnumerable enumerable = (PlatformHooks.IsCompiledByIL2CPP ? null : null);
			IDictionary<string, object> dictionary = Conversion.As<IDictionary<string, object>>(root);
			if (dictionary != null)
			{
				enumerable = dictionary.Values;
			}
			else
			{
				IList<object> list = Conversion.As<IList<object>>(root);
				if (list != null)
				{
					enumerable = list;
				}
				else if (traverseParseObjects)
				{
					ParseObject obj = root as ParseObject;
					if (obj != null)
					{
						enumerable = from k in obj.Keys.ToList()
							select obj[k];
					}
				}
			}
			if (enumerable == null)
			{
				yield break;
			}
			foreach (object i in enumerable)
			{
				if (seen.Contains(i))
				{
					continue;
				}
				yield return i;
				IEnumerable<object> enumerable2 = DeepTraversalInternal(i, traverseParseObjects, seen);
				foreach (object item in enumerable2)
				{
					yield return item;
				}
			}
		}

		private IEnumerable<ParseObject> FindUnsavedChildren()
		{
			return from o in DeepTraversal(estimatedData).OfType<ParseObject>()
				where o.IsDirty
				select o;
		}

		private IDictionary<string, ParseObject> CollectFetchedObjects()
		{
			return (from o in DeepTraversal(estimatedData).OfType<ParseObject>()
				where o.ObjectId != null && o.IsDataAvailable
				group o by o.ObjectId).ToDictionary((IGrouping<string, ParseObject> group) => group.Key, (IGrouping<string, ParseObject> group) => group.Last());
		}

		internal static IDictionary<string, object> ToJSONObjectForSaving(IDictionary<string, IParseFieldOperation> operations)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			foreach (KeyValuePair<string, IParseFieldOperation> operation in operations)
			{
				IParseFieldOperation value = operation.Value;
				dictionary[operation.Key] = PointerOrLocalIdEncoder.Instance.Encode(value);
			}
			return dictionary;
		}

		internal IDictionary<string, object> ServerDataToJSONObjectForSerialization()
		{
			return PointerOrLocalIdEncoder.Instance.Encode(state.ToDictionary((KeyValuePair<string, object> t) => t.Key, (KeyValuePair<string, object> t) => t.Value)) as IDictionary<string, object>;
		}

		internal IDictionary<string, IParseFieldOperation> StartSave()
		{
			lock (mutex)
			{
				IDictionary<string, IParseFieldOperation> currentOperations = CurrentOperations;
				operationSetQueue.AddLast(new Dictionary<string, IParseFieldOperation>());
				OnPropertyChanged("IsDirty");
				return currentOperations;
			}
		}

		internal virtual Task SaveAsync(Task toAwait, CancellationToken cancellationToken)
		{
			IDictionary<string, IParseFieldOperation> currentOperations = null;
			if (!IsDirty)
			{
				return Task.FromResult(0);
			}
			string sessionToken;
			Task task;
			lock (mutex)
			{
				currentOperations = StartSave();
				sessionToken = ParseUser.CurrentSessionToken;
				task = DeepSaveAsync(estimatedData, sessionToken, cancellationToken);
			}
			return task.OnSuccess((Task _) => toAwait).Unwrap().OnSuccess((Task _) => ObjectController.SaveAsync(state, currentOperations, sessionToken, cancellationToken))
				.Unwrap()
				.ContinueWith(delegate(Task<IObjectState> t)
				{
					if (t.IsFaulted || t.IsCanceled)
					{
						HandleFailedSave(currentOperations);
					}
					else
					{
						IObjectState result = t.Result;
						HandleSave(result);
					}
					return t;
				})
				.Unwrap();
		}

		public Task SaveAsync()
		{
			return SaveAsync(CancellationToken.None);
		}

		public Task SaveAsync(CancellationToken cancellationToken)
		{
			return taskQueue.Enqueue((Task toAwait) => SaveAsync(toAwait, cancellationToken), cancellationToken);
		}

		internal virtual Task<ParseObject> FetchAsyncInternal(Task toAwait, CancellationToken cancellationToken)
		{
			return toAwait.OnSuccess(delegate
			{
				if (ObjectId == null)
				{
					throw new InvalidOperationException("Cannot refresh an object that hasn't been saved to the server.");
				}
				return ObjectController.FetchAsync(state, ParseUser.CurrentSessionToken, cancellationToken);
			}).Unwrap().OnSuccess(delegate(Task<IObjectState> t)
			{
				HandleFetchResult(t.Result);
				return this;
			});
		}

		private static Task DeepSaveAsync(object obj, string sessionToken, CancellationToken cancellationToken)
		{
			List<ParseObject> list = new List<ParseObject>();
			CollectDirtyChildren(obj, list);
			HashSet<ParseObject> uniqueObjects = new HashSet<ParseObject>(list, new IdentityEqualityComparer<ParseObject>());
			return Task.WhenAll((from f in DeepTraversal(obj, true).OfType<ParseFile>()
				where f.IsDirty
				select f.SaveAsync(cancellationToken)).ToList()).OnSuccess(delegate
			{
				IEnumerable<ParseObject> remaining = new List<ParseObject>(uniqueObjects);
				return InternalExtensions.WhileAsync(() => Task.FromResult(remaining.Any()), delegate
				{
					List<ParseObject> current = remaining.Where((ParseObject item) => item.CanBeSerialized).ToList();
					List<ParseObject> list2 = remaining.Where((ParseObject item) => !item.CanBeSerialized).ToList();
					remaining = list2;
					if (current.Count == 0)
					{
						throw new InvalidOperationException("Unable to save a ParseObject with a relation to a cycle.");
					}
					return EnqueueForAll(current, (Task toAwait) => toAwait.OnSuccess(delegate
					{
						List<IObjectState> states = current.Select((ParseObject item) => item.state).ToList();
						List<IDictionary<string, IParseFieldOperation>> operationsList = current.Select((ParseObject item) => item.StartSave()).ToList();
						return Task.WhenAll(ObjectController.SaveAllAsync(states, operationsList, sessionToken, cancellationToken)).ContinueWith(delegate(Task<IObjectState[]> t)
						{
							if (t.IsFaulted || t.IsCanceled)
							{
								foreach (var item in current.Zip(operationsList, (ParseObject item, IDictionary<string, IParseFieldOperation> ops) => new { item, ops }))
								{
									item.item.HandleFailedSave(item.ops);
								}
							}
							else
							{
								IObjectState[] result = t.Result;
								foreach (var item2 in current.Zip(result, (ParseObject item, IObjectState state) => new { item, state }))
								{
									item2.item.HandleSave(item2.state);
								}
							}
							cancellationToken.ThrowIfCancellationRequested();
							return t;
						}).Unwrap();
					}).Unwrap().OnSuccess((Task<IObjectState[]> t) => (object)null), cancellationToken);
				});
			}).Unwrap();
		}

		public static Task SaveAllAsync<T>(IEnumerable<T> objects) where T : ParseObject
		{
			return SaveAllAsync(objects, CancellationToken.None);
		}

		public static Task SaveAllAsync<T>(IEnumerable<T> objects, CancellationToken cancellationToken) where T : ParseObject
		{
			return DeepSaveAsync(objects.ToList(), ParseUser.CurrentSessionToken, cancellationToken);
		}

		internal Task<ParseObject> FetchAsyncInternal(CancellationToken cancellationToken)
		{
			return taskQueue.Enqueue((Task toAwait) => FetchAsyncInternal(toAwait, cancellationToken), cancellationToken);
		}

		internal Task<ParseObject> FetchIfNeededAsyncInternal(Task toAwait, CancellationToken cancellationToken)
		{
			if (!IsDataAvailable)
			{
				return FetchAsyncInternal(toAwait, cancellationToken);
			}
			return Task.FromResult(this);
		}

		internal Task<ParseObject> FetchIfNeededAsyncInternal(CancellationToken cancellationToken)
		{
			return taskQueue.Enqueue((Task toAwait) => FetchIfNeededAsyncInternal(toAwait, cancellationToken), cancellationToken);
		}

		public static Task<IEnumerable<T>> FetchAllIfNeededAsync<T>(IEnumerable<T> objects) where T : ParseObject
		{
			return FetchAllIfNeededAsync(objects, CancellationToken.None);
		}

		public static Task<IEnumerable<T>> FetchAllIfNeededAsync<T>(IEnumerable<T> objects, CancellationToken cancellationToken) where T : ParseObject
		{
			return EnqueueForAll(objects.Cast<ParseObject>(), (Task toAwait) => FetchAllInternalAsync(objects, false, toAwait, cancellationToken), cancellationToken);
		}

		public static Task<IEnumerable<T>> FetchAllAsync<T>(IEnumerable<T> objects) where T : ParseObject
		{
			return FetchAllAsync(objects, CancellationToken.None);
		}

		public static Task<IEnumerable<T>> FetchAllAsync<T>(IEnumerable<T> objects, CancellationToken cancellationToken) where T : ParseObject
		{
			return EnqueueForAll(objects.Cast<ParseObject>(), (Task toAwait) => FetchAllInternalAsync(objects, true, toAwait, cancellationToken), cancellationToken);
		}

		private static Task<IEnumerable<T>> FetchAllInternalAsync<T>(IEnumerable<T> objects, bool force, Task toAwait, CancellationToken cancellationToken) where T : ParseObject
		{
			return toAwait.OnSuccess(delegate
			{
				if (objects.Any((T obj) => obj.state.ObjectId == null))
				{
					throw new InvalidOperationException("You cannot fetch objects that haven't already been saved.");
				}
				List<T> objectsToFetch = objects.Where((T obj) => force || !obj.IsDataAvailable).ToList();
				if (objectsToFetch.Count == 0)
				{
					return Task.FromResult(objects);
				}
				Dictionary<string, Task<IEnumerable<ParseObject>>> findsByClass = (from obj in objectsToFetch
					group obj.ObjectId by obj.ClassName into classGroup
					where classGroup.Count() > 0
					select new
					{
						ClassName = classGroup.Key,
						FindTask = new ParseQuery<ParseObject>(classGroup.Key).WhereContainedIn("objectId", classGroup).FindAsync(cancellationToken)
					}).ToDictionary(pair => pair.ClassName, pair => pair.FindTask);
				return Task.WhenAll(findsByClass.Values.ToList()).OnSuccess(delegate
				{
					if (cancellationToken.IsCancellationRequested)
					{
						return objects;
					}
					foreach (var item in from obj in objectsToFetch
						from result in findsByClass[obj.ClassName].Result
						where result.ObjectId == obj.ObjectId
						select new { obj, result })
					{
						item.obj.MergeFromObject(item.result);
						item.obj.hasBeenFetched = true;
					}
					return objects;
				});
			}).Unwrap();
		}

		internal Task DeleteAsync(Task toAwait, CancellationToken cancellationToken)
		{
			if (ObjectId == null)
			{
				return Task.FromResult(0);
			}
			string sessionToken = ParseUser.CurrentSessionToken;
			return toAwait.OnSuccess((Task _) => ObjectController.DeleteAsync(State, sessionToken, cancellationToken)).Unwrap().OnSuccess((Task _) => IsDirty = true);
		}

		public Task DeleteAsync()
		{
			return DeleteAsync(CancellationToken.None);
		}

		public Task DeleteAsync(CancellationToken cancellationToken)
		{
			return taskQueue.Enqueue((Task toAwait) => DeleteAsync(toAwait, cancellationToken), cancellationToken);
		}

		public static Task DeleteAllAsync<T>(IEnumerable<T> objects) where T : ParseObject
		{
			return DeleteAllAsync(objects, CancellationToken.None);
		}

		public static Task DeleteAllAsync<T>(IEnumerable<T> objects, CancellationToken cancellationToken) where T : ParseObject
		{
			HashSet<ParseObject> uniqueObjects = new HashSet<ParseObject>(objects.OfType<ParseObject>().ToList(), new IdentityEqualityComparer<ParseObject>());
			return EnqueueForAll(uniqueObjects, delegate(Task toAwait)
			{
				List<IObjectState> states = uniqueObjects.Select((ParseObject t) => t.state).ToList();
				return toAwait.OnSuccess((Task _) => Task.WhenAll(ObjectController.DeleteAllAsync(states, ParseUser.CurrentSessionToken, cancellationToken))).Unwrap().OnSuccess(delegate
				{
					foreach (ParseObject item in uniqueObjects)
					{
						item.IsDirty = true;
					}
					return (object)null;
				});
			}, cancellationToken);
		}

		private static void CollectDirtyChildren(object node, IList<ParseObject> dirtyChildren, ICollection<ParseObject> seen, ICollection<ParseObject> seenNew)
		{
			foreach (ParseObject item in DeepTraversal(node).OfType<ParseObject>())
			{
				ICollection<ParseObject> collection;
				if (item.ObjectId != null)
				{
					collection = new HashSet<ParseObject>(new IdentityEqualityComparer<ParseObject>());
				}
				else
				{
					if (seenNew.Contains(item))
					{
						throw new InvalidOperationException("Found a circular dependency while saving");
					}
					collection = new HashSet<ParseObject>(seenNew, new IdentityEqualityComparer<ParseObject>());
					collection.Add(item);
				}
				if (seen.Contains(item))
				{
					break;
				}
				seen.Add(item);
				CollectDirtyChildren(item.estimatedData, dirtyChildren, seen, collection);
				if (item.CheckIsDirty(false))
				{
					dirtyChildren.Add(item);
				}
			}
		}

		private static void CollectDirtyChildren(object node, IList<ParseObject> dirtyChildren)
		{
			CollectDirtyChildren(node, dirtyChildren, new HashSet<ParseObject>(new IdentityEqualityComparer<ParseObject>()), new HashSet<ParseObject>(new IdentityEqualityComparer<ParseObject>()));
		}

		private static bool CanBeSerializedAsValue(object value)
		{
			return DeepTraversal(value, false, true).OfType<ParseObject>().All((ParseObject o) => o.ObjectId != null);
		}

		private static Task<T> EnqueueForAll<T>(IEnumerable<ParseObject> objects, Func<Task, Task<T>> taskStart, CancellationToken cancellationToken)
		{
			TaskCompletionSource<object> readyToStart = new TaskCompletionSource<object>();
			LockSet lockSet = new LockSet(objects.Select((ParseObject o) => o.taskQueue.Mutex));
			lockSet.Enter();
			try
			{
				Task<T> fullTask = taskStart(readyToStart.Task);
				List<Task> childTasks = new List<Task>();
				foreach (ParseObject @object in objects)
				{
					@object.taskQueue.Enqueue(delegate(Task task)
					{
						childTasks.Add(task);
						return fullTask;
					}, cancellationToken);
				}
				Task.WhenAll(childTasks.ToArray()).ContinueWith(delegate
				{
					readyToStart.SetResult(null);
				});
				return fullTask;
			}
			finally
			{
				lockSet.Exit();
			}
		}

		public virtual void Remove(string key)
		{
			lock (mutex)
			{
				CheckKeyIsMutable(key);
				PerformOperation(key, ParseDeleteOperation.Instance);
			}
		}

		private void ApplyOperations(IDictionary<string, IParseFieldOperation> operations, IDictionary<string, object> map)
		{
			lock (mutex)
			{
				foreach (KeyValuePair<string, IParseFieldOperation> operation in operations)
				{
					object value;
					map.TryGetValue(operation.Key, out value);
					object obj = operation.Value.Apply(value, operation.Key);
					if (obj != ParseDeleteOperation.DeleteToken)
					{
						map[operation.Key] = obj;
					}
					else
					{
						map.Remove(operation.Key);
					}
				}
			}
		}

		internal void RebuildEstimatedData()
		{
			lock (mutex)
			{
				estimatedData.Clear();
				foreach (KeyValuePair<string, object> item in state)
				{
					estimatedData.Add(item);
				}
				foreach (IDictionary<string, IParseFieldOperation> item2 in operationSetQueue)
				{
					ApplyOperations(item2, estimatedData);
				}
				OnFieldsChanged(null);
			}
		}

		internal void PerformOperation(string key, IParseFieldOperation operation)
		{
			lock (mutex)
			{
				object value;
				estimatedData.TryGetValue(key, out value);
				object obj = operation.Apply(value, key);
				if (obj != ParseDeleteOperation.DeleteToken)
				{
					estimatedData[key] = obj;
				}
				else
				{
					estimatedData.Remove(key);
				}
				bool num = CurrentOperations.Count > 0;
				IParseFieldOperation value2;
				CurrentOperations.TryGetValue(key, out value2);
				IParseFieldOperation value3 = operation.MergeWithPrevious(value2);
				CurrentOperations[key] = value3;
				if (!num)
				{
					OnPropertyChanged("IsDirty");
				}
				OnFieldsChanged(new string[1] { key });
			}
		}

		internal virtual void OnSettingValue(ref string key, ref object value)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
		}

		internal void Set(string key, object value)
		{
			lock (mutex)
			{
				OnSettingValue(ref key, ref value);
				if (!ParseEncoder.IsValidType(value))
				{
					throw new ArgumentException("Invalid type for value: " + value.GetType().ToString());
				}
				PerformOperation(key, new ParseSetOperation(value));
			}
		}

		internal void SetIfDifferent<T>(string key, T value)
		{
			T result;
			bool flag = TryGetValue<T>(key, out result);
			if (value == null)
			{
				if (flag)
				{
					PerformOperation(key, ParseDeleteOperation.Instance);
				}
			}
			else if (!flag || !value.Equals(result))
			{
				Set(key, value);
			}
		}

		public void Increment(string key)
		{
			Increment(key, 1L);
		}

		public void Increment(string key, long amount)
		{
			lock (mutex)
			{
				CheckKeyIsMutable(key);
				PerformOperation(key, new ParseIncrementOperation(amount));
			}
		}

		public void Increment(string key, double amount)
		{
			lock (mutex)
			{
				CheckKeyIsMutable(key);
				PerformOperation(key, new ParseIncrementOperation(amount));
			}
		}

		public void AddToList(string key, object value)
		{
			AddRangeToList(key, new object[1] { value });
		}

		public void AddRangeToList<T>(string key, IEnumerable<T> values)
		{
			lock (mutex)
			{
				CheckKeyIsMutable(key);
				PerformOperation(key, new ParseAddOperation(values.Cast<object>()));
			}
		}

		public void AddUniqueToList(string key, object value)
		{
			AddRangeUniqueToList(key, new object[1] { value });
		}

		public void AddRangeUniqueToList<T>(string key, IEnumerable<T> values)
		{
			lock (mutex)
			{
				CheckKeyIsMutable(key);
				PerformOperation(key, new ParseAddUniqueOperation(values.Cast<object>()));
			}
		}

		public void RemoveAllFromList<T>(string key, IEnumerable<T> values)
		{
			lock (mutex)
			{
				CheckKeyIsMutable(key);
				PerformOperation(key, new ParseRemoveOperation(values.Cast<object>()));
			}
		}

		public bool ContainsKey(string key)
		{
			lock (mutex)
			{
				return estimatedData.ContainsKey(key);
			}
		}

		public T Get<T>(string key)
		{
			return (T)Conversion.ConvertTo<T>(this[key]);
		}

		public ParseRelation<T> GetRelation<T>(string key) where T : ParseObject
		{
			ParseRelation<T> result = null;
			TryGetValue<ParseRelation<T>>(key, out result);
			return result ?? new ParseRelation<T>(this, key);
		}

		public bool TryGetValue<T>(string key, out T result)
		{
			lock (mutex)
			{
				if (ContainsKey(key))
				{
					object obj = Conversion.ConvertTo<T>(this[key]);
					if (obj is T || (obj == null && (!typeof(T).GetTypeInfo().IsValueType || ReflectionHelpers.IsNullable(typeof(T)))))
					{
						result = (T)obj;
						return true;
					}
				}
				result = default(T);
				return false;
			}
		}

		private bool CheckIsDataAvailable(string key)
		{
			lock (mutex)
			{
				return IsDataAvailable || estimatedData.ContainsKey(key);
			}
		}

		private void CheckGetAccess(string key)
		{
			lock (mutex)
			{
				if (!CheckIsDataAvailable(key))
				{
					throw new InvalidOperationException("ParseObject has no data for this key. Call FetchIfNeededAsync() to get the data.");
				}
			}
		}

		private void CheckKeyIsMutable(string key)
		{
			if (!IsKeyMutable(key))
			{
				throw new InvalidOperationException("Cannot change the `" + key + "` property of a `" + ClassName + "` object.");
			}
		}

		internal virtual bool IsKeyMutable(string key)
		{
			return true;
		}

		public bool HasSameId(ParseObject other)
		{
			lock (mutex)
			{
				return other != null && object.Equals(ClassName, other.ClassName) && object.Equals(ObjectId, other.ObjectId);
			}
		}

		public bool IsKeyDirty(string key)
		{
			lock (mutex)
			{
				return CurrentOperations.ContainsKey(key);
			}
		}

		private bool CheckIsDirty(bool considerChildren)
		{
			lock (mutex)
			{
				return dirty || CurrentOperations.Count > 0 || (considerChildren && HasDirtyChildren);
			}
		}

		private void SetObjectIdInternal(string objectId)
		{
			lock (mutex)
			{
				MutateState(delegate(MutableObjectState mutableClone)
				{
					mutableClone.ObjectId = objectId;
				});
				OnPropertyChanged("ObjectId");
			}
		}

		public void Add(string key, object value)
		{
			lock (mutex)
			{
				if (ContainsKey(key))
				{
					throw new ArgumentException("Key already exists", key);
				}
				this[key] = value;
			}
		}

		IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator()
		{
			lock (mutex)
			{
				return estimatedData.GetEnumerator();
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			lock (mutex)
			{
				return ((IEnumerable<KeyValuePair<string, object>>)this).GetEnumerator();
			}
		}

		public static ParseQuery<ParseObject> GetQuery(string className)
		{
			if (SubclassingController.GetType(className) != null)
			{
				throw new ArgumentException("Use the class-specific query properties for class " + className, "className");
			}
			return new ParseQuery<ParseObject>(className);
		}

		protected void OnFieldsChanged(IEnumerable<string> fieldNames)
		{
			IDictionary<string, string> propertyMappings = SubclassingController.GetPropertyMappings(ClassName);
			IEnumerable<string> enumerable = ((fieldNames != null && propertyMappings != null) ? (from m in propertyMappings
				join f in fieldNames on m.Value equals f
				select m.Key) : ((propertyMappings == null) ? Enumerable.Empty<string>() : propertyMappings.Keys));
			foreach (string item in enumerable)
			{
				OnPropertyChanged(item);
			}
			OnPropertyChanged("Item[]");
		}

		protected void OnPropertyChanged(string propertyName)
		{
			propertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
