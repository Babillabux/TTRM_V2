using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

public class WorldManager : MonoBehaviour
{
    public BlocksRegistery _blocksRegistery;
    private GameObject _blockPrefab;
    [SerializeField] GameObject _playerPrefab, _chunkPrefab;
    public GameObject _player;

    [Header("Player Chunk Location")]
    public Dictionary<Vector3Int, GameObject> _chunkPosition = new Dictionary<Vector3Int, GameObject>();

    [Header("World Configuration")]
    public int _chunkSize;
    public int _renderDistance;
    [SerializeField] int _chunkMaxLayer, _seed;

    private Vector3Int _lastPlayerChunkCoord;

    public static WorldManager instance;
    private void Awake()
    {
        if(instance != null)
        {
            Debug.LogWarning("There is more than one instance of WorldManager");
            return;
        }

        instance = this;
    }

    private void Start()
    {
        if(_blocksRegistery != null)
        {
            _blockPrefab = _blocksRegistery._blockPrefab;

            _player = Instantiate(_playerPrefab, new Vector3(_chunkSize / 2, _chunkSize +2, _chunkSize / 2), Quaternion.identity);
            _player.transform.SetParent(transform);

            //GenerateWorld();

            _lastPlayerChunkCoord = GetPlayerChunkCoord();
            GenerateChunksAroundPlayer();
        }
    }

    private void Update()
    {
        Vector3Int currentChunkCoord = GetPlayerChunkCoord();

        if (currentChunkCoord != _lastPlayerChunkCoord)
        {
            _lastPlayerChunkCoord = currentChunkCoord;
            GenerateChunksAroundPlayer();
        }
    }

    private Vector3Int GetPlayerChunkCoord()
    {
        Vector3 playerPos = _player.transform.position;

        return new Vector3Int(
            Mathf.FloorToInt(playerPos.x / _chunkSize) * _chunkSize,
            0,
            Mathf.FloorToInt(playerPos.z / _chunkSize) * _chunkSize
            );
    }

    private void GenerateChunksAroundPlayer()
    {
        Vector3Int playerChunkCoord = _lastPlayerChunkCoord;

        List<Vector3Int> chunksToActivate = new List<Vector3Int>();
        List<Vector3Int> chunksToDeactivate = new List<Vector3Int>();

        for (int x = -_renderDistance; x <= _renderDistance; x++)
        {
            for (int z = -_renderDistance; z <= _renderDistance; z++)
            {
                Vector3Int chunkCoord = new Vector3Int(playerChunkCoord.x + x * _chunkSize, 0, playerChunkCoord.z + z * _chunkSize);
                chunksToActivate.Add(chunkCoord);

                if (!_chunkPosition.ContainsKey(chunkCoord))
                {
                    CreateChunk(chunkCoord);
                }
            }
        }

        foreach (var chunk in _chunkPosition)
        {
            Vector3Int chunkCoord = chunk.Key;
            float distanceToPlayer = Vector3.Distance(new Vector3(chunkCoord.x, 0, chunkCoord.z), new Vector3(playerChunkCoord.x, 0, playerChunkCoord.z));

            if (distanceToPlayer > _renderDistance * _chunkSize)
            {
                chunksToDeactivate.Add(chunkCoord);
            }
        }

        foreach (var chunkCoord in chunksToActivate)
        {
            if (_chunkPosition.ContainsKey(chunkCoord))
            {
                _chunkPosition[chunkCoord].SetActive(true);
            }
        }

        foreach (var chunkCoord in chunksToDeactivate)
        {
            if (_chunkPosition.ContainsKey(chunkCoord))
            {
                _chunkPosition[chunkCoord].SetActive(false);
            }
        }
    }

    private void CreateChunk(Vector3Int chunkCoord)
    {
        GameObject chunk = Instantiate(_chunkPrefab, chunkCoord, Quaternion.identity);
        chunk.transform.SetParent(transform);

        Chunk chunkSC = chunk.GetComponent<Chunk>();
        chunkSC._chunkSize = _chunkSize;
        chunkSC._blockPrefab = _blockPrefab;
        chunkSC._blocksRegistery = _blocksRegistery;
        chunkSC._chunkMaxLayer = _chunkMaxLayer;
        chunkSC.name = chunkCoord.ToString();
        chunkSC.GenerateChunk(chunkCoord, _seed);

        _chunkPosition.Add(chunkCoord, chunk);
    }
}
