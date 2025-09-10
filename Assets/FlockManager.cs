using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class FlockManager : MonoBehaviour
{
    public struct Boid
    {
        public Vector3 position;
        public Vector3 direction;
        public float noise_offset;

        public Boid(Vector3 pos, Vector3 dir, float offset)
        {
            position = pos;
            direction = dir;
            noise_offset = offset;
        }
    }
    
    public int numOfBoids = 10;
    public float boidSpeed = 2f;
    public float boidSpeedVariation = 0.5f;
    public float rotationSpeed = 2f;
    public float neighbourDistance = 3f;
    public float separationDistance = 1f;
    public float spawnRadius = 5f;
    
    public Transform target;
    public GameObject[] sheepObjects;
    private Boid[] boidsArray;

    private void Start()
    {
        InitBoids();
    }

    void InitBoids()
    {
        numOfBoids = sheepObjects.Length;
        boidsArray = new Boid[numOfBoids];

        for (int i = 0; i < numOfBoids; i++)
        {
            Vector3 pos = sheepObjects[i].transform.position;
            float offset = Random.value * 1000f;
            boidsArray[i] = new Boid(pos, Vector3.zero, offset);
        }
    }
    
    private void Update()
    {
        for (int i = 0; i < numOfBoids; i++)
        {
            Vector3 center = Vector3.zero;
            Vector3 avoid = Vector3.zero;
            Vector3 align = Vector3.zero;
            int groupSize = 0;

            for (int j = 0; j < numOfBoids; j++)
            {
                if (i == j) continue;
                Vector3 posI = boidsArray[i].position;
                Vector3 posJ = boidsArray[j].position;

                posI.y = 0;
                posJ.y = 0;

                float distance = Vector3.Distance(posI, posJ);
                if (distance <= neighbourDistance)
                {
                    center += posJ;
                    align += boidsArray[j].direction;
                    groupSize++;

                    if (distance < separationDistance)
                        avoid += (posI - posJ) / distance;
                }
            }

            Vector3 direction = boidsArray[i].direction;

            if (groupSize > 0)
            {
                center /= groupSize;
                align /= groupSize;

                Vector3 cohesion = (center - new Vector3(boidsArray[i].position.x, 0, boidsArray[i].position.z)).normalized;
                Vector3 alignment = align;
                alignment.y = 0;
                alignment.Normalize();
                Vector3 separation = avoid;
                separation.y = 0;
                separation.Normalize();

                direction += cohesion * 0.5f + alignment * 0.5f + separation * 1.0f;
            }

            Vector3 toTarget = target.position - boidsArray[i].position;
            toTarget.y = 0;
            direction += toTarget.normalized * 0.2f;

            Vector3 randomOffset = Random.insideUnitSphere;
            randomOffset.y = 0;
            direction += randomOffset * 0.05f;

            direction.y = 0;
            boidsArray[i].direction = direction.normalized;

            float speed = boidSpeed + Random.Range(-boidSpeedVariation, boidSpeedVariation);
            Vector3 newPos = boidsArray[i].position + boidsArray[i].direction * speed * Time.deltaTime;
            newPos.y = boidsArray[i].position.y;
            boidsArray[i].position = newPos;

            sheepObjects[i].transform.position = boidsArray[i].position;
            if (boidsArray[i].direction != Vector3.zero)
                sheepObjects[i].transform.rotation = Quaternion.LookRotation(boidsArray[i].direction);
        }
    }
}
