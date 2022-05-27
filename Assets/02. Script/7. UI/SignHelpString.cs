using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignHelpString : MonoSingleton<SignHelpString>
{
    [System.Serializable]
    public class SignDictionary : CustomDic.SerializableDictionary<SignHelpStringState, string>
    {

    }

    public SignDictionary signDictionary;
}
