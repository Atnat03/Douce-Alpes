using System;
using UnityEngine;
using UnityEngine.UI;

public class TimerManager : MonoBehaviour
{
    [Header("UI references")] 
    
    [Header("Buttons")] 
    [SerializeField] public Button grangeButton;
    [SerializeField] public Button tonteButton;
    [SerializeField] public Button cleanButton;
    
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

    private void UpdateCooldownUI(TypeAmelioration type, float remainingTime, bool state = true)
    {
        int displayTime = Mathf.CeilToInt(remainingTime);
        switch (type)
        {
            case TypeAmelioration.Rentree:
                grangeMiniGameText.text = displayTime.ToString();
                grangeButton.interactable = remainingTime <= 0;
                break;
            case TypeAmelioration.Sortie:
                grangeMiniGameText.text = displayTime.ToString();
                grangeButton.interactable = remainingTime <= 0;
                break;
            case TypeAmelioration.Tonte:
                tonteMiniGameText.text = displayTime.ToString();
                tonteButton.interactable = remainingTime <= 0 && state;
                break;
            case TypeAmelioration.Nettoyage:
                cleanMiniGameText.text = displayTime.ToString();
                cleanButton.interactable = remainingTime <= 0 && state;
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
            case TypeAmelioration.Sortie:
                grangeMiniGameText.text = "Grange";
                grangeButton.interactable = true;
                break;
            case TypeAmelioration.Tonte:
                tonteMiniGameText.text = "Tonte";
                tonteButton.interactable = true;
                break;
            case TypeAmelioration.Nettoyage:
                cleanMiniGameText.text = "Nettoyage";
                cleanButton.interactable = true;
                break;
        }
    }
}
