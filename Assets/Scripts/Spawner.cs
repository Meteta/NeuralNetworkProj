using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public int numberToSpawn;
    public int spawnForce;
    public float spawnDelay = 3f;
    public GameObject objectToSpawn;
    public enum SpawnerType : int
    {
        Organic,
        Actor,
        Other
    }
    public SpawnerType spawnerType;
    public ActorBehaviour.Species species;
    private int spawnCount = 0;
    public int SpawnCount
    {
        get
        {
            return spawnCount;
        }
        set
        {
            spawnCount = value;
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("SpawnObjects",0f,spawnDelay);
    }
    public void SpawnObjects()
    {
        int currNumberToSpawn = numberToSpawn - spawnCount;
        float force = UnityEngine.Random.Range(0.1f, 2f) * spawnForce;
        Vector2 randomVector = new Vector2(UnityEngine.Random.Range(-1f,1f), UnityEngine.Random.Range(-1f,1f));
        Quaternion randomRotation = Quaternion.Euler(0, 0, UnityEngine.Random.Range(0.0f, 360.0f));


        if(currNumberToSpawn > 0)
        {
            spawnCount += 1;
            GameObject new_object = Instantiate(objectToSpawn, transform.position, randomRotation);
            // new_object.GetComponent<Rigidbody2D>().AddForce(randomVector.normalized * force, ForceMode2D.Impulse);
            new_object.GetComponent<Rigidbody2D>().velocity = randomVector.normalized * force;
            if (spawnerType == SpawnerType.Organic){
                OrganicsBehaviour behaviour_script = new_object.GetComponent<OrganicsBehaviour>();
                behaviour_script.spawner = this;
            } else if (spawnerType == SpawnerType.Actor){
                ActorBehaviour behaviour_script = new_object.GetComponent<ActorBehaviour>();
                behaviour_script.spawner = this;
                behaviour_script.race = species;
            }
            
        }
    }

}
