using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MsgBox : MonoBehaviour
{
#pragma warning disable 0649
    public enum MsgBoxType
    {
        ToolTip,
        GhostMsg
    }

    private static Dictionary<MsgBoxType, MsgBox> _instances = new Dictionary<MsgBoxType, MsgBox>();

    public bool IsShowingMessage { get; private set; }

    [SerializeField] private MsgBoxType myType;
    [SerializeField] private FadeUI fader = null;
    [SerializeField] private TextMeshProUGUI text;
    
    private float _timeLeftUntilFadeOut = -1f;

    public static MsgBox GetInstance(MsgBoxType type)
    {
        if(_instances.TryGetValue(type, out MsgBox instance))
        {
            return instance;
        }
        return null;
    }

    public void ShowMessage(string message, float displayTime)
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

    public void HideMessage()
    {
        fader.FadeOut(() => IsShowingMessage = false);
    }

    private void Awake()
    {
        if (GetInstance(myType))
            Destroy(gameObject);
        else
            _instances[myType] = this;
        
        if (!IsShowingMessage) text.text = "";
    }

    private void OnDestroy()
    {
        if (GetInstance(myType) == this)
            _instances[myType] = null;
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
