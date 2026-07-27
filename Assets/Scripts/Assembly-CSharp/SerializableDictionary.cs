using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using UnityEngine;

[Serializable]
public class SerializableDictionary<TKey, TVal> : Dictionary<TKey, TVal>, ISerializationCallbackReceiver, IXmlSerializable, ISerializable
{
	[SerializeField]
	private List<TKey> keys;

	[SerializeField]
	private List<TVal> values;

	private const string DictionaryNodeName = "Dictionary";

	private const string ItemNodeName = "Item";

	private const string KeyNodeName = "Key";

	private const string ValueNodeName = "Value";

	private XmlSerializer keySerializer;

	private XmlSerializer valueSerializer;

	protected XmlSerializer ValueSerializer
	{
		get
		{
			if (valueSerializer == null)
			{
				valueSerializer = new XmlSerializer(typeof(TVal));
			}
			return valueSerializer;
		}
	}

	private XmlSerializer KeySerializer
	{
		get
		{
			if (keySerializer == null)
			{
				keySerializer = new XmlSerializer(typeof(TKey));
			}
			return keySerializer;
		}
	}

	public SerializableDictionary()
	{
	}

	public SerializableDictionary(IDictionary<TKey, TVal> dictionary)
		: base(dictionary)
	{
	}

	public SerializableDictionary(IEqualityComparer<TKey> comparer)
		: base(comparer)
	{
	}

	public SerializableDictionary(int capacity)
		: base(capacity)
	{
	}

	public SerializableDictionary(IDictionary<TKey, TVal> dictionary, IEqualityComparer<TKey> comparer)
		: base(dictionary, comparer)
	{
	}

	public SerializableDictionary(int capacity, IEqualityComparer<TKey> comparer)
		: base(capacity, comparer)
	{
	}

	protected SerializableDictionary(SerializationInfo info, StreamingContext context)
	{
		int @int = info.GetInt32("ItemCount");
		for (int i = 0; i < @int; i++)
		{
			KeyValuePair<TKey, TVal> keyValuePair = (KeyValuePair<TKey, TVal>)info.GetValue(string.Format("Item{0}", i), typeof(KeyValuePair<TKey, TVal>));
			Add(keyValuePair.Key, keyValuePair.Value);
		}
	}

	public void OnBeforeSerialize()
	{
		keys = base.Keys.ToList();
		values = base.Values.ToList();
	}

	public void OnAfterDeserialize()
	{
		for (int i = 0; i < keys.Count; i++)
		{
			Add(keys[i], values[i]);
		}
		keys.Clear();
		values.Clear();
	}

	void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
	{
		info.AddValue("ItemCount", base.Count);
		int num = 0;
		using (Enumerator enumerator = GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				KeyValuePair<TKey, TVal> current = enumerator.Current;
				info.AddValue(string.Format("Item{0}", num), current, typeof(KeyValuePair<TKey, TVal>));
				num++;
			}
		}
	}

	void IXmlSerializable.WriteXml(XmlWriter writer)
	{
		using (Enumerator enumerator = GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				KeyValuePair<TKey, TVal> current = enumerator.Current;
				writer.WriteStartElement("Item");
				writer.WriteStartElement("Key");
				KeySerializer.Serialize(writer, current.Key);
				writer.WriteEndElement();
				writer.WriteStartElement("Value");
				ValueSerializer.Serialize(writer, current.Value);
				writer.WriteEndElement();
				writer.WriteEndElement();
			}
		}
	}

	void IXmlSerializable.ReadXml(XmlReader reader)
	{
		if (!reader.IsEmptyElement)
		{
			if (!reader.Read())
			{
				throw new XmlException("Error in Deserialization of Dictionary");
			}
			while (reader.NodeType != XmlNodeType.EndElement)
			{
				reader.ReadStartElement("Item");
				reader.ReadStartElement("Key");
				TKey key = (TKey)KeySerializer.Deserialize(reader);
				reader.ReadEndElement();
				reader.ReadStartElement("Value");
				TVal value = (TVal)ValueSerializer.Deserialize(reader);
				reader.ReadEndElement();
				reader.ReadEndElement();
				Add(key, value);
				reader.MoveToContent();
			}
			reader.ReadEndElement();
		}
	}

	XmlSchema IXmlSerializable.GetSchema()
	{
		return null;
	}
}
