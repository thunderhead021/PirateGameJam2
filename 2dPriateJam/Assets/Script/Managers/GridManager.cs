using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField]
    private int width, height;  
    [SerializeField]
    private Tile normalTilePrefab;
    [SerializeField]
    private Tile blockTilePrefab;
    [SerializeField]
    private Transform cam;

    public Dictionary<Vector2, Tile> Tiles;
    public static GridManager instance;

    private void Awake()
    {
        if(instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public void GenerateGrid() 
    {
        Tiles = new();

        //hexagon flat top
        //float hexWidth = 1.0f; 
        //float hexHeight = Mathf.Sqrt(3) / 2.0f * hexWidth; 

        //for (int x = 0; x < width; x++)
        //{
        //    for (int y = 0; y < height; y++)
        //    {
        //        float xPos = x * hexWidth * 0.75f; 
        //        float yPos = y * hexHeight;

        //        if (x % 2 != 0)
        //        {
        //            yPos += hexHeight / 2.0f;
        //        }

        //        var newTile = Instantiate(normalTilePrefab, new Vector3(xPos, yPos, 0), Quaternion.identity);
        //        newTile.name = $"Tile {x} {y}";
        //        newTile.Init(x, y);
        //    }
        //}



        //Iso
        //for (int x = 0; x < width; x++)
        //{
        //    for (int y = 0; y < height; y++)
        //    {
        //        float posX = (x - y) * 0.5f; 
        //        float posY = (x + y) * 0.25f; 

        //        var newTile = Instantiate(normalTilePrefab, new Vector3(posX, posY, 0), Quaternion.identity);
        //        newTile.name = $"Tile {x} {y}";
        //        newTile.Init(x, y);
        //    }
        //}


        //Normal
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                bool shouldBeBlockTile = Random.Range(0, 100) > 30;
                var newTile = Instantiate(shouldBeBlockTile ? normalTilePrefab : blockTilePrefab, new Vector3(x, y), Quaternion.identity);
                newTile.name = $"Tile {x} {y}";
                newTile.Init(x,y);
                Tiles[new Vector2 (x,y)] = newTile;
            }
        }

        cam.transform.position = new((float)width/2 - 0.5f, (float)height/2 - 0.5f, -10);
        GameManager.instance.ChangeState(GameState.SpawnPlayerUnit);
    }

    public Tile GetTileAtPos(Vector2 pos) 
    {
        if(Tiles.TryGetValue(pos, out Tile tile)) return tile;
        return null;
    }

    public Tile GetStartTile( bool forPlayer ) 
    {
        return Tiles.Where( t => (forPlayer ? t.Key.x < width / 2 : t.Key.x > width / 2) && t.Value.WalkAble()).OrderBy(t=>Random.value).First().Value;
    }

    public List<Tile> GetAllMoveableTiles(Tile tile, int range, MoveType moveType) 
    {
        List<Tile> result = new();

        switch (moveType) 
        {
            case MoveType.Square:
                for (int x = -range; x <= range; x++)
                {
                    for (int y = -range; y <= range; y++)
                    {
                        Tile neighborTile = GetTileAtPos(new Vector2(tile.pos.x + x, tile.pos.y + y));
                        if (neighborTile != null && neighborTile.WalkAble())
                        {
                            result.Add(neighborTile);
                        }
                    }
                }
                break;
            case MoveType.Diamond:
                for (int x = -range; x <= range; x++)
                {
                    for (int y = -range; y <= range; y++)
                    {
                        if (Mathf.Abs(x) + Mathf.Abs(y) <= range)
                        {
                            Tile neighborTile = GetTileAtPos(new Vector2(tile.pos.x + x, tile.pos.y + y));
                            if (neighborTile != null && neighborTile.WalkAble())
                            {
                                result.Add(neighborTile);
                            }
                        }
                    }
                }
                break;
        }

        return result;  
    }

    public void SetTilesMoveable(Tile tile, int range, MoveType moveType, bool isSelecting) 
    {
        switch (moveType) 
        {
            case MoveType.Square:
                for (int x = -range; x <= range; x++) 
                {
                    for (int y = -range; y <= range; y++) 
                    {
                        Tile neighborTile = GetTileAtPos(new Vector2(tile.pos.x + x, tile.pos.y + y));
                        if (neighborTile != null && neighborTile.WalkAble()) 
                        {
                            neighborTile.SetSelectable(isSelecting);
                        }
                    }
                }
                break;
            case MoveType.Diamond:
                for (int x = -range; x <= range; x++)
                {
                    for (int y = -range; y <= range; y++)
                    {
                        if (Mathf.Abs(x) + Mathf.Abs(y) <= range)
                        {
                            Tile neighborTile = GetTileAtPos(new Vector2(tile.pos.x + x, tile.pos.y + y));
                            if (neighborTile != null && neighborTile.WalkAble())
                            {
                                neighborTile.SetSelectable(isSelecting);
                            }
                        }
                    }
                }
                break;
        }
    }

    public void SetTilesAttackable(Tile tile, int range, MoveType attackType, bool isSelecting)
    {
        bool haveAttack = false;
        switch (attackType)
        {
            case MoveType.Square:   
                for (int x = -range; x <= range; x++)
                {
                    for (int y = -range; y <= range; y++)
                    {
                        Tile neighborTile = GetTileAtPos(new Vector2(tile.pos.x + x, tile.pos.y + y));
                        if (neighborTile != null && neighborTile.AttackAble())
                        {
                            haveAttack = true;
                            neighborTile.SetSelectable(isSelecting, true);
                        }
                    }
                }
                break;
            case MoveType.Diamond:
                for (int x = -range; x <= range; x++)
                {
                    for (int y = -range; y <= range; y++)
                    {
                        if (Mathf.Abs(x) + Mathf.Abs(y) <= range)
                        {
                            Tile neighborTile = GetTileAtPos(new Vector2(tile.pos.x + x, tile.pos.y + y));
                            if (neighborTile != null && neighborTile.AttackAble())
                            {
                                haveAttack = true;
                                neighborTile.SetSelectable(isSelecting, true);
                            }
                        }
                    }
                }
                break;
        }
        if (!haveAttack)
            UIManager.instance.turnsDisplay.ChangeTurn();
    }
}

public enum MoveType 
{
    Square,
    Diamond
}