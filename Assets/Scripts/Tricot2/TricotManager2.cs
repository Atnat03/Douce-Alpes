using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TricotManager2 : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject okImage;
    public Image imageProduct;
    public Text testTxt;
    public UILineDrawer uiLineRenderer;

    [Header("Settings")]
    public float fillSpeed = 2f;

    [HideInInspector] public bool isHover;

    private List<int> currentPassagePoint = new List<int>();
    private List<ModelDraw> currentPattern = new List<ModelDraw>();
    private int currentModel = 0;
    private int numberModelOfThisPattern = 0;
    private bool canShowNext = true;
    private float targetFill = 0f;

    [HideInInspector] public List<Vector2> linePoints = new List<Vector2>();

    private void Start()
    {
        if (okImage != null) okImage.SetActive(false);
        if (uiLineRenderer != null) uiLineRenderer.enabled = false;
    }

    private void Update()
    {
        // Si on est en train de hover, mettre à jour le dernier point de la ligne vers le doigt
        if (isHover && uiLineRenderer != null && TouchManager.instance != null && linePoints.Count > 0)
        {
            Vector2 screenPos = TouchManager.instance.PrimaryPosition();
            Vector2 localPoint;
            RectTransform canvasRect = uiLineRenderer.rectTransform;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPos, null, out localPoint);

            // Mettre à jour le dernier point pour suivre le doigt
            if (linePoints.Count >= 2)
                linePoints[linePoints.Count - 1] = localPoint;
            else
                linePoints.Add(localPoint);

            uiLineRenderer.points = linePoints.ToArray();
            uiLineRenderer.SetVerticesDirty();
        }

        // Fill de l'image produit
        if (imageProduct.fillAmount < targetFill)
        {
            imageProduct.fillAmount += Time.deltaTime * fillSpeed;
            if (imageProduct.fillAmount > targetFill)
                imageProduct.fillAmount = targetFill;
        }
    }

    public void SetHover(bool hover)
    {
        isHover = hover;

        // Activer/désactiver la ligne
        if (uiLineRenderer != null)
            uiLineRenderer.enabled = hover;

        // Si on commence un hover, ajouter un point temporaire pour suivre le doigt
        if (hover && linePoints.Count == 0 && TouchManager.instance != null)
        {
            Vector2 screenPos = TouchManager.instance.PrimaryPosition();
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(uiLineRenderer.rectTransform, screenPos, null, out localPoint);
            linePoints.Add(localPoint);
            uiLineRenderer.points = linePoints.ToArray();
            uiLineRenderer.SetVerticesDirty();
        }
    }

    public void AddPointInList(int id, Vector2 buttonLocalPos)
    {
        if (!isHover) return;

        // Ajouter seulement si le point n'est pas déjà le dernier
        if (linePoints.Count > 0 && Vector2.Distance(linePoints[linePoints.Count - 1], buttonLocalPos) < 1f)
            return;

        // Ajouter le point bouton
        linePoints.Add(buttonLocalPos);
        uiLineRenderer.points = linePoints.ToArray();
        uiLineRenderer.SetVerticesDirty();

        // Ajouter à la logique du motif
        if (!currentPassagePoint.Contains(id))
        {
            currentPassagePoint.Add(id);
            if (testTxt != null)
                testTxt.text += id + " / ";
        }
    }

    public void CheckModel()
    {
        if (!canShowNext || currentModel >= currentPattern.Count)
        {
            currentPassagePoint.Clear();
            linePoints.Clear();
            if (uiLineRenderer != null)
            {
                uiLineRenderer.points = linePoints.ToArray();
                uiLineRenderer.SetVerticesDirty();
                uiLineRenderer.enabled = false;
            }
            return;
        }

        List<int> modelPoints = currentPattern[currentModel].pointsList;
        if (currentPassagePoint.Count != modelPoints.Count)
        {
            currentPassagePoint.Clear();
            linePoints.Clear();
            if (uiLineRenderer != null)
            {
                uiLineRenderer.points = linePoints.ToArray();
                uiLineRenderer.SetVerticesDirty();
                uiLineRenderer.enabled = false;
            }
            return;
        }

        for (int j = 0; j < modelPoints.Count; j++)
        {
            if (currentPassagePoint[j] != modelPoints[j])
            {
                currentPassagePoint.Clear();
                linePoints.Clear();
                if (uiLineRenderer != null)
                {
                    uiLineRenderer.points = linePoints.ToArray();
                    uiLineRenderer.SetVerticesDirty();
                    uiLineRenderer.enabled = false;
                }
                return;
            }
        }

        // Motif correct
        if (okImage != null) okImage.SetActive(true);
        currentPassagePoint.Clear();
        linePoints.Clear();
        if (uiLineRenderer != null)
        {
            uiLineRenderer.points = linePoints.ToArray();
            uiLineRenderer.SetVerticesDirty();
            uiLineRenderer.enabled = false;
        }

        StartCoroutine(HideOkAndNextModel());
    }

    private IEnumerator HideOkAndNextModel()
    {
        yield return new WaitForSeconds(0.5f);
        if (okImage != null) okImage.SetActive(false);
        NextModel();
    }

    private void NextModel()
    {
        currentModel++;
        targetFill = (float)currentModel / numberModelOfThisPattern;

        if (currentModel < numberModelOfThisPattern)
        {
            StartCoroutine(ShowModelWithDelay(currentPattern[currentModel]));
        }
        else
        {
            Debug.Log("Pattern terminé !");
        }
    }

    public void InitalizePattern(ModelDrawSO patternSO)
    {
        currentPattern = patternSO.pattern;
        numberModelOfThisPattern = currentPattern.Count;
        currentModel = 0;
        currentPassagePoint.Clear();
        linePoints.Clear();
        targetFill = 0f;
        if (imageProduct != null) imageProduct.sprite = patternSO.image;
        if (imageProduct != null) imageProduct.fillAmount = 0f;
        if (uiLineRenderer != null)
        {
            uiLineRenderer.points = linePoints.ToArray();
            uiLineRenderer.SetVerticesDirty();
            uiLineRenderer.enabled = false;
        }

        if (currentPattern.Count > 0)
            StartCoroutine(ShowModelWithDelay(currentPattern[currentModel]));
    }

    private IEnumerator ShowModelWithDelay(ModelDraw model)
    {
        canShowNext = false;
        yield return new WaitForSeconds(0.5f);
        currentPassagePoint.Clear();
        linePoints.Clear();
        if (uiLineRenderer != null)
        {
            uiLineRenderer.points = linePoints.ToArray();
            uiLineRenderer.SetVerticesDirty();
            uiLineRenderer.enabled = false;
        }
        canShowNext = true;
        Debug.Log("Nouveau modèle : " + string.Join(",", model.pointsList));
    }
}
