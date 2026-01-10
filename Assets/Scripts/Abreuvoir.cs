using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Abreuvoir : MiniGameParent
{
    public static Abreuvoir instance;

    [Header("Water")]
    [SerializeField] private float maximumWater = 100f;
    [SerializeField] private float currentWater = 100f;
    [SerializeField] private float waterDecreaseRate = 0.5f;
    [SerializeField] private float waterAddValue = 5f;
    [SerializeField] private Animator animatorPompe;
    [SerializeField] private Image curDrinkImage;
    [SerializeField] private GameObject ui;
    private bool IsInAbreuvoir = false;
    [SerializeField] private RectTransform uiBonheurSpawn;
    
    [Header("Drink Places")]
    [SerializeField] private Transform drinkPlace1;
    [SerializeField] private Transform drinkPlace2;
    private bool isOccupied1 = false;
    private bool isOccupied2 = false;

    [Header("Eau")]
    private Material eau;
    [SerializeField] private GameObject Eau;
    [SerializeField] private GameObject water;
    [SerializeField] private Vector3 noWater;
    [SerializeField] private Vector3 fullWater;
    [SerializeField] private ParticleSystem splashParticle;
    bool isPomping = false;
    
    private void Awake() => instance = this;

    public bool alreadyBubble = false;

    private void Start()
    {
        eau = Eau.GetComponent<Renderer>().material;
        eau.SetFloat("_Apparition", 3);

        Eau.transform.rotation = Quaternion.Euler(-90, 0, 0);
        
        Eau.gameObject.SetActive(false);
    }
    
    private void Update()
    {
        float t = Mathf.Clamp01(currentWater / maximumWater);
            
        curDrinkImage.GetComponent<Image>().material.SetFloat("_Slider", t);

        if (currentWater >= maximumWater && IsInAbreuvoir)
        {
            IsInAbreuvoir = false;
            DisableEau();
            Camera.main.GetComponent<ChangingCamera>().ResetPosition();
            BonheurCalculator.instance.AddBonheur(uiBonheurSpawn.position, GameData.instance.GetLevelUpgrade(TypeAmelioration.Abreuvoir));
        }
        
        if (ui != null)
            ui.SetActive(GameManager.instance.currentCameraState == CamState.Drink);

        //if (!isOccupied1 && !isOccupied2) return;

        if(!GameData.instance.isSheepInside)
            currentWater -= Time.deltaTime * waterDecreaseRate;

        if (float.IsNaN(currentWater) || float.IsInfinity(currentWater) || currentWater < 0)
            currentWater = 0;

        if (currentWater <= 0 && !alreadyBubble)
        {
            alreadyBubble = true;
            GameManager.instance.CheckBubble(true);
        }

        water.transform.localPosition = Vector3.Lerp(noWater, fullWater, currentWater /  maximumWater);
        
        if(splashParticle != null)
            splashParticle.gameObject.transform.position = new Vector3(
                splashParticle.gameObject.transform.position.x, 
                water.transform.position.y,
                splashParticle.gameObject.transform.position.z);
    }

    public void AddWater(SwipeType type)
    {
        if (type != SwipeType.Down) return;
        if (GameManager.instance.currentCameraState != CamState.Drink) return;
        if (isPomping) return;
        
        if (!SwipeDetection.instance.IsStartInRightThird()) return;
        
        AudioManager.instance.PlaySound(27);

        StartCoroutine(AddWaterSmooth());

        EndMiniGame(TypeAmelioration.Abreuvoir);
        GameManager.instance.DisableDinkBubble();
    }


    IEnumerator AddWaterSmooth()
    {
        isPomping = true;
        
        eau.SetFloat("_Apparition", 3);
        
        float value = eau.GetFloat("_Apparition");

        animatorPompe?.SetTrigger("Pompe");
        
        float startWater = currentWater;
        float targetWater = Mathf.Min(currentWater + waterAddValue, maximumWater);
        float duration = 0.5f;
        
        while (value > 0f)
        {
            value -= 0.6f;
            eau.SetFloat("_Apparition", value);
            
            if(value < 1 && value > 0.9)
                splashParticle.Play();
            
            yield return null;
        }
        
        float timer = 0f;
        
        while (timer < duration)
        {
            timer += Time.deltaTime;
            currentWater = Mathf.Lerp(startWater, targetWater, timer / duration);
            yield return null;
        }

        currentWater = targetWater;
        
        eau.SetFloat("_Fermeture_Robinet", 1);

        eau.SetFloat("_Apparition", 3f);

        value = eau.GetFloat("_Apparition");
        while (value > 0f)
        {
            value -= 0.6f;
            eau.SetFloat("_Apparition", value);
            yield return null;
        }
        
        eau.SetFloat("_Fermeture_Robinet", 0);

        eau.SetFloat("_Apparition", 3f);
        
        isPomping = false;
    }

    public void EnableEau()
    {
        IsInAbreuvoir = true;
        StartCoroutine(WaitEndOfTransitionToEnableEau());
    }

    IEnumerator WaitEndOfTransitionToEnableEau()
    {
        yield return new WaitForSeconds(0.2f);
        Eau.gameObject.SetActive(true);
    }

    public void DisableEau()
    {
        Eau.gameObject.SetActive(false);
    }

    public bool TryReservePlace(out Transform place)
    {
        place = null;

        if (!isOccupied1)
        {
            isOccupied1 = true;
            place = drinkPlace1;
            return true;
        }

        if (!isOccupied2)
        {
            isOccupied2 = true;
            place = drinkPlace2;
            return true;
        }

        return false;
    }

    public void FreePlace(Transform place)
    {
        if (place == drinkPlace1) isOccupied1 = false;
        else if (place == drinkPlace2) isOccupied2 = false;
    }

    void OnEnable() => SwipeDetection.instance.OnSwipeDetected += AddWater;
    void OnDisable() => SwipeDetection.instance.OnSwipeDetected -= AddWater;
}
