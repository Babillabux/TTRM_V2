using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BlocksRegistery;

[CreateAssetMenu(fileName = "Items Registery", menuName = "Items/Items Registery", order = 0)]
public class ItemsRegistery : ScriptableObject
{
    public GameObject _itemInventoryPrefab, _itemDropPrefab;
    public List<IData> _itemsData;

    [Serializable]
    public class IData
    {
        public ItemData _itemData;
        public int _id;
    }
}
