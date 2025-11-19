using System;
using System.Collections;
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
    
    private void Awake() => instance = this;

    private void Start()
    {
        eau = Eau.GetComponent<Renderer>().material;
        eau.SetFloat("_Apparition", 3);

        Eau.transform.rotation = Quaternion.Euler(-90, 0, 0);
    }

    private void Update()
    {
        if (maximumWater > 0)
            curDrinkImage.fillAmount = Mathf.Clamp01(currentWater / maximumWater);
        else
            curDrinkImage.fillAmount = 0;

        if (ui != null)
            ui.SetActive(GameManager.instance.currentCameraState == CamState.Drink);

        if (!isOccupied1 && !isOccupied2) return;

        currentWater -= Time.deltaTime * waterDecreaseRate;

        if (float.IsNaN(currentWater) || float.IsInfinity(currentWater) || currentWater < 0)
            currentWater = 0;

        water.transform.position = Vector3.Lerp(noWater, fullWater, currentWater /  maximumWater);
    }

    public void AddWater(SwipeType type)
    {
        if (type != SwipeType.Down) return;
        if (GameManager.instance.currentCameraState != CamState.Drink) return;

        currentWater = Mathf.Min(currentWater + waterAddValue, maximumWater);
        
        StartCoroutine(AddWaterSmooth());
        
        EndMiniGame(TypeAmelioration.Abreuvoir);
    }

    IEnumerator AddWaterSmooth()
    {
        Eau.transform.rotation = Quaternion.Euler(-90f, 0f, 0f);

        float value = eau.GetFloat("_Apparition");

        animatorPompe?.SetTrigger("Pompe");
        
        while (value > 0f)
        {
            value -= 0.2f; 
            eau.SetFloat("_Apparition", value);
            yield return null;
        }

        eau.SetFloat("_Apparition", 0f);

        yield return new WaitForSeconds(1f);

        Eau.transform.rotation = Quaternion.Euler(90f, 0f, 0f);

        value = eau.GetFloat("_Apparition");
        while (value < 3f)
        {
            value += 0.2f;
            eau.SetFloat("_Apparition", value);
            yield return null;
        }

        eau.SetFloat("_Apparition", 3f);
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
