using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NbtToObj.Helpers
{
    class MultiValueDictionary<TKey, TValue>
    {
        public Dictionary<TKey, List<TValue>> dict = new Dictionary<TKey, List<TValue>>();

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
    }
}
