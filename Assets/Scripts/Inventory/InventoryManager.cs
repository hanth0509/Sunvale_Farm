
using UnityEngine;
using Utilities;
using System;

namespace Inventory
{
    /// <summary>
    /// Manages the player's inventory and holding item.
    /// </summary>
    public class InventoryManager : Singleton<InventoryManager>
    {
        private const int INVENTORY_SLOT_COUNT = 8;
        private const int MAX_STACK_SIZE = 9;

        [Header("Inventory")]
        [SerializeField] private ItemSlot[] _itemSlots;

        [Header("Holding Item")]
        [SerializeField] private ItemSlot _holdingItemSlot;

        public ItemSlot[] ItemSlots => _itemSlots;
        public ItemSlot HoldingItemSlot => _holdingItemSlot;

        protected override void AwakeSingleton()
        {
            InitializeInventory();
        }

        /// <summary>
        /// Initialize inventory slots.
        /// </summary>
        private void InitializeInventory()
        {
            if (_itemSlots == null ||
                _itemSlots.Length != INVENTORY_SLOT_COUNT)
            {
                _itemSlots = new ItemSlot[INVENTORY_SLOT_COUNT];
            }
        }

        /// <summary>
        /// Clear all inventory slots and holding item.
        /// </summary>
        public void ClearInventory()
        {
            for (int i = 0; i < _itemSlots.Length; i++)
            {
                _itemSlots[i] = null;
            }

            _holdingItemSlot = null;
        }
        /// <summary>
        /// Add items to the inventory.
        /// Returns the remaining quantity.
        /// </summary>
        public int AddItem(ItemData itemData, int quantity)
        {
            if (itemData == null)
                throw new ArgumentNullException(nameof(itemData));

            if (quantity <= 0)
                throw new ArgumentOutOfRangeException(nameof(quantity));

            int remaining = quantity;

            // First pass: Stack with existing items.
            if (itemData.IsStackable)
            {
                for (int i = 0; i < _itemSlots.Length; i++)
                {
                    ItemSlot slot = _itemSlots[i];

                    if (slot == null || slot.ItemData != itemData)
                        continue;

                    int availableSpace = MAX_STACK_SIZE - slot.Quantity;

                    if (availableSpace <= 0)
                        continue;

                    int amountToAdd = Mathf.Min(remaining, availableSpace);

                    slot.Add(amountToAdd);
                    remaining -= amountToAdd;

                    if (remaining == 0)
                        return 0;
                }
            }

            // Second pass: Create new slots.
            for (int i = 0; i < _itemSlots.Length; i++)
            {
                if (_itemSlots[i] != null && !_itemSlots[i].EmptyItem())
                    continue;

                int amountToAdd = itemData.IsStackable
                    ? Mathf.Min(remaining, MAX_STACK_SIZE)
                    : 1;

                _itemSlots[i] = new ItemSlot(itemData, amountToAdd);
                remaining -= amountToAdd;

                if (remaining == 0)
                    return 0;
            }

            return remaining;
        }
        /// <summary>
        /// Check whether the inventory has enough items.
        /// </summary>
        public bool HasItem(ItemData itemData, int quantity = 1)
        {
            if (itemData == null)
                throw new ArgumentNullException(nameof(itemData));

            if (quantity <= 0)
                throw new ArgumentOutOfRangeException(nameof(quantity));

            int totalQuantity = 0;

            for (int i = 0; i < _itemSlots.Length; i++)
            {
                ItemSlot slot = _itemSlots[i];

                if (slot == null || slot.ItemData != itemData)
                    continue;

                totalQuantity += slot.Quantity;

                if (totalQuantity >= quantity)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Remove items from the inventory.
        /// Returns false if there are not enough items.
        /// </summary>
        public bool RemoveItem(ItemData itemData, int quantity = 1)
        {
            if (itemData == null)
                throw new ArgumentNullException(nameof(itemData));

            if (quantity <= 0)
                throw new ArgumentOutOfRangeException(nameof(quantity));

            // Check availability before modifying inventory.
            if (!HasItem(itemData, quantity))
                return false;

            int remaining = quantity;

            for (int i = 0; i < _itemSlots.Length; i++)
            {
                ItemSlot slot = _itemSlots[i];

                if (slot == null || slot.ItemData != itemData)
                    continue;

                int amountToRemove = Mathf.Min(remaining, slot.Quantity);

                slot.Remove(amountToRemove);
                remaining -= amountToRemove;

                if (slot.EmptyItem())
                    _itemSlots[i] = null;

                if (remaining == 0)
                    return true;
            }

            return true;
        }

        /// <summary>
        /// Move or swap an item between inventory and holding slot.
        /// </summary>
        public bool Equip(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= _itemSlots.Length)
                throw new ArgumentOutOfRangeException(nameof(slotIndex));

            ItemSlot selectedSlot = _itemSlots[slotIndex];

            bool selectedIsEmpty =
                selectedSlot == null || selectedSlot.EmptyItem();

            bool holdingIsEmpty =
                _holdingItemSlot == null || _holdingItemSlot.EmptyItem();

            // Move holding item to an empty inventory slot.
            if (selectedIsEmpty)
            {
                if (holdingIsEmpty)
                    return false;

                _itemSlots[slotIndex] = _holdingItemSlot;
                _holdingItemSlot = null;

                return true;
            }

            // Move selected item to an empty holding slot.
            if (holdingIsEmpty)
            {
                _holdingItemSlot = selectedSlot;
                _itemSlots[slotIndex] = null;

                return true;
            }
            // Prevent swapping identical items when holding slot is full.
            if (_holdingItemSlot.ItemData == selectedSlot.ItemData
                && _holdingItemSlot.ItemData.IsStackable
                && _holdingItemSlot.Quantity >= MAX_STACK_SIZE)
            {
                return false;
            }
            // Stack items when they are compatible.
            if (_holdingItemSlot.Stackable(selectedSlot))
            {
                ItemSlot remainingSlot =
                    _holdingItemSlot.Stack(selectedSlot);

                _itemSlots[slotIndex] = remainingSlot;

                return true;
            }

            // Swap two different items.
            _itemSlots[slotIndex] = _holdingItemSlot;
            _holdingItemSlot = selectedSlot;

            return true;
        }


    }
}
