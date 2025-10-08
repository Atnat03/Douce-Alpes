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
    public float snapSpeed = 10f;
    public float snapTriggerVelocity = 0.1f;

    private bool isSnapping = false;
    private Vector3 targetPosition;

    private float itemWidth;
    private int clonesCount;

    [Header("Skins")]
    [SerializeField] private SkinScriptable skinData;
    [SerializeField] private GameObject skinPrefab;
    public RectTransform[] itemList;
    public RectTransform currentSnappedItem { get; private set; }

    private void CreateAllSkinUI()
    {
        if (skinData == null || skinPrefab == null) return;

        itemList = new RectTransform[skinData.skins.Count];

        for (int i = 0; i < skinData.skins.Count; i++)
        {
            SkinSkelete skin = skinData.skins[i];
            GameObject go = Instantiate(skinPrefab, contentTransform);
            Image img = go.transform.GetChild(0).GetComponent<Image>();
            img.sprite = skin.logo;

            SkinUnit unit = go.transform.GetChild(0).GetComponent<SkinUnit>();
            unit.id = skin.id;
            go.transform.GetChild(0).name = skin.name;
            
            Button btn = go.transform.GetComponent<Button>();
            int index = i;
            btn.onClick.AddListener(() => OnSkinClicked(index));

            itemList[i] = go.GetComponent<RectTransform>();
        }
    }

    private void Start()
    {
        CreateAllSkinUI();

        itemWidth = itemList[0].rect.width + horizontalLayoutGroup.spacing;
        clonesCount = Mathf.CeilToInt(viewPortTransform.rect.width / itemWidth);

        // Clones à la fin
        for (int i = 0; i < clonesCount; i++)
        {
            RectTransform item = Instantiate(itemList[i % itemList.Length], contentTransform);
            item.SetAsLastSibling();
        }

        // Clones au début
        for (int i = 0; i < clonesCount; i++)
        {
            int index = (itemList.Length - i - 1 + itemList.Length) % itemList.Length;
            RectTransform item = Instantiate(itemList[index], contentTransform);
            item.SetAsFirstSibling();
        }

        float offset = itemWidth * clonesCount;
        contentTransform.localPosition = new Vector3(-offset, 0, 0);
    }

    private void Update()
    {
        if (!isSnapping)
            HandleInfiniteScroll();

        HandleSnap();
    }

    private void HandleInfiniteScroll()
    {
        float totalWidth = itemList.Length * itemWidth;

        if (contentTransform.localPosition.x > 0)
            contentTransform.localPosition -= new Vector3(totalWidth, 0, 0);
        else if (contentTransform.localPosition.x < -totalWidth)
            contentTransform.localPosition += new Vector3(totalWidth, 0, 0);
    }

    private void HandleSnap()
    {
        if (scrollRect.velocity.magnitude > snapTriggerVelocity)
        {
            isSnapping = false;
            return;
        }

        if (!isSnapping)
            SnapToClosestItem();

        if (isSnapping)
        {
            contentTransform.localPosition = Vector3.Lerp(contentTransform.localPosition, targetPosition, snapSpeed * Time.deltaTime);

            if (Vector3.Distance(contentTransform.localPosition, targetPosition) < snapTriggerVelocity)
            {
                contentTransform.localPosition = targetPosition;
                isSnapping = false;

                if (currentSnappedItem && currentSnappedItem.GetChild(0).GetComponent<SkinUnit>())
                {
                    SheepWindow.instance.SetNewCurrentSkin(currentSnappedItem.GetChild(0).GetComponent<SkinUnit>().id);
                }
            }
        }
    }

    private void SnapToClosestItem()
    {
        float closestDistance = float.MaxValue;
        Vector3 closestPos = contentTransform.localPosition;

        float viewportCenterX = viewPortTransform.rect.width / 2;

        // On parcourt uniquement les items originaux
        for (int i = clonesCount; i < clonesCount + itemList.Length; i++)
        {
            RectTransform item = contentTransform.GetChild(i) as RectTransform;
            float distance = Mathf.Abs((item.localPosition.x + contentTransform.localPosition.x) - viewportCenterX);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                float offset = contentTransform.localPosition.x + (viewportCenterX - (item.localPosition.x + contentTransform.localPosition.x));
                closestPos = new Vector3(offset, contentTransform.localPosition.y, 0);

                currentSnappedItem = item;
            }
        }

        targetPosition = closestPos;
        isSnapping = true;
    }
    
    private void OnSkinClicked(int index)
    {
        if (index < 0 || index >= itemList.Length) return;

        RectTransform item = itemList[index];
        currentSnappedItem = item;

        // Centre du viewport
        float viewportCenterX = viewPortTransform.rect.width / 2;

        // Calcul de la position target pour centrer cet item
        float offset = contentTransform.localPosition.x + (viewportCenterX - (item.localPosition.x + contentTransform.localPosition.x));
        targetPosition = new Vector3(offset, contentTransform.localPosition.y, 0);

        isSnapping = true;

        // Appliquer le skin immédiatement
        if (item.GetComponentInChildren<SkinUnit>())
        {
            SheepWindow.instance.SetNewCurrentSkin(item.GetComponentInChildren<SkinUnit>().id);
        }
    }

}
