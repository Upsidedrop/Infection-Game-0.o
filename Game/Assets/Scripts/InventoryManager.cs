using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryManager : MonoBehaviour
{
    public Item[] inventory = new Item[3];
    public TMP_Text text;
    public ItemFunctions ItemFunctions;

    InputAction changeItem;
    InputAction useItem;

    int currentIndex;
    private void Start()
    {
        changeItem = InputSystem.actions.FindAction("Change Item");
        useItem = InputSystem.actions.FindAction("Use");
        changeItem.performed += context =>
        {
            currentIndex = (currentIndex + (int)context.ReadValue<float>() + 3) % 3;
            UpdateIndicator();
        };
        useItem.performed += context =>
        {
            ItemFunctions.UseItem(inventory[currentIndex]);
        };
    }
    public void TryPickup(GameObject item)
    {
        if(inventory[currentIndex] != 0)
        {
            return;
        }
        inventory[currentIndex] = item.GetComponent<ItemID>().id;
        Destroy(item);
        UpdateIndicator();
    }
    public enum Item
    {
        None = 0,
        Beeper = 1
    }
    void UpdateIndicator()
    {
        text.text = inventory[currentIndex].ToString();
    }
}
