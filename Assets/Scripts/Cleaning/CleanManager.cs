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
    public static CleanManager instance;

    [Header("References")]
    public Camera camera;
    public Transform sheepTarget;

    [Header("Current Tool")]
    public CleaningTool currentTool;

    [Header("Tool Particles")] 
    [SerializeField] private GameObject shampoo;
    [SerializeField] private GameObject shower;

    [Header("UI")] 
    [SerializeField] private GameObject shampooContour;
    [SerializeField] private GameObject showerContour;
    [SerializeField] private Button showerButton;
    
    private List<GameObject> shampooList = new List<GameObject>();

    [Header("Clean Values")] 
    private float cleanValue = 0;
    private float totalValueCleaned = 0;
    public int maxShampoo = 100;

    [Header("Anti-Spam Shampoo Settings")]
    [SerializeField] private float minDistanceBetweenShampoos = 0.05f;
    private Vector3 lastShampooPos = Vector3.zero;
    private bool hasLastPos = false;

    public float GetCleanValue() { return cleanValue; }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        SetShampoo();
    }

    private void Update()
    {
        showerButton.interactable = totalValueCleaned >= maxShampoo;
    }

    public void ResetValueClean()
    {
        totalValueCleaned += cleanValue;
        cleanValue = 0;
        hasLastPos = false;
        lastShampooPos = Vector3.zero;
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
        ApplyClean(pos);
    }

    public void ApplyClean(Vector3 pos)
    {
        if (totalValueCleaned >= maxShampoo && currentTool == CleaningTool.Shampoo)
            return;

        switch (currentTool)
        {
            case CleaningTool.Shampoo:
                TryAddShampoo(pos);
                break;

            case CleaningTool.Shower:
                CheckShampoo(pos);
                break;
        }
    }

    private void TryAddShampoo(Vector3 pos)
    {
        if (!hasLastPos || Vector3.Distance(lastShampooPos, pos) >= minDistanceBetweenShampoos)
        {
            GameObject s = Instantiate(shampoo, pos, Quaternion.identity);
            shampooList.Add(s);
            cleanValue += 1f;
            lastShampooPos = pos;
            hasLastPos = true;
        }
    }

    private void CheckShampoo(Vector3 pos)
    {
        GameObject d = Instantiate(shower, pos, Quaternion.identity);
        Destroy(d, 0.3f);

        float radius = 0.025f;
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

    public void ResetLastShampooPos()
    {
        hasLastPos = false;
    }

    public int GetCounterListShampoo()
    {
        return shampooList.Count;
    }

    private void OnDisable()
    {
        foreach (GameObject shampoo in shampooList)
        {
            Destroy(shampoo);
        }
    }
}
