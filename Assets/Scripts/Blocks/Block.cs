using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.Collections.AllocatorManager;

public class Block : MonoBehaviour
{
    public int _id;
    public int _durability;
    public BlockData _data;
    public string _name;
    public MeshRenderer _meshRenderer;

    public void InitializeBlock(int id)
    {
        _data = TakeBlockData(id);

        if (_data != null)
        {
            _id = id;
            _meshRenderer.material.color = _data._texture;
            _durability = _data._durability;
        }
        else
        {
            Debug.LogError("Try to generate an inexistant block, id :" + id);
        }

    }

    private BlockData TakeBlockData(int id)
    {
        foreach (var block in WorldManager.instance._blocksRegistery._blocksData)
        {
            if (block._id == id)
            {
                return block._blockData;
            }
        }

        return null;
    }
}
