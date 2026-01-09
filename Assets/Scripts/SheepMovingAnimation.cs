using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class SheepMovingAnimation : TouchableObject
{
    public float speedMove;
    public float speedRotation;
    bool hasNewDestination = false;
    Vector3 destination;
    public float raduisCircle = 2f;

    public Transform center;
    
    public Animator animator;

    private void Start()
    {
        animator.SetBool("Walk", true);
    }

    private void Update()
    {
        if (!hasNewDestination)
        {
            destination = NewPosition();
            hasNewDestination = true;
            return;
        }

        Vector3 direction = destination - transform.position;
        direction.y = 0;
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            speedRotation * Time.deltaTime
        );

        float angle = Quaternion.Angle(transform.rotation, targetRotation);
        
        if (angle < 5f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                destination,
                speedMove * Time.deltaTime
            );
        }

        if (Vector3.Distance(transform.position, destination) < 0.1f)
        {
            hasNewDestination = false;
        }
    }

    Vector3 NewPosition()
    {
        float angle = Random.Range(0f, 2f * Mathf.PI);

        float x = center.position.x + Mathf.Cos(angle) * raduisCircle;
        float z = center.position.z + Mathf.Sin(angle) * raduisCircle;

        return new Vector3(x, transform.position.y, z);
    }

}