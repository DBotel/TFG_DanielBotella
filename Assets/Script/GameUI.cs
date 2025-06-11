using TMPro;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    [Header("Referencias")]
    public TownHall townHall;               
    public TMP_Text woodText;               
    public TMP_Text stoneText;              
    public TMP_Text moneyText;
    public TMP_Text foodText;

    [ContextMenu("RefreshUI")]
    public void RefreshUI()
    {
        woodText.text = $"Madera: {townHall.town_resources.Wood}";
        stoneText.text = $"Piedra: {townHall.town_resources.Stone}";
        moneyText.text = $"Dinero: {townHall.town_resources.Money}";
        foodText.text = $"Comida: {townHall.town_resources.Food}";
    }
}
