using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.UIElements;


public class Movement : MonoBehaviour
{

    List<GameObject> mates = new List<GameObject>();
    public GameObject Leader;
    static double radius = 1.5;
    static float rotSpeed = 1f; // in degres

    public float coughtUpDistance;
    public float standartSpeed;
    public float maxSpeed;

    public float ForceValue;
    public Quaternion rotation;
    private Vector2 front;

    public float sepScale;
    public float alignScale;
    public float cohScale;
    public float leadScale;

    public bool fly;
    private int index;
    float minimalDistanceToBorder = 0.4f;
    private bool EnableBoidBehavior = true;
    private bool isInAvoid = false;
    private float omega = 0;
    Vector2 dir = Vector2.zero;
    int n = 0;
    int i = 0;
    // Start is called before the first frame update
    void Start()
    {
        getNearBoids();
        //transform.Rotate( 0,0, Random.Range(0,360));
    }

    // Update is called once per frame
    void Update()
    {
        index++;
        if (index == 100) 
        {
            getNearBoids();
            index = 0;
        }
        dir = Vector2.zero;
        if (DistanceToCameraBorder() < minimalDistanceToBorder && !isInAvoid)
        {
            dir = -transform.up;
            omega = (180 * standartSpeed)/ (DistanceToCameraBorder()/3);
            n =  (int) (180 / omega);
            i = 0;
            isInAvoid = true;
            tag = "Untagged";
            EnableBoidBehavior = false;
            GetComponent<CircleCollider2D>().enabled = false;
        }
        else if (DistanceToCameraBorder() < (minimalDistanceToBorder * 2) && isInAvoid )
        {
            if (i < n)
            {
                i++;
                transform.Rotate(new Vector3(0, 0, omega));
            }
            transform.position += transform.up * standartSpeed;
        }
        else 
        {
            EnableBoidBehavior = true;
            isInAvoid = false;
            tag = "Boid";
            GetComponent<CircleCollider2D>().enabled = true;
        }
        if (EnableBoidBehavior) 
        {
            dir += SeparationRule();
            dir += AlignmentRule();
            dir += CohesionRule();
            dir += FollowLeaderRule();
            float rot = Vector2.SignedAngle(transform.up, dir);
            Debug.DrawLine(transform.position, transform.position + new Vector3 (dir.x, dir.y, transform.position.z), Color.white);
            transform.Rotate(new Vector3(0, 0, Mathf.Abs(rot) < rotSpeed ? rot : rotSpeed * Mathf.Sign(rot)));
            Debug.Log(DistanceToCameraBorder());
            
            if (fly)
            {
                transform.position += transform.up * standartSpeed;
            }
        } 
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

    }

    Vector2 SeparationRule()
    {
        Vector3 direction = new Vector3(0, 0, 0);
        foreach (GameObject obj in mates)
        {
            direction += (transform.position - obj.transform.position).normalized * Mathf.Min(1 / (Vector3.Distance(transform.position, obj.transform.position)), 1f)* sepScale;
        }
        Debug.DrawLine(transform.position, transform.position + direction,Color.red);
        return direction;
    }
    Vector2 AlignmentRule()
    {
        Vector3 direction = new Vector3(0, 0, 0);
        foreach (GameObject obj in mates)
        {
            direction += obj.transform.up;
        }
        direction = direction / mates.Count;
        Debug.DrawLine(transform.position, transform.position + (direction * alignScale), Color.yellow);
        return direction * alignScale;
    }
    Vector2 CohesionRule()
    {
        Vector3 center = new Vector3(0, 0);
        foreach (GameObject obj in mates)
        {
            center += obj.transform.position;
        }
        center = center / mates.Count;
        Vector2 direction = (transform.position - center).normalized * Mathf.Min(Vector3.Distance(transform.position, center), 1.0f) * cohScale;
        Debug.DrawLine(transform.position, transform.position + new Vector3(-direction.x, -direction.y, transform.position.z), Color.blue);
        return -direction;

    }

    Vector2 FollowLeaderRule()
    {
        Vector3 direction = new Vector3(0, 0);
        if (Vector3.Distance(Leader.transform.position, transform.position) <= radius*10) 
        {
            direction += (Leader.transform.position - transform.position).normalized * Mathf.Min(Vector3.Distance(transform.position, Leader.transform.position), 1f) * leadScale;

        }
        return direction;

    }

    float DistanceToCameraBorder() 
    {
        RaycastHit2D[] rayArray = new RaycastHit2D[4];
        Vector2[] directions = { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
        for (int i =0; i < 4; i++) 
        {
            rayArray[i] = Physics2D.Raycast(transform.position, directions[i], Mathf.Infinity, LayerMask.GetMask("Walls"));
        }
        return rayArray.Min(x => x.distance);

    }
}
