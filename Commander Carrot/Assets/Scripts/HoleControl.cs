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
    [SerializeField] GameObject holePrefab;

    int holeWidth = 2;
    Vector3 pos;

    float terrainScale;


    private void Start()
    {
        ResetTerrain();

        terrainScale = terrain.terrainData.size.x / terrain.terrainData.heightmapResolution;

        pos = terrainTransform.InverseTransformPoint(spawnPoint.position)
            * (terrain.terrainData.heightmapResolution / terrain.terrainData.size.x);

        pos = new Vector3(Mathf.Floor(pos.x), 0, Mathf.Floor(pos.z));

        spawnPoint.position = terrainTransform.TransformPoint(
            pos * terrainScale);

        CreateHole();

        spawnPoint.position = new Vector3(spawnPoint.position.x + (terrainScale / 2), 0, spawnPoint.position.z + (terrainScale / 2));
        Instantiate(holePrefab, spawnPoint.position, Quaternion.identity);
        
    }

    void CreateHole()
    {
        //Vector3 pos = terrainTransform.InverseTransformPoint(spawnPoint.position)
        //    * (terrain.terrainData.heightmapResolution / terrain.terrainData.size.x);

        Debug.Log(pos);

        int xPos = Convert.ToInt32(pos.x - (holeWidth / 2));
        int zPos = Convert.ToInt32(pos.z - (holeWidth / 2));

        var b = new bool[holeWidth, holeWidth];

        for (var x = 0; x < holeWidth; x++)
            for (var y = 0; y < holeWidth; y++)
                b[x, y] = !(x >= 0 && x <= holeWidth && y >= 0 && y <= holeWidth);

        terrain.terrainData.SetHoles(xPos, zPos, b);
    }
    void ResetTerrain()
    {
        int resolution = terrain.terrainData.heightmapResolution - 1;

        var b = new bool[resolution, resolution];

        for (var x = 0; x < resolution; x++)
            for (var y = 0; y < resolution; y++)
                b[x, y] = true;

        terrain.terrainData.SetHoles(0, 0, b);
    }
}
