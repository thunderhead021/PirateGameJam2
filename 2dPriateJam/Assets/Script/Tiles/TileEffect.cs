using UnityEngine;

public abstract class TileEffect : MonoBehaviour
{
    public bool effectApplied = false;

    public virtual void ApplyEffect(BaseUnit unit)
    {
        
    }

    public virtual void RemoveEffect(BaseUnit unit)
    {

    }
}
