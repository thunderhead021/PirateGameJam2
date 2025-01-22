using UnityEngine;

[CreateAssetMenu(fileName = "New TileRule", menuName = "Tile rule")]
public class TileRule : ScriptableObject
{
  public Sprite TileSprite;
  public bool isForMovingTile = false;

  public TileRuleState IsTopLeftSameTag;
  public TileRuleState IsTopCenterSameTag;
  public TileRuleState IsTopRightSameTag;
  public TileRuleState IsMiddleLeftSameTag;
  public TileRuleState IsMiddleRightSameTag;
  public TileRuleState IsBottomLeftSameTag;
  public TileRuleState IsBottomCenterSameTag;
  public TileRuleState IsBottomRightSameTag;
}

public enum TileRuleState 
{
  DontMatter,
  Same,
  Diffrent,
}
