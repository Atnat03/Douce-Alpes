using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TricotManager : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject okImage;
    public Image imageProduct;
    public Text testTxt;
    public UILineDrawer uiLineRenderer;
    public UILineDrawer modelLineRenderer;
    public RectTransform[] _3x3Ui;
    public Button sellButton;
    
    [Header("Settings")]
    public float fillSpeed = 2f;

    [HideInInspector] public bool isHover;

    private List<int> currentPassagePoint = new List<int>();
    private List<ModelDraw> currentPattern = new List<ModelDraw>();
    private int currentModel = 0;
    private int currentPriceSell = 0;
    private int numberModelOfThisPattern = 0;
    private bool canShowNext = true;
    private float targetFill = 0f;

    [HideInInspector] public List<Vector2> linePoints = new List<Vector2>();
    [HideInInspector] public List<Vector2> modelLinePoints = new List<Vector2>();
    
    [SerializeField] VisualTricot visualTricot;

    private void Start()
    {
        if (okImage != null) okImage.SetActive(false);
        if (uiLineRenderer != null) uiLineRenderer.enabled = false;
        if (modelLineRenderer != null) modelLineRenderer.enabled = false;
    }

    private void Update()
    {
        if (isHover && uiLineRenderer != null && TouchManager.instance != null && linePoints.Count > 0)
        {
            Vector2 screenPos = TouchManager.instance.PrimaryPosition();
            Vector2 localPoint;
            RectTransform canvasRect = uiLineRenderer.rectTransform;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPos, null, out localPoint);

            if (linePoints.Count >= 2)
                linePoints[linePoints.Count - 1] = localPoint;
            else
                linePoints.Add(localPoint);

            uiLineRenderer.points = linePoints.ToArray();
            uiLineRenderer.SetVerticesDirty();
        }
/*
        if (imageProduct.fillAmount < targetFill)
        {
            imageProduct.fillAmount += Time.deltaTime * fillSpeed;
            if (imageProduct.fillAmount > targetFill)
                imageProduct.fillAmount = targetFill;
        }*/
        
        
    }

    public void SetHover(bool hover)
    {
        isHover = hover;

        if (uiLineRenderer != null)
            uiLineRenderer.enabled = hover;

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

        if (linePoints.Count > 0 && Vector2.Distance(linePoints[linePoints.Count - 1], buttonLocalPos) < 1f)
            return;

        linePoints.Add(buttonLocalPos);
        uiLineRenderer.points = linePoints.ToArray();
        uiLineRenderer.SetVerticesDirty();

        if (!currentPassagePoint.Contains(id))
        {
            currentPassagePoint.Add(id);
            if (testTxt != null)
                testTxt.text += id + " / ";
        }
    }

    public void CheckModel()
    {
        testTxt.text = "";
        modelLinePoints.Clear();
        
        if (!canShowNext || currentModel >= currentPattern.Count)
        {
            ResetDrawing();
            return;
        }

        List<int> modelPoints = currentPattern[currentModel].pointsList;
        if (currentPassagePoint.Count != modelPoints.Count)
        {
            ResetDrawing();
            return;
        }

        for (int j = 0; j < modelPoints.Count; j++)
        {
            if (currentPassagePoint[j] != modelPoints[j])
            {
                ResetDrawing();
                return;
            }
        }

        // Motif correct
        
        visualTricot.AddLaine(numberModelOfThisPattern);
        
        if (okImage != null) okImage.SetActive(true);
        ResetDrawing();
        StartCoroutine(HideOkAndNextModel());
    }

    private void ResetDrawing()
    {
        currentPassagePoint.Clear();
        linePoints.Clear();
        if (uiLineRenderer != null)
        {
            uiLineRenderer.points = linePoints.ToArray();
            uiLineRenderer.SetVerticesDirty();
            uiLineRenderer.enabled = false;
        }
    }

    private IEnumerator HideOkAndNextModel()
    {
        yield return new WaitForSeconds(0.5f);
        if (okImage != null) okImage.SetActive(false);
        NextModel();
    }

    private void NextModel()
    {
        if (!PlayerMoney.instance.isEnoughtWhool(currentPattern[currentModel].neededWool))
        {
            Debug.LogError("Pas assez de laine pour completer le produit");
            return;
        }
        
        PlayerMoney.instance.AddWhool(-currentPattern[currentModel].neededWool);
        
        currentModel++;
        targetFill = (float)currentModel / numberModelOfThisPattern;

        if (currentModel < numberModelOfThisPattern)
        {
            StartCoroutine(ShowModelWithDelay(currentPattern[currentModel]));
            ApplyPrevisualisationLine();
        }
        else
        {
            Debug.Log("Pattern terminé !");         
            if (modelLineRenderer != null)
            {
                modelLineRenderer.points = new Vector2[0];
                modelLineRenderer.SetVerticesDirty();
                modelLineRenderer.enabled = false;
            }
            
            if (sellButton != null)
            {
                sellButton.gameObject.SetActive(true);
            }
        }
    }
    
    private void ApplyPrevisualisationLine()
    {
        //StopAllCoroutines();
        StartCoroutine(AnimatePreviewLine());
    }

    private IEnumerator AnimatePreviewLine()
    {
        if (modelLineRenderer == null || _3x3Ui == null || currentPattern.Count == 0)
            yield break;

        modelLineRenderer.enabled = true;
        modelLinePoints.Clear();
        modelLineRenderer.points = modelLinePoints.ToArray();
        modelLineRenderer.SetVerticesDirty();

        List<int> modelPoints = currentPattern[currentModel].pointsList;
        RectTransform lineRect = modelLineRenderer.rectTransform;
        
        int firstPointId = modelPoints[0];
        foreach (RectTransform t in _3x3Ui)
        {
            ButtonShapeDrawing button = t.GetComponent<ButtonShapeDrawing>();
            if (button != null && button.id == firstPointId)
            {
                button.SetFirstPoint();
                break;
            }
        }

        for (int i = 0; i < modelPoints.Count; i++)
        {
            foreach (RectTransform t in _3x3Ui)
            {
                ButtonShapeDrawing button = t.GetComponent<ButtonShapeDrawing>();
                
                if (button != null && button.id == modelPoints[i])
                {
                    Vector2 worldPos = t.TransformPoint(Vector3.zero);
                    Vector2 localPoint;
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(
                        lineRect,
                        RectTransformUtility.WorldToScreenPoint(null, worldPos),
                        null,
                        out localPoint
                    );

                    modelLinePoints.Add(localPoint);
                    modelLineRenderer.points = modelLinePoints.ToArray();
                    modelLineRenderer.SetVerticesDirty();

                    yield return new WaitForSeconds(0.2f);
                    break;
                }
            }
        }

        // Petit délai à la fin du tracé (optionnel)
        yield return new WaitForSeconds(0.2f);
    }

    public void InitalizePattern(ModelDrawSO patternSO)
    {
        currentPattern = patternSO.pattern;
        numberModelOfThisPattern = currentPattern.Count;
        currentModel = 0;
        currentPassagePoint.Clear();
        linePoints.Clear();
        targetFill = 0f;
        currentPriceSell = patternSO.sellPrice;

        if (imageProduct != null)
        {
            imageProduct.sprite = patternSO.image;
            imageProduct.fillAmount = 0f;
        }
        
        sellButton.gameObject.SetActive(false);
        
        ResetDrawing();

        if (currentPattern.Count > 0)
        {
            StartCoroutine(ShowModelWithDelay(currentPattern[currentModel]));
            ApplyPrevisualisationLine();
        }
    }

    private IEnumerator ShowModelWithDelay(ModelDraw model)
    {
        canShowNext = false;
        yield return new WaitForSeconds(0.5f);
        ResetDrawing();
        canShowNext = true;
        Debug.Log("Nouveau modèle : " + string.Join(",", model.pointsList));
    }

    public void SellProduct()
    {
        PlayerMoney.instance.AddMoney(currentPriceSell);
        
        currentPattern = null;
        numberModelOfThisPattern = 0;
        currentModel = 0;
        currentPassagePoint.Clear();
        linePoints.Clear();
        targetFill = 0f;
        currentPriceSell = 0;

        if (imageProduct != null)
        {
            imageProduct.sprite = null;
            imageProduct.fillAmount = 0f;
        }
        
        sellButton.gameObject.SetActive(false);
    }
}
