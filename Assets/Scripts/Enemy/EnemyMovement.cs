using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyMovement : MonoBehaviour
{
    List<PlayerMovement> players;
    //Transform[] players;
    //PlayerHealth playerHealth;
    EnemyHealth enemyHealth;
    UnityEngine.AI.NavMeshAgent nav;

    void Awake ()
    {
        players = new List<PlayerMovement>();
        GameObject[] playerObjects = GameObject.FindGameObjectsWithTag ("Player");

        for (var i = 0; i < playerObjects.Length; i++)
        {
            players.Add(playerObjects[i].GetComponent<PlayerMovement>());
        }


    //        foreach (GameObject playa in playerObjects)
    //    {
    //        Debug.Log("Iterating a player");
    //        Debug.Log(playa);
    //        players.Add(playa.transform);
    //    }
        //playerHealth = player.GetComponent <PlayerHealth> ();
        enemyHealth = GetComponent <EnemyHealth> ();
        nav = GetComponent <UnityEngine.AI.NavMeshAgent> ();
    }

    Transform findClosestPlayer()
    {
        Transform closestPlayer = this.transform;
        float closestDistance = Mathf.Infinity;
        foreach(PlayerMovement playa in players)
        {
            
            float distanceFromTarget = Vector3.Distance(this.transform.position, playa.gameObject.transform.position);
            if (distanceFromTarget < closestDistance && playa.currentHealth > 0)
            {
                //We have a new closest target.
                closestPlayer = playa.gameObject.transform;
                closestDistance = distanceFromTarget;
            }
        }

        return closestPlayer; // Shenanigans will occur if this thing can't find any players
    }

    bool someoneLives()
    {
        foreach (PlayerMovement playa in players)
        {
            if (playa.currentHealth > 0)
            {
                return true;
            }
        }
        return false;
    }


    void Update ()
    {  
         
        if(enemyHealth.currentHealth > 0 && someoneLives())
        {
            nav.SetDestination(findClosestPlayer().position);
        }
        else
        {
            nav.enabled = false;
        }
    }
}
