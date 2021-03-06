using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class Zombie : MonoBehaviour
{
    [Header("Public")]
    public Transform postoWalkTo;
    public float health = 3;
    public float MoveForce = 30f;
    public float RangeOfNextPath = 5f;

    [Header("Compontes")]
    private NavMeshPath navPath;
    private Rigidbody rb;
    [Header("privates")]
    private int waypointCount = 1;

    private void Start()
    {
        navPath = new NavMeshPath();
        rb = GetComponent<Rigidbody>();

    }

    public void takeDamage(int damage)
    {
        Debug.Log("got hit for "+ damage);

        health -= damage;
        if (health <= 0)
        {
            Game_Mannger.Instance.removeZombieFromList(this);
            Destroy(gameObject);
        }
    }

    private void FixedUpdate()
    {
        rb.AddForce(GoToPoint());
    }

    private void Update()
    {
        if(Game_Mannger.Instance.survivor == null) return;
        //Debug.Log(Vector3.Distance(transform.position,Game_Mannger.Instance.survivor.transform.position));
        if (Vector3.Distance(transform.position,Game_Mannger.Instance.survivor.transform.position) < 3f)
        {
            Game_Mannger.Instance.survivor.takeDamage();
        }
    }


    private Vector3 GoToPoint()
    {
        //TODO should not happen every frame
        if (!NavMesh.CalculatePath(transform.position.removeY(), postoWalkTo.position, NavMesh.AllAreas, navPath))
            Debug.Log("goal is out of reach");
        waypointCount = 1;

        
        
        if(navPath.corners.Length <= 1) return Vector3.zero;

        if ((transform.position - navPath.corners[waypointCount]).magnitude < RangeOfNextPath)
        {
            if(navPath.corners.Length <= waypointCount)
                waypointCount++;
        }

        return (navPath.corners[waypointCount].removeY() - transform.position.removeY() ).normalized*MoveForce;
    }

    private void OnDrawGizmos()
    {
        if(navPath == null) return;
        if(navPath.corners.Length <= 1) return;
        
        for (int i = 0; i < navPath.corners.Length; i++)
        {
#if UNITY_EDITOR
            Handles.DrawSolidDisc(navPath.corners[i],Vector3.up, 0.25f);
            #endif
        }
        
        Gizmos.DrawRay(transform.position.removeY(), (navPath.corners[waypointCount].removeY() - transform.position.removeY()).normalized*MoveForce);
    }
}
