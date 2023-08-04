using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostDialogueManager : MonoBehaviour
{
    #region Singleton
    public static GhostDialogueManager Instance { get; private set; }

    private void Awake()
    {
        if(Instance)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void OnDestroy()
    {
        if(Instance == this)
        {
            Instance = null;
        }
    }
    #endregion

#pragma warning disable 0649
    [SerializeField] private TextAsset rawDialogueOptions;
    [SerializeField] private float messageDuration = 3f;

    private string[] _dialogueOptions;
    private int _currentDialogueIndex;

    private MsgBox _ghostMsgInstance;

    private void Start()
    {
        // Looking for a .csv file
        // Probably gotta make this a bit fancier, using a struct with a , split if we use columns
        _dialogueOptions = rawDialogueOptions.text.Split('\n');
        for(int i = 0; i < _dialogueOptions.Length; ++i)
        {
            _dialogueOptions[i] = _dialogueOptions[i].Trim('\r', '\"');
            //// Special case where message is encapsulated in "" to denote usage of commas
            //if (_dialogueOptions[i][0] == '\"')
            //{
            //    _dialogueOptions[i] = _dialogueOptions[i].Substring(1, _dialogueOptions[i].Length - 2);
            //    // Will have to refactor substring length here to use position of last ", if I ever use columns
            //}
        }
        RefreshDialogue();

        _ghostMsgInstance = MsgBox.GetInstance(MsgBox.MsgBoxType.GhostMsg);
    }

    public void RefreshDialogue()
    {
        Util.ShuffleCollection(_dialogueOptions);
        _currentDialogueIndex = 0;
    }

    public void ShowDialogue()
    {
        if(_currentDialogueIndex >= _dialogueOptions.Length)
        {
            RefreshDialogue();
        }

        _ghostMsgInstance.ShowMessage(_dialogueOptions[_currentDialogueIndex], messageDuration);

        ++_currentDialogueIndex;
    }
}
