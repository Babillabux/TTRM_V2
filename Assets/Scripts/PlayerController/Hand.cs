using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class Hand : MonoBehaviour
{
    [SerializeField] int _blockId;
    [SerializeField] float _reach;

    [Header("Raycast Block Detector")]
    public LayerMask _layerMask;
    private RaycastHit _hitBD;
    private bool _hasHitBD;
    private Ray _rayBD;

    [Header("Raycast Block Placer")]
    Vector3 _center;
    Vector3 _halfExtents = new Vector3(0.49999f, 0.49999f, 0.49999f);
    Collider[] _hitColliders;

    private void Update()
    {
        _rayBD = Camera.main.ScreenPointToRay(Input.mousePosition);
        _hasHitBD = Physics.Raycast(_rayBD, out _hitBD, _reach, _layerMask);

        if(_hasHitBD && !InventoryManager.instance._inventoryPanel.activeSelf)
        {
            _center = _hitBD.transform.position + _hitBD.normal;
            _hitColliders = Physics.OverlapBox(_center, _halfExtents, Quaternion.identity);

            if (Input.GetMouseButtonDown(0))
            {
                DestroyBlock();
            }
            else if (Input.GetMouseButtonDown(1) && _hitColliders.Length == 0)
            {
                PlaceBlock();
            }
        }
        else
        {
            _hitColliders = new Collider[0];
        }
    }

    private void PlaceBlock()
    {
        GameObject block = Instantiate(WorldManager.instance._blocksRegistery._blockPrefab, _center, Quaternion.identity);
        block.GetComponent<Block>().InitializeBlock(_blockId);

        Vector3Int chunkPos = GetClosestChunkCoord(_center);
        GameObject _chunk = null;

        foreach(var chunk in WorldManager.instance._chunkPosition)
        {
            if(chunk.Key == chunkPos)
            {
                _chunk = chunk.Value;
                break;
            }
        }

        if(_chunk != null)
        {
            block.transform.SetParent(_chunk.transform);
            _chunk.GetComponent<Chunk>()._blocks.Add(block);
        }

    }

    private void DestroyBlock()
    {
        GameObject blockHit = _hitBD.collider.gameObject;
        if(blockHit.GetComponent<Block>()._durability >= 0)
        {
            InventoryManager.instance.AddItemInInventory(blockHit.GetComponent<Block>()._id);

            blockHit.GetComponentInParent<Chunk>()._blocks.Remove(blockHit);

            if(blockHit.GetComponent<Block>()._id == InventoryManager.instance._takenItemId)
            {
                InventoryManager.instance.UpdateHotbar(blockHit.GetComponent<Block>()._id);
            }

            Destroy(blockHit);
        }
    }


    private Vector3Int GetClosestChunkCoord(Vector3 pos)
    {
        return new Vector3Int(
            Mathf.FloorToInt(pos.x / WorldManager.instance._chunkSize) * WorldManager.instance._chunkSize,
            0,
            Mathf.FloorToInt(pos.z / WorldManager.instance._chunkSize) * WorldManager.instance._chunkSize
            );
    }

    void OnDrawGizmos()
    {
        if (_rayBD.origin != null)
        {
            if (_hasHitBD)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(_rayBD.origin, _hitBD.point);
                Gizmos.DrawSphere(_hitBD.point, 0.1f);

                if (_hitColliders.Length == 0)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawWireCube(_center, _halfExtents * 2);
                }
                else
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawWireCube(_center, _halfExtents * 2);
                }
            }
            else
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(_rayBD.origin, _rayBD.origin + _rayBD.direction * _reach);
            }
        }
    }
}
