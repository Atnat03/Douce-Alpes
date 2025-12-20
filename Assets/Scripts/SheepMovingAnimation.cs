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
        Vector2 randomCircle = Random.insideUnitCircle * raduisCircle;
        return new Vector3(
            randomCircle.x,
            transform.position.y,
            randomCircle.y
        );
    }
}