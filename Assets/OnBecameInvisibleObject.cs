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
        if (uiActiveByClick)
        {
            Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
            bool isVisible = GeometryUtility.TestPlanesAABB(planes, rend.bounds);

            if (!isVisible)
            {
                if(build != null && build.UI != null)
                    build.CloseUI();

                uiActiveByClick = false;
            }
        }
    }

    
    public void ActivateUI()
    {
        if (build != null && build.UI != null)
            build.OpenUI();

        uiActiveByClick = true;
    }
}
