using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Abreuvoir : MiniGameParent
{
    public static Abreuvoir instance;

    [Header("Water")]
    [SerializeField] private float maximumWater = 100f;
    [SerializeField] private float currentWater = 100f;
    [SerializeField] private float waterDecreaseRate = 0.5f;
    [SerializeField] private float waterAddValue = 5f;
    [SerializeField] private Animator animatorPompe;
    [SerializeField] private UnityEngine.UI.Image curDrinkImage;
    [SerializeField] private GameObject ui;
    [SerializeField] private GameObject buttonQuit;

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
        buttonQuit.SetActive(false);
    }

    private void Update()
    {
        if (maximumWater > 0)
            curDrinkImage.fillAmount = Mathf.Clamp01(currentWater / maximumWater);
        else
            curDrinkImage.fillAmount = 0;
        
        if(currentWater >= maximumWater && !buttonQuit.activeSelf)
            buttonQuit.SetActive(true);
        
        if (ui != null)
            ui.SetActive(GameManager.instance.currentCameraState == CamState.Drink);

        //if (!isOccupied1 && !isOccupied2) return;

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

        currentWater = Mathf.Min(currentWater + waterAddValue, maximumWater);
        
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
        
        while (value > 0f)
        {
            value -= 0.2f;
            eau.SetFloat("_Apparition", value);
            
            if(value < 1 && value > 0.9)
                splashParticle.Play();
            
            yield return null;
        }


        yield return new WaitForSeconds(1f);

        eau.SetFloat("_Fermeture_Robinet", 1);

        eau.SetFloat("_Apparition", 3f);

        value = eau.GetFloat("_Apparition");
        while (value > 0f)
        {
            value -= 0.2f;
            eau.SetFloat("_Apparition", value);
            yield return null;
        }
        
        eau.SetFloat("_Fermeture_Robinet", 0);

        eau.SetFloat("_Apparition", 3f);
        
        isPomping = false;
    }

    public void EnableEau()
    {
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
