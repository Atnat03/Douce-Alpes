using UnityEngine;

public class OnBecameInvisibleObject : MonoBehaviour
{
    public Build build;
    private Renderer rend;

    private bool uiActiveByClick = false;

    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    void Update()
    {
        if (uiActiveByClick) // On ne checke le frustum que si l'UI a été activée
        {
            Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
            bool isVisible = GeometryUtility.TestPlanesAABB(planes, rend.bounds);

            if (!isVisible)
            {
                Debug.Log($"{gameObject.name} est hors champ !");
                if(build != null && build.UI != null)
                    build.UI.SetActive(false);

                uiActiveByClick = false; // Désactive le suivi jusqu'à un nouveau clic
            }
        }
    }

    
    public void ActivateUI()
    {
        if (build != null && build.UI != null)
            build.UI.SetActive(true);

        uiActiveByClick = true;
    }
}
