using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    void Awake()
    {
        SceneManager.LoadScene("InteractionUI", LoadSceneMode.Additive);
    }
}
