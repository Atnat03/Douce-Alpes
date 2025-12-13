using System;
using UnityEngine;

public class SheepSkinManager : MonoBehaviour
{
    [Header("Références")]
    [SerializeField] private SkinListManagerInterior skinListManager;

    [Header("Skin actuelle du mouton")]
    [SerializeField] private int currentSkinHat = 0;
    [SerializeField] private int currentSkinClothe = 0;

    [Header("Infos mouton")]
    [SerializeField] private int sheepId;
    [SerializeField] private string sheepName;
    [SerializeField] private int colorID;
    
    [SerializeField] public bool hasLaine;
    [SerializeField] private MeshRenderer Laine;
    [SerializeField] private MeshRenderer laineDessous;
    [SerializeField] private ColorSO colorData;

    private bool isTonte = false;
    
    [SerializeField] Animator animator;

    public void Initialize(int id, string name, bool hasLaine, int colorID, int skinHatId, int skinClotheId, bool isTonte = false)
    {
        sheepId = id;
        sheepName = name;
        this.colorID = colorID;
        this.hasLaine = hasLaine;
        this.isTonte = isTonte;
        currentSkinHat = skinHatId;
        currentSkinClothe = skinClotheId;
        
        SetCurrentSkinHat(currentSkinHat);
        SetCurrentSkinClothe(currentSkinClothe);
    }

    private void Update()
    {
        if (isTonte)
            return;
            
        Laine.gameObject.SetActive(hasLaine);
        
        if(Laine != null)
            Laine.material = colorData.colorData[colorID].material;
        
        var mats = laineDessous.GetComponent<MeshRenderer>().materials;
        mats[1] = colorData.colorData[colorID].material;
        laineDessous.GetComponent<MeshRenderer>().materials = mats;
    }

    public void SetCurrentSkinHat(int skinId)
    {
        currentSkinHat = skinId;
        if (skinListManager != null)
            skinListManager.UpdateSkinListHat(currentSkinHat);
    }
    
    public void SetCurrentSkinClothe(int skinId)
    {
        currentSkinClothe = skinId;
        if (skinListManager != null)
            skinListManager.UpdateSkinListClothe(currentSkinClothe);
    }
    
    public int GetCurrentSkinHat()
    {
        return currentSkinHat;
    }

    public int GetCurrentSkinClothe()
    {
        return currentSkinClothe;
    }

    public string GetSheepName()
    {
        return sheepName;
    }
    
    public int GetSheepId()
    {
        return sheepId;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("SauteBaignoire"))
        {
            animator.SetTrigger("Jump");
            Debug.Log("Sauter !!!");
        }
    }

    public void PlayShakeAnimation()
    {
        animator.SetTrigger("Shake");
    }
}