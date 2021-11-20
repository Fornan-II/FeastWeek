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
    private int _checkpointIndex = -1;
    private Util.CursorMode _cachedCursorMode;

    private NoClipPawn _activeNoClipPawn;

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
                _cachedCursorMode = Util.CursorMode.GetCurrent();
                Util.CursorMode.Default.Apply();
            }
            else
            {
                if(Util.CursorMode.GetCurrent() == Util.CursorMode.Default)
                    _cachedCursorMode.Apply();
            }
        }

        _wasPressed = isPressed;
    }

    private void OnGUI()
    {
        if (!_menuOpen) return;

        float height = 25f;
        Rect rect = new Rect(0, 0, 250f, height);

        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            if (GUI.Button(rect, "Load completed game menu")) LoadCompletedGameMenu();
        }
        else
        {
            if (GUI.Button(rect, "Toggle noclip")) ToggleNoClip();
            rect.y += height;
            if (GUI.Button(rect, "Open castle door")) OpenCastleDoor();
            rect.y += height;
            if (_checkpoints.Length > 0)
            {
                if (GUI.Button(rect, "Warp to next Checkpoint")) NextCheckpoint();
                rect.y += height;
                if (GUI.Button(rect, "Warp to previous Checkpoint")) PreviousCheckpoint();
                rect.y += height;
            }
            if (GUI.Button(rect, "Skip intro sequence")) SkipIntroSequence();
            rect.y += height;
        }
    }

    private void OnSceneLoad(Scene loadedScene, LoadSceneMode mode) => _checkpoints = FindObjectsOfType<Checkpoint>();
    private void OnSceneUnloaded(Scene unloadedScene) => _checkpoints = FindObjectsOfType<Checkpoint>();

    private void LoadCompletedGameMenu()
    {
        GlobalData.HasCompletedGame = true;
        SceneManager.LoadScene(0);
    }

    private void ToggleNoClip()
    {
        Controller playerController = FindObjectOfType<Controller>();
        if (!playerController)
        {
            Debug.LogError("[DebugMenu] Failed to toggle noclip because there is no player controller!");
            return;
        }
        if (!playerController.ControlledPawn)
        {
            Debug.LogError("[DebugMenu] Failed to toggle noclip because player is not controlling a pawn!");
            return;
        }

        if(!_activeNoClipPawn)
        {
            _activeNoClipPawn = Instantiate(Resources.Load<NoClipPawn>("NoClipCharacter"));
        }
        
        if(playerController.ControlledPawn == _activeNoClipPawn)
        {
            // Player is currently noclipping and we want to toggle them out of noclipping.
            _activeNoClipPawn.OwnerPawn.transform.position = _activeNoClipPawn.transform.position;

            _activeNoClipPawn.OwnerPawn.gameObject.SetActive(true);
            _activeNoClipPawn.gameObject.SetActive(false);

            _activeNoClipPawn.ReturnControl();
        }
        else
        {
            // Player is not currently noclipping, and we want to toggle them into noclipping.
            _activeNoClipPawn.gameObject.SetActive(true);
            playerController.ControlledPawn.gameObject.SetActive(false);

            _activeNoClipPawn.transform.position = playerController.ControlledPawn.transform.position;

            _activeNoClipPawn.TakeControlFrom(playerController.ControlledPawn);
        }
    }

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

        if (PlayerRef.Transform && PlayerRef.Transform.TryGetComponent<ICheckpointUser>(out ICheckpointUser player))
            _checkpoints[_checkpointIndex].ResetAt(player);
        else
            Debug.LogError("[DebugMenu] Can't move player because there is no player!");
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

        if (PlayerRef.Transform && PlayerRef.Transform.TryGetComponent<ICheckpointUser>(out ICheckpointUser player))
            _checkpoints[_checkpointIndex].ResetAt(player);
        else
            Debug.LogError("[DebugMenu] Can't move player because there is no player!");
    }

    private void SkipIntroSequence()
    {
        bool skipIntroFailed = false;
        IntroHelper intro = FindObjectOfType<IntroHelper>();
        Controller playerController = FindObjectOfType<Controller>();
        FPSChar playerPawn = FindObjectOfType<FPSChar>();

        if(playerController)
        {
            if (playerController.ControlledPawn == playerPawn)
            {
                Debug.Log("[DebugMenu] Already past intro sequence.");
                return;
            }
        }
        else
        {
            Debug.LogError("[DebugMenu] Failed to skip intro sequence, no player controller found!");
            skipIntroFailed = true;
        }
        if (!playerPawn)
        {
            Debug.LogError("[DebugMenu] Failed to skip intro sequence, no player pawn found!");
            skipIntroFailed = true;
        }
        if (!intro)
        {
            Debug.LogError("[DebugMenu] Failed to skip intro sequence, no IntroHelper found!");
            skipIntroFailed = true;
        }
        if (skipIntroFailed) return;

        intro.FinishFlying();
        intro.gameObject.SetActive(false);
    }
}
#endif