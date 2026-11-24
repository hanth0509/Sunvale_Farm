
using System;
using UnityEngine;

namespace Inventory
{
    [System.Serializable]
    /// <summary>
    /// An individual item slot.
    /// </summary>
    public class ItemSlot
    {
        private const int MAX_QUANTITY = 9;

        [SerializeField] private ItemData _itemData;
        [SerializeField] private int _quantity;

        public ItemData ItemData => _itemData;
        public int Quantity => _quantity;

        public ItemSlot(ItemData itemData, int quantity)
        {
            if (itemData == null)
            {
                throw new ArgumentNullException(nameof(itemData));
            }

            if (quantity <= 0 || quantity > MAX_QUANTITY)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(quantity),
                    $"Quantity must be between 1 and {MAX_QUANTITY}."
                );
            }

            _itemData = itemData;
            _quantity = quantity;
        }

        public ItemSlot(ItemData itemData)
            : this(itemData, 1)
        {
        }

        /// <summary>
        /// Check whether two slots can stack together.
        /// </summary>
        public bool Stackable(ItemSlot slot)
        {
            if (slot == null || ReferenceEquals(this, slot))
                return false;

            if (_itemData == null || slot._itemData == null)
                return false;

            return _itemData == slot._itemData
                && _quantity < MAX_QUANTITY
                && _itemData.IsStackable;
        }

        /// <summary>
        /// Check whether the slot is empty.
        /// </summary>
        public bool EmptyItem()
        {
            return _itemData == null;
        }

        /// <summary>
        /// Stack all possible items from another slot.
        /// Returns the remaining slot, or null if fully transferred.
        /// </summary>
        public ItemSlot Stack(ItemSlot slot)
        {
            if (slot == null)
                throw new ArgumentNullException(nameof(slot));

            return Stack(slot, slot.Quantity);
        }

        /// <summary>
        /// Stack a specific quantity from another slot.
        /// Returns the remaining slot, or null if fully transferred.
        /// </summary>
        public ItemSlot Stack(ItemSlot slot, int quantityToAdd)
        {
            if (!Stackable(slot))
            {
                throw new InvalidOperationException(
                    "The two slots cannot be stacked."
                );
            }

            if (quantityToAdd <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(quantityToAdd),
                    "Quantity must be greater than zero."
                );
            }

            int availableSpace = MAX_QUANTITY - _quantity;
            int quantityToTransfer = Mathf.Min(
                quantityToAdd,
                Mathf.Min(slot._quantity, availableSpace)
            );

            _quantity += quantityToTransfer;
            slot._quantity -= quantityToTransfer;

            if (slot._quantity <= 0)
            {
                slot.Empty();
                return null;
            }

            return slot;
        }

        /// <summary>
        /// Add quantity to this slot.
        /// </summary>
        public void Add(int quantity = 1)
        {
            if (_itemData == null)
                throw new InvalidOperationException("Cannot add to an empty slot.");

             if (quantity <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(quantity));
            }

            if (quantity > MAX_QUANTITY - _quantity)
            {
                throw new InvalidOperationException(
                    $"Slot quantity cannot exceed {MAX_QUANTITY}."
                );
            }

            _quantity += quantity;
        }

        /// <summary>
        /// Remove quantity from this slot.
        /// </summary>
        public void Remove(int quantity = 1)
        {
            if (_itemData == null)
                throw new InvalidOperationException("Cannot remove from an empty slot.");

            if (quantity <= 0 || quantity > _quantity)
            {
                throw new ArgumentOutOfRangeException(nameof(quantity));
            }

            _quantity -= quantity;
            CheckQuantity();
        }

        /// <summary>
        /// Check and update the slot quantity.
        /// </summary>
        private void CheckQuantity()
        {
            if (_quantity <= 0 || _itemData == null)
            {
                Empty();
            }
        }

        /// <summary>
        /// Clear the slot.
        /// </summary>
        public void Empty()
        {
            _itemData = null;
            _quantity = 0;
        }
    }
}
