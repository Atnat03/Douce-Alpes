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

    private void Awake()
    {
        if (scrollSnapBridge != null)
            snap = scrollSnapBridge.ScrollSnap;

        if (GameData.instance != null)
            GameData.instance.OnSkinsUpdated += UpdateSkinsDisplay;

        UpdateSkinsDisplay();

        if (snap != null)
            snap.OnPanelCentered.AddListener(OnPanelCentered);
    }
    
    private void OnDestroy()
    {
        if (GameData.instance != null)
            GameData.instance.OnSkinsUpdated -= UpdateSkinsDisplay;

        if (snap != null)
            snap.OnPanelCentered.RemoveListener(OnPanelCentered);
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

            scrollSnapBridge.AddExistingPanel(skinGO);
        }

        ResizePanels(snap, skinPrefab);
        snap.Setup();
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

    private void OnPanelCentered(int newIndex, int previousIndex)
    {
        if (previousIndex >= 0 && previousIndex < snap.Panels.Length)
        {
            Image prevImage = snap.Panels[previousIndex].GetComponent<Image>();
            if (prevImage != null)
                prevImage.sprite = unselectedSprite;
        }

        if (newIndex >= 0 && newIndex < snap.Panels.Length)
        {
            Image newImage = snap.Panels[newIndex].GetComponent<Image>();
            if (newImage != null)
                newImage.sprite = selectedSprite;

            int skinId = snap.Panels[newIndex].GetComponent<SkinUnit>().id;
            switch (skinType)
            {
                case SkinType.Hat:
                    SheepWindow.instance.SetNewCurrentSkinHat(skinId);
                    break;
                case SkinType.Clothe:
                    SheepWindow.instance.SetNewCurrentSkinClothe(skinId);
                    break;
            }
        }
    }
}
