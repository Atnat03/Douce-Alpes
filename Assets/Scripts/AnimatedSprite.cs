using UnityEngine;

public class AnimatedSprite : MonoBehaviour
{
    public Vector2 targetPosition;
    float speed = 1200f;
    
    void Update()
    {
        if (Vector2.Distance(transform.position, targetPosition) > 0.1f)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, Time.deltaTime * speed);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
