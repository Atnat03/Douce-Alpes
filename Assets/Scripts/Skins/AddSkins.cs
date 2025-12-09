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
    [Tooltip("Quand activé, affiche tous les skins du ScriptableObject, même ceux non débloqués.")]
    [SerializeField] private bool isTesting = false;

    private SimpleScrollSnap snap;
    
    [Header("Stack Display")]  // Ajout
    [SerializeField] private bool showStackCount = true;
    [SerializeField] private Color unavailableColor = Color.gray;
    [SerializeField] private Color availableColor = Color.white;

    [Header("References")]  // Ajout
    [SerializeField] private SheepWindow sheepWindow;

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
            if (!isTesting && !GameData.instance.HasSkin(skin.id))
                continue;

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

private void UpdateStackDisplays() 
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
    Debug.Log($"[AddSkins {skinType}] OnPanelCentered appelée ! New: {newIndex}, Prev: {previousIndex}");  // Log 1 : Événement déclenché ?

    // Null checks
    if (snap == null || sheepWindow == null || SkinAgency.instance == null) 
    {
        Debug.LogError($"[AddSkins {skinType}] SKIP: snap={snap!=null}, sheepWindow={sheepWindow!=null}, SkinAgency={SkinAgency.instance!=null}");
        return;
    }

    if (newIndex < 0 || newIndex >= snap.Panels.Length) 
    {
        Debug.LogWarning($"[AddSkins {skinType}] Invalid index {newIndex}");
        return;
    }

    SkinUnit newSkinUnit = snap.Panels[newIndex].GetComponent<SkinUnit>();
    if (newSkinUnit == null) 
    {
        Debug.LogError($"[AddSkins {skinType}] No SkinUnit on panel {newIndex}");
        return;
    }
    int skinId = newSkinUnit.id;
    Debug.Log($"[AddSkins {skinType}] Skin ID sélectionné: {skinId}");  // Log 2 : ID OK ?

    int currentSkinId = (skinType == SkinType.Hat) ? sheepWindow.currentSkinHat : sheepWindow.currentSkinClothe;
    Debug.Log($"[AddSkins {skinType}] Current skin ID: {currentSkinId}");  // Log 3 : Current OK ?

    // Stacks
    Dictionary<int, int> stacksDict = (skinType == SkinType.Hat) ? SkinAgency.instance.dicoHatSkinStack : SkinAgency.instance.dicoClotheSkinStack;
    int stacks = stacksDict.ContainsKey(skinId) ? stacksDict[skinId] : 0;
    bool canSelect = (skinId == currentSkinId) || (stacks > 0);
    Debug.Log($"[AddSkins {skinType}] Stacks: {stacks}, CanSelect: {canSelect}");  // Log 4 : Check OK ?

    if (!canSelect) 
    {
        Debug.Log($"[AddSkins {skinType}] BLOCK: Snap back to {previousIndex}");
        snap.ScrollRect.inertia = false;
        StartCoroutine(SnapBackWithDelay(previousIndex, 0.05f));
        return;
    }

    // Visuels (inchangés)
    if (previousIndex >= 0 && previousIndex < snap.Panels.Length) 
    {
        Image prevImage = snap.Panels[previousIndex].GetComponent<Image>();
        if (prevImage != null) prevImage.sprite = unselectedSprite;
    }
    Image newImage = snap.Panels[newIndex].GetComponent<Image>();
    if (newImage != null) newImage.sprite = selectedSprite;

    // Log avant appel
    Debug.Log($"[AddSkins {skinType}] Appel SetNewCurrentSkin{skinType}({skinId})");  // Log 5 : Avant switch

    // Switch
    switch (skinType) 
    {
        case SkinType.Hat:
            sheepWindow.SetNewCurrentSkinHat(skinId);
            break;
        case SkinType.Clothe:
            sheepWindow.SetNewCurrentSkinClothe(skinId);
            break;
        default:
            Debug.LogError($"[AddSkins] Unknown skinType: {skinType}");
            break;
    }

    Debug.Log($"[AddSkins {skinType}] Switch fini, refresh displays");  // Log 6 : Après switch
    UpdateStackDisplays();
}    // Ajout : Coroutine pour snap back
    private IEnumerator SnapBackWithDelay(int targetIndex, float delay) 
    {
        yield return new WaitForSeconds(delay);
        snap.GoToPanel(targetIndex);
        snap.ScrollRect.inertia = true;
    }

    // Ajout : Pour centrer sur current à l'init
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
    }}
