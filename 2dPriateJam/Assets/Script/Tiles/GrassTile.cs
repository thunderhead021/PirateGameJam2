using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassTile : Tile
{
    [SerializeField]
    private Sprite baseSprite, offsetSprite;

    public override void Init(int x, int y, bool haveEffect = false)
    {
        base.Init(x, y, haveEffect);
        var isOffset = (x % 2 == 0 && y % 2 != 0) || (x % 2 != 0 && y % 2 == 0);
        spriteRenderer.sprite = isOffset ? baseSprite : offsetSprite;
    }
}
