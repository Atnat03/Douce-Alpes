using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonShapeDrawing : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerUpHandler
{
    public int id;
    [SerializeField] private TricotManager2 manager;
    private RectTransform rect;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    void StartHover()
    {
        if (manager == null) return;
        manager.SetHover(true);

        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(manager.uiLineRenderer.rectTransform, rect.position, null, out localPos);
        manager.AddPointInList(id, localPos);
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
    }
}