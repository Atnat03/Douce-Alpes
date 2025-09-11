using System;
using UnityEngine;
using UnityEngine.UI;


public class Sheep : TouchableObject
{
    [SerializeField] private int sheepId;
    [SerializeField] private string sheepName;

    [SerializeField] private int currentSkin;

    [SerializeField] private float currentCaressesValue;
    [SerializeField] private float maxCaressesValue = 100;

    [Header("Timer")]
    [SerializeField] private float holdingTimer;
    private float timer; 
    private bool startTimer = false;

    [Header("References")]
    [SerializeField] private Transform cameraPosition;
    [SerializeField] private GameObject wheelTimerUI;
    [SerializeField] private Image valueWheelTimerImage;

    private void Start()
    {
        wheelTimerUI.SetActive(false);
        holdingTimer = TouchManager.instance.holdThreshold;
    }

    private void Update()
    {
        if (startTimer)
        {
            timer += Time.deltaTime;
            valueWheelTimerImage.fillAmount = timer / holdingTimer;

            if (timer >= holdingTimer)
            {
                wheelTimerUI.SetActive(false);
                startTimer = false;
                WidowOpen();
            }
        }
    }

    public Vector3 GetCameraPosition()
    {
        return cameraPosition.transform.position;
    }

    public void AddCaresse()
    {
        currentCaressesValue += 1f;
        GameManager.instance.Caresse();
    }

    public override void TouchEvent()
    {
        print(gameObject.name + " touched");
    }

    public void StartHolding()
    {
        timer = 0f;
        startTimer = true;
        wheelTimerUI.SetActive(true);
        valueWheelTimerImage.fillAmount = 0f;
    }

    public void CancelHolding()
    {
        startTimer = false;
        wheelTimerUI.SetActive(false);
    }

    public void WidowOpen()
    {
        GameManager.instance.ChangeCameraPos(
            cameraPosition.transform.position,
            cameraPosition.transform.rotation.eulerAngles
        );
        
        GameManager.instance.GetSheepWindow().SetActive(true);
        
        SheepWindow.instance.Initialize(sheepName, currentSkin, sheepId);
    }

    public void OnTouchEnd()
    {
        CancelHolding();
    }

    public void SetCurrentSkin(int skinId)
    {
        currentSkin = skinId;
    }
}
