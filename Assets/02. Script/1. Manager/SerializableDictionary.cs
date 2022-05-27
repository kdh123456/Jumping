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
        /// ?덈줈??KeyValuePair??異붽??섎ŉ, ?몄뒪?숉꽣???낅뜲?댄듃
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public new void Add(TKey key, TValue value)
        {
            base.Add(key, value);
            SyncInspectorFromDictionary();
        }
        /// <summary>
        /// KeyValuePair????젣?섎ŉ, ?몄뒪?숉꽣???낅뜲?댄듃
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
        /// ?몄뒪?숉꽣瑜??뺤뀛?덈━濡?珥덇린??
        /// </summary>
        public void SyncInspectorFromDictionary()
        {
            //?몄뒪?숉꽣 ??諛몃쪟 由ъ뒪??珥덇린??
            g_InspectorKeys.Clear();
            g_InspectorValues.Clear();

            foreach (KeyValuePair<TKey, TValue> pair in this)
            {
                g_InspectorKeys.Add(pair.Key); g_InspectorValues.Add(pair.Value);
            }
        }

        /// <summary>
        /// ?뺤뀛?덈━瑜??몄뒪?숉꽣濡?珥덇린??
        /// </summary>
        public void SyncDictionaryFromInspector()
        {
            //?뺤뀛?덈━ ??諛몃쪟 由ъ뒪??珥덇린??
            foreach (var key in Keys.ToList())
            {
                base.Remove(key);
            }

            for (int i = 0; i < g_InspectorKeys.Count; i++)
            {
                //以묐났???ㅺ? ?덈떎硫??먮윭 異쒕젰
                if (this.ContainsKey(g_InspectorKeys[i]))
                {
                    Debug.LogError("以묐났???ㅺ? ?덉뒿?덈떎.");
                    break;
                }
                base.Add(g_InspectorKeys[i], g_InspectorValues[i]);
            }
        }

        public void OnAfterDeserialize()
        {
            Debug.Log(this + string.Format("?몄뒪?숉꽣 ????: {0} 媛???: {1}", g_InspectorKeys.Count, g_InspectorValues.Count));

            //?몄뒪?숉꽣??Key Value媛 KeyValuePair ?뺥깭瑜???寃쎌슦
            if (g_InspectorKeys.Count == g_InspectorValues.Count)
            {
                SyncDictionaryFromInspector();
            }
        }
    }
}