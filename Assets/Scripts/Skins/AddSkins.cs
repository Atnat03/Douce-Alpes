using UnityEngine;
using DanielLochner.Assets.SimpleScrollSnap;

public class AddSkins : MonoBehaviour
{
    [Header("Data Sources")]
    [SerializeField] private SkinScriptable skinData;
    [SerializeField] private GameObject skinPrefab;
    [SerializeField] private SimpleScrollSnapBridge scrollSnapBridge;

    [Header("Testing Mode")]
    [Tooltip("Quand activé, affiche tous les skins du ScriptableObject, même ceux non débloqués.")]
    [SerializeField] private bool isTesting = false;

    private SimpleScrollSnap snap;

    private void Awake()
    {
        snap = scrollSnapBridge.ScrollSnap;

        if (GameData.instance != null)
        {
            GameData.instance.OnSkinsUpdated += UpdateSkinsDisplay;
        }

        UpdateSkinsDisplay();
    }

    private void OnDestroy()
    {
        if (GameData.instance != null)
            GameData.instance.OnSkinsUpdated -= UpdateSkinsDisplay;
    }

    private void UpdateSkinsDisplay()
    {
        foreach (Transform child in snap.Content)
        {
            Destroy(child.gameObject);
        }

        foreach (SkinSkelete skin in skinData.skins)
        {
            if (!isTesting && !GameData.instance.HasSkin(skin.id))
                continue;

            // Création du skin
            GameObject skinGO = Instantiate(skinPrefab);
            SkinUnit s = skinGO.GetComponent<SkinUnit>();
            s.id = skin.id;
            s.name = skin.name;
            s.GetComponent<UnityEngine.UI.Image>().sprite = skin.logo;

            scrollSnapBridge.AddExistingPanel(skinGO);
        }

        RectTransform prefabRect = skinPrefab.GetComponent<RectTransform>();
        float panelWidth = snap.Viewport.rect.width / 5f;
        float panelHeight = prefabRect.rect.height / prefabRect.rect.width * panelWidth;
        snap.Size = new Vector2(panelWidth, panelHeight);

        snap.Setup();
    }

    public GameObject GetSelectedPanel()
    {
        if (scrollSnapBridge == null || scrollSnapBridge.scrollSnap == null)
            return null;

        int selectedIndex = scrollSnapBridge.scrollSnap.CenteredPanel;
        return scrollSnapBridge.scrollSnap.Panels[selectedIndex].gameObject;
    }

    private void Update()
    {
        if (scrollSnapBridge.scrollSnap.Panels.Length == 0) return;

        SheepWindow.instance.SetNewCurrentSkin(
            GetSelectedPanel().GetComponent<SkinUnit>().id
        );
    }
}
