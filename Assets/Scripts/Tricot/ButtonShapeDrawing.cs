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

        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(manager.uiLineRenderer.rectTransform, rect.position, null, out localPos);
        manager.AddPointInList(id, localPos);
    }

    public void SetFirstPoint()
    {
        animator.enabled = true;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        StartHover();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(manager.uiLineRenderer.rectTransform, rect.position, null, out localPos);
        manager.AddPointInList(id, localPos);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if(manager != null)
            manager.CheckModel();
        
        animator.enabled = false;
        transform.localScale = startScale;
    }
}