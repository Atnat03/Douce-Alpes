using System;
using UnityEngine;
using UnityEngine.UI;

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
    
    [Header("Bulle")]
    [SerializeField] private GameObject Bubble;
    [SerializeField] private Image ImageInBubble;
    [SerializeField] private Sprite wantToTonte;
    [SerializeField] private Sprite wantToClean;
    [SerializeField] private Sprite wantToGoOut;

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
            PlayJumpAnimation();
            Debug.Log("Sauter !!!");
        }
    }

    public void PlayJumpAnimation()
    {
        animator.SetTrigger("Jump");
    }

    public void PlayShakeAnimation()
    {
        animator.SetTrigger("Shake");
    }
    
    public void DisableBubble()
    {
        if (!Bubble.activeSelf)
            return;
        
        Bubble.SetActive(false);
    }

    public void ActivatedBubble(bool isTonte, bool isGetOut = false)
    {
        Debug.Log("ActivatedBubble : " + isTonte);
        
        if (Bubble.activeSelf)
            return;
        
        Bubble.SetActive(true);

        if(!isGetOut)
        {
            Action a = isTonte
                ? SwapSceneToTonte
                : SwapSceneToClean;

            Sprite s = isTonte ? wantToTonte : wantToClean;
                    
            Bubble.GetComponent<Button>().onClick.AddListener(() => a());
            Bubble.GetComponent<Button>().onClick.AddListener(DisableBubble);
            ImageInBubble.sprite = s;
        }
        else
        {
            Bubble.GetComponent<Button>().onClick.AddListener(GetOffGrange);
            Bubble.GetComponent<Button>().onClick.AddListener(DisableBubble);
            ImageInBubble.sprite = wantToGoOut;
        }
    }

    public void GetOffGrange()
    {
        InteriorSceneManager.instance.DisableSortieBubble();
        
        SwapSceneManager.instance.SwapScene(0);
        
        if(GameManager.instance != null)
            GameManager.instance.grange.LaunchMiniGame();
    }

    public void SwapSceneToTonte() => SwapSceneManager.instance.SwapScene(2);
    public void SwapSceneToClean() => SwapSceneManager.instance.SwapScene(3);
    
    public bool HasActiveBubble()
    {
        return Bubble.activeSelf;
    }
}