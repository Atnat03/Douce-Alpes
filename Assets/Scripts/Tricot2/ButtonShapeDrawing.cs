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
        if (manager == null)
            manager = FindObjectOfType<TricotManager2>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        manager.SetHover(true);
        AddPointToLine();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        AddPointToLine();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        manager.SetHover(false);
        manager.CheckModel();
    }

    private void AddPointToLine()
    {
        if (manager == null) return;

        Vector2 localPoint = rect.anchoredPosition;
        manager.AddPointInList(id, localPoint);
    }
}