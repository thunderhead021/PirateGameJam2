using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Level", menuName = "Scriptable Level")]
public class ScriptableLevel : ScriptableObject
{
    public int percentageOfBlockingTileOutOf100 = 10;
    public int maxNumberOfSpecialTiles = 5;
    public int numberOfEnemies = 3;
    public int numberOfPlayerUnits = 3;
    public List<BaseUnit> playerUnitsPool = new();
    public List<BaseUnit> enemyUnitsPool = new();
}
