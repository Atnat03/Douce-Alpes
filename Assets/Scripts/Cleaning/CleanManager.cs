using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum CleaningTool
{
    Shampoo,
    Shower,
}

public class CleanManager : MonoBehaviour
{
    public Camera camera;
    public Transform sheepTarget;

    public CleaningTool currentTool;

    [Header("Tool particles")] 
    [SerializeField] private GameObject shampoo;
    [SerializeField] private GameObject shower;

    [Header("UI")] 
    [SerializeField] private GameObject shampooContour;

    [SerializeField] private GameObject showerContour;
    [SerializeField] private Button showerButton;
    
    List<GameObject> shampooList = new List<GameObject>();

    [Header("Clean Values")] 
    private float cleanValue = 0;

    private float totalValueCleaned = 0;
    
    [HideInInspector] public int maxShampoo = 50;
    
    public float GetCleanValue(){return cleanValue;}

    private void Start()
    {
        SetShampoo();
    }

    private void Update()
    {
        showerButton.interactable = totalValueCleaned >= maxShampoo;
        
        print(GetCounterListShampoo());
    }

    private void OnEnable()
    {
        TouchManager.instance.OnCleanTouch += PerformClean;
    }

    private void OnDisable()
    {
        TouchManager.instance.OnCleanTouch -= PerformClean;
    }

    public void ResetValueClean()
    {
        totalValueCleaned += cleanValue;
        cleanValue = 0;
    }

    public void SetShampoo()
    {
        currentTool = CleaningTool.Shampoo;
        shampooContour.SetActive(true);
        showerContour.SetActive(false);
    }

    public void SetShower()
    {
        currentTool = CleaningTool.Shower;
        shampooContour.SetActive(false);
        showerContour.SetActive(true);
    }
    

    public void PerformClean(Vector3 pos)
    {
        if (totalValueCleaned >= maxShampoo && currentTool == CleaningTool.Shampoo) return;
        
        GameObject effect = null;

        switch (currentTool)
        {
            case CleaningTool.Shampoo:
                AddShampoo(pos);
                break;
            case CleaningTool.Shower:
                CheckShampoo(pos);
                break;
        }
    }

    private void CheckShampoo(Vector3 pos)
    {
        GameObject d = Instantiate(shower, pos, Quaternion.identity);
        Destroy(d, 0.3f);

        float radius = 0.075f;
        Collider[] hits = Physics.OverlapSphere(pos, radius);

        foreach (Collider hit in hits)
        {
            if (shampooList.Contains(hit.gameObject))
            {
                Destroy(hit.gameObject);
                shampooList.Remove(hit.gameObject);
            }
        }
    }

    public void AddShampoo(Vector3 pos)
    {
        GameObject s = Instantiate(shampoo, pos, Quaternion.identity);
        shampooList.Add(s);
        
        cleanValue += 1f;
    }
    
    public int GetCounterListShampoo(){return shampooList.Count;}
}
