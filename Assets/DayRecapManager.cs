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
    [SerializeField] private Text nextDayButtonTxt;

    private void OnEnable()
    {
        DesactivatePannel();
    }

    public void Recap(int numberDay, float happiness, int money, int wool, int numberNextDay)
    {
        dayRecapPannel.SetActive(true);
        
        happinessText.text = happiness +" %";
        moneyText.text = " + "+money;
        woolText.text = " + " + wool;
        this.numberDay.text = "jour " + numberDay;
        nextDayButtonTxt.text = "Passer au jour " + numberNextDay;
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
