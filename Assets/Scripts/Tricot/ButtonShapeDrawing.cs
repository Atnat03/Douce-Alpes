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

    public Color baseColor;
    public Color startColor;
    public Color passageColor;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        
        animator = GetComponent<Animator>();
        animator.enabled = false;
        
        startScale = transform.localScale;
    }

    void StartHover()
    {
        if (manager == null) return;
        manager.SetHover(true);
        GetComponent<Image>().color = passageColor;
        SnapLineToButtonCenter();
    }

    public void SetFirstPoint()
    {
        animator.enabled = true;
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

        RectTransform lineRect = manager.uiLineRenderer.rectTransform;

        Vector3 worldCenter = rect.TransformPoint(rect.rect.center);

        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, worldCenter);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            lineRect,
            screenPoint,
            Camera.main,
            out Vector2 localPos
        );

        manager.AddPointInList(id, localPos);
    }

    
    public void OnPointerUp(PointerEventData eventData)
    {
        if(manager != null)
            manager.CheckModel();
    }

    public void ResetButton()
    {
        animator.enabled = false;
        transform.localScale = startScale;
        GetComponent<Image>().color = baseColor;
    }
}