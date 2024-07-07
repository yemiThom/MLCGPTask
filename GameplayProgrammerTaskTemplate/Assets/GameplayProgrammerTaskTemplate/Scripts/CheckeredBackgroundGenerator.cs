using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckeredBackgroundGenerator : MonoBehaviour
{
    [SerializeField] private GameObject _whiteTilePrefab;
    [SerializeField] private GameObject _blackTilePrefab;
    [SerializeField] private int _gridWidth = 10;
    [SerializeField] private int _gridHeight = 10;
    [SerializeField] private float _tileSize = 1.0f;
    [SerializeField] private float _scrollSpeed = 2.0f;

    private Transform[,] _tiles;
    private float _screenWidth;
    private float _screenHeight;

    private void Start()
    {
        _screenWidth = Camera.main.orthographicSize * 2.0f;
        _screenHeight = Camera.main.orthographicSize * 2.0f;
        GenerateCheckeredBackground();
    }

    private void Update()
    {
        ScrollBackground();
    }

    private void GenerateCheckeredBackground()
    {
        GameObject tilesParent = new GameObject();
        tilesParent.transform.position = Vector3.zero;
        tilesParent.name = "ScrollingBackgroundTiles";

        _tiles = new Transform[_gridWidth, _gridHeight];
        Vector3 gridStartPos = new Vector3(-_gridWidth * _tileSize / 2 + _tileSize / 2, -_gridHeight * _tileSize / 2 + _tileSize / 2, 0);

        for (int y = 0; y < _gridHeight; y++)
        {
            for (int x = 0; x < _gridWidth; x++)
            {
                GameObject tilePrefab = (x + y) % 2 == 0 ? _whiteTilePrefab : _blackTilePrefab;
                Vector3 position = gridStartPos + new Vector3(x * _tileSize, y * _tileSize, 0);
                GameObject tile = Instantiate(tilePrefab, position, Quaternion.identity, tilesParent.transform);
                _tiles[x, y] = tile.transform;
            }
        }
    }

    private void ScrollBackground()
    {
        for (int y = 0; y < _gridHeight; y++)
        {
            for (int x = 0; x < _gridWidth; x++)
            {
                Transform tile = _tiles[x, y];
                Vector3 position = tile.localPosition;
                position.y -= Time.deltaTime * _scrollSpeed;
                position.x -= Time.deltaTime * _scrollSpeed;

                if(position.x < -_screenWidth - _tileSize)
                    position.x += _gridWidth * _tileSize;

                if(position.y < -_screenHeight / 2 - _tileSize / 2)
                    position.y += _gridHeight * _tileSize;

                tile.localPosition = position;
            }
        }
    }
}
