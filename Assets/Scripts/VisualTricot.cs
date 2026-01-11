using System;
using System.Collections;
using UnityEngine;

public class VisualTricot : MonoBehaviour
{
    [SerializeField] public bool gradient_Droite = false;
    [SerializeField] public float value_Vertical = 0;
    [SerializeField] public float value_Horizontal = 0;
    [SerializeField] public float maxNumberHorizontal = 5;
    public float maxNumberVertical;
    [SerializeField] private Renderer obejctVisual;
    private Material shader = null;
    [SerializeField] float duree = 2f;
    
    [Header("Spline")]
    private Vector2 positionMin;
    [SerializeField] private Vector2 positionMax;
    [SerializeField] private GameObject pointAccroche;
    [SerializeField] private SplineMaison splineLaine;
    
    [SerializeField] private ParticleSystem particle;
    private bool particlePlayed = false;
    
    public void Initialise(MeshRenderer mesh, int numberVertical)
    {
        shader = new Material(mesh.sharedMaterial);
        mesh.material = shader;
        
        positionMin = mesh.transform.localPosition;
        maxNumberVertical = shader.GetFloat("_MaxVertical")+1;
        maxNumberHorizontal = shader.GetFloat("_MaxHorizontal");
    }

    [ContextMenu("AddLaine")]
    public void AddLaine()
    {
        StartCoroutine(SmoothAddedLaine());
    }

    IEnumerator SmoothAddedLaine()
    { 
        particlePlayed = false;
        
        if (value_Vertical >= maxNumberVertical)
        {
            Debug.Log("Fin du tricot");
            yield break;
        }

        float depart = value_Horizontal;
        float cible = gradient_Droite ? 0 : maxNumberHorizontal;
        float temps = 0f;

        float pitch = Mathf.Lerp(0.8f, 1.2f, value_Vertical / maxNumberHorizontal);
        
        AudioManager.instance.PlaySound(37, pitch);

        while (temps < duree)
        {
            value_Horizontal = Mathf.Lerp(depart, cible, temps / duree);
            temps += Time.deltaTime;
            yield return null;
        }

        value_Horizontal = cible;

        if (value_Vertical < maxNumberVertical)
        {
            value_Vertical++;
            gradient_Droite = !gradient_Droite;
        }
        else
        {
            value_Horizontal = maxNumberHorizontal;
            Debug.Log("Tricot terminé !");
        }
    }


    [ContextMenu("ResetLaine")]
    public void ResetLaine()
    {
        StopAllCoroutines();

        value_Vertical = 1;
        value_Horizontal = 0;
        gradient_Droite = false;

        pointAccroche.transform.position = splineLaine.points[0];
        splineLaine.SetDernierPoint(pointAccroche.transform.position);
    }


    void Update()
    {
        if (shader == null)
            return;
        
        shader.SetInt("_Gradient_droite", gradient_Droite ? 1 : 0);
        shader.SetFloat("_ValueVertical", value_Vertical);
        shader.SetFloat("_ValueHorizontal", value_Horizontal);

        float normH = maxNumberHorizontal > 0 ? value_Horizontal / maxNumberHorizontal : 0f;
        float normV = maxNumberVertical > 0 ? value_Vertical / maxNumberVertical : 0f;

        normH = Mathf.Clamp01(normH);
        normV = Mathf.Clamp01(normV);

        Vector3 minPos = positionMin;
        Vector3 maxPos = new Vector3(positionMax.x, positionMax.y, minPos.z);

        Vector3 cibleLocal = new Vector3(
            Mathf.Lerp(minPos.x, maxPos.x, normH),
            Mathf.Lerp(minPos.y, maxPos.y, normV),
            minPos.z
        );

        pointAccroche.transform.localPosition = cibleLocal;
        splineLaine.SetDernierPoint(pointAccroche.transform.position);
        
        Vector3 secondPoint = splineLaine.points.Count > 1 ? splineLaine.points[1] : splineLaine.points[0];

        if (!particlePlayed)
        {
            particle.transform.position = secondPoint;
            particle.Play();
            particlePlayed = true;
        }
        
        particle.transform.position = pointAccroche.transform.position;
    }
}
