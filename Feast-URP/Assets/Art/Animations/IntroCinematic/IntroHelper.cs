using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class IntroHelper : MonoBehaviour
{
    [SerializeField] private FadeUI[] introPanels;
    [SerializeField] private ViewRequester cameraView;
    [SerializeField] private Animator anim;
    [SerializeField] private Pawn playerPawn;
    [SerializeField] private Controller playerController;

    private int _currentPanel = 0;
    private bool _readyForNextPanel = false;
    private bool _inPanelSequence = true;

    public void StartButton(FadeUI button)
    {
        button.FadeOut(() =>
        {
            button.gameObject.SetActive(false);
            _currentPanel = 0;
            ShowPanel();
        });
    }

    private void ShowPanel()
    {
        if (_currentPanel >= introPanels.Length)
        {
            _inPanelSequence = false;
            cameraView.RequestView();
            anim.SetTrigger("StartFlying");
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

    private void OnInteract(InputValue value)
    {
        if(_readyForNextPanel && _inPanelSequence)
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

    private void FinishFlying()
    {
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
