
using UnityEngine;

namespace Inventory
{
    /// <summary>
    /// Defines the category of an inventory item.
    /// </summary>
    public enum ItemType
    {
        /// <summary>
        /// Special item that the player can equip and use.
        /// </summary>
        Tool,

        /// <summary>
        /// Normal item.
        /// </summary>
        Item,

        HarvestItem
    }

    /// <summary>
    /// Stores the data of an inventory item.
    /// </summary>
    [CreateAssetMenu(fileName = "New Item", menuName = "Inventory/New Item Data")]
    public class ItemData : ScriptableObject
    {
        [Header("Details")]
        [Tooltip("Unique identifier of the item. Must not be -1 and must be unique.")]
        [SerializeField] private int _id = -1;

        [Tooltip("Name of the item.")]
        [SerializeField] private string _itemName = "Item";

        [Tooltip("Description of the item.")]
        [SerializeField] private string _description = "";

        [Tooltip("Type of the item, used for inventory checking and item usage.")]
        [SerializeField] private ItemType _type = ItemType.Tool;

        [Header("Render")]
        [Tooltip("Thumbnail displayed in the inventory.")]
        [SerializeField] private Sprite _thumbnail;

        [Tooltip("3D model of the object for the player to equip in hand.")]
        [SerializeField] private GameObject _itemPrefab;

        [Header("Item Properties")]
        [Tooltip("Can this item stack with the same item?")]
        [SerializeField] private bool _isStackable = true;

        [Tooltip("Will the quantity decrease when this item is consumed?")]
        [SerializeField] private bool _isConsumable = false;

        [Header("Item Price")]
        [SerializeField] private int _price;
        [SerializeField] private int _sellPrice;

        public int Id => _id;
        public string ItemName => _itemName;
        public string Description => _description;
        public ItemType Type => _type;
        public Sprite Thumbnail => _thumbnail;
        public GameObject ItemPrefab => _itemPrefab;
        public bool IsStackable => _isStackable;
        public bool IsConsumable => _isConsumable;
        public int Price => _price;
        public int SellPrice => _sellPrice;
    }
}
