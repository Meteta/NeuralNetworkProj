using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrganicsBehaviour : MonoBehaviour
{
    public Spawner spawner;

    public float organicSize = 10; // Organic Size is equal to an organics health. If the organic runs out of health it de-spawns
    public float digestionSize = 0; // Digestion Size is the number of points it has saved up that it will grow.
    private bool growing = true;
    private bool killed = false;
    private bool dying = false;
    void OnCollisionEnter2D(Collision2D col){
        GameObject other = col.gameObject;
        // If we collide with another Organic, we want to combine organicSize and remove the remaining one.
        if (other.CompareTag("Organic"))
        {
            OrganicsBehaviour other_script = other.GetComponent<OrganicsBehaviour>();
            if(other_script.organicSize > organicSize && !other_script.dying && !dying){
                other_script.digestionSize += organicSize;
                killed = true;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("LifeCycle",0f,0.1f);
    }

    void SetDying(){
        dying = true;
    }

    public void LifeCycle(){
        float currScale = organicSize * 0.01f;
        transform.localScale = new Vector3(currScale, currScale, currScale);
        // If the organic is over 100 in size, stop growing.
        if(organicSize > 100)
        {
            Invoke("SetDying", 15);
            growing = false;
        } else if (organicSize <= 0)
        {
            RemoveOrganic();
        }
        
        if(digestionSize > 0)
        {
            organicSize ++;
            digestionSize --;
        }

        if(killed){
            organicSize -= 1f;
        } else if(growing){
            organicSize += UnityEngine.Random.Range(0.1f,0.5f);
        } else if (dying) {
            organicSize -= UnityEngine.Random.Range(0.1f,0.5f);
        }
    }
    void RemoveOrganic()
    {
        Destroy(gameObject);
        spawner.SpawnCount -= 1;
    }
}
