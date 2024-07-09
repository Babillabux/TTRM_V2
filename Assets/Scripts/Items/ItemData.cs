using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using Inventory;

[CreateAssetMenu(fileName = "Items Registery", menuName = "Items/Items Data", order = 0)]
public class ItemData : ScriptableObject
{
    public string _name;
    public int _maxStack;
    public InventoryTypes _type = InventoryTypes.item;
    public Sprite _sprite;

    [Header("Block Item")]
    public bool _isPlaceable;
    public BlockData _blockData;
}