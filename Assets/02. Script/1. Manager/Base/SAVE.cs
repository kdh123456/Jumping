using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SAVE
{
    public Vector2 position = new Vector2(-9f, 4f);
    public float timer = 0f;
    public bool isFirst = true;
    //public List<float> timeList = null;
    public float[] timeList = new float[10] { 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f };
}
