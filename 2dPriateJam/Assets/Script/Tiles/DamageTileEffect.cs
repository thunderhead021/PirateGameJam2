public class DamageTileEffect : TileEffect
{
    public int extraDamage = 1;
    
    public override void ApplyEffect(BaseUnit unit)
    {
        if (!effectApplied) 
        {
            effectApplied = true;
            unit.AttackPower += extraDamage;
            UnitManager.instance.SpawnUnit(unit.isCurControl);
        }    
    }
}
