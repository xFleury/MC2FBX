using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NbtToObj.Helpers
{
    class MultiValueDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, List<TValue>>>
    {
        public Dictionary<TKey, List<TValue>> dict = new Dictionary<TKey, List<TValue>>();

        public List<TValue> this[TKey key]
        {
            get
            {
                return dict[key];
            }
        }

        public void Add(TKey key, TValue value)
        {
            List<TValue> list;
            if (dict.TryGetValue(key, out list))
            {
                list.Add(value);
            }
            else
            {
                dict.Add(key, new List<TValue>() { value });
            }
        }

        public IEnumerator<KeyValuePair<TKey, List<TValue>>> GetEnumerator()
        {
            return dict.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
