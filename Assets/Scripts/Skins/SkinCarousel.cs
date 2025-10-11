using UnityEngine;
using UnityEngine.UI;

public class SkinCarousel : MonoBehaviour
{
    [Header("References")]
    public ScrollRect scrollRect;
    public RectTransform viewPortTransform;
    public RectTransform contentTransform;
    public HorizontalLayoutGroup horizontalLayoutGroup;

    [Header("Snap Settings")]
    [Range(1f, 20f)] public float snapSpeed = 8f;
    [Range(0.01f, 1f)] public float snapTriggerVelocity = 0.2f;

    private bool isSnapping = false;
    private Vector3 targetPosition;
    private float itemWidth;

    [Header("Skins")]
    [SerializeField] private SkinScriptable skinData;
    [SerializeField] private GameObject skinPrefab;

    private RectTransform[] itemList;
    public RectTransform currentSnappedItem { get; private set; }

    private void Start()
    {
        CreateAllSkinUI();

        if (itemList == null || itemList.Length == 0)
        {
            Debug.LogWarning("Aucun skin détecté dans le carousel !");
            return;
        }

        // Calcule la largeur d’un item (image + espacement)
        itemWidth = itemList[0].rect.width + horizontalLayoutGroup.spacing;

        // Démarre positionné au centre du premier élément
        SnapToItem(itemList[0], instant: true);
    }

    private void Update()
    {
        HandleSnap();
    }

    private void HandleSnap()
    {
        // Si on scroll encore, pas de snap
        if (scrollRect.velocity.magnitude > snapTriggerVelocity)
        {
            isSnapping = false;
            return;
        }

        // Si on est immobile et pas encore en snapping → on choisit l’élément le plus proche
        if (!isSnapping)
            SnapToClosestItem();

        // Mouvement de snapping
        if (isSnapping)
        {
            contentTransform.localPosition = Vector3.Lerp(
                contentTransform.localPosition,
                targetPosition,
                Time.deltaTime * snapSpeed
            );

            if (Vector3.Distance(contentTransform.localPosition, targetPosition) < 0.01f)
            {
                contentTransform.localPosition = targetPosition;
                isSnapping = false;

                if (currentSnappedItem && currentSnappedItem.GetComponentInChildren<SkinUnit>())
                {
                    SheepWindow.instance.SetNewCurrentSkin(
                        currentSnappedItem.GetComponentInChildren<SkinUnit>().id
                    );
                }
            }
        }
    }

    private void SnapToClosestItem()
    {
        float closestDistance = float.MaxValue;
        RectTransform closestItem = null;

        Vector3 viewportCenterWorld = viewPortTransform.TransformPoint(
            new Vector3(viewPortTransform.rect.width / 2, 0, 0)
        );

        foreach (RectTransform item in itemList)
        {
            Vector3 itemWorldPos = item.TransformPoint(Vector3.zero);
            float distance = Mathf.Abs(viewportCenterWorld.x - itemWorldPos.x);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestItem = item;
            }
        }

        if (closestItem != null)
            SnapToItem(closestItem);
    }

    private void SnapToItem(RectTransform item, bool instant = false)
    {
        if (item == null) return;

        currentSnappedItem = item;

        Vector3 itemWorldPos = item.TransformPoint(Vector3.zero);
        Vector3 viewportCenterWorld = viewPortTransform.TransformPoint(
            new Vector3(viewPortTransform.rect.width / 2, 0, 0)
        );

        float deltaX = viewportCenterWorld.x - itemWorldPos.x;
        targetPosition = contentTransform.localPosition + new Vector3(deltaX, 0, 0);

        if (instant)
        {
            contentTransform.localPosition = targetPosition;
            isSnapping = false;
        }
        else
        {
            isSnapping = true;
        }

        // Met à jour le skin courant
        SkinUnit unit = item.GetComponentInChildren<SkinUnit>();
        if (unit)
            SheepWindow.instance.SetNewCurrentSkin(unit.id);
    }

    private void CreateAllSkinUI()
    {
        if (skinData == null || skinPrefab == null)
        {
            Debug.LogError("Skin data ou prefab manquant !");
            return;
        }

        itemList = new RectTransform[skinData.skins.Count];

        for (int i = 0; i < skinData.skins.Count; i++)
        {
            SkinSkelete skin = skinData.skins[i];
            GameObject go = Instantiate(skinPrefab, contentTransform);
            go.name = $"Skin_{skin.name}";

            Image img = go.transform.GetChild(0).GetComponent<Image>();
            img.sprite = skin.logo;

            SkinUnit unit = go.transform.GetChild(0).GetComponent<SkinUnit>();
            unit.id = skin.id;

            // Ajout bouton cliquable
            Button btn = go.GetComponent<Button>();
            if (btn == null) btn = go.AddComponent<Button>();

            // Setup du SkinButton
            SkinButton skinBtn = go.GetComponent<SkinButton>();
            if (skinBtn == null) skinBtn = go.AddComponent<SkinButton>();
            skinBtn.Initialize(this);

            itemList[i] = go.GetComponent<RectTransform>();
        }
    }

    public void OnSkinClicked(RectTransform clickedItem)
    {
        if (clickedItem == null) return;
        SnapToItem(clickedItem);
    }
}
