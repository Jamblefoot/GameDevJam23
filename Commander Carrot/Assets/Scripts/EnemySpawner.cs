using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] List<GameObject> enemies;

    ShmupControl shmupControl;

    bool spawning;

    // Start is called before the first frame update
    void Start()
    {
        shmupControl = GetComponentInParent<ShmupControl>();

        //otherwise we need to check for perhaps a hole script with the relevant constraint plane stuff

        // or just wander i guess as like a foot soldier
    }

    public void StartSpawning(int count)
    {
        if(enemies == null || enemies.Count <= 0)
        {
            Debug.LogError("NO ENEMY PREFABS TO SPAWN!!!");
            return;
        }

        StartCoroutine(SpawnCo(count));
    }

    IEnumerator SpawnCo(int count)//, float intervalMin, float intervalMax)
    {
        int c = 0; 
        GameObject go = null;
        while(c < count)
        {
            if(shmupControl != null)
            {
                go = Instantiate(enemies[0], transform.position + Vector3.right * Random.Range(-shmupControl.width, shmupControl.width), transform.rotation);
                Rigidbody rb = go.GetComponent<Rigidbody>();
                rb.isKinematic = true;
                EnemyShip es = go.GetComponent<EnemyShip>();
                es.shmupControl = shmupControl;
                es.vertical = 0.5f;
                es.horizontal = Random.Range(-1f, 1f);
            }

            c++;
            yield return new WaitForSeconds(Random.Range(1f, 3f));
        }
    }
}
