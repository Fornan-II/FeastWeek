using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class VehiclePawn : Pawn
{
    public Pawn OwnerPawn => _ownerPawn;

    protected Pawn _ownerPawn;

    public void TakeControlFrom(Pawn owner)
    {
        if (!owner) return;
        _ownerPawn = owner;
        owner.MyController.TakeControlOf(this);
    }

    public void ReturnControl() => MyController.TakeControlOf(_ownerPawn);
}
