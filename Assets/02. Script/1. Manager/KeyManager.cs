using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public enum KeyAction { LEFT, RIGHT, JUMP, SWALLOW, SKILL, KEYCOUNT }
public static class KeySetting { public static Dictionary<KeyAction, KeyCode> keys = new Dictionary<KeyAction, KeyCode>(); }
public class KeyManager : MonoBehaviour
{
    private KEY saveKey;

    private readonly string SAVE_FILENAME = "/SaveKey.txt";

    KeyCode[] defaultKeys = new KeyCode[] { KeyCode.A, KeyCode.D, KeyCode.Space, KeyCode.F, KeyCode.E };
    private void Awake()
    {
        saveKey = GameManager.Instance.LoadJsonFile<KEY>(GameManager.Instance.SAVE_PATH, SAVE_FILENAME);

        for (int i = 0; i < (int)KeyAction.KEYCOUNT; i++)
        {
            //KeySetting.keys.Add((KeyAction)i, defaultKeys[i]);
            KeySetting.keys.Add((KeyAction)i, saveKey.keys[i]);
        }
    }
    private void OnGUI()
    {
        Event keyEvent = Event.current;
        if (keyEvent.isKey)
        {
            if (keyEvent.keyCode == KeyCode.Escape || key == -1) return;
            KeySetting.keys[(KeyAction)key] = keyEvent.keyCode;

            saveKey.keys[key] = keyEvent.keyCode;
            GameManager.Instance.SaveJson<KEY>(GameManager.Instance.SAVE_PATH, SAVE_FILENAME, saveKey);

            key = -1;

            UIManager.Instance.UpdateKey();
        }
    }
    int key = -1;
    public void ChangeKey(int num)
    {
        key = num;
    }
}