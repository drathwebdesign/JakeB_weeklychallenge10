using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShopController : MonoBehaviour {
    public GameObject prefabToInstantiate;
    public Transform spawnLocation;
    public TextMeshProUGUI costText;
    public float stayTimeRequired = 1f;
    public float initialCost = 100f;
    public LayerMask playerLayer;

    private float currentCost;
    private bool playerInTrigger = false;
    private float stayTimer = 0f;

    void Start() {
        currentCost = initialCost;
        UpdateCostText();
    }

    void Update() {
        if (playerInTrigger) {
            stayTimer += Time.deltaTime;

            if (stayTimer >= stayTimeRequired) {
                TryToPurchase();
                stayTimer = 0f;
            }
        } else {
            stayTimer = 0f;
        }
    }

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player")) {
            playerInTrigger = true;
        }
    }

    void OnTriggerExit(Collider other) {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player")) {
            playerInTrigger = false;
        }
    }

    void TryToPurchase() {
        if (CurrencyManager.instance.GetCurrency() >= currentCost) {
            CurrencyManager.instance.SpendCurrency((int)currentCost);

            Instantiate(prefabToInstantiate, spawnLocation.position, spawnLocation.rotation);

            currentCost *= 1.5f;

            UpdateCostText();
        }
    }

    void UpdateCostText() {
        costText.text = "$" + currentCost.ToString("F0");
    }
}