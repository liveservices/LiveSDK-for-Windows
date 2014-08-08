namespace Microsoft.Live
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Dynamic;

    internal sealed class DynamicDictionary : DynamicObject, IDictionary<string, object>
    {
        private readonly IDictionary<string, object> impl;

        #region Contructor

        public DynamicDictionary()
        {
            this.impl = new Dictionary<string, object>();
        }

        #endregion

        #region Properties

        public ICollection<string> Keys
        {
            get { return this.impl.Keys; }
        }

        #endregion

        #region DynamicObject Methods

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            // NOTE: return null if key not found.
            bool found = this.impl.TryGetValue(binder.Name, out result);
            if (!found)
            {
                result = null;
            }

            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            this.impl[binder.Name] = value;

            return true;
        }

        #endregion

        #region IDictionary<string, object> methods

        public void Add(string key, object value)
        {
            this.impl.Add(key, value);
        }

        public bool ContainsKey(string key)
        {
            return this.impl.ContainsKey(key);
        }

        public bool Remove(string key)
        {
            return this.impl.Remove(key);
        }

        public bool TryGetValue(string key, out object value)
        {
            return this.impl.TryGetValue(key, out value);
        }

        public ICollection<object> Values
        {
            get { return this.impl.Values; }
        }

        public object this[string key]
        {
            get
            {
                return this.impl[key];
            }
            set
            {
                this.impl[key] = value;
            }
        }

        public void Add(KeyValuePair<string, object> item)
        {
            this.impl.Add(item);
        }

        public void Clear()
        {
            this.impl.Clear();
        }

        public bool Contains(KeyValuePair<string, object> item)
        {
            return this.impl.Contains(item);
        }

        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            this.impl.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return this.impl.Count; }
        }

        public bool IsReadOnly
        {
            get { return this.impl.IsReadOnly; }
        }

        public bool Remove(KeyValuePair<string, object> item)
        {
            return this.impl.Remove(item);
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return this.impl.GetEnumerator();
        }

        #endregion

        #region IEnumerable Impl

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.impl.GetEnumerator();
        }

        #endregion
    }
}
