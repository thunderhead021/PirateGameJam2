using System.Collections.Generic;
using System.Data;
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
    [SerializeField]
    private GameObject Wall1;
    [SerializeField]
    private GameObject Wall1Left;
    [SerializeField]
    private GameObject Wall1Right;
    [SerializeField]
    private GameObject Wall2;
    public Dictionary<Vector2, Tile> Tiles;
    private List<TileRule> blockTileRules;
    private List<TileRule> normalTileRules;
    public static GridManager instance;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            normalTileRules = Resources.LoadAll<TileRule>("Floor Tiles").ToList();
            blockTileRules = Resources.LoadAll<TileRule>("Block Tiles").ToList();
        }
        else
            Destroy(gameObject);
    }

    public void ResetLevel() 
    {
        while (transform.childCount > 0)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
    }

    public void GenerateGrid(int blockPercentage, int numberOfSpecialTiles) 
    {
        Tiles = new();
        #region commented
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

        #endregion
        
        //Normal
        for (int x = 0; x < width + 2; x++)
        {
            for (int y = 0; y < height + 2; y++)
            {
                if (y == 0 || (y == height && (x >= 1 && x <= width)))
                {
                    var wall2 = Instantiate(Wall2, new Vector3(x, y), Quaternion.identity, transform);
                    wall2.name = $"Tile {x} {y}"; 
                }
                else if (x == width + 1 && (y >= 1 && y <= height))
                {
                    var wall1right = Instantiate(Wall1Right, new Vector3(x, y), Wall1Right.transform.rotation, transform);
                    wall1right.name = $"Tile {x} {y}";
                }
                else if (x == 0 && (y >= 1 && y <= height))
                {
                    var wall1left = Instantiate(Wall1Left, new Vector3(x, y), Wall1Left.transform.rotation, transform);
                    wall1left.name = $"Tile {x} {y}";
                }
                else if ((y == 1 && (x >= 1 && x <= width)) || (y == height + 1 && (x >= 0 && x <= width + 2))) 
                {
                    var wall1 = Instantiate(Wall1, new Vector3(x, y), Quaternion.identity, transform);
                    wall1.name = $"Tile {x} {y}";
                }
                else 
                {
                    bool shouldBeBlockTile = Random.Range(0, 100) > (100 - blockPercentage);
                    var newTile = Instantiate(shouldBeBlockTile ? blockTilePrefab : normalTilePrefab, new Vector3(x, y), Quaternion.identity, transform);
                    newTile.name = $"Tile {x} {y}";

                    if (!shouldBeBlockTile && numberOfSpecialTiles > 0 && Random.Range(0, 100) > 70)
                    {
                        newTile.Init(x, y, true);
                        numberOfSpecialTiles--;
                    }
                    else
                        newTile.Init(x, y);

                    Tiles[new Vector2(x, y)] = newTile;
                } 
            }
        }

        foreach (Tile tile in Tiles.Values)
        {
            UpdateTile(tile);
        }
        cam.transform.position = new((float)(width + 2) /2 - 0.5f, (float)(height + 2) /2 - 0.5f, -10);
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

    public Tile GetRandomTile()
    {
        return Tiles.Where(t => t.Value.WalkAble()).OrderBy(t => Random.value).First().Value;
    }

    public List<Tile> GetAllMoveableTiles(Tile tile, int range, MoveType moveType) 
    {
        List<Tile> result = new();
        List<int> blockXs = new();

        switch (moveType) 
        {
            case MoveType.Square:
                for (int x = -range; x <= range; x++)
                {
                    for (int y = -range; y <= range; y++)
                    {
                        Tile neighborTile = GetTileAtPos(new Vector2(tile.pos.x + x, tile.pos.y + y));
                        if (neighborTile != null )
                        {
                            if (neighborTile.WalkAble() && !blockXs.Contains(x))
                                result.Add(neighborTile);
                            else if (neighborTile.curUnit != null && neighborTile.curUnit is BlockerUnit && neighborTile.curUnit.unitSide != tile.curUnit.unitSide)
                                blockXs.Add(x);
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
                            if (neighborTile != null)
                            {
                                if(neighborTile.WalkAble() && !blockXs.Contains(x))
                                    result.Add(neighborTile);
                                else if (neighborTile.curUnit != null && neighborTile.curUnit is BlockerUnit && neighborTile.curUnit.unitSide != tile.curUnit.unitSide)
                                    blockXs.Add(x);
                            }
                        }
                    }
                }
                break;
        }

        return result;  
    }

    public void SetTilesMoveable(Tile tile, int range, MoveType moveType, bool isSelecting, bool forDisplay = false) 
    {
        List<int> blockXs = new();

        switch (moveType) 
        {
            case MoveType.Square:
                for (int x = -range; x <= range; x++) 
                {
                    for (int y = -range; y <= range; y++) 
                    {
                        Tile neighborTile = GetTileAtPos(new Vector2(tile.pos.x + x, tile.pos.y + y));
                        if (neighborTile != null) 
                        {
                            if (neighborTile.WalkAble() && !blockXs.Contains(x))
                                neighborTile.SetSelectable(isSelecting, forDisplay: forDisplay);
                            else if (neighborTile.curUnit != null && neighborTile.curUnit is BlockerUnit && neighborTile.curUnit.unitSide != tile.curUnit.unitSide)
                                blockXs.Add(x);
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
                            if (neighborTile != null)
                            {
                                if (neighborTile.WalkAble() && !blockXs.Contains(x))
                                    neighborTile.SetSelectable(isSelecting);
                                else if (neighborTile.curUnit != null && neighborTile.curUnit is BlockerUnit && neighborTile.curUnit.unitSide != tile.curUnit.unitSide)
                                    blockXs.Add(x);
                            }
                        }
                    }
                }
                break;
        }
    }

    public void SetTilesAttackable(Tile tile, int range, AttackType attackType, bool isSelecting, bool callFromCode = true)
    {
        bool haveAttack = false;
        List<int> blockXs = new();

        switch (attackType)
        {
            case AttackType.Square:   
                for (int x = -range; x <= range; x++)
                {
                    for (int y = -range; y <= range; y++)
                    {
                        Tile neighborTile = GetTileAtPos(new Vector2(tile.pos.x + x, tile.pos.y + y));
                        if (neighborTile != null)
                        {
                            if (neighborTile.AttackAble() && !blockXs.Contains(x))
                            {
                                haveAttack = true;
                                neighborTile.SetSelectable(isSelecting, true);
                            }
                            else if (neighborTile.curUnit != null && neighborTile.curUnit is BlockerUnit && neighborTile.curUnit.unitSide != tile.curUnit.unitSide)
                                blockXs.Add(x);
                        }
                    }
                }
                break;
            case AttackType.Diamond:
                for (int x = -range; x <= range; x++)
                {
                    for (int y = -range; y <= range; y++)
                    {
                        if (Mathf.Abs(x) + Mathf.Abs(y) <= range)
                        {
                            Tile neighborTile = GetTileAtPos(new Vector2(tile.pos.x + x, tile.pos.y + y));
                            if (neighborTile != null)
                            {
                                if (neighborTile.AttackAble() && !blockXs.Contains(x))
                                {
                                    haveAttack = true;
                                    neighborTile.SetSelectable(isSelecting, true);
                                }
                                else if (neighborTile.curUnit != null && neighborTile.curUnit is BlockerUnit && neighborTile.curUnit.unitSide != tile.curUnit.unitSide)
                                    blockXs.Add(x);
                            }
                        }
                    }
                }
                break;
        }
        if (!haveAttack && isSelecting && callFromCode)
            tile.curUnit.Move();
    }

    public void ShowUnitAttackRange() 
    {
        SetTilesAttackable(UnitManager.instance.SelectedUnit.curTile, UnitManager.instance.SelectedUnit.attackRange, UnitManager.instance.SelectedUnit.attackType, true, false);
    }

    public List<Tile> GetAllAttackableTiles(Tile tile, int range, AttackType moveType)
    {
        List<Tile> result = new();
        List<int> blockXs = new();
        switch (moveType)
        {
            case AttackType.Square:
                for (int x = -range; x <= range; x++)
                {
                    for (int y = -range; y <= range; y++)
                    {
                        Tile neighborTile = GetTileAtPos(new Vector2(tile.pos.x + x, tile.pos.y + y));
                        if (neighborTile != null)
                        {
                            if (neighborTile.AttackAble() && !neighborTile.curUnit.CompareTag("Enemy") && !blockXs.Contains(x) )
                            {
                                result.Add(neighborTile);
                            }
                            else if (neighborTile.curUnit != null && neighborTile.curUnit is BlockerUnit && neighborTile.curUnit.unitSide != tile.curUnit.unitSide)
                                blockXs.Add(x);
                            
                        }
                    }
                }
                break;
            case AttackType.Diamond:
                for (int x = -range; x <= range; x++)
                {
                    for (int y = -range; y <= range; y++)
                    {
                        if (Mathf.Abs(x) + Mathf.Abs(y) <= range)
                        {
                            Tile neighborTile = GetTileAtPos(new Vector2(tile.pos.x + x, tile.pos.y + y));
                            if (neighborTile != null)
                            {
                                if (neighborTile.AttackAble() && !neighborTile.curUnit.CompareTag("Enemy") && !blockXs.Contains(x))
                                {
                                    result.Add(neighborTile);
                                }
                                else if (neighborTile.curUnit != null && neighborTile.curUnit is BlockerUnit && neighborTile.curUnit.unitSide != tile.curUnit.unitSide)
                                    blockXs.Add(x);

                            }
                        }
                    }
                }
                break;
        }

        return result;
    }

    private bool RuleMatch(Tile tile, TileRule tileRule) 
    {
        Vector2Int[] neighborOffsets = new Vector2Int[]
        {
                new(0, 1),   // Up
                new(1, 1),   // Up Right
                new(1, 0),   // Right
                new(1, -1),  // Bottom Right
                new(0, -1),  // Bottom 
                new(-1, -1), // Bottom Left
                new(-1, 0),  // Left
                new(-1, 1)   // Up Left
        };

        TileRuleState[] tileRules = new TileRuleState[]
        {
                tileRule.IsTopCenterSameTag   ,   // Up
                tileRule.IsTopRightSameTag    ,   // Up Right
                tileRule.IsMiddleRightSameTag ,   // Right
                tileRule.IsBottomRightSameTag ,   // Bottom Right
                tileRule.IsBottomCenterSameTag,   // Bottom 
                tileRule.IsBottomLeftSameTag  ,   // Bottom Left
                tileRule.IsMiddleLeftSameTag  ,   // Left
                tileRule.IsTopLeftSameTag         // Up Left
        };

        for(int i = 0; i < neighborOffsets.Length; i++) 
        {
            Tile neighborTile = GetTileAtPos(new Vector2(tile.pos.x + neighborOffsets[i].x, tile.pos.y + neighborOffsets[i].y));
            if( tileRule.isForMovingTile != tile.isWalkable)
                return false;
            else if( tileRules[i] == TileRuleState.Same && neighborTile != null && neighborTile.isWalkable != tile.isWalkable )
                return false;
            else if( tileRules[i] == TileRuleState.Diffrent && neighborTile != null && neighborTile.isWalkable == tile.isWalkable )
                return false;
        }
        return true;
    }

    private void UpdateTile(Tile tile) 
    {
        if (tile.isWalkable) 
        {
            int rand = Random.Range(0, 100);
            int index = rand <= 50 ? 0 : rand <= 75 ? 1 : rand <= 95 ? 2 : 3;
            tile.UpdateTile(normalTileRules[index].TileSprite);
        }
        else 
        {
            if(blockTileRules.Count > 0) 
            {
                foreach (TileRule rule in blockTileRules)
                {
                    if (RuleMatch(tile, rule))
                    {
                        tile.UpdateTile(rule.TileSprite);
                        break;
                    }
                }
            }
        } 
    }
}

public enum MoveType 
{
    Square,
    Diamond
}

public enum AttackType
{
    Square,
    Diamond
}