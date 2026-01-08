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
        SnapLineToButtonCenter();
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
        SnapLineToButtonCenter();
    }


    private void SnapLineToButtonCenter()
    {
        if (manager == null || manager.uiLineRenderer == null) return;

        RectTransform lineRect = manager.uiLineRenderer.rectTransform;

        // Obtenir le centre exact du bouton en world space
        Vector3 worldCenter = rect.position;

        // Convertir ce point en coordonnées locales par rapport au UILineDrawer
        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            lineRect,
            RectTransformUtility.WorldToScreenPoint(null, worldCenter),
            null,  // si ton Canvas a un RenderMode ScreenSpaceOverlay, sinon mettre la caméra
            out localPos
        );

        // Ajouter ce point dans la liste de la ligne
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