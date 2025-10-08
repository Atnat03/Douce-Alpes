using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TricotManager2 : MonoBehaviour
{
    public List<int> currentPassagePoint = new List<int>();
    private List<ModelDraw> currentPattern = new List<ModelDraw>();
    [SerializeField] private GameObject okImage;
    [SerializeField] private Image imageProduct;
    [SerializeField] private float fillSpeed = 2f;

    // Ligne UI
    [SerializeField] private UILineDrawer uiLineRenderer;
    private List<Vector2> linePoints = new List<Vector2>();

    private int numberModelOfThisPattern = 0;
    private int currentModel = 0;
    private bool canShowNext = true;
    private float targetFill = 0f;

    public bool isHover;
    public Text testTxt;

    private void Start()
    {
        okImage.SetActive(false);
        linePoints.Clear();
        if(uiLineRenderer != null)
            uiLineRenderer.points = linePoints.ToArray();
    }

    private void Update()
    {
        uiLineRenderer.enabled = linePoints.Count > 1;
        
        // Mise à jour fill
        if (imageProduct.fillAmount < targetFill)
        {
            imageProduct.fillAmount += Time.deltaTime * fillSpeed;
            if (imageProduct.fillAmount > targetFill)
                imageProduct.fillAmount = targetFill;
        }

        // Suivi du doigt pour la ligne
        if (isHover && uiLineRenderer != null && TouchManager.instance != null)
        {
            Vector2 screenPos = TouchManager.instance.PrimaryPosition();
            Vector2 localPoint;
            RectTransform canvasRect = uiLineRenderer.rectTransform;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPos, null, out localPoint);

            if(linePoints.Count == 0 || Vector2.Distance(localPoint, linePoints[linePoints.Count-1]) > .5f)
            {
                linePoints.Add(localPoint);
                uiLineRenderer.points = linePoints.ToArray();
                uiLineRenderer.SetVerticesDirty();
            }
        }
    }

    public void SetHover(bool hover)
    {
        isHover = hover;
        if (!hover)
        {
            linePoints.Clear();
            if(uiLineRenderer != null)
            {
                uiLineRenderer.points = linePoints.ToArray();
                uiLineRenderer.SetVerticesDirty();
            }
        }
    }

    public void AddPointInList(int i, Vector2 buttonLocalPos)
    {
        if (!isHover || currentPassagePoint.Contains(i)) return;
        
        if(linePoints.Count > 0 && Vector2.Distance(linePoints[linePoints.Count-1], buttonLocalPos) < 1f)
            return; // ne pas ajouter le même point

        currentPassagePoint.Add(i);
        linePoints.Add(buttonLocalPos);
        if(uiLineRenderer != null)
        {
            uiLineRenderer.points = linePoints.ToArray();
            uiLineRenderer.SetVerticesDirty();
        }

        if (testTxt != null)
            testTxt.text += i + " / ";
    }

    // --- Motifs / modèles ---
    public void InitalizePattern(ModelDrawSO patternSO)
    {
        currentPattern = patternSO.pattern;
        numberModelOfThisPattern = currentPattern.Count;
        currentModel = 0;
        currentPassagePoint.Clear();
        targetFill = 0f;
        imageProduct.sprite = patternSO.image;
        imageProduct.fillAmount = 0f;

        if (currentPattern.Count > 0)
            StartCoroutine(ShowModelWithDelay(currentPattern[currentModel]));
    }

    public void CheckModel()
    {
        testTxt.text = "";

        if (!canShowNext || currentModel >= currentPattern.Count)
        {
            currentPassagePoint.Clear();
            return;
        }

        List<int> modelPoints = currentPattern[currentModel].pointsList;

        if (currentPassagePoint.Count != modelPoints.Count)
        {
            currentPassagePoint.Clear();
            return;
        }

        for (int j = 0; j < modelPoints.Count; j++)
        {
            if (currentPassagePoint[j] != modelPoints[j])
            {
                currentPassagePoint.Clear();
                return;
            }
        }

        okImage.SetActive(true);
        currentPassagePoint.Clear();
        StartCoroutine(HideOkAndNextModel());
    }

    private IEnumerator HideOkAndNextModel()
    {
        yield return new WaitForSeconds(0.5f);
        okImage.SetActive(false);
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

    private IEnumerator ShowModelWithDelay(ModelDraw model)
    {
        canShowNext = false;
        yield return new WaitForSeconds(0.5f);

        Debug.Log("Nouveau modèle à suivre : " + string.Join(",", model.pointsList));

        currentPassagePoint.Clear();
        canShowNext = true;
    }
}
