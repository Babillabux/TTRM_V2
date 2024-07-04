using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    public bool _isCombined, _canRunProgram;
    public int _chunkMaxLayer;
    public int _chunkSize;
    public GameObject _blockPrefab;
    public BlocksRegistery _blocksRegistery;
    public List<GameObject> _blocks = new List<GameObject>();
    MeshFilter _combinedMeshFilter;
    MeshRenderer _combinedMeshRenderer;

    public void GenerateChunk(Vector3Int chunkPos, int seed)
    {
        _canRunProgram = false;
        _isCombined = false;
        for (int x = 0; x < _chunkSize; x++)
        {
            for (int z = 0; z < _chunkSize; z++)
            {
                float perlinValue = Mathf.PerlinNoise((chunkPos.x + x) / 10f + seed, (chunkPos.z + z) / 10f + seed);
                float y = Mathf.RoundToInt(perlinValue * _chunkMaxLayer);


                for (int i = 0; i <= y; i++)
                {
                    Vector3 blockPos = new Vector3(x, i, z) + this.transform.position;

                    int blockId;

                    if (i == 0)
                    {
                        blockId = 4;
                    }
                    else if (i == y)
                    {
                        blockId = 2;
                    }
                    else if (i == y - 1 || i == y - 2)
                    {
                        blockId = 1;
                    }
                    else
                    {
                        blockId = 0;
                    }

                    GenerateBlock(blockPos, blockId);
                }
            }
        }

        CombineMeshes();
        _canRunProgram = true;
    }

    private void GenerateBlock(Vector3 pos, int id)
    {
        GameObject block = Instantiate(_blockPrefab, pos, Quaternion.identity);
        block.transform.SetParent(transform);

        block.GetComponent<Block>().InitializeBlock(id);

        _blocks.Add(block);
    }

    private void CombineMeshes()
    {
        List<MeshRenderer> meshRenderers = new List<MeshRenderer>();

        foreach (var block in _blocks)
        {
            MeshRenderer mr = block.GetComponent<MeshRenderer>();
            if (mr != null)
            {
                meshRenderers.Add(mr);
            }
        }

        if (meshRenderers.Count == 0)
        {
            Debug.LogWarning("No MeshRenderers found to combine.");
            return;
        }

        List<CombineInstance> combineInstances = new List<CombineInstance>();

        foreach (MeshRenderer mr in meshRenderers)
        {
            MeshFilter mf = mr.GetComponent<MeshFilter>();
            if (mf != null && mf.sharedMesh != null)
            {
                CombineInstance ci = new CombineInstance();
                ci.mesh = mf.sharedMesh;
                ci.transform = transform.worldToLocalMatrix * mr.transform.localToWorldMatrix;
                combineInstances.Add(ci);
                mr.enabled = false;
            }
        }

        if (combineInstances.Count == 0)
        {
            Debug.LogWarning("No meshes to combine.");
            return;
        }

        Mesh combinedMesh = new Mesh();
        combinedMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        combinedMesh.CombineMeshes(combineInstances.ToArray(), true, true);

        _combinedMeshFilter = GetComponent<MeshFilter>();
        if (_combinedMeshFilter == null)
        {
            _combinedMeshFilter = gameObject.AddComponent<MeshFilter>();
        }
        _combinedMeshFilter.mesh = combinedMesh;

        _combinedMeshRenderer = GetComponent<MeshRenderer>();
        if (_combinedMeshRenderer == null)
        {
            _combinedMeshRenderer = gameObject.AddComponent<MeshRenderer>();
        }
        _combinedMeshRenderer.sharedMaterial = meshRenderers[0]?.sharedMaterial;

        _isCombined = true;
    }

    private void SeparateMeshes()
    {
        if (_combinedMeshFilter != null)
        {
            Destroy(_combinedMeshFilter);
            _combinedMeshFilter = null;
        }

        if (_combinedMeshRenderer != null)
        {
            Destroy(_combinedMeshRenderer);
            _combinedMeshRenderer = null;
        }

        foreach (var block in _blocks)
        {
            MeshRenderer mr = block.GetComponent<MeshRenderer>();
            if (mr != null)
            {
                mr.enabled = true;
            }
        }

        _isCombined = false;
    }

    private void FixedUpdate()
    {
        if (_canRunProgram)
        {
            float chunkRadius = _chunkSize * 2f;
            float distanceToPlayer = Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z), new Vector3(WorldManager.instance._player.transform.position.x, 0, WorldManager.instance._player.transform.position.z));

            if (_isCombined && distanceToPlayer < chunkRadius)
            {
                SeparateMeshes();
            }
            else if (!_isCombined && distanceToPlayer >= chunkRadius)
            {
                CombineMeshes();
            }
        }
    }
}