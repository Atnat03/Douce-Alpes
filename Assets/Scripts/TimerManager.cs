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
    
    [Header("Bools")]
    [SerializeField] public bool canButtonG = true;
    [SerializeField] public bool canButtonT = false;
    [SerializeField] public bool canButtonC = false;
    
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

    private void Start()
    {
        tonteButton.interactable = false;
        cleanButton.interactable = false;
    }

    private void UpdateButtons(Button button, bool state, bool finishTimer = true)
    {
        button.interactable = state && finishTimer;
    }

    public void UpdateAllButton()
    {
        grangeButton.interactable = canButtonG;
        tonteButton.interactable = canButtonT;
        cleanButton.interactable = canButtonC;
    }

    private void UpdateCooldownUI(TypeAmelioration type, float remainingTime, bool state = true)
    {
        int displayTime = Mathf.CeilToInt(remainingTime);
        switch (type)
        {
            case TypeAmelioration.Rentree:
                grangeMiniGameText.text = displayTime.ToString();
                UpdateButtons(grangeButton, canButtonG, (remainingTime <= 0));
                break;
            case TypeAmelioration.Sortie:
                grangeMiniGameText.text = displayTime.ToString();
                UpdateButtons(grangeButton, canButtonG, (remainingTime <= 0));
                break;
            case TypeAmelioration.Tonte:
                tonteMiniGameText.text = displayTime.ToString();
                UpdateButtons(tonteButton, canButtonT, (remainingTime <= 0));
                break;
            case TypeAmelioration.Nettoyage:
                cleanMiniGameText.text = displayTime.ToString();
                UpdateButtons(cleanButton, canButtonC, (remainingTime <= 0));
                break;
        }
    }

    private void OnTimerFinished(TypeAmelioration type)
    {
        switch (type)
        {
            case TypeAmelioration.Rentree:
                grangeMiniGameText.text = "Grange";
                UpdateButtons(grangeButton, canButtonG);
                break;
            case TypeAmelioration.Sortie:
                grangeMiniGameText.text = "Grange";
                UpdateButtons(grangeButton, canButtonG);
                break;
            case TypeAmelioration.Tonte:
                tonteMiniGameText.text = "Tonte";
                UpdateButtons(grangeButton, canButtonT);
                break;
            case TypeAmelioration.Nettoyage:
                cleanMiniGameText.text = "Nettoyage";
                UpdateButtons(grangeButton, canButtonC);
                break;
        }
    }
}
