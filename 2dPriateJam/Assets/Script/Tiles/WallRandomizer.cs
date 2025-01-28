using System.Collections.Generic;
using UnityEngine;

public class WallRandomizer : MonoBehaviour
{
    public List<Sprite> sprites;
    public SpriteRenderer spriteRenderer;
    // Start is called before the first frame update
    void Start()
    {
        int rand = Random.Range(0, 100);
        int index = rand <= 90 ? 0 : rand <= 95 ? 1 : rand <= 99 ? 2 : 3;
        spriteRenderer.sprite = sprites[index];
    }
}
