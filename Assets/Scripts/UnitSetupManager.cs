using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UnitSetupManager : MonoBehaviour
{
    [Header("Data References")]
    public CurrentDeckData currentDeckData;
    public List<UnitData> allAvailableUnits; // List of all owned unit data assets

    [Header("UI References")]
    public Transform inventoryContent; // Parent panel for generated unit buttons
    public GameObject unitButtonPrefab; // Button prefab to instantiate
    public Image[] slotImages; // 3 UI Image components representing the deck slots

    void Start()
    {
        InitializeInventoryUI();
        UpdateDeckUI();
    }

    // Populate the inventory panel with available units
    private void InitializeInventoryUI()
    {
        foreach (UnitData unit in allAvailableUnits)
        {
            GameObject btnObj = Instantiate(unitButtonPrefab, inventoryContent);
            
            Image btnImage = btnObj.GetComponent<Image>();
            btnImage.sprite = unit.unitIcon;

            // Capture variables for the closure in the listener
            int capturedId = unit.unitId;
            
            Button btn = btnObj.GetComponent<Button>();
            btn.onClick.AddListener(() => OnInventoryUnitClicked(capturedId));
        }
    }

    // Assign clicked unit to the first empty slot
    private void OnInventoryUnitClicked(int unitId)
    {
        // Check if the unit is already in the deck to prevent duplicates
        foreach (int id in currentDeckData.selectedUnitIds)
        {
            if (id == unitId)
            {
                Debug.Log("This unit is already in the deck.");
                return; 
            }
        }

        // Find the first empty slot (-1) and assign the unit
        for (int i = 0; i < currentDeckData.selectedUnitIds.Length; i++)
        {
            if (currentDeckData.selectedUnitIds[i] == -1)
            {
                currentDeckData.SetUnitInSlot(i, unitId);
                UpdateDeckUI();
                return;
            }
        }
        Debug.Log("All slots are full.");
    }

    // Refresh slot visuals based on CurrentDeckData
    private void UpdateDeckUI()
    {
        for (int i = 0; i < currentDeckData.selectedUnitIds.Length; i++)
        {
            int unitId = currentDeckData.selectedUnitIds[i];
            if (unitId != -1)
            {
                UnitData unit = allAvailableUnits.Find(u => u.unitId == unitId);
                if (unit != null)
                {
                    slotImages[i].sprite = unit.unitIcon;
                    slotImages[i].color = Color.white;
                }
            }
            else
            {
                // Visual representation of an empty slot
                slotImages[i].sprite = null;
                slotImages[i].color = Color.gray;
            }
        }
    }
    
    // Bind this to the OnClick event of the Slot Buttons in the Inspector
    public void OnSlotClicked(int slotIndex)
    {
        // Remove unit from the slot
        currentDeckData.SetUnitInSlot(slotIndex, -1);
        UpdateDeckUI();
    }
}