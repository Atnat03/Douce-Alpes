using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DanielLochner.Assets.SimpleScrollSnap;
using UnityEngine.UI;

public class AddSkins : MonoBehaviour
{
    public enum SkinType { Hat, Clothe }

    [Header("Skin Settings")]
    [SerializeField] private SkinType skinType;
    [SerializeField] private SkinScriptable skinData;
    [SerializeField] private GameObject skinPrefab;
    [SerializeField] private SimpleScrollSnapBridge scrollSnapBridge;
    
    [SerializeField] Sprite unselectedSprite;
    [SerializeField] Sprite selectedSprite;

    [Header("Testing Mode")]

    private SimpleScrollSnap snap;
    
    [Header("Stack Display")]  
    [SerializeField] private bool showStackCount = true;
    [SerializeField] private Color unavailableColor = Color.gray;
    [SerializeField] private Color availableColor = Color.white;

    [Header("References")] 
    [SerializeField] private SheepWindow sheepWindow;
    
    [SerializeField] private Sprite centerSprite;
    [SerializeField] private Sprite nearSprite;
    [SerializeField] private Sprite farSprite;
    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private Sprite centerLockSprite;
    [SerializeField] private Sprite notCenterLockSprite;

    private void Awake()
    {
        if (scrollSnapBridge != null)
            snap = scrollSnapBridge.ScrollSnap;

        if (GameData.instance != null)
            GameData.instance.OnSkinsUpdated += UpdateSkinsDisplay;

        UpdateSkinsDisplay();

        if (snap != null)
            snap.OnPanelCentered.AddListener(OnPanelCentered);
        
        if (SkinAgency.instance != null) 
            SkinAgency.instance.OnStacksChanged += UpdateStackDisplays;
    }
    
    private void OnDestroy()
    {
        if (GameData.instance != null)
            GameData.instance.OnSkinsUpdated -= UpdateSkinsDisplay;

        if (snap != null)
            snap.OnPanelCentered.RemoveListener(OnPanelCentered);
        
        if (SkinAgency.instance != null) 
            SkinAgency.instance.OnStacksChanged -= UpdateStackDisplays;
    }

    private void UpdateSkinsDisplay()
    {
        if (snap == null || skinData == null)
            return;

        foreach (Transform child in snap.Content)
            Destroy(child.gameObject);

        foreach (SkinSkelete skin in skinData.skins)
        {
            GameObject skinGO = Instantiate(skinPrefab, snap.Content);
            SkinUnit s = skinGO.GetComponent<SkinUnit>();
            s.id = skin.id;
            s.name = skin.name;
            
            s.logoImage = s.transform.GetChild(0).GetComponent<Image>();
            s.logoImage.sprite = skin.logo;
            
            s.type = skinData.skins.Find(x => x.id == skin.id).type;

            scrollSnapBridge.AddExistingPanel(skinGO);
        }

        ResizePanels(snap, skinPrefab);
        snap.Setup();
        StartCoroutine(UpdateLockSpritesNextFrame());
        
        if (snap != null) UpdateStackDisplays();
    }

    private IEnumerator UpdateLockSpritesNextFrame()
    {
        yield return null;
        UpdateLockSprites();
        UpdateStackDisplays();
    }

    
    private void ResizePanels(SimpleScrollSnap snap, GameObject prefab)
    {
        RectTransform prefabRect = prefab.GetComponent<RectTransform>();
        float panelWidth = snap.Viewport.rect.width / 5f;
        float panelHeight = prefabRect.rect.height / prefabRect.rect.width * panelWidth;
        snap.Size = new Vector2(panelWidth, panelHeight);
    }

    public GameObject GetSelectedPanel()
    {
        if (scrollSnapBridge == null || scrollSnapBridge.ScrollSnap == null)
            return null;

        int selectedIndex = scrollSnapBridge.ScrollSnap.CenteredPanel;
        return scrollSnapBridge.ScrollSnap.Panels[selectedIndex].gameObject;
    }

    public void UpdateStackDisplays()
    {
        if (snap == null || skinData == null || sheepWindow == null || SkinAgency.instance == null)
            return;

        int currentSkinId = (skinType == SkinType.Hat)
            ? sheepWindow.currentSkinHat
            : sheepWindow.currentSkinClothe;

        Dictionary<int, int> stacksDict =
            (skinType == SkinType.Hat)
                ? SkinAgency.instance.dicoHatSkinStack
                : SkinAgency.instance.dicoClotheSkinStack;

        for (int i = 0; i < snap.NumberOfPanels; i++)
        {
            SkinUnit s = snap.Panels[i].GetComponent<SkinUnit>();
            if (s == null) continue;

            int stacks = stacksDict.ContainsKey(s.id) ? stacksDict[s.id] : 0;

            if (showStackCount && s.stackText != null)
                s.stackText.text = stacks.ToString();

            bool shouldLock = ShouldLockSkin(s.id, stacks, currentSkinId);

            if (s.lockImage != null)
                s.lockImage.gameObject.SetActive(shouldLock);
        }
    }
    
private void OnPanelCentered(int newIndex, int previousIndex) 
{
    if (snap == null || sheepWindow == null || SkinAgency.instance == null) return;
    if (newIndex < 0 || newIndex >= snap.Panels.Length) return;

    SkinUnit newSkinUnit = snap.Panels[newIndex].GetComponent<SkinUnit>();
    if (newSkinUnit == null) return;

    int skinId = newSkinUnit.id;

    Dictionary<int, int> stacksDict = (skinType == SkinType.Hat) ? SkinAgency.instance.dicoHatSkinStack : SkinAgency.instance.dicoClotheSkinStack;
    int stacks = stacksDict.ContainsKey(skinId) ? stacksDict[skinId] : 0;
    bool canSelect = (skinType == SkinType.Hat ? sheepWindow.currentSkinHat : sheepWindow.currentSkinClothe) == skinId || stacks > 0;

    if (previousIndex >= 0 && previousIndex < snap.Panels.Length) 
    {
        Image prevImage = snap.Panels[previousIndex].GetComponent<Image>();
        if (prevImage != null) prevImage.sprite = unselectedSprite;
    }
    Image newImage = snap.Panels[newIndex].GetComponent<Image>();
    if (newImage != null) newImage.sprite = selectedSprite;

    if (canSelect)
    {
        AudioManager.instance.PlaySound(22);

        // ✅ Mettre à jour le skin courant **avant** UpdateStackDisplays
        switch (skinType) 
        {
            case SkinType.Hat:
                sheepWindow.SetNewCurrentSkinHat(skinId);
                break;
            case SkinType.Clothe:
                sheepWindow.SetNewCurrentSkinClothe(skinId);
                break;
        }
    }

    UpdateStackDisplays();
    UpdateLockSprites();
}


    private bool ShouldLockSkin(int skinId, int stacks, int currentSkinId)
    {
        return stacks == 0 && skinId != currentSkinId;
    }

    public void SetStartingPanelToCurrent() 
    {
        if (snap == null || sheepWindow == null || snap.NumberOfPanels == 0) return;
        int currentId = (skinType == SkinType.Hat) ? sheepWindow.currentSkinHat : sheepWindow.currentSkinClothe;
        for (int i = 0; i < snap.NumberOfPanels; i++) 
        {
            if (snap.Panels[i].GetComponent<SkinUnit>().id == currentId) 
            {
                snap.GoToPanel(i);
                return;
            }
        }
    }
    
    private void UpdateLockSprites()
    {
        if (snap == null || sheepWindow == null || SkinAgency.instance == null) return;

        int centerIndex = snap.CenteredPanel;
        int currentSkinId = (skinType == SkinType.Hat) ? sheepWindow.currentSkinHat : sheepWindow.currentSkinClothe;

        Dictionary<int, int> stacksDict = (skinType == SkinType.Hat)
            ? SkinAgency.instance.dicoHatSkinStack
            : SkinAgency.instance.dicoClotheSkinStack;

        for (int i = 0; i < snap.NumberOfPanels; i++)
        {
            SkinUnit s = snap.Panels[i].GetComponent<SkinUnit>();
            if (s == null || s.lockImage == null) continue;

            int stacks = stacksDict.ContainsKey(s.id) ? stacksDict[s.id] : 0;

            // ✅ La règle :
            // Lock activé seulement si stacks == 0 ET que ce n'est pas le skin courant
            bool shouldLock = stacks == 0 && s.id != currentSkinId;

            s.lockImage.gameObject.SetActive(shouldLock);

            // Changement du sprite selon la position centrale
            s.lockImage.sprite = (i == centerIndex) ? centerLockSprite : notCenterLockSprite;
        }
    }

    
    public void SelectPanelVisual(int skinId)
    {
        if (snap == null) return;

        for (int i = 0; i < snap.NumberOfPanels; i++)
        {
            SkinUnit s = snap.Panels[i].GetComponent<SkinUnit>();
            if (s == null) continue;

            Image panelImage = snap.Panels[i].GetComponent<Image>();
            if (panelImage == null) continue;

            if (s.id == skinId)
                panelImage.sprite = selectedSprite;
            else
                panelImage.sprite = unselectedSprite;
        }
    }

    
}
