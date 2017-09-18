using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyAttack : MonoBehaviour
{
    public float timeBetweenAttacks = 0.5f;
    public int attackDamage = 10;
    
    private Animator anim;
    private Dictionary<int, PlayerMovement> players;
    //PlayerHealth playerHealth;
    private EnemyHealth enemyHealth;
    private Dictionary<int, bool> playersInRange;
    //private List<bool> playersInRange;
    private float timer;

    void Awake ()
    {
        players = new Dictionary<int, PlayerMovement>();
        playersInRange = new Dictionary<int, bool>();
        //playersInRange = new List<bool>();
        GameObject[] playerObjects = GameObject.FindGameObjectsWithTag("Player");

        for (var i = 0; i < playerObjects.Length; i++)
        {
            int playerID = playerObjects[i].GetComponent<PlayerMovement>().getID();

            Debug.Log("Instance Added to dictionary: " + playerID);
            players.Add(playerID, playerObjects[i].GetComponent<PlayerMovement>());
            playersInRange.Add(playerID, false);
        }
        enemyHealth = GetComponent<EnemyHealth>();
        anim = GetComponent <Animator> ();
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //Debug.Log("Coming in constact with instance: " + other.GetInstanceID());
            playersInRange[other.GetComponent<PlayerMovement>().getID()] = true;
        }
    }
 
    void OnTriggerExit (Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //Debug.Log("Leaving Contact with instance: " + other.GetInstanceID());
            playersInRange[other.GetComponent<PlayerMovement>().getID()] = false;
        }
    }
    
    void Update ()
    {
        timer += Time.deltaTime;

        if (playersInRange.ContainsValue(true))
        {
            if (timer >= timeBetweenAttacks && enemyHealth.currentHealth > 0)
            {
                Attack();
            }
        }


        /* TO DO
        if(playerHealth.currentHealth <= 0)
        {
            anim.SetTrigger ("PlayerDead");
        }*/
    }


    void Attack ()
    {
        timer = 0f;
        foreach (int index in playersInRange.Keys)
        {
            Debug.Log("Have an index: " + index);
        }

            foreach (int touchKey in playersInRange.Keys)
        {
            Debug.Log("Checking instance number:  " + playersInRange[touchKey] + " with index: " + touchKey);
            if (playersInRange[touchKey] && players[touchKey].currentHealth > 0)
            {

                
                //PlayerMovement touchingPlayer = players[touchKey];
                //awDebug.Log("Attacking player" + touchingPlayer.getID());
                players[touchKey].TakeDamage(attackDamage);
                //touchingPlayer.TakeDamage(attackDamage);
            }

        }

        /* TO DO 
        timer = 0f;
        if(playerHealth.currentHealth > 0)
        {
            playerHealth.TakeDamage (attackDamage);
        } */
    }
}
