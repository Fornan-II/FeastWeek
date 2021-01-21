#if UNITY_EDITOR || DEVELOPMENT_BUILD
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class DebugMenu : MonoBehaviour
{
    public static DebugMenu Instance { get; private set; }

    private bool _menuOpen = false;
    private bool _wasPressed = false;

    private Checkpoint[] _checkpoints;
    int _checkpointIndex = -1;
    Util.CursorMode cachedCursorMode;

    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoad;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
            _checkpoints = FindObjectsOfType<Checkpoint>();
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
            SceneManager.sceneLoaded -= OnSceneLoad;
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
        }
    }

    private void Update()
    {
        bool isPressed = Keyboard.current.backquoteKey.isPressed;

        if(isPressed && !_wasPressed)
        {
            _menuOpen = !_menuOpen;
            if(_menuOpen)
            {
                cachedCursorMode = Util.CursorMode.GetCurrent();
                Util.CursorMode.Default.Apply();
            }
            else
            {
                cachedCursorMode.Apply();
            }
        }

        _wasPressed = isPressed;
    }

    private void OnGUI()
    {
        if (!_menuOpen) return;

        float height = 25f;
        Rect rect = new Rect(0, 0, 250f, height);

        if (GUI.Button(rect, "OpenCastleDoor")) OpenCastleDoor();
        rect.y += height;
        if (_checkpoints.Length > 0)
        {
            if (GUI.Button(rect, "Warp to next Checkpoint")) NextCheckpoint();
            rect.y += height;
            if (GUI.Button(rect, "Warp to previous Checkpoint")) PreviousCheckpoint();
            rect.y += height;
        }
    }

    private void OnSceneLoad(Scene loadedScene, LoadSceneMode mode) => _checkpoints = FindObjectsOfType<Checkpoint>();
    private void OnSceneUnloaded(Scene unloadedScene) => _checkpoints = FindObjectsOfType<Checkpoint>();

    private void OpenCastleDoor() => FindObjectOfType<DoorMechanic>()?.OpenDoor();

    private void NextCheckpoint()
    {
        if (_checkpointIndex < 0)
        {
            for (int i = 0; i < _checkpoints.Length; ++i)
            {
                if (_checkpoints[i] == Checkpoint.LastCheckpoint)
                {
                    _checkpointIndex = i;
                }
            }
        }

        ++_checkpointIndex;
        if (_checkpointIndex >= _checkpoints.Length)
            _checkpointIndex = 0;

        _checkpoints[_checkpointIndex].ResetAt(FindObjectOfType<CheckpointUser>());
    }

    private void PreviousCheckpoint()
    {
        if (_checkpointIndex < 0)
        {
            for (int i = 0; i < _checkpoints.Length; ++i)
            {
                if (_checkpoints[i] == Checkpoint.LastCheckpoint)
                {
                    _checkpointIndex = i;
                }
            }
        }

        --_checkpointIndex;
        if (_checkpointIndex < 0)
            _checkpointIndex = _checkpoints.Length - 1;

        _checkpoints[_checkpointIndex].ResetAt(FindObjectOfType<CheckpointUser>());
    }
}
#endif