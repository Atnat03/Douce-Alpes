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
    
    [Header("Stack Display")]  // Ajout
    [SerializeField] private bool showStackCount = true;
    [SerializeField] private Color unavailableColor = Color.gray;
    [SerializeField] private Color availableColor = Color.white;

    [Header("References")]  // Ajout
    [SerializeField] private SheepWindow sheepWindow;
    
    [SerializeField] private Sprite centerSprite;
    [SerializeField] private Sprite nearSprite;
    [SerializeField] private Sprite farSprite;
    [SerializeField] private Sprite defaultSprite;

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
            s.transform.GetChild(0).GetComponent<Image>().sprite = skin.logo;
            s.type = skinData.skins.Find(x => x.id == skin.id).type;

            scrollSnapBridge.AddExistingPanel(skinGO);
        }

        ResizePanels(snap, skinPrefab);
        snap.Setup();
        
        if (snap != null) UpdateStackDisplays();
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
        if (snap == null || skinData == null || sheepWindow == null) return;

        int currentSkinId = (skinType == SkinType.Hat) ? sheepWindow.currentSkinHat : sheepWindow.currentSkinClothe;

        for (int i = 0; i < snap.NumberOfPanels; i++) 
        {
            SkinUnit s = snap.Panels[i].GetComponent<SkinUnit>();
            if (s == null) continue;

            Dictionary<int, int> stacksDict = (skinType == SkinType.Hat) ? SkinAgency.instance.dicoHatSkinStack : SkinAgency.instance.dicoClotheSkinStack;
            int stacks = stacksDict.ContainsKey(s.id) ? stacksDict[s.id] : 0;

            // Affichage count
            if (showStackCount && s.stackText != null) 
                s.stackText.text = stacks.ToString();

            // Grisage
            Image panelImage = snap.Panels[i].GetComponent<Image>();
            if (panelImage != null) 
            {
                bool isUnavailable = (stacks == 0 && s.id != currentSkinId);
                panelImage.color = isUnavailable ? unavailableColor : availableColor;
            }
        }
    }

    private void OnPanelCentered(int newIndex, int previousIndex) 
    {
        if (snap == null || sheepWindow == null || SkinAgency.instance == null) return;
        if (newIndex < 0 || newIndex >= snap.Panels.Length) return;

        SkinUnit newSkinUnit = snap.Panels[newIndex].GetComponent<SkinUnit>();
        if (newSkinUnit == null) return;

        int skinId = newSkinUnit.id;
        int currentSkinId = (skinType == SkinType.Hat) ? sheepWindow.currentSkinHat : sheepWindow.currentSkinClothe;

        // Vérifier les stacks mais ne bloque pas le snap
        Dictionary<int, int> stacksDict = (skinType == SkinType.Hat) ? SkinAgency.instance.dicoHatSkinStack : SkinAgency.instance.dicoClotheSkinStack;
        int stacks = stacksDict.ContainsKey(skinId) ? stacksDict[skinId] : 0;
        bool canSelect = (skinId == currentSkinId) || (stacks > 0);

        // Mettre à jour les visuels (sélection)
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
