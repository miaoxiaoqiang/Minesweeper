using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

using MvvmLight;

namespace Minesweeper.Model
{
    [Serializable]
    public sealed class ObservableDictionary<TKey, TValue> : SortedDictionary<TKey, TValue>, INotifyCollectionChanged, INotifyPropertyChanged, IXmlSerializable
    {
        private int _index;
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableDictionary() : base()
        {

        }

        public ObservableDictionary(IDictionary<TKey, TValue> dictionary) : base(dictionary)
        {

        }

        public ObservableDictionary(IComparer<TKey> comparer) : base(comparer)
        {

        }

        public ObservableDictionary(IDictionary<TKey, TValue> dictionary, IComparer<TKey> comparer) : base(dictionary, comparer)
        {

        }

        //public ObservableDictionary(int capacity) : base(capacity)
        //{

        //}

        //public ObservableDictionary(IEqualityComparer<TKey> comparer) : base(comparer)
        //{

        //}

        //public ObservableDictionary(IDictionary<TKey, TValue> dictionary) : base(dictionary)
        //{

        //}

        //public ObservableDictionary(int capacity, IEqualityComparer<TKey> comparer) : base(capacity, comparer)
        //{

        //}

        //public ObservableDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer) : base(dictionary, comparer)
        //{

        //}

        public new KeyCollection Keys => base.Keys;

        public new ValueCollection Values => base.Values;

        public new int Count => base.Count;

        public new TValue this[TKey key]
        {
            get { return this.GetValue(key); }
            set { this.SetValue(key, value); }
        }

        public TValue this[int index]
        {
            get { return this.GetIndexValue(index); }
            set { this.SetIndexValue(index, value); }
        }

        public new void Add(TKey key, TValue value)
        {
            base.Add(key, value);

            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, this.FindPair(key), _index));
            OnPropertyChanged("Keys");
            OnPropertyChanged("Values");
            OnPropertyChanged("Count");
        }

        public new void Clear()
        {
            base.Clear();

            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            OnPropertyChanged("Keys");
            OnPropertyChanged("Values");
            OnPropertyChanged("Count");
        }

        public new bool Remove(TKey key)
        {
            var pair = this.FindPair(key);
            if (base.Remove(key))
            {
                this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, pair, _index));
                OnPropertyChanged("Keys");
                OnPropertyChanged("Values");
                OnPropertyChanged("Count");
                return true;
            }
            return false;
        }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            //reader.Read();
            //XmlSerializer KeySerializer = new XmlSerializer(typeof(TKey));
            //XmlSerializer ValueSerializer = new XmlSerializer(typeof(TValue));

            //while (reader.NodeType != XmlNodeType.EndElement)
            //{
            //    reader.ReadStartElement("record");

            //    reader.ReadStartElement("level");
            //    TKey tk = (TKey)KeySerializer.Deserialize(reader);
            //    reader.ReadEndElement();

            //    reader.ReadStartElement("histories");
            //    TValue vl = (TValue)ValueSerializer.Deserialize(reader);
            //    reader.ReadEndElement();

            //    reader.ReadEndElement();

            //    this.Add(tk, vl);
            //    reader.MoveToContent();
            //}
            //reader.ReadEndElement();

            XmlSerializer kvSerializer = new XmlSerializer(typeof(PlayerRecord));

            bool wasEmpty = reader.IsEmptyElement;
            reader.Read();
            if (wasEmpty)
            {
                return;
            }

            while (reader.NodeType != XmlNodeType.EndElement)
            {
                PlayerRecord kv = (PlayerRecord)kvSerializer.Deserialize(reader);
                this.Add(kv.Level, kv.Record);
                reader.MoveToContent();
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            XmlSerializerNamespaces namespaces = new();
            namespaces.Add(string.Empty, string.Empty);

            //XmlSerializer KeySerializer = new XmlSerializer(typeof(TKey));
            //XmlSerializer ValueSerializer = new XmlSerializer(typeof(TValue));

            //foreach (KeyValuePair<TKey, TValue> kv in this)
            //{
            //    writer.WriteStartElement("record");

            //    writer.WriteStartElement("level");
            //    KeySerializer.Serialize(writer, kv.Key, namespaces);
            //    writer.WriteEndElement();

            //    writer.WriteStartElement("histories");
            //    ValueSerializer.Serialize(writer, kv.Value, namespaces);
            //    writer.WriteEndElement();

            //    writer.WriteEndElement();
            //}

            XmlSerializer kvSerializer = new XmlSerializer(typeof(PlayerRecord));
            foreach (KeyValuePair<TKey, TValue> kv in this)
            {
                PlayerRecord item = new PlayerRecord
                {
                    Level = kv.Key,
                    Record = kv.Value
                };
                kvSerializer.Serialize(writer, item, namespaces);
            }
        }

        private void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            this.CollectionChanged?.Invoke(this, e);

            //if (SortingSelector == null || e.Action == NotifyCollectionChangedAction.Reset || e.Action == NotifyCollectionChangedAction.Remove)
            //{
            //    return;
            //}

            //var query = this.Select((item, index) => (Item: item, Index: index));
            //query = Descending ? query.OrderBy(tuple => SortingSelector(tuple.Item.Key)) : query.OrderByDescending(tuple => SortingSelector(tuple.Item.Key));

            //IEnumerable<(int OldIndex, int NewIndex)> map = query.Select((tuple, index) => (OldIndex: tuple.Index, NewIndex: index)).Where(o => o.OldIndex != o.NewIndex);
            //using (var enumerator = map.GetEnumerator())
            //{
            //    if (enumerator.MoveNext())
            //    {
            //        //Move(enumerator.Current.OldIndex, enumerator.Current.NewIndex);
            //    }
            //}
        }

        private void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #region private方法
        private TValue GetIndexValue(int index)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (i == index)
                {
                    var pair = this.ElementAt(i);
                    return pair.Value;
                }
            }

            return default(TValue);
        }

        private void SetIndexValue(int index, TValue value)
        {
            try
            {
                var pair = this.ElementAtOrDefault(index);
                SetValue(pair.Key, value);
            }
            catch (Exception)
            {

            }
        }

        private TValue GetValue(TKey key)
        {
            if (base.ContainsKey(key))
            {
                return base[key];
            }
            else
            {
                return default(TValue);
            }
        }

        private void SetValue(TKey key, TValue value)
        {
            if (base.ContainsKey(key))
            {
                var pair = this.FindPair(key);
                int index = _index;
                base[key] = value;
                var newpair = this.FindPair(key);
                this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, newpair, pair, index));
                OnPropertyChanged("Values");
                OnPropertyChanged("Item[]");
            }
            else
            {
                this.Add(key, value);
            }
        }

        private KeyValuePair<TKey, TValue> FindPair(TKey key)
        {
            _index = 0;
            foreach (var item in this)
            {
                if (item.Key.Equals(key))
                {
                    return item;
                }
                _index++;
            }
            return default(KeyValuePair<TKey, TValue>);
        }

        private int IndexOf(TKey key)
        {
            int index = 0;
            foreach (var item in this)
            {
                if (item.Key.Equals(key))
                {
                    return index;
                }
                index++;

            }
            return -1;
        }
        #endregion

        [XmlRoot(ElementName = "record")]
        [Serializable]
        public sealed class PlayerRecord : ViewModelBase
        {
            private TKey level;
            [XmlElement(ElementName = "level")]
            public TKey Level
            {
                get
                {
                    return level;
                }
                set
                {
                    level = value;
                    RaisePropertyChanged();
                }
            }

            private TValue record;
            [XmlElement(ElementName = "game")]
            public TValue Record
            {
                get
                {
                    return record;
                }
                set
                {
                    record = value;
                    RaisePropertyChanged();
                }
            }
        }

        public sealed class LevelComparer : IComparer<GameLevel>
        {
            public int Compare(GameLevel x, GameLevel y)
            {
                return x.CompareTo(y);
            }
        }
    }
}
