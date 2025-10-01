using System;
using UnityEngine;
using UnityEngine.UI;

public class Abreuvoir : MonoBehaviour
{
    public static Abreuvoir instance;
    
    private float maximumWater;
    [SerializeField] private float currentWater = 100;
    [SerializeField] private bool isEmptyWater = false;

    [SerializeField] private float valueAdded = 5;
    [SerializeField] private float speedDecrement = 0.5f;
    [SerializeField] private Animator animatorPompe;
    
    [Header("UI")]
    [SerializeField] public GameObject ui;
    [SerializeField] private Image curDrinkImage;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        maximumWater = currentWater;
        
        ui.SetActive(false);
    }

    private void Update()
    {
        curDrinkImage.fillAmount = currentWater / maximumWater;

        ui.SetActive(GameManager.instance.currentCameraState == CamState.Drink);
        
        if(currentWater >= maximumWater)
            currentWater = maximumWater;
        if(currentWater <= 0)
        {
            currentWater = 0;
            isEmptyWater = true;
        }
        else
        {
            isEmptyWater = true;
            
            if(!GameData.instance.isSheepInside)
                currentWater -= Time.deltaTime * 0.5f;
        }
    }

    public void AddWater()
    {
        if (GameManager.instance.currentCameraState == CamState.Drink)
        {
            currentWater += valueAdded;
            animatorPompe.SetTrigger("Pompe");
        }
    }
}
