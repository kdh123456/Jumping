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
        /// ???‰ì¨®??KeyValuePair???ê³•ë–½????? ?ï§ê¾¨ë®????ˆ²?????…ì‘“??ê¾¨ë±œ
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public new void Add(TKey key, TValue value)
        {
            base.Add(key, value);
            SyncInspectorFromDictionary();
        }
        /// <summary>
        /// KeyValuePair????????? ?ï§ê¾¨ë®????ˆ²?????…ì‘“??ê¾¨ë±œ
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
        /// ?ï§ê¾¨ë®????ˆ²???ï§ã…»????¿ë´º???Î»?ƒç”±??
        /// </summary>
        public void SyncInspectorFromDictionary()
        {
            //?ï§ê¾¨ë®????ˆ² ???„ì…ì±¶ï§Ÿ??ê·ë”…ë®???Î»?ƒç”±??
            g_InspectorKeys.Clear();
            g_InspectorValues.Clear();

            foreach (KeyValuePair<TKey, TValue> pair in this)
            {
                g_InspectorKeys.Add(pair.Key); g_InspectorValues.Add(pair.Value);
            }
        }

        /// <summary>
        /// ?ï§ã…»????¿ë´º???ï§ê¾¨ë®????ˆ²???Î»?ƒç”±??
        /// </summary>
        public void SyncDictionaryFromInspector()
        {
            //?ï§ã…»????¿ë´º ???„ì…ì±¶ï§Ÿ??ê·ë”…ë®???Î»?ƒç”±??
            foreach (var key in Keys.ToList())
            {
                base.Remove(key);
            }

            for (int i = 0; i < g_InspectorKeys.Count; i++)
            {
                //é¤“Î»ìµ?????? ???ˆë¼„ç­???????ê³—ë®†??
                if (this.ContainsKey(g_InspectorKeys[i]))
                {
                    Debug.LogError("é¤“Î»ìµ?????? ???°ë????ˆë¼„.");
                    break;
                }
                base.Add(g_InspectorKeys[i], g_InspectorValues[i]);
            }
        }

        public void OnAfterDeserialize()
        {
            Debug.Log(this + string.Format("?ï§ê¾¨ë®????ˆ² ????: {0} ????: {1}", g_InspectorKeys.Count, g_InspectorValues.Count));

            //?ï§ê¾¨ë®????ˆ²??Key Value?¶ì›? KeyValuePair ?ï§Îºë¬¶?????ê»‹???
            if (g_InspectorKeys.Count == g_InspectorValues.Count)
            {
                SyncDictionaryFromInspector();
            }
        }
    }
}