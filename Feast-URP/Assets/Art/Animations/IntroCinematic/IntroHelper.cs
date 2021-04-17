using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class IntroHelper : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField] private FadeUI[] introPanels;
    [SerializeField] private ViewRequester cameraView = null;
    [SerializeField] private Animator anim;
    [SerializeField] private Pawn playerPawn;
    [SerializeField] private Controller playerController;
    [SerializeField] private MusicManager musicManager;

    private int _currentPanel = 0;
    private bool _readyForNextPanel = false;
    private bool _inPanelSequence = true;

    private void Start()
    {
        _currentPanel = 0;
        ShowPanel();
    }

    #region Input
    private void OnInteract(InputValue value) => AdvancePanel();
    private void OnJump(InputValue value) => AdvancePanel();
    private void OnSprint(InputValue value) => AdvancePanel();
    #endregion

    private void ShowPanel()
    {
        if (_currentPanel >= introPanels.Length)
        {
            _inPanelSequence = false;
            cameraView.RequestView();
            anim.SetTrigger("StartFlying");
            musicManager.enabled = true;
            musicManager.PlayImmediately(1);
        }
        else
        {
            introPanels[_currentPanel].gameObject.SetActive(true);
            introPanels[_currentPanel].FadeIn(() =>
            {
                _readyForNextPanel = true;
            });
        }
    }

    private void AdvancePanel()
    {
        if (_readyForNextPanel && _inPanelSequence)
        {
            _readyForNextPanel = false;
            introPanels[_currentPanel].FadeOut(() =>
            {
                introPanels[_currentPanel].gameObject.SetActive(false);
                ++_currentPanel;
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
            musicManager.enabled = true;
            musicManager.PlayImmediately(1);
        }

        playerController.TakeControlOf(playerPawn);
        PauseManager.Instance.PausingAllowed = true;
        StartCoroutine(FPS_Tutorial());
    }

    private IEnumerator FPS_Tutorial()
    {
        yield return new WaitForSeconds(1f);
        MsgBox.ShowMessage("WASD to move", 3f);
        yield return new WaitForSeconds(4f);
        MsgBox.ShowMessage("Mouse to look", 3f);
    }
}
