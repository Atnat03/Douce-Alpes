using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonShapeDrawing : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerUpHandler
{
    public int id;
    [SerializeField] private TricotManager manager;
    private RectTransform rect;

    private Vector3 startScale;
    
    Animator animator;

    public RectTransform center;

    public Color baseColor;
    public Color startColor;
    public Color passageColor;

    public bool isFirst = false;
    
    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        
        animator = GetComponent<Animator>();
        animator.enabled = false;
        
        startScale = transform.localScale;
    }

    void StartHover()
    {
        print("Hover");
        
        if (manager == null) return;
        manager.SetHover(true);
        SnapLineToButtonCenter();
    }

    public void SetFirstPoint()
    {
        animator.enabled = true;
        isFirst = true;
        GetComponent<Image>().color = startColor;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        StartHover();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SnapLineToButtonCenter();
    }

    private void SnapLineToButtonCenter()
    {
        if (manager == null || manager.uiLineRenderer == null)
            return;

        GetComponent<Image>().color = passageColor;

        AudioManager.instance.PlaySound(49);

        RectTransform lineRect = manager.uiLineRenderer.rectTransform;

        Vector2 localCenter = rect.anchoredPosition;

        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            lineRect,
            RectTransformUtility.WorldToScreenPoint(null, rect.position),
            null,
            out localPoint
        );

        manager.AddPointInList(id, localPoint);
    }
    
    public void OnPointerUp(PointerEventData eventData)
    {
        if(manager != null)
            manager.CheckModel();
    }

    public void ResetButton()
    {
        if (isFirst)
            return;
        
        animator.enabled = false;
        transform.localScale = startScale;
        GetComponent<Image>().color = baseColor;
    }
}