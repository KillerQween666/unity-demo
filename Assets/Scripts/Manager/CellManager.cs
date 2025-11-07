using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Tilemaps.TilemapRenderer;

public class CellManager : MonoBehaviour {
    public static CellManager Instance { get; private set; }

    public List<Cell> cellList = new List<Cell>();

    public GameObject[] tombstonePrefabs;

    private bool isStartSpawn = false;

    public float spawnTime = 5f;
    private float spawnTimer = 0;

    public List<Cell> allCell = new List<Cell>();
    private List<Cell> canSpawnZombieCells = new List<Cell>();

    private void Awake() {
        Instance = this;

        foreach (var cell in FindObjectsOfType<Cell>()) {
            allCell.Add(cell);
            if (cell.canSpawnBrave) cellList.Add(cell);
            if (cell.canSpawnWaterZombie) canSpawnZombieCells.Add(cell);
        }
    }

    public void OpenAllCollider2D() {
        foreach (var cell in allCell) {
            cell.OpenCollider2D();
        }
    }

    private void Update() {
        if (isStartSpawn) {
            spawnTimer += Time.deltaTime;
            if (spawnTimer > spawnTime) {
                spawnTimer = 0;

                if (IsCanSpawn()) {
                    AudioManager.Instance.PlayClip(Config.spawnGrave);
                    SpawnTombstone();
                }
               
            }
        }
    }

    public void SetSpawnTime(float time) {
        spawnTime = time;
    }

    private bool IsCanSpawn() {
        foreach (var cell in cellList) {
            if (!cell.isBrave) {
                return true;
            }
        }
        return false;
    }

    public void StartSpawnTombstones(int count) {
        if (IsCanSpawn()) {
            AudioManager.Instance.PlayClip(Config.spawnGrave);
            for (int i = 0; i < count; i++) {
                SpawnTombstone();
            }
        }
    }

    private void SpawnTombstone() {
        var emptyCellList = new List<Cell>();
        foreach (var cell in cellList) {
            if (!cell.isBrave && !cell.isHole && cell.currentPlant == null) {
                emptyCellList.Add(cell);
            }
        }
        if (emptyCellList.Count > 0) {
            SpawnOnCell(emptyCellList[UnityEngine.Random.Range(0, emptyCellList.Count)]);
            return;
        }

        var plantCellList = new List<Cell>();
        foreach (var cell in cellList) {
            if (!cell.isBrave && cell.currentPlant != null) {
                plantCellList.Add(cell);
            }
        }
        if (plantCellList.Count > 0) {
            var target = plantCellList[UnityEngine.Random.Range(0, plantCellList.Count)];
            target.SubPlant();
            SpawnOnCell(target);
            return;
        }

        var holeCellList = new List<Cell>();
        foreach (var cell in cellList) {
            if (!cell.isBrave && cell.isHole) {
                holeCellList.Add(cell);
            }
        }
        if (holeCellList.Count > 0) {
            SpawnOnCell(holeCellList[UnityEngine.Random.Range(0, holeCellList.Count)]);
            return;
        }
    }

    private void SpawnOnCell(Cell cell) {
        StartCoroutine(SpawnOnCellTombstone(cell));
    }

    IEnumerator SpawnOnCellTombstone(Cell cell) {
        Vector3 position = cell.transform.position;
        position.y -= 0.5f;

        cell.isBrave = true;
        ObjectPoolManager.Instance.PlayDirtSmallParticalIEnumrator(position);

        yield return new WaitForSeconds(1);

        GameObject tombstone = tombstonePrefabs[UnityEngine.Random.Range(0, tombstonePrefabs.Length)];

        tombstone.GetComponent<Grave>().row = cell.row;
        SpriteRenderer[] sprites = tombstone.GetComponentsInChildren<SpriteRenderer>(true);
        foreach (var sprite in sprites) {
            sprite.sortingLayerName = ZombieManager.Instance.layerNames[cell.row];
        }

        cell.bravePrefab = Instantiate(tombstone, position, Quaternion.identity);
        cell.bravePrefab.transform.DOMoveY(cell.transform.position.y, 0.2f);
    }

    public void StartSpawn(int count) {
        isStartSpawn = true;
        StartSpawnTombstones(count);
    }

    public void EndSpawn() {
        isStartSpawn = false;
    }

    public List<Cell> GetAllHaveBraveCell() {
        List<Cell> tombstonePositions = new List<Cell>();
        foreach (var cell in cellList) {
            if (cell.isBrave) {
                tombstonePositions.Add(cell);
            }
        }
        return tombstonePositions;
    }

    public List<Cell> GetAllCanSpawnWaterZombieCell() {
        return canSpawnZombieCells;
    }
}
