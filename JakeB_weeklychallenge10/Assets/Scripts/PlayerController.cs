using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HarvesterUnit : MonoBehaviour {
    public bool isPlayerControlled = true;
    public float moveSpeed = 5f;
    public float rotationSpeed = 100f;
    public float harvestingDistance = 2f;
    public float harvestDelay = 2f;

    float xRange = 10f;

    public Rigidbody rb;
    public CapsuleCollider frontCollider;
    public NavMeshAgent agent;
    public LayerMask plantLayer;

    private Vector3 movement;
    private float rotation;
    private Transform currentTargetCrop;

    private bool isHarvesting = false;

    void Start() {
        if (isPlayerControlled) {
            rb = GetComponent<Rigidbody>();
        } else {
            agent = GetComponent<NavMeshAgent>();
            StartCoroutine(CheckForFullyGrownCrops());
        }
    }

    void Update() {
        if (isPlayerControlled) {
            HandlePlayerControlledMovement();
            Bounds();
        } else {
            HandleAIAutomaticMovement();
        }
    }

    void FixedUpdate() {
        if (isPlayerControlled) {
            rb.MovePosition(rb.position + movement * Time.fixedDeltaTime);
            Quaternion turnRotation = Quaternion.Euler(0f, rotation, 0f);
            rb.MoveRotation(rb.rotation * turnRotation);
        }
    }

    void HandlePlayerControlledMovement() {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        movement = transform.forward * moveZ * moveSpeed;
        rotation = moveX * rotationSpeed * Time.deltaTime;
    }

    void Bounds() {
        if (transform.position.x >= xRange) {
            transform.position = new Vector3(xRange, 0, transform.position.z);
        }
        if (transform.position.x <= -xRange) {
            transform.position = new Vector3(-xRange, 0, transform.position.z);
        } else if (transform.position.z > 6) {
            transform.position = new Vector3(transform.position.x, transform.position.y, 6);
        }
        if (transform.position.z < -10) {
            transform.position = new Vector3(transform.position.x, transform.position.y, -10);
        }
    }

    IEnumerator CheckForFullyGrownCrops() {
        while (true) {
            if (!isHarvesting && currentTargetCrop == null) {
                FindClosestFullyGrownCrop();
            }

            yield return new WaitForSeconds(2f);
        }
    }

    void HandleAIAutomaticMovement() {
        if (!isHarvesting && currentTargetCrop != null) {
            agent.SetDestination(currentTargetCrop.position);

            if (!agent.pathPending && agent.remainingDistance <= harvestingDistance) {
                isHarvesting = true;
                HarvestPlant(currentTargetCrop.gameObject);
                StartCoroutine(ResumeHarvestingAfterDelay(harvestDelay));
            }
        }
    }

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.layer == LayerMask.NameToLayer("Plant")) {
            PlantGrowth plantGrowth = other.GetComponent<PlantGrowth>();
            if (plantGrowth != null && plantGrowth.isFullyGrown) {
                HarvestPlant(other.gameObject);
            }
        }
    }

    void HarvestPlant(GameObject plant) {
        Destroy(plant);
        ScoreManager.instance.IncreaseScore(2);
        CurrencyManager.instance.IncreaseCurrency(2);

        Transform soil = plant.transform.parent;
        if (soil != null) {
            soil.tag = "Soil";
            soil.GetComponent<Collider>().enabled = true;
        }
    }

    IEnumerator ResumeHarvestingAfterDelay(float delay) {
        yield return new WaitForSeconds(delay);
        isHarvesting = false;
        FindClosestFullyGrownCrop();
    }

    void FindClosestFullyGrownCrop() {
        GameObject[] allCrops = GameObject.FindGameObjectsWithTag("Plant");

        float closestDistance = Mathf.Infinity;
        Transform closestCrop = null;

        foreach (GameObject crop in allCrops) {
            PlantGrowth plantGrowth = crop.GetComponent<PlantGrowth>();
            if (plantGrowth != null && plantGrowth.isFullyGrown) {
                float distanceToCrop = Vector3.Distance(transform.position, crop.transform.position);

                if (distanceToCrop < closestDistance) {
                    closestDistance = distanceToCrop;
                    closestCrop = crop.transform;
                }
            }
        }

        currentTargetCrop = closestCrop;
    }
}