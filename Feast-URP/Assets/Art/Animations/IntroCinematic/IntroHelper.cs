using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class IntroHelper : Pawn, DefaultControls.IFPSCharacterActions
{
#pragma warning disable 0649
    [SerializeField] private FadeUI[] introPanels;
    [SerializeField] private CameraView cameraView = null;
    [SerializeField] private Animator anim;
    [SerializeField] private Pawn playerPawn;
    [SerializeField] private Controller playerController;
    [SerializeField] private MusicManager musicManager;
    [SerializeField] private float advanceAlphaThreshold = 0.67f;
    [SerializeField] private AudioClip themeSong;
    [SerializeField] private AudioClip mainAmbient;
    [SerializeField] private float musicCrossfadeTime = 135f;

    private int _currentPanel = 0;
    private bool _canAdvancePanel = false;
    private bool _inPanelSequence = true;

    private void Start()
    {
        _currentPanel = 0;
        ShowPanel();

        if (!IsBeingControlled)
            playerController.TakeControlOf(this);
    }

    #region Input
    protected override void ActivateInput()
    {
        GameManager.Instance.Controls.FPSCharacter.SetCallbacks(this);
        GameManager.Instance.Controls.FPSCharacter.Enable();
    }

    protected override void DeactivateInput()
    {
        GameManager.Instance.Controls.FPSCharacter.SetCallbacks(null);
        GameManager.Instance.Controls.FPSCharacter.Disable();
    }

    public void OnWalk(InputAction.CallbackContext context) { /* Do nothing */ }
    public void OnLook(InputAction.CallbackContext context) { /* Do nothing */ }
    public void OnSprint(InputAction.CallbackContext context) { /* Do nothing */ }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.started)
            AdvancePanel();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started)
            AdvancePanel();
    }
    #endregion

    private void ShowPanel()
    {
        if (_currentPanel >= introPanels.Length)
        {
            _inPanelSequence = false;
            cameraView.RequestView();
            anim.SetTrigger("StartFlying");
            StartMusic();
        }
        else
        {
            introPanels[_currentPanel].gameObject.SetActive(true);
            introPanels[_currentPanel].FadeIn(() => _canAdvancePanel = true);
        }
    }

    private void AdvancePanel()
    {
        if (_inPanelSequence && introPanels[_currentPanel].Alpha >= advanceAlphaThreshold && _canAdvancePanel)
        {
            _canAdvancePanel = false;
            introPanels[_currentPanel].FadeOut(() =>
            {
                introPanels[_currentPanel].gameObject.SetActive(false);
                ++_currentPanel;
                _canAdvancePanel = true;
                ShowPanel();
            });
        }
    }

    public void FinishFlying()
    {
        if (!musicManager.enabled)
        {
            // This is handled in ShowPanel(),
            // but in Debug mode this method is called directly skipping ShowPanel()
            StartMusic();
        }

        playerController.TakeControlOf(playerPawn);
        PauseManager.Instance.PausingAllowed = true;
        StartCoroutine(FPS_Tutorial());
    }

    private void StartMusic()
    {
        musicManager.enabled = true;
        Song mainTheme = musicManager.PlaySongDirectly(themeSong, false);

        mainTheme.AddSongEvent(
            musicCrossfadeTime,
            () => musicManager.CrossfadeInNewSong(mainAmbient, 3f, true)
            );
    }

    private IEnumerator FPS_Tutorial()
    {
        yield return new WaitForSeconds(1f);
        MsgBox toolTip = MsgBox.GetInstance(MsgBox.MsgBoxType.ToolTip);
        toolTip.ShowMessage(GameManager.Instance.UsingGamepadControls()
            ? "Left stick to move"
            : "WASD to move"
            , 3f );

        yield return new WaitForSeconds(4f);
        toolTip.ShowMessage(GameManager.Instance.UsingGamepadControls()
            ? "Right stick to look"
            : "Mouse to look"
            , 3f );
    }
}
