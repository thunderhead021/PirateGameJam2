public class MovementTileEffect : TileEffect
{
    public int extraMovementRange = 1;
    public int damage = 1;

    public override void ApplyEffect(BaseUnit unit)
    {
        if (!effectApplied)
        {
            unit.moveRange += extraMovementRange;
            effectApplied = true;
        }
        else
            unit.DealDamage(unit, damage);
    }

    public override void RemoveEffect(BaseUnit unit)
    {
        unit.moveRange -= extraMovementRange;
        effectApplied = false;
    }
}
