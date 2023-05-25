using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    //TODO build hole

    [SerializeField] Terrain terrain;
    [SerializeField] Transform terrainTransform;
    [SerializeField] GameObject[] spawnLayouts;
    [SerializeField] GameObject[] holePrefabList;
    [SerializeField] GameObject[] housePrefabList;

    
    Vector3 positionOfSpawnPoint;
    Transform[] spawnPointList;
    Transform spawnPoint;

    int holeWidth = 2;
    int terrainResolution;
    float terrainScale;

    private void Start()
    {
        terrainResolution = terrain.terrainData.heightmapResolution;
        terrainScale = terrain.terrainData.size.x / terrainResolution;

        

        ResetTerrain();
        SpawnNewLevel();
    }
    public void SpawnNewLevel()
    {
        int spawnLayoutIndex = UnityEngine.Random.Range(0, spawnLayouts.Length);
        spawnPointList = spawnLayouts[spawnLayoutIndex].GetComponentsInChildren<Transform>();

        //foreach (Transform sp in spawnPointList)
        for (int i = 1; i < spawnPointList.Length; i++)
        {
            spawnPoint = spawnPointList[i];
            CreateHole();
            InstantiateLevel();
        }
    }

    void AdjustSpawnPointToHoleCenter()
    {
        positionOfSpawnPoint = terrainTransform.InverseTransformPoint(spawnPoint.position)
            * (terrainResolution / terrain.terrainData.size.x);

        positionOfSpawnPoint = new Vector3(
            Mathf.Floor(positionOfSpawnPoint.x), 
            0, 
            Mathf.Floor(positionOfSpawnPoint.z)
        );

        spawnPoint.position = terrainTransform.TransformPoint(positionOfSpawnPoint * terrainScale);
    }
    void AdjustSpawnpointForInstatiation()
    {
        spawnPoint.position = new Vector3(
            spawnPoint.position.x + (terrainScale / 2), 
            0, 
            spawnPoint.position.z + (terrainScale / 2)
        );
    }
    void CreateHole()
    {
        AdjustSpawnPointToHoleCenter();

        int xPos = Convert.ToInt32(positionOfSpawnPoint.x - (holeWidth / 2));
        int zPos = Convert.ToInt32(positionOfSpawnPoint.z - (holeWidth / 2));

        var b = new bool[holeWidth, holeWidth];

        for (var x = 0; x < holeWidth; x++)
            for (var y = 0; y < holeWidth; y++)
                b[x, y] = !(x >= 0 && x <= holeWidth && y >= 0 && y <= holeWidth);

        terrain.terrainData.SetHoles(xPos, zPos, b);
    }
    void InstantiateLevel()
    {
        AdjustSpawnpointForInstatiation();

        //Spawn the hole
        GameObject holePrefab = holePrefabList[GetRandomIndex(holePrefabList.Length)];
        Instantiate(holePrefab, spawnPoint.position, spawnPoint.rotation);

        //Spawn the building
        GameObject housePrefab = housePrefabList[GetRandomIndex(housePrefabList.Length)];
        Instantiate(housePrefab, spawnPoint.position, spawnPoint.rotation);

        //Spawn the Fort


    }
    void ResetTerrain()
    {
        int resolution = terrainResolution - 1;

        var b = new bool[resolution, resolution];

        for (var x = 0; x < resolution; x++)
            for (var y = 0; y < resolution; y++)
                b[x, y] = true;

        terrain.terrainData.SetHoles(0, 0, b);
    }

   
    int GetRandomIndex(int indexMax)
    {
        int rng = UnityEngine.Random.Range(0, indexMax);
        return rng;
    }
}
