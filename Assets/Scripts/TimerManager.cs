using System;
using UnityEngine;
using UnityEngine.UI;

public class TimerManager : MonoBehaviour
{
    [Header("UI references")] 
    
    [Header("Buttons")] 
    [SerializeField] public Button grangeButton;
    
    [Header("Text")]
    [SerializeField] public Text grangeMiniGameText;
    [SerializeField] public Text tonteMiniGameText;
    [SerializeField] public Text cleanMiniGameText;
    
    private void OnEnable()
    {
        GameData.instance.OnCooldownUpdated += UpdateCooldownUI;
        GameData.instance.OnCooldownFinish += OnTimerFinished;
    }

    private void OnDisable()
    {
        if (GameData.instance != null)
        {
            GameData.instance.OnCooldownUpdated -= UpdateCooldownUI;
            GameData.instance.OnCooldownFinish -= OnTimerFinished;
        }
    }

    private void UpdateCooldownUI(TypeAmelioration type, float remainingTime)
    {
        int displayTime = Mathf.CeilToInt(remainingTime);
        switch (type)
        {
            case TypeAmelioration.Rentree:
                grangeMiniGameText.text = displayTime.ToString();
                grangeButton.interactable = remainingTime <= 0;
                break;
            case TypeAmelioration.Tonte:
                tonteMiniGameText.text = displayTime.ToString();
                break;
            case TypeAmelioration.Nettoyage:
                cleanMiniGameText.text = displayTime.ToString();
                break;
            default:
                break;
        }
    }

    private void OnTimerFinished(TypeAmelioration type)
    {
        switch (type)
        {
            case TypeAmelioration.Rentree:
                grangeMiniGameText.text = "Grange";
                grangeButton.interactable = true;
                break;
            case TypeAmelioration.Tonte:
                tonteMiniGameText.text = "Tonte";
                break;
            case TypeAmelioration.Nettoyage:
                cleanMiniGameText.text = "Nettoyage";
                break;
        }
    }
}
