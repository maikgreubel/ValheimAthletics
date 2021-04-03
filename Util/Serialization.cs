using System;
using System.Collections.Generic;
using UnityEngine;

namespace ValheimAthletics.Util
{
    /**
     * <summary>
     * Wrapper class for generic dictionary types.
     * </summary>
     * <seealso cref="Dictionary{TKey, TValue}"/>
     **/
    [Serializable]
    public class Serialization<TKey, TValue> : ISerializationCallbackReceiver
    {
        [SerializeField]
        List<TKey> keys;
        [SerializeField]
        List<TValue> values;

        Dictionary<TKey, TValue> target;
        /**
         * <summary>Retrieve the deserialized key-value pairs as <see cref="Dictionary{TKey, TValue}"/></summary>
         **/
        public Dictionary<TKey, TValue> ToDictionary() { return this.target; }

        /**
         * <summary>Create a new instance for wrapping <see cref="Dictionary{TKey, TValue}"/> objects to <see cref="SerializableAttribute"/></summary>
         **/
        public Serialization(Dictionary<TKey, TValue> target)
        {
            this.target = target;
        }

        /**
         * <summary>Callback which is executed before serialization takes place.</summary>
         **/
        public void OnBeforeSerialize()
        {
            this.keys = new List<TKey>(target.Keys);
            this.values = new List<TValue>(target.Values);
        }

        /**
         * <summary>Callback which is executed after deserialization.</summary>
         **/
        public void OnAfterDeserialize()
        {
            int count = Math.Min(keys.Count, values.Count);
            this.target = new Dictionary<TKey, TValue>(count);
            for(int i = 0; i < count; i++)
            {
                target.Add(keys[i], values[i]);
            }
        }
    }
}
