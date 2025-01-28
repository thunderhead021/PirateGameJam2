using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InfoDisplay : MonoBehaviour
{
    public Image Icon;
    public TextMeshProUGUI AttackNumber;


    public void Setup(Sprite sprite, int attack) 
    {
        Icon.sprite = sprite;
        AttackNumber.text = attack.ToString();
    }
}
