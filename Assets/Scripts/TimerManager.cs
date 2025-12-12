using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public enum MiniGames
{
    Rentree,
    Tonte,
    Nettoyage,
    Sortie
}

public class TimerManager : MonoBehaviour
{
    public MiniGames currentMiniJeuToDo;
    
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
    
    [Header("Horloge")]
    [SerializeField] public RectTransform horloge;
    [SerializeField] public Image logo1;
    [SerializeField] public Image logo2;
    [SerializeField] public Sprite[] spriteMiniJeux;
    private bool isLogo1Visible = true;
    
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
        
        logo1.sprite = spriteMiniJeux[0];
        logo2.sprite = spriteMiniJeux[1];
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
        
        NextMiniGameToDo();
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
                UpdateButtons(tonteButton, canButtonT);
                break;
            case TypeAmelioration.Nettoyage:
                cleanMiniGameText.text = "Nettoyage";
                UpdateButtons(cleanButton, canButtonC);
                break;
        }
    }

    IEnumerator UpdateHorloge(int nextIndex)
    {
        float duration = 2f;
        float elapsed = 0f;

        Image invisibleLogo = isLogo1Visible ? logo2 : logo1;
        invisibleLogo.sprite = spriteMiniJeux[nextIndex];

        // 2. rotation smooth
        Quaternion startRot = horloge.rotation;
        Quaternion endRot = startRot * Quaternion.Euler(0, 0, 180f);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);
            horloge.rotation = Quaternion.Lerp(startRot, endRot, t);
            yield return null;
        }

        horloge.rotation = endRot;

        isLogo1Visible = !isLogo1Visible;
    }
    
    public void NextMiniGameToDo()
    {
        MiniGames[] values = (MiniGames[])Enum.GetValues(typeof(MiniGames));

        int index = Array.IndexOf(values, currentMiniJeuToDo);
        int nextIndex = (index + 1) % values.Length;

        currentMiniJeuToDo = values[nextIndex];

        StartCoroutine(UpdateHorloge(nextIndex));
    }
}
