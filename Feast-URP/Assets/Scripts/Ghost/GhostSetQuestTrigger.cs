using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostSetQuestTrigger : LimitedTrigger
{
#pragma warning disable 0649
    [SerializeField] private GhostAI targetGhost;
    [SerializeField] private GhostQuest quest;

    protected override void OnTrigger()
    {
        targetGhost.SetQuest(quest);
    }
}
