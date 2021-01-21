using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MsgBox : MonoBehaviour
{
    private static MsgBox _instance;

    public bool IsShowingMessage { get; private set; }

    [SerializeField] private FadeUI fader;
    [SerializeField] private TextMeshProUGUI text;
    
    private float _timeLeftUntilFadeOut = -1f;

    public static void ShowMessage(string message, float displayTime) => _instance.Internal_ShowMessage(message, displayTime);
    public static void HideMessage() => _instance.Internal_HideMessage();

    private void Internal_ShowMessage(string message, float displayTime)
    {
        if (IsShowingMessage)
        {
            fader.FadeOut(() =>
            {
                text.text = message;
                fader.FadeIn();
                _timeLeftUntilFadeOut = displayTime;
            });
        }
        else
        {
            text.text = message;
            fader.FadeIn();
            _timeLeftUntilFadeOut = displayTime;
        }

        IsShowingMessage = true;
    }

    private void Internal_HideMessage()
    {
        fader.FadeOut(() => IsShowingMessage = false);
    }

    private void Awake()
    {
        if (_instance)
            Destroy(gameObject);
        else
            _instance = this;
        // Already DontDestroyOnLoad because this is attached to PauseManager
        if (!IsShowingMessage) text.text = "";
    }

    private void OnDestroy()
    {
        if (_instance == this)
            _instance = null;
    }

    private void Update()
    {
        if(_timeLeftUntilFadeOut > 0f)
        {
            _timeLeftUntilFadeOut -= Time.deltaTime;
            if(_timeLeftUntilFadeOut < 0f)
            {
                HideMessage();
            }
        }
    }
}
