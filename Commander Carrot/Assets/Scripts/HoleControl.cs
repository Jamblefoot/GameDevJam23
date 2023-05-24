using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoleControl : MonoBehaviour
{
    //TODO build hole

    [SerializeField] Terrain terrain;
    [SerializeField] Transform terrainTransform;
    [SerializeField] Transform spawnPoint;
    [SerializeField] GameObject[] holePrefabList;

    int holeWidth = 2;
    Vector3 positionOfSpawnPoint;

    float terrainScale;
    int terrainResolution;

   
    private void Start()
    {
        terrainResolution = terrain.terrainData.heightmapResolution;
        terrainScale = terrain.terrainData.size.x / terrainResolution;

        ResetTerrain();
        CreateHole();
        InstantiateLevel();  
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
        Instantiate(holePrefab, spawnPoint.position, Quaternion.identity);

        //Spawn the building

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

    public void InstantiateNewLevel()
    {

    }
    int GetRandomIndex(int indexMax)
    {
        int rng = UnityEngine.Random.Range(0, indexMax);
        return rng;
    }
}
