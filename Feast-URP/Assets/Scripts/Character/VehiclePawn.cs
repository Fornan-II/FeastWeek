using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class VehiclePawn : Pawn
{
    protected Pawn _ownerPawn;

    public void TakeControlFrom(Pawn owner)
    {
        if (!owner) return;
        _ownerPawn = owner;
        owner.MyController.TakeControlOf(this);
    }

    public void ReturnControl() => MyController.TakeControlOf(_ownerPawn);
}
