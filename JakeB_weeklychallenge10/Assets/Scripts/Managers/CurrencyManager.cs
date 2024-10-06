using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CurrencyManager : MonoBehaviour {
    public static CurrencyManager instance;

    public TextMeshProUGUI currencyText;
    private int currency = 0;

    private void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    public void IncreaseCurrency(int amount) {
        currency += amount;
        UpdateCurrencyUI();
    }

    public bool SpendCurrency(int amount) {
        if (currency >= amount) {
            currency -= amount;
            UpdateCurrencyUI();  // Update the UI after spending
            return true;
        } else {
            Debug.Log("Not enough currency!");
            return false;
        }
    }

    public int GetCurrency() {
        return currency;
    }

    private void UpdateCurrencyUI() {
        currencyText.text = "Currency: $" + currency;
    }

    public void ResetCurrency() {
        currency = 0;
        UpdateCurrencyUI();
    }
}