using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Block Data", menuName = "Blocks/Block Data", order = 1)]
public class BlockData : ScriptableObject
{
    [Header("Appearance")]
    public Color _texture;

    [Header("Utilitys")]
    public int _durability;
}
