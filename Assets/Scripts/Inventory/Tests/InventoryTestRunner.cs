
using UnityEngine;
using System;

namespace Inventory
{
    public class InventoryTestRunner : MonoBehaviour
    {
        [SerializeField] private ItemData _stackableItem;
        [SerializeField] private ItemData _nonStackableItem;



        private void Start()
        {
            TestAddItemInvalidInput();
            TestAddItemStacking();
            TestAddItemWhenInventoryFull();
            TestRemoveItemAcrossStacks();
            TestAddNonStackableItems();
            TestRemoveNonStackableItems();
            TestEquipEmptySlots();
        }

        private void TestRemoveNonStackableItems()
        {
            InventoryManager.Instance.ClearInventory();

            InventoryManager.Instance.AddItem(_nonStackableItem, 3);

            bool removeResult =
                InventoryManager.Instance.RemoveItem(_nonStackableItem, 2);

            Debug.Log($"Remove Success: {removeResult}");
            Debug.Log($"Has 1 Item: {InventoryManager.Instance.HasItem(_nonStackableItem, 1)}");
            Debug.Log($"Has 2 Items: {InventoryManager.Instance.HasItem(_nonStackableItem, 2)}");

            int occupiedSlots = 0;

            foreach (ItemSlot slot in InventoryManager.Instance.ItemSlots)
            {
                if (slot != null && !slot.EmptyItem())
                    occupiedSlots++;
            }

            Debug.Log($"Occupied Slots: {occupiedSlots}");
        }

        private void TestAddNonStackableItems()
        {
            InventoryManager.Instance.ClearInventory();

            int remaining = InventoryManager.Instance.AddItem(_nonStackableItem, 3);

            Debug.Log($"Remaining: {remaining}");

            for (int i = 0; i < InventoryManager.Instance.ItemSlots.Length; i++)
            {
                ItemSlot slot = InventoryManager.Instance.ItemSlots[i];

                if (slot != null)
                {
                    Debug.Log($"Slot {i + 1}: {slot.ItemData.ItemName} - Quantity: {slot.Quantity}");
                }
            }
        }

        private void TestRemoveItemAcrossStacks()
        {

            InventoryManager.Instance.ClearInventory();

            InventoryManager.Instance.AddItem(_stackableItem, 12);

            bool removeResult = InventoryManager.Instance.RemoveItem(_stackableItem, 10);

            Debug.Log($"Remove Success: {removeResult}");
            Debug.Log($"Has 2 Items: {InventoryManager.Instance.HasItem(_stackableItem, 2)}");
            Debug.Log($"Has 3 Items: {InventoryManager.Instance.HasItem(_stackableItem, 3)}");
            Debug.Log($"Slot 1 Empty: {InventoryManager.Instance.ItemSlots[0] == null}");
            Debug.Log($"Slot 2 Quantity: {InventoryManager.Instance.ItemSlots[1].Quantity}");

        }

        private void TestAddItemWhenInventoryFull()
        {
            InventoryManager.Instance.ClearInventory();

            InventoryManager.Instance.AddItem(_stackableItem, 72);

            int remaining = InventoryManager.Instance.AddItem(_stackableItem, 5);

            Debug.Log($"Remaining: {remaining}");
            Debug.Log($"Slot Count: {InventoryManager.Instance.ItemSlots.Length}");

            int totalQuantity = 0;
            int occupiedSlots = 0;

            foreach (ItemSlot slot in InventoryManager.Instance.ItemSlots)
            {
                if (slot == null || slot.EmptyItem())
                    continue;

                occupiedSlots++;
                totalQuantity += slot.Quantity;
            }

            Debug.Log($"Occupied Slots: {occupiedSlots}");
            Debug.Log($"Total Quantity: {totalQuantity}");
        }

        private void TestAddItemStacking()
        {
            InventoryManager.Instance.ClearInventory();

            int remaining = InventoryManager.Instance.AddItem(_stackableItem, 12);

            Debug.Log($"Remaining: {remaining}");
            Debug.Log($"Slot 1 Quantity: {InventoryManager.Instance.ItemSlots[0].Quantity}");
            Debug.Log($"Slot 2 Quantity: {InventoryManager.Instance.ItemSlots[1].Quantity}");
        }
        private void TestEquipEmptySlots()
        {
            InventoryManager.Instance.ClearInventory();

            bool equipResult = InventoryManager.Instance.Equip(0);

            Debug.Log($"Equip Success: {equipResult}");
            Debug.Log(
                $"Inventory Empty: {InventoryManager.Instance.ItemSlots[0] == null}"
            );
            Debug.Log(
                $"Holding Empty: {InventoryManager.Instance.HoldingItemSlot == null}"
            );
        }




        private void TestAddItemInvalidInput()
        {
            InventoryManager.Instance.ClearInventory();

            // Test 1: AddItem rejects null ItemData
            try
            {
                InventoryManager.Instance.AddItem(null, 1);
                Debug.LogError("Test 1 Failed: AddItem accepted null ItemData.");
            }
            catch (ArgumentNullException)
            {
                Debug.Log("Test 1 Passed: AddItem rejects null ItemData.");
            }

            // Test 2: AddItem rejects zero quantity
            try
            {
                InventoryManager.Instance.AddItem(_stackableItem, 0);
                Debug.LogError("Test 2 Failed: AddItem accepted zero quantity.");
            }
            catch (ArgumentOutOfRangeException)
            {
                Debug.Log("Test 2 Passed: AddItem rejects zero quantity.");
            }

            // Test 3: HasItem rejects null ItemData
            try
            {
                InventoryManager.Instance.HasItem(null);
                Debug.LogError("Test 3 Failed: HasItem accepted null ItemData.");
            }
            catch (ArgumentNullException)
            {
                Debug.Log("Test 3 Passed: HasItem rejects null ItemData.");
            }

            // Test 4: HasItem rejects zero quantity
            try
            {
                InventoryManager.Instance.HasItem(_stackableItem, 0);
                Debug.LogError("Test 4 Failed: HasItem accepted zero quantity.");
            }
            catch (ArgumentOutOfRangeException)
            {
                Debug.Log("Test 4 Passed: HasItem rejects zero quantity.");
            }

            // Test 5: RemoveItem rejects null ItemData
            try
            {
                InventoryManager.Instance.RemoveItem(null);
                Debug.LogError("Test 5 Failed: RemoveItem accepted null ItemData.");
            }
            catch (ArgumentNullException)
            {
                Debug.Log("Test 5 Passed: RemoveItem rejects null ItemData.");
            }

            // Test 6: RemoveItem rejects zero quantity
            try
            {
                InventoryManager.Instance.RemoveItem(_stackableItem, 0);
                Debug.LogError("Test 6 Failed: RemoveItem accepted zero quantity.");
            }
            catch (ArgumentOutOfRangeException)
            {
                Debug.Log("Test 6 Passed: RemoveItem rejects zero quantity.");
            }

            // Test 7: AddItem rejects negative quantity
            try
            {
                InventoryManager.Instance.AddItem(_stackableItem, -1);
                Debug.LogError("Test 7 Failed: AddItem accepted negative quantity.");
            }
            catch (ArgumentOutOfRangeException)
            {
                Debug.Log("Test 7 Passed: AddItem rejects negative quantity.");
            }

            // Test 8: HasItem rejects negative quantity
            try
            {
                InventoryManager.Instance.HasItem(_stackableItem, -1);
                Debug.LogError("Test 8 Failed: HasItem accepted negative quantity.");
            }
            catch (ArgumentOutOfRangeException)
            {
                Debug.Log("Test 8 Passed: HasItem rejects negative quantity.");
            }

            // Test 9: RemoveItem rejects negative quantity
            try
            {
                InventoryManager.Instance.RemoveItem(_stackableItem, -1);
                Debug.LogError("Test 9 Failed: RemoveItem accepted negative quantity.");
            }
            catch (ArgumentOutOfRangeException)
            {
                Debug.Log("Test 9 Passed: RemoveItem rejects negative quantity.");
            }

            Debug.Log(
                $"Inventory Empty: {InventoryManager.Instance.ItemSlots[0] == null}"
            );
        }

    }
}
