using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonController : MonoBehaviour {
    private Animator animator;
    private Rigidbody[] ragdollRigidbodies;
    private Collider[] ragdollColliders;
    private CharacterJoint[] ragdollJoints;
    private Rigidbody mainRigidbody;
    private Collider mainCollider;
    private bool isRagdoll = false;
    private bool isDead = false;

    public float moveSpeed = 0.5f;
    public float explosionForce = 5f;
    public float explosionRadius = 2f;

    private GameObject targetPlant;
    private float plantCheckInterval = 1f;

    void Start() {
        animator = GetComponent<Animator>();
        ragdollRigidbodies = GetComponentsInChildren<Rigidbody>();
        ragdollColliders = GetComponentsInChildren<Collider>();
        ragdollJoints = GetComponentsInChildren<CharacterJoint>();
        mainRigidbody = GetComponent<Rigidbody>();
        mainCollider = GetComponent<Collider>();

        SetRagdollState(false);

        InvokeRepeating("FindClosestPlant", 0f, plantCheckInterval);
    }

    void Update() {
        if (isDead) return;

        if (!isRagdoll && targetPlant != null) {
            MoveTowardsPlant();
        } else {
            animator.SetBool("IsWalking", false);
        }
    }

    void MoveTowardsPlant() {
        if (targetPlant != null) {
            Vector3 direction = (targetPlant.transform.position - transform.position).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;
            transform.LookAt(targetPlant.transform.position);
            animator.SetBool("IsWalking", true);
        }
    }

    void FindClosestPlant() {
        if (isDead) return;

        GameObject[] plants = GameObject.FindGameObjectsWithTag("Plant");
        float closestDistance = Mathf.Infinity;
        GameObject closestPlant = null;

        foreach (GameObject plant in plants) {
            float distance = Vector3.Distance(transform.position, plant.transform.position);
            if (distance < closestDistance) {
                closestDistance = distance;
                closestPlant = plant;
            }
        }
        targetPlant = closestPlant;
    }

    void OnTriggerEnter(Collider other) {
        if (!isRagdoll && other.gameObject == targetPlant) {
            DestroyPlantAndSelf(other.gameObject);
        }
    }

    void DestroyPlantAndSelf(GameObject plant) {
        isDead = true;
        animator.SetBool("IsWalking", false);
        Destroy(plant);

        Transform soil = plant.transform.parent;
        if (soil != null) {
            soil.tag = "Soil";
            Collider soilCollider = soil.GetComponent<Collider>();
            if (soilCollider != null) {
                soilCollider.enabled = true;
            }
        }
        SetRagdollState(true);
        ApplyExplosionToLimbs();
        StartCoroutine(DestroyAfterDelay(3f));
    }

    void OnCollisionEnter(Collision collision) {
        if (!isRagdoll && collision.gameObject.CompareTag("Harvester")) {
            SetRagdollState(true);
            isRagdoll = true;
            ApplyExplosionToLimbs();
            StartCoroutine(DestroyAfterDelay(3f));
        }
    }

    void SetRagdollState(bool state) {
        foreach (var rb in ragdollRigidbodies) {
            if (rb != mainRigidbody)
                rb.isKinematic = !state;
        }

        foreach (var col in ragdollColliders) {
            if (col != mainCollider)
                col.enabled = state;
        }

        animator.enabled = !state;

        if (state) {
            mainRigidbody.isKinematic = true;
            mainCollider.enabled = false;
        } else {
            mainRigidbody.isKinematic = false;
            mainCollider.enabled = true;
        }
    }

    void ApplyExplosionToLimbs() {
        foreach (var joint in ragdollJoints) {
            Destroy(joint);
        }

        foreach (var rb in ragdollRigidbodies) {
            if (rb != mainRigidbody) {
                Vector3 explosionPos = rb.transform.position + Random.insideUnitSphere * explosionRadius;
                rb.AddExplosionForce(explosionForce, explosionPos, explosionRadius);
            }
        }
    }

    private IEnumerator DestroyAfterDelay(float delay) {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}