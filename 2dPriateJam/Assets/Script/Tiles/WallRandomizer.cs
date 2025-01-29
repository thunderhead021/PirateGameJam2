using System.Collections.Generic;
using UnityEngine;

public class WallRandomizer : MonoBehaviour
{
    public List<Sprite> sprites;
    public SpriteRenderer spriteRenderer;
    public bool sideWall = false;
    // Start is called before the first frame update
    void Start()
    {
        int index = 0;
        int rand = Random.Range(0, 100);
        if (sideWall)
        {
            index = rand <= 70 ? 0 : rand <= 90 ? 1 : 2;
        }
        else 
        {
            index = rand <= 45 ? 0 : rand <= 90 ? 1 : rand <= 95 ? 2 : 3;
        }
        spriteRenderer.sprite = sprites[index];
    }
}
