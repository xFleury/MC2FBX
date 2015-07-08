using System;
using System.Collections.Generic;

namespace MC2UE
{
    public class DictionaryList<Key, Value>
    {
        public readonly Dictionary<Key, List<Value>> dictionary = new Dictionary<Key, List<Value>>();

        public void Add(Key key, Value value)
        {
            List<Value> valueList;
            if (dictionary.TryGetValue(key, out valueList))
                valueList.Add(value);
            else
            {
                valueList = new List<Value>();
                valueList.Add(value);
                dictionary.Add(key, valueList);
            }
        }
    }
}

