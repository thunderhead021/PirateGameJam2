using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New TileRule", menuName = "Tile rule")]
public class TileRule : ScriptableObject
{
  public List<Sprite> TileSprite;
  public bool isForMovingTile = false;

  public TileRuleState IsTopLeftSameTag;
  public TileRuleState IsTopCenterSameTag;
  public TileRuleState IsTopRightSameTag;
  public TileRuleState IsMiddleLeftSameTag;
  public TileRuleState IsMiddleRightSameTag;
  public TileRuleState IsBottomLeftSameTag;
  public TileRuleState IsBottomCenterSameTag;
  public TileRuleState IsBottomRightSameTag;

  public Sprite GetSprite()
  {
    return TileSprite[Random.Range(0, TileSprite.Count)];
  }
}

public enum TileRuleState 
{
  DontMatter,
  Same,
  Diffrent,
}
