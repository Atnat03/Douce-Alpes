using System;
using UnityEngine;
using UnityEngine.UI;

public class DayRecapManager : MonoBehaviour
{
    [SerializeField] GameObject dayRecapPannel;
    [SerializeField] private Text happinessText;
    [SerializeField] private Text moneyText;
    [SerializeField] private Text woolText;
    [SerializeField] private Text numberDay;

    private void OnEnable()
    {
        DesactivatePannel();
    }

    public void Recap(int numberDay, float happiness, int money, int wool)
    {
        dayRecapPannel.SetActive(true);
        
        happinessText.text = happiness +" %";
        moneyText.text = "+"+money;
        woolText.text = "+" + wool;
        this.numberDay.text = "jour " + numberDay;
    }

    public void DesactivatePannel()
    {
        dayRecapPannel.SetActive(false);
    }
    
    public void NextDay()
    {
        DesactivatePannel();
        GameData.instance.numberDay++;
        GameData.instance.ResetDayStats();
    }
}
