public class HostUnit : BaseUnit
{
    public override void Death()
    {
        switch (unitSide) 
        {
            case Side.Player:
                UIManager.instance.GameOver();
                break;
            case Side.Enemy:
                GameManager.instance.NextLevel();
                break;
        }
    }
}
