using UnityEngine;
using UnityEngine.UI;

public class TurnIndicator : MonoBehaviour
{
    public Image Icon;
    public BaseUnit Entity;
    public void Setup(BaseUnit entity)
    {
        Entity = entity;
        Icon.sprite = entity.TurnIcon;
    }
}