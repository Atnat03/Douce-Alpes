using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public enum MiniGames { Rentree, Tonte, Nettoyage, Sortie, Abreuvoir }

public class TimerManager : MonoBehaviour
{
    public MiniGames currentMiniJeuToDo;

    [Header("UI references")]
    [Header("Buttons")]
    [SerializeField] public Button grangeButton;
    [SerializeField] public Button tonteButton;
    [SerializeField] public Button cleanButton;

    [Header("Fill Images")]
    [SerializeField] public Image grangeFillImage;
    [SerializeField] public Image tonteFillImage;
    [SerializeField] public Image cleanFillImage;

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
        currentMiniJeuToDo = MiniGames.Rentree;
        canButtonG = true;
        canButtonT = false;
        canButtonC = false;
        tonteButton.interactable = false;
        cleanButton.interactable = false;
        logo1.sprite = spriteMiniJeux[0];
        logo2.sprite = spriteMiniJeux[1];
        UpdateFills();
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
        UpdateFills();
        NextMiniGameToDo();
    }

    private void Update()
    {
        UpdateFills();
    }

    public void UpdateFills()
    {
        UpdateSingleFill(grangeFillImage, canButtonG, TypeAmelioration.Rentree, "Grange");
        UpdateSingleFill(tonteFillImage, canButtonT, TypeAmelioration.Tonte, "Tonte");
        UpdateSingleFill(cleanFillImage, canButtonC, TypeAmelioration.Nettoyage, "Nettoyage");
    }

    private void UpdateSingleFill(Image fillImg, bool canDo, TypeAmelioration type, string debugName)
    {
        if (fillImg == null) return;

        float fillAmount = 0f;

        if (canDo)
        {
            int currentTimer = GameData.instance.GetCurrentTimer(type);
            int maxTime = GameData.instance.GetCooldownUpgrade(type);

            if (currentTimer <= 0)
            {
                fillAmount = 1f;
            }
            else if (maxTime > 0)
            {
                fillAmount = 1f - (currentTimer / (float)maxTime);
            }
            else
            {
                fillAmount = 1f;
            }
        
            fillImg.GetComponent<UnityEngine.UI.Outline>().enabled = (currentTimer <= 0);
        }
        else
        {
            fillImg.GetComponent<UnityEngine.UI.Outline>().enabled = false;
        }

        fillImg.fillAmount = Mathf.Clamp01(fillAmount);
    }
    private void UpdateCooldownUI(TypeAmelioration type, float remainingTime, bool state = true)
    {
        int displayTime = Mathf.CeilToInt(remainingTime);
        float maxTime = GameData.instance.GetCooldownUpgrade(type);
        float fillAmount = (remainingTime <= 0f) ? 1f : Mathf.Clamp01(1f - (remainingTime / maxTime));

        switch (type)
        {
            case TypeAmelioration.Rentree:
            case TypeAmelioration.Sortie:
                UpdateButtons(grangeButton, canButtonG, (remainingTime <= 0));
                if (grangeFillImage != null) grangeFillImage.fillAmount = fillAmount;
                break;
            case TypeAmelioration.Tonte:
                UpdateButtons(tonteButton, canButtonT, (remainingTime <= 0));
                if (tonteFillImage != null) tonteFillImage.fillAmount = fillAmount;
                break;
            case TypeAmelioration.Nettoyage:
                UpdateButtons(cleanButton, canButtonC, (remainingTime <= 0));
                if (cleanFillImage != null) cleanFillImage.fillAmount = fillAmount;
                break;
        }

        UpdateFills();
    }

    private void OnTimerFinished(TypeAmelioration type)
    {
        UpdateFills();

        switch (type)
        {
            case TypeAmelioration.Rentree:
            case TypeAmelioration.Sortie:
                UpdateButtons(grangeButton, canButtonG);
                break;
            case TypeAmelioration.Tonte:
                UpdateButtons(tonteButton, canButtonT);
                break;
            case TypeAmelioration.Nettoyage:
                UpdateButtons(cleanButton, canButtonC);
                break;
        }
    }

    public IEnumerator UpdateHorloge(int nextIndex)
    {
        float duration = 2f;
        float elapsed = 0f;
        Image invisibleLogo = isLogo1Visible ? logo2 : logo1;
        invisibleLogo.sprite = spriteMiniJeux[nextIndex];

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
        int nextIndex = (index + 1) % values.Length-1;
        currentMiniJeuToDo = values[nextIndex];
        
        GameData.instance.dayMoment.NextMoment();
        
        StartCoroutine(UpdateHorloge(nextIndex));
    }
}