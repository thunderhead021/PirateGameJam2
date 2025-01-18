using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseUnit : MonoBehaviour
{
    public Tile curTile;
    public bool isCurControl = false;
    public int moveRange;
    public int attackRange;
    public MoveType moveType;
    public MoveType attackType;
}
