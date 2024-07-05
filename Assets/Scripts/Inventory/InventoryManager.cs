using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static InventoryManager;

public enum InventoryTypes
{
    item,
    block,
    armor_head,
    armor_chest,
    armor_legs,
    armor_feet
}

public class InventoryManager : MonoBehaviour
{
    [Header("Inventory")]
    public GameObject _inventoryTilePrefab;
    public GameObject _inventoryPanel, _itemsContent;
    public ItemsRegistery _itemRegistery;

    public List<I_Content> I_content = new List<I_Content> ();

    [Header("Hotbar")]
    [SerializeField] GameObject _hotbarPanel;
    [SerializeField] Image _hotbarImage;
    [SerializeField] TMP_Text _hotbarQuantity;
    [SerializeField] TMP_Text _hotbarName;
    public int _takenItemId;

    public static InventoryManager instance;
    private void Awake()
    {
        if(instance != null)
        {
            Debug.LogWarning("There is more than one instance of InventoryManager");
            return;
        }
        instance = this;
    }

    private void Start()
    {
        if (_hotbarPanel != null)
        {
            _hotbarPanel.SetActive(false);
        }

        _itemsContent.transform.localPosition = new Vector3(0, 0, 0);
        _inventoryPanel.SetActive(false);
    }

    private float _waitTime = 0;
    private void Update()
    {
        _waitTime += Time.deltaTime;
        if(Input.GetKeyDown(KeyCode.E) && _waitTime >= 0.1f)
        {
            if(_inventoryPanel.activeSelf)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                _inventoryPanel.SetActive(false);
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                _inventoryPanel.SetActive(true);
            }
            UpdateInventory();
            _waitTime = 0;
        }
    }

    void UpdateInventory()
    {
        foreach(var item in I_content)
        {
            item._content.GetComponent<InventoryTile>().T_quantity.text = item._quantity.ToString();
        }
    }

    public void AddItemInInventory(int id)
    {
        List<I_Content> tiles = new List<I_Content>();

        foreach(var item in I_content)
        {
            if(item._id == id && !item._isFull)
            {
                tiles.Add(item);
            }
        }

        if(tiles.Count > 0)
        {
            foreach(var tile in tiles)
            {
                if(tile._quantity < tile._itemData._maxStack)
                {
                    tile._quantity++;
                    UpdateInventory();
                    return;
                }
            }
        }

        CreateNewTile(TakeItemData(id), id, 1);
    }

    public void RemoveItemInInventory(int id)
    {
        /*
        List<I_Content> contentQuantity = new List<I_Content>();

        foreach(I_Content content in I_content)
        {
            if(content._id == id)
            {
                contentQuantity.Add(content);
            }
        }

        if(contentQuantity.Count > 0)
        {
            
        }
        */
    }

    void CreateNewTile(ItemData itemData, int id, int quantity)
    {
        GameObject tile = Instantiate(_inventoryTilePrefab);
        tile.transform.SetParent(_itemsContent.transform);
        tile.transform.position = _itemsContent.transform.position;

        InventoryTile inventoryTile = tile.GetComponent<InventoryTile>();
        inventoryTile._image.sprite = itemData._sprite;
        inventoryTile.T_quantity.text = quantity.ToString();
        inventoryTile.T_name.text = itemData._name;

        I_Content tileContent = new I_Content();
        tileContent._content = tile;
        tileContent._itemData = itemData;
        tileContent._quantity = quantity;
        tileContent._id = id;
        tileContent._isFull = false;

        I_content.Add(tileContent);

        tile.GetComponent<Button>().onClick.AddListener(() => OnItemTile(id));
    }

    private ItemData TakeItemData(int id)
    {
        foreach (var item in _itemRegistery._itemsData)
        {
            if (item._id == id)
            {
                return item._itemData;
            }
        }

        return null;
    }

    int ItemsQuantity(int id)
    {
        int totalItemCount = 0;

        foreach (var item in I_content)
        {
            if (item._id == id && !item._isFull)
            {
                totalItemCount += item._quantity;
            }
        }

        return totalItemCount;
    }

    void OnItemTile(int id)
    {
        UpdateHotbar(id);

        //Debug.Log(_takenItemId + " : " + ItemsQuantity(_takenItemId));
    }

    public void UpdateHotbar(int id)
    {
        _takenItemId = id;

        ItemData itemData = TakeItemData(id);

        _hotbarPanel.SetActive(true);

        _hotbarImage.sprite = itemData._sprite;
        _hotbarQuantity.text = ItemsQuantity(_takenItemId).ToString();
        _hotbarName.text = itemData._name;
    }

    [Serializable]
    public class I_Content
    {
        public GameObject _content;
        public ItemData _itemData;
        public int _quantity;
        public int _id;
        public bool _isFull;
    }
}
