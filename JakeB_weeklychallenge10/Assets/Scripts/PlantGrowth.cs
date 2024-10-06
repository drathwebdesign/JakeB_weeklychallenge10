using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantGrowth : MonoBehaviour {
    public GameObject[] plantStages;
    public float growthSpeed = 5f;
    public bool isFullyGrown = false;

    private int currentStage = 0;
    private float growthTimer = 0f;

    void Start() {
        UpdatePlantStage();
    }

    void Update() {
        growthTimer += Time.deltaTime;

        if (growthTimer >= growthSpeed && currentStage < plantStages.Length - 1) {
            GrowPlant();
        }
    }

    void GrowPlant() {
        currentStage++;
        growthTimer = 0f;

        UpdatePlantStage();
    }

    void UpdatePlantStage() {
        for (int i = 0; i < plantStages.Length; i++) {
            plantStages[i].SetActive(i == currentStage);
        }
        if (currentStage == plantStages.Length - 1) {
            isFullyGrown = true;
        } else {
            isFullyGrown = false;
        }
    }
}