using System;
using System.Collections;
using UnityEngine;

public class VisualTricot : MonoBehaviour
{
    [SerializeField] private bool gradient_Droite = false;
    [SerializeField] private float value_Vertical = 0;
    [SerializeField] private float value_Horizontal = 0;
    [SerializeField] private float maxNumberHorizontal = 5;
    private float maxNumberVertical;
    [SerializeField] private Renderer obejctVisual;
    private Material shader;
    [SerializeField] float duree = 2f;
    private float valueVerticalTemp;
    
    [Header("Spline")]
    private Vector2 positionMin;
    [SerializeField] private Vector2 positionMax;
    [SerializeField] private GameObject pointAccroche;
    [SerializeField] private SplineMaison splineLaine;

    private void Start()
    {
        shader = obejctVisual.material;
        positionMin = obejctVisual.transform.position;
    }

    [ContextMenu("AddLaine")]
    public void AddLaine(float c)
    {
        StartCoroutine(SmoothAddedLaine(c));
    }

    IEnumerator SmoothAddedLaine(float c)
    {
        maxNumberVertical = c;

        if (value_Vertical >= maxNumberVertical)
        {
            Debug.Log("Fin du tricot");
            yield break;
        }
        
        float depart = value_Horizontal;
        float cible = gradient_Droite ? 0 : maxNumberHorizontal;
        float temps = 0f;

        while (temps < duree)
        {
            value_Horizontal = Mathf.Lerp(depart, cible, temps / duree);
            temps += Time.deltaTime;
            yield return null;
        }
        
        value_Horizontal = cible;
        value_Vertical++;
        
        gradient_Droite = !gradient_Droite;
    }


    [ContextMenu("ResetLaine")]
    public void ResetLaine()
    {
        value_Horizontal = 0;
        value_Vertical = 0;
        gradient_Droite = true;

        pointAccroche.transform.position = splineLaine.points[0];
        splineLaine.SetDernierPoint(pointAccroche.transform.position);
    }

    void Update()
    {
        shader.SetInt("_Gradient_droite", gradient_Droite ? 1 : 0);
        shader.SetFloat("_ValueVertical", value_Vertical);
        shader.SetFloat("_ValueHorizontal", value_Horizontal);
        shader.SetFloat("_MaxVertical", maxNumberVertical);
        shader.SetFloat("_MaxHorizontal", maxNumberHorizontal);

        float normH = maxNumberHorizontal > 0 ? value_Horizontal / maxNumberHorizontal : 0f;
        float normV = maxNumberVertical > 0 ? value_Vertical / maxNumberVertical : 0f;

        normH = Mathf.Clamp01(normH);
        normV = Mathf.Clamp01(normV);

        Vector3 minPos = positionMin;
        Vector3 maxPos = new Vector3(positionMax.x, positionMax.y, minPos.z);

        Vector3 cible = new Vector3(
            Mathf.Lerp(minPos.x, maxPos.x, normH),
            Mathf.Lerp(minPos.y, maxPos.y, normV),
            pointAccroche.transform.position.z
        );

        pointAccroche.transform.position = cible;
        
        splineLaine.SetDernierPoint(cible);
    }
}
