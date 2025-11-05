using UnityEngine;
using DanielLochner.Assets.SimpleScrollSnap;

public class AddSkins : MonoBehaviour
{
    public enum SkinType { Hat, Clothe }

    [Header("Skin Settings")]
    [SerializeField] private SkinType skinType;
    [SerializeField] private SkinScriptable skinData;
    [SerializeField] private GameObject skinPrefab;
    [SerializeField] private SimpleScrollSnapBridge scrollSnapBridge;

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
    }

    private void OnDestroy()
    {
        if (GameData.instance != null)
            GameData.instance.OnSkinsUpdated -= UpdateSkinsDisplay;
    }

    private void UpdateSkinsDisplay()
    {
        if (snap == null || skinData == null)
            return;

        // Nettoyage des anciens panels
        foreach (Transform child in snap.Content)
            Destroy(child.gameObject);

        // Ajout des skins
        foreach (SkinSkelete skin in skinData.skins)
        {
            if (!isTesting && !GameData.instance.HasSkin(skin.id))
                continue;

            GameObject skinGO = Instantiate(skinPrefab, snap.Content);
            SkinUnit s = skinGO.GetComponent<SkinUnit>();
            s.id = skin.id;
            s.name = skin.name;
            s.GetComponent<UnityEngine.UI.Image>().sprite = skin.logo;

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

    private void Update()
    {
        var selectedPanel = GetSelectedPanel();
        if (selectedPanel == null) return;

        int skinId = selectedPanel.GetComponent<SkinUnit>().id;

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
