using UnityEngine;
using DanielLochner.Assets.SimpleScrollSnap;

public class SimpleScrollSnapBridge : MonoBehaviour
{
    [SerializeField] public SimpleScrollSnap scrollSnap;

    public SimpleScrollSnap ScrollSnap => scrollSnap;

    public void AddExistingPanel(GameObject panel)
    {
        if (scrollSnap != null)
        {
            panel.transform.SetParent(scrollSnap.Content, false);

            scrollSnap.Setup();
        }
    }
}