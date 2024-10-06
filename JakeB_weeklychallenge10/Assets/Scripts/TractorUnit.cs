using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TractorUnit : MonoBehaviour {

    public GameObject plantPrefab;
    public float plantingDistance = 2f;
    public float detectionRadius = 100f;

    private NavMeshAgent agent;
    private Transform currentTargetSoil;
    private bool isPlanting = false;

    void Start() {
        agent = GetComponent<NavMeshAgent>();
        FindClosestUnplantedSoil();
    }

    void Update() {
        if (currentTargetSoil != null && !isPlanting) {
            agent.SetDestination(currentTargetSoil.position);

            // Check if the tractor has reached the soil patch
            if (!agent.pathPending && agent.remainingDistance <= plantingDistance) {
                isPlanting = true;
                PlantInSoil(currentTargetSoil.position, currentTargetSoil.gameObject);
                MarkSoilAsPlanted(currentTargetSoil.gameObject);
                FindClosestUnplantedSoil();
            }
        }
    }

    void PlantInSoil(Vector3 position, GameObject soil) {
        GameObject newPlant = Instantiate(plantPrefab, position, Quaternion.identity);
        newPlant.transform.SetParent(soil.transform);
        newPlant.transform.localPosition = Vector3.zero;
    }


    void MarkSoilAsPlanted(GameObject soil) {
        soil.tag = "Planted";
        //soil.GetComponent<Collider>().enabled = false;
    }

    void FindClosestUnplantedSoil() {
        // Find all objects tagged as "Soil" within the detection radius
        GameObject[] allSoilPatches = GameObject.FindGameObjectsWithTag("Soil");

        float closestDistance = Mathf.Infinity; // Track the shortest distance
        Transform closestSoil = null;           // Store reference to closest unplanted soil

        foreach (GameObject soil in allSoilPatches) {
            // Skip soil patches that have already been planted
            if (soil.CompareTag("Planted")) continue;

            float distanceToSoil = Vector3.Distance(transform.position, soil.transform.position);
            if (distanceToSoil < closestDistance && distanceToSoil <= detectionRadius) {
                closestSoil = soil.transform;
                closestDistance = distanceToSoil;
            }
        }

        if (closestSoil != null) {
            currentTargetSoil = closestSoil;   // Set the closest unplanted soil as the next target
            isPlanting = false;               // Reset the planting flag so the tractor can plant again
        } else {
            currentTargetSoil = null;          // No more soil, stop moving
            isPlanting = true;                 // Ensure no further planting happens
            agent.isStopped = true;            // Stop the tractor from moving
            Debug.Log("No more unplanted soil patches found!");
        }
    }
}