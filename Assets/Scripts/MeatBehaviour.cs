using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeatBehaviour : MonoBehaviour
{
    public Spawner spawner;
    private Rigidbody2D rigidBody2D;
    public float organicSize = 1f; // Organic Size is equal to an organics health. If the organic runs out of health it de-spawns


    void OnCollisionEnter2D(Collision2D col){
        GameObject other = col.gameObject;
    }
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("LifeCycle",0f,0.1f);
    }

    public void LifeCycle(){
        float currScale = organicSize * 0.005f;
        transform.localScale = new Vector3(currScale, currScale, currScale);
        organicSize -= 0.00001f;

        if(organicSize <= 0){
            RemoveOrganic();
        }
    }
    void RemoveOrganic()
    {
        Destroy(gameObject);
        spawner.SpawnCount -= 1;
    }
}
