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
        /// ??덉쨮??KeyValuePair???곕떽???렽? ?紐꾨뮞??됯숲????낅쑓??꾨뱜
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public new void Add(TKey key, TValue value)
        {
            base.Add(key, value);
            SyncInspectorFromDictionary();
        }
        /// <summary>
        /// KeyValuePair???????렽? ?紐꾨뮞??됯숲????낅쑓??꾨뱜
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
        /// ?紐꾨뮞??됯숲???類ㅻ??댿봺嚥??λ뜃由??
        /// </summary>
        public void SyncInspectorFromDictionary()
        {
            //?紐꾨뮞??됯숲 ??獄쏅챶履??귐딅뮞???λ뜃由??
            g_InspectorKeys.Clear();
            g_InspectorValues.Clear();

            foreach (KeyValuePair<TKey, TValue> pair in this)
            {
                g_InspectorKeys.Add(pair.Key); g_InspectorValues.Add(pair.Value);
            }
        }

        /// <summary>
        /// ?類ㅻ??댿봺???紐꾨뮞??됯숲嚥??λ뜃由??
        /// </summary>
        public void SyncDictionaryFromInspector()
        {
            //?類ㅻ??댿봺 ??獄쏅챶履??귐딅뮞???λ뜃由??
            foreach (var key in Keys.ToList())
            {
                base.Remove(key);
            }

            for (int i = 0; i < g_InspectorKeys.Count; i++)
            {
                //餓λ쵎?????? ??덈뼄筌??癒?쑎 ?곗뮆??
                if (this.ContainsKey(g_InspectorKeys[i]))
                {
                    Debug.LogError("餓λ쵎?????? ??됰뮸??덈뼄.");
                    break;
                }
                base.Add(g_InspectorKeys[i], g_InspectorValues[i]);
            }
        }

        public void OnAfterDeserialize()
        {
            Debug.Log(this + string.Format("?紐꾨뮞??됯숲 ????: {0} 揶???: {1}", g_InspectorKeys.Count, g_InspectorValues.Count));

            //?紐꾨뮞??됯숲??Key Value揶쎛 KeyValuePair ?類κ묶????野껋럩??
            if (g_InspectorKeys.Count == g_InspectorValues.Count)
            {
                SyncDictionaryFromInspector();
            }
        }
    }
}