using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;

namespace CustomDic
{
    [System.Serializable]
    //[CanEditMultipleObjects]
    //[ExecuteInEditMode]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        public List<TKey> g_InspectorKeys;
        public List<TValue> g_InspectorValues;

        public SerializableDictionary()
        {
            g_InspectorKeys = new List<TKey>();
            g_InspectorValues = new List<TValue>();
            SyncInspectorFromDictionary();
        }
        /// <summary>
        /// ???�쨮??KeyValuePair???곕떽????? ?紐꾨�????��?????�쑓??꾨뱜
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public new void Add(TKey key, TValue value)
        {
            base.Add(key, value);
            SyncInspectorFromDictionary();
        }
        /// <summary>
        /// KeyValuePair????????? ?紐꾨�????��?????�쑓??꾨뱜
        /// </summary>
        /// <param name="key"></param>
        public new void Remove(TKey key)
        {
            base.Remove(key);
            SyncInspectorFromDictionary();
        }

        public void OnBeforeSerialize()
        {
        }
        /// <summary>
        /// ?紐꾨�????��???類ㅻ????�봺???λ?�由??
        /// </summary>
        public void SyncInspectorFromDictionary()
        {
            //?紐꾨�????�� ???�쏅챶履??귐딅�???λ?�由??
            g_InspectorKeys.Clear();
            g_InspectorValues.Clear();

            foreach (KeyValuePair<TKey, TValue> pair in this)
            {
                g_InspectorKeys.Add(pair.Key); g_InspectorValues.Add(pair.Value);
            }
        }

        /// <summary>
        /// ?類ㅻ????�봺???紐꾨�????��???λ?�由??
        /// </summary>
        public void SyncDictionaryFromInspector()
        {
            //?類ㅻ????�봺 ???�쏅챶履??귐딅�???λ?�由??
            foreach (var key in Keys.ToList())
            {
                base.Remove(key);
            }

            for (int i = 0; i < g_InspectorKeys.Count; i++)
            {
                //餓λ쵎?????? ???�뼄�???????곗뮆??
                if (this.ContainsKey(g_InspectorKeys[i]))
                {
                    Debug.LogError("餓λ쵎?????? ???��????�뼄.");
                    break;
                }
                base.Add(g_InspectorKeys[i], g_InspectorValues[i]);
            }
        }

        public void OnAfterDeserialize()
        {

            //?紐꾨�????��??Key Value?�쎛? KeyValuePair ?類κ묶?????�껋???
            if (g_InspectorKeys.Count == g_InspectorValues.Count)
            {
                SyncDictionaryFromInspector();
            }
        }
    }
}