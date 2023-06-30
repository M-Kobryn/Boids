using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BoidBehavior : MonoBehaviour
{
    List<GameObject> mates = new List<GameObject>();
    static double radius = 3;
    static float speed = 0.1f;


    public float sepScale;
    public float alignScale;
    public float cohScale;
    // Start is called before the first frame update
    void Start()
    {
       // GetComponent<Rigidbody2D>().AddForce(Random.onUnitSphere * speed/2);
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 direction = Vector2.zero;
        getNearBoids();
        direction += SeparationRule();
        direction += AlignmentRule();
        direction += CohesionRule();
        //direction += FollowChiefRule();
        GetComponent<Rigidbody2D>().velocity = direction * speed;

    }

    void getNearBoids() 
    {
        mates = new List<GameObject>();
        GameObject[] candidates = GameObject.FindGameObjectsWithTag("Boid");
        foreach (GameObject candidate in candidates) 
        {
            if (Vector3.Distance(candidate.transform.position, transform.position) <= radius)
            { 
                mates.Add(candidate);
            }
        }
        Debug.Log(mates.Count);
    }
    Vector2 SeparationRule() 
    {
        Vector3 direction = new Vector3(0,0,0);
        foreach (GameObject obj in mates)
        {
            direction += (transform.position - obj.transform.position).normalized * Mathf.Min(1 / (Vector3.Distance(transform.position, obj.transform.position)), 1f) *sepScale;
        }
        Debug.Log(direction);
        return direction;
    }

    Vector2 AlignmentRule() 
    {
        Vector2 direction = new Vector2(0, 0);
        foreach (GameObject obj in mates)
        {
            direction += obj.GetComponent<Rigidbody2D>().velocity.normalized;
        }
        return direction * alignScale;

    }
    Vector2 CohesionRule() 
    {
        Vector3 center = new Vector3(0, 0);
        foreach (GameObject obj in mates)
        {
            center += obj.transform.position;
        }
        Vector2 direction = (transform.position - center).normalized * Mathf.Min(1 / (Vector3.Distance(transform.position, center)), 1.0f) * cohScale;
        return direction;

    }
    Vector2 FollowChiefRule() 
    {
        GameObject obj = GameObject.Find("Player");
        Vector2 direction = new Vector2(0, 0);
        if (obj != null)
        {
            direction = -(transform.position - obj.transform.position).normalized * Mathf.Max((Vector3.Distance(transform.position, obj.transform.position)), 1.0f) * 1.5f;
        }
        return direction;
        
    }
}
