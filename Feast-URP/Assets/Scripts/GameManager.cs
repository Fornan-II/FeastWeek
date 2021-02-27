using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (!_instance)
                CreateInstance();
            return _instance;
        }
    }

    private static void CreateInstance()
    {
        _instance = new GameObject("~GameManager").AddComponent<GameManager>();
        DontDestroyOnLoad(_instance.gameObject);
    }

    
}
