using System;
using UnityEngine;
using UnityEngine.UI;

public class DesactivateWhenTraveling : MonoBehaviour
{
    public ChangingCamera camera;

    private void Update()
    {
        GetComponent<Button>().interactable = !camera.isInTransition;
    }
}
