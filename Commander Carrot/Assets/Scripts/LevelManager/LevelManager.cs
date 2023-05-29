using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    //TODO build hole

    [Header("Terrain: Add these in!")]
    [SerializeField] Terrain terrain;
    /*[SerializeField]*/ Transform terrainTransform;

    [Header("Lists for the Level building blocks")]
    [SerializeField] GameObject[] spawnLayouts;
    [SerializeField] GameObject[] holePrefabList;
    [SerializeField] GameObject[] housePrefabList;

    
    Vector3 positionOfSpawnPoint;
    Transform[] spawnPointList;
    Transform spawnPoint;
    List<GameObject> listOfSpawnedObjects = new List<GameObject>();

    int holeWidth = 2;
    int terrainResolution;
    float terrainScale;

    [SerializeField][Range(0f, 1f)] float spawnProbability = 0.8f;

    private void Start()
    {
        if(terrain == null)
            Debug.LogWarning("NO TERRAIN REFERENCED BY LevelManager");
        terrainTransform = terrain.transform;

        terrainResolution = terrain.terrainData.heightmapResolution;
        terrainScale = terrain.terrainData.size.x / terrainResolution;
        
        SpawnNewLevel();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N)) { SpawnNewLevel(); }
    }
    public void SpawnNewLevel()
    {
        ResetTerrain();

        int spawnLayoutIndex = GetRandomIndex(spawnLayouts.Length);
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

        if(UnityEngine.Random.value < spawnProbability)
        {
            //Spawn the hole
            GameObject holePrefab = holePrefabList[GetRandomIndex(holePrefabList.Length)];
            GameObject hole = Instantiate(holePrefab, spawnPoint.position, spawnPoint.rotation);
            AlignBuildingContents(hole.transform);

            //Add Spawned Objects to list
            listOfSpawnedObjects.Add(hole);
        }

        if (UnityEngine.Random.value < spawnProbability)
        {
            //Spawn the building
            GameObject housePrefab = housePrefabList[GetRandomIndex(housePrefabList.Length)];
            GameObject house = Instantiate(housePrefab, spawnPoint.position, spawnPoint.rotation);
            AlignBuildingContents(house.transform);

            //Add Spawned Objects to list
            listOfSpawnedObjects.Add(house);
        }


        
        

    }
    void ResetTerrain()
    {
        int resolution = terrainResolution - 1;

        var b = new bool[resolution, resolution];

        for (var x = 0; x < resolution; x++)
            for (var y = 0; y < resolution; y++)
                b[x, y] = true;

        terrain.terrainData.SetHoles(0, 0, b);

        foreach (GameObject g in listOfSpawnedObjects)
        {
            Destroy(g);
        }
        listOfSpawnedObjects = new List<GameObject>();
    }

   
    int GetRandomIndex(int indexMax)
    {
        int rng = UnityEngine.Random.Range(0, indexMax);
        return rng;
    }

    void AlignBuildingContents(Transform building)
    {
        if(!building.GetComponentInChildren<MoveStyleTrigger>())
            return;

        foreach (Pickup p in building.GetComponentsInChildren<Pickup>())
        {
            Vector3 localPos = p.transform.localPosition;
            p.transform.localPosition = new Vector3(localPos.x, localPos.y, 0);
        }
        foreach (RigidbodyControl rbc in building.GetComponentsInChildren<RigidbodyControl>())
        {
            rbc.enabled = true;
            rbc.ConstrainToTransformPlane(building);
        }
    }
}
