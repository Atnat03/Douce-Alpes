using UnityEngine;

public class TutoPointerArrow : MonoBehaviour
{
    public Transform targetPos;
    public RectTransform pointerRectTransform;

    public float borderSize = 400f;
    public float angleOffset = 90f; 
    public float yOffset = 20f;     

    private void Update()
    {
        Vector3 targetScreenPos = Camera.main.WorldToScreenPoint(targetPos.position);

        if (targetScreenPos.z < 0)
        {
            targetScreenPos *= -1f;
        }

        Vector3 screenCenter = new Vector3(Screen.width, Screen.height, 0) / 2f;

        Vector3 dir = (targetScreenPos - screenCenter).normalized;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + angleOffset;
        pointerRectTransform.rotation = Quaternion.Euler(0, 0, angle);

        Vector3 pointerPos = targetScreenPos;
        pointerPos.x = Mathf.Clamp(pointerPos.x, borderSize, Screen.width - borderSize);
        pointerPos.y = Mathf.Clamp(pointerPos.y, borderSize, Screen.height - borderSize);

        pointerPos.y += yOffset;

        pointerRectTransform.position = pointerPos;
    }
}