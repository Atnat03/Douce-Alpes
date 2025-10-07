using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TricoManager : MonoBehaviour
{
    [SerializeField] private List<SwipeType> currentPattern;
    [SerializeField] private GameObject[] modelShape;
    [SerializeField] private GameObject okImage;
    [SerializeField] private Image imageProduct;
    [SerializeField] private float fillSpeed = 2f;

    private int numberModelOfThisPattern = 0;
    private int currentModel = 0;
    private bool canShowNext = true;
    private float targetFill = 0f;

    private void Start()
    {
        ActivateModel(-1);
        okImage.SetActive(false);
        SwipeDetection.instance.OnSwipeDetected += CheckNewSwipe;
    }

    public void InitializeProduct(PatternsTricotsSO pattern)
    {
        okImage.SetActive(false);
        
        currentPattern = new List<SwipeType>(pattern.pattern);
        numberModelOfThisPattern = currentPattern.Count;
        currentModel = 0;

        imageProduct.sprite = pattern.image;
        imageProduct.fillAmount = 0f;
        targetFill = 0f;

        if (numberModelOfThisPattern > 0)
            StartCoroutine(ShowModelWithDelay(currentPattern[currentModel]));
        else
            ActivateModel(-1);
    }

    private void Update()
    {
        if (imageProduct.fillAmount < targetFill)
        {
            imageProduct.fillAmount += Time.deltaTime * fillSpeed;
            if (imageProduct.fillAmount > targetFill)
                imageProduct.fillAmount = targetFill;
        }
    }

    void GetNextModel()
    {
        currentModel++;

        targetFill = (float)currentModel / numberModelOfThisPattern;

        if (currentModel < numberModelOfThisPattern)
        {
            StartCoroutine(ShowModelWithDelay(currentPattern[currentModel]));
        }
        else
        {
            Debug.Log("Fin du pattern");
            
            okImage.SetActive(true);
            ActivateModel(-1);
        }
    }

    public void SetModel(SwipeType type)
    {
        switch (type)
        {
            case SwipeType.Up: ActivateModel(0); break;
            case SwipeType.Down: ActivateModel(1); break;
            case SwipeType.Left: ActivateModel(2); break;
            case SwipeType.Right: ActivateModel(3); break;
            case SwipeType.Circle: ActivateModel(4); break;
            case SwipeType.Square: ActivateModel(5); break;
            default: ActivateModel(-1); break;
        }
    }

    void ActivateModel(int x)
    {
        for (int i = 0; i < modelShape.Length; i++)
            modelShape[i].SetActive(i == x);
    }

    private void CheckNewSwipe(SwipeType type)
    {
        if (!canShowNext) return;
        if (currentModel >= numberModelOfThisPattern) return;

        if (type == currentPattern[currentModel])
        {
            Debug.Log("Good shape");
            GetNextModel();
        }
        else
        {
            Debug.Log("Wrong shape");
        }
    }

    private IEnumerator ShowModelWithDelay(SwipeType type)
    {
        canShowNext = false;
        ActivateModel(-1);
        yield return new WaitForSeconds(0.5f);
        SetModel(type);
        canShowNext = true;
    }
}
