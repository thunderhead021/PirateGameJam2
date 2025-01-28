using UnityEngine;
using UnityEngine.UI;

public class AttackButton : MonoBehaviour
{
    public Button Button;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (UnitManager.instance.SelectedUnit != null) 
        {
            var unit = UnitManager.instance.SelectedUnit;
            if (GridManager.instance.GetAllAttackableTiles(unit.curTile, unit.attackRange, unit.attackType).Count > 0)
                Button.interactable = true;
            else
                Button.interactable = false;
        }
        else
            Button.interactable = false;
    }
}
