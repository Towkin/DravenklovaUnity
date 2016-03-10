using UnityEngine;
using System.Collections;
using System;

public class NPC : Pawn
{
    protected Vector3 m_TargetDirection = Vector3.zero;
    public Vector3 TargetDirection
    {
        get { return m_TargetDirection; }
        set { m_TargetDirection = value; }
    }
    protected float m_TargetDistance = 0f;
    public float TargetDistance
    {
        get { return m_TargetDistance; }
        set { m_TargetDistance = value; }
    }

    protected override void UpdateInput()
    {
        InputMoveDirection = new Vector2(TargetDirection.x, TargetDirection.z) * TargetDistance * 0.25f;
    }
}
