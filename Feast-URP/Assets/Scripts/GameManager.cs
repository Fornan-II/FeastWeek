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
            if (!_initialized) CreateInstance();
            return _instance;
        }
    }

    private static bool _initialized = false;

    private static void CreateInstance()
    {
        _initialized = true;
        _instance = new GameObject("~GameManager").AddComponent<GameManager>();
        DontDestroyOnLoad(_instance.gameObject);

        _instance.Controls = new DefaultControls();
    }

    public DefaultControls Controls { get; private set; }

    private void OnDestroy() => _instance = null;
}
