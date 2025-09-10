using UnityEngine;

public class Sheep : TouchableObject
{
    [SerializeField] private string sheepName;

    private bool isHolding = false;

    public override void TouchEvent()
    {
        GameManager.instance.ChangeCameraPos(transform.position);
    }

    public void OnTouchStart()
    {
        isHolding = true;
        GetComponent<MeshRenderer>().material.color = Color.blue;
    }

    public void OnTouchEnd()
    {
        isHolding = false;
        GetComponent<MeshRenderer>().material.color = Color.green;
    }

    private void Update()
    {
        if (isHolding)
        {
            Vector2 screenPos = TouchManager.instance.playerInput.actions["TouchPosition"].ReadValue<Vector2>();
            Ray ray = Camera.main.ScreenPointToRay(screenPos);

            if (Physics.Raycast(ray, out RaycastHit hit, 100f, LayerMask.GetMask("Ground")))
            {
                transform.position = hit.point + Vector3.up * 0.5f;
            }
        }
    }
}