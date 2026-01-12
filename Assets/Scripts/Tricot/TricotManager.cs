using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public enum ModelTricot
{
    Bonnet, Chaussettes, Echarpe, Gants, GiletManche, GiletCourt, Pull
}

public class TricotManager : MonoBehaviour
{
    public static TricotManager instance;
    
    [Header("UI Elements")]
    public GameObject okImage;
    public Image imageProduct;
    public UILineDrawer uiLineRenderer;
    public UILineDrawer modelLineRenderer;
    public RectTransform[] _3x3Ui;
    public GameObject gridParent;
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
    [SerializeField] private GameObject spawnVisual;

    [SerializeField] private RectTransform spawnMoney;
    
    [Header("Pages")]
    [SerializeField] private Transform carnetParent;
    [SerializeField] private GameObject pagePrefab;
    [SerializeField] private ModelListeSO models;
    [SerializeField] private GameObject NotEnougthWool;
    [SerializeField] private GameObject DontBuyWool;
    
    public Dictionary<ModelTricot, bool> ModelPossede = new Dictionary<ModelTricot, bool>();

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        if (okImage != null) okImage.SetActive(false);
        if (uiLineRenderer != null) uiLineRenderer.enabled = false;
        if (modelLineRenderer != null) modelLineRenderer.enabled = false;

        CreateAllCarnet();
        visualTricot.ResetLaine();
    }

    private void CreateAllCarnet()
    {
        int i = 0;
        foreach (ModelDrawSO model in models.listeModel)
        {
            CreatePage(model, i);
            i++;
        }
        
        carnetParent.GetComponent<CarnetTricot>().UpdateCarnet();
    }

    private void CreatePage(ModelDrawSO model, int id)
    {
        GameObject go = Instantiate(pagePrefab, carnetParent);
        go.transform.SetAsFirstSibling();
        TricotPage page = go.GetComponent<TricotPage>();
        page.Initialize(model, this, id);
        
        ModelPossede[(ModelTricot)id] = id == 0;

        page.isBuy = id==0;

        page.buttonSelect.onClick.AddListener(() =>
        {
            if(page.isBuy)
            {
                if (!PlayerMoney.instance.isEnoughtWhool(numberTotalWool(model.pattern)))
                {
                    Debug.Log("Pas assez de laine pour commencer ce modèle !");
                    AudioManager.instance.PlaySound(5);
                    Instantiate(NotEnougthWool, carnetParent);
                    return;
                }

                AudioManager.instance.PlaySound(40);
                InitalizePattern(model);
            }
            else
            {
                AudioManager.instance.PlaySound(5);
                Instantiate(DontBuyWool, carnetParent);
            }
        });
        
        carnetParent.GetComponent<CarnetTricot>().AddNewPage(page.gameObject);
    }

    public void BuyNewPage(int id)
    {
        ModelPossede[(ModelTricot)id] = true;
    }

    public int numberTotalWool(List<ModelDraw> l)
    {
        int total = 0;
        for (int i = 0; i < l.Count; i++)
        {
            total += l[i].neededWool;
        }
        return total;
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
        
        gridParent.SetActive(!carnetParent.gameObject.activeSelf);
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
        }
    }

    public void CheckModel()
    {
        modelLinePoints.Clear();
        
        if (!canShowNext || currentModel >= currentPattern.Count)
        {
            AudioManager.instance.PlaySound(42);
            ResetDrawing();
            return;
        }

        List<int> modelPoints = currentPattern[currentModel].pointsList;
        if (currentPassagePoint.Count != modelPoints.Count)
        {
            AudioManager.instance.PlaySound(42);
            ResetDrawing();
            return;
        }

        for (int j = 0; j < modelPoints.Count; j++)
        {
            if (currentPassagePoint[j] != modelPoints[j])
            {
                AudioManager.instance.PlaySound(42);
                ResetDrawing();
                return;
            }
        }

        visualTricot.AddLaine();
        
        if (okImage != null) okImage.SetActive(true);
        ResetDrawing(true);
        AudioManager.instance.PlaySound(36);
        StartCoroutine(HideOkAndNextModel());
    }

    private void ResetDrawing(bool sucess = false)
    {
        currentPassagePoint.Clear();
        linePoints.Clear();
        
        if (uiLineRenderer != null)
        {
            uiLineRenderer.points = linePoints.ToArray();
            uiLineRenderer.SetVerticesDirty();
            uiLineRenderer.enabled = false;
        }

        foreach (RectTransform b in _3x3Ui)
        {
            b.GetComponent<ButtonShapeDrawing>().ResetButton();
        }

        if (!sucess)
        {
            ApplyPrevisualisationLine();
        }
        else
        {
            foreach (RectTransform b in _3x3Ui)
            {
                b.GetComponent<ButtonShapeDrawing>().isFirst = false;
            }
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
        
        PlayerMoney.instance.RemoveWhool(currentPattern[currentModel].neededWool);
        
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
    
    private Coroutine previewCoroutine;
    
    private void ApplyPrevisualisationLine()
    {
        if(previewCoroutine != null)
            StopCoroutine(previewCoroutine);

        previewCoroutine = StartCoroutine(AnimatePreviewLine());
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

        yield return new WaitForSeconds(0.2f);
    }

    public void InitalizePattern(ModelDrawSO patternSO)
    {
        StartCoroutine(WaitBeforeRemoveCarnet(patternSO));
    }

    IEnumerator WaitBeforeRemoveCarnet(ModelDrawSO patternSO)
    {
        carnetParent.GetComponent<Animator>().SetTrigger("GetOut");
        
        yield return new WaitForSeconds(1f);
        
        currentPattern = patternSO.pattern;
        numberModelOfThisPattern = currentPattern.Count;
        currentModel = 0;
        currentPassagePoint.Clear();
        linePoints.Clear();
        targetFill = 0f;
        currentPriceSell = patternSO.sellPrice;
        
        GameObject meshVisual = Instantiate(patternSO.prefabVisual, spawnVisual.transform);

        if (imageProduct != null)
        {
            imageProduct.sprite = patternSO.image;
            imageProduct.fillAmount = 0f;
        }
        
        sellButton.gameObject.SetActive(false);
        
        ResetDrawing(true);
        
        visualTricot.Initialise(meshVisual.GetComponent<MeshRenderer>(), patternSO.pattern.Count);

        if (currentPattern.Count > 0)
        {
            StartCoroutine(ShowModelWithDelay(currentPattern[currentModel]));
            ApplyPrevisualisationLine();
        }
        
        carnetParent.gameObject.SetActive(false);
    }

    private IEnumerator ShowModelWithDelay(ModelDraw model)
    {
        canShowNext = false;
        yield return new WaitForSeconds(0.5f);
        ResetDrawing(true);
        canShowNext = true;
        Debug.Log("Nouveau modèle : " + string.Join(",", model.pointsList));
    }

    public void SellProduct()
    {
        print(currentPriceSell);
        
        PlayerMoney.instance.AddMoney(currentPriceSell, spawnMoney.position);
        AudioManager.instance.PlaySound(41);
        
        currentPattern = null;
        numberModelOfThisPattern = 0;
        currentModel = 0;
        currentPassagePoint.Clear();
        linePoints.Clear();
        targetFill = 0f;
        currentPriceSell = 0;

        visualTricot.ResetLaine();
        
        Destroy(spawnVisual.transform.GetChild(0).gameObject);
        
        sellButton.gameObject.SetActive(false);
        carnetParent.gameObject.SetActive(true);
    }
    
    public TricotSaveData GetCurrentTricotSaveData()
    {

        int patternIndex = -1;
        for (int i = 0; i < models.listeModel.Count; i++)
        {
            if (models.listeModel[i].pattern == currentPattern) 
            {
                patternIndex = i;
                break;
            }
        }

        TricotSaveData data = new TricotSaveData
        {
            currentPatternIndex = patternIndex,
            currentModelStep = currentModel,
        };

        if (visualTricot != null)
        {
            data.visualValueVertical   = visualTricot.value_Vertical;
            data.visualValueHorizontal = visualTricot.value_Horizontal;
            data.visualGradientDroite  = visualTricot.gradient_Droite;
        }

        return data;
    }


    public void LoadTricotState(TricotSaveData data)
    {
        if (data == null || data.currentPatternIndex < 0)
        {
            return;
        }

        ModelDrawSO patternSO = models.listeModel[data.currentPatternIndex];

        currentPattern = patternSO.pattern;
        numberModelOfThisPattern = currentPattern.Count;
        currentModel = Mathf.Clamp(data.currentModelStep, 0, numberModelOfThisPattern);

        if (spawnVisual.transform.childCount > 0)
            Destroy(spawnVisual.transform.GetChild(0).gameObject);

        GameObject meshVisual = Instantiate(patternSO.prefabVisual, spawnVisual.transform);
        
        if (imageProduct != null)
        {
            imageProduct.sprite = patternSO.image;
            imageProduct.fillAmount = (float)currentModel / numberModelOfThisPattern;
        }

        if (visualTricot != null)
        {
            visualTricot.Initialise(meshVisual.GetComponent<MeshRenderer>(), patternSO.pattern.Count);

            visualTricot.value_Vertical   = data.visualValueVertical;
            visualTricot.value_Horizontal = data.visualValueHorizontal;
            visualTricot.gradient_Droite  = data.visualGradientDroite;
        }

        sellButton.gameObject.SetActive(currentModel >= numberModelOfThisPattern);

        carnetParent.gameObject.SetActive(false);
    }
}

[Serializable]
public class TricotSaveData
{
    public int currentPatternIndex = -1;        
    public int currentModelStep = 0;           

    public float visualValueVertical = 1f;
    public float visualValueHorizontal = 0f;
    public bool visualGradientDroite = false;
}
