using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Blocks Registery", menuName = "Blocks/Blocks Registery", order = 0)]
public class BlocksRegistery : ScriptableObject
{
    public GameObject _blockPrefab;
    public List<BData> _blocksData;


    [Serializable]
    public class BData
    {
        public BlockData _blockData;
        public int _id;
    }
}
