using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour {
    public GameObject skeletonPrefab;
    public float spawnRate = 2f;
    public float spawnRangeX = 14f;
    public float spawnRangeZ = 13f;

    private float playerXRange = 10f;
    private float playerZMin = -6f;
    private float playerZMax = 6f;

    void Start() {
        InvokeRepeating("SpawnSkeleton", 5f, spawnRate);
    }

    void SpawnSkeleton() {
        Vector3 spawnPosition;

        do {
            bool spawnAlongX = Random.value > 0.5f;

            if (spawnAlongX) {
                float spawnPosX = Random.Range(-spawnRangeX, spawnRangeX);
                float spawnPosZ = Random.Range(-spawnRangeZ, spawnRangeZ);
                spawnPosition = new Vector3(spawnPosX, 0, spawnPosZ);
            } else {
                float spawnPosX = Random.Range(-spawnRangeX, spawnRangeX);
                float spawnPosZ = Random.Range(-spawnRangeZ, spawnRangeZ);
                spawnPosition = new Vector3(spawnPosX, 0, spawnPosZ);
            }

        } while (IsWithinPlayerBounds(spawnPosition));
        Instantiate(skeletonPrefab, spawnPosition, Quaternion.identity);
    }

    bool IsWithinPlayerBounds(Vector3 position) {
        bool withinXBounds = position.x >= -playerXRange && position.x <= playerXRange;
        bool withinZBounds = position.z >= playerZMin && position.z <= playerZMax;

        return withinXBounds && withinZBounds;
    }
}