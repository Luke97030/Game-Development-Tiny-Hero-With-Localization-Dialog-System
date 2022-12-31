using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyGolemController : EnemyController
{ 
    // adding the push back the play animation logic
    [Header("Skill Attack Info")]
    public float pushBackForce = 25;

    // variables for rock 
    public GameObject rockPrefab;
    public Transform throwPosition;
    
    // Attacking animation event for Attack2 
    public void pushBack()
    {
        // is there a player obj and Golem facing it,
        if (playerObj != null && transform.isFacingPlayer(playerObj.transform))
        {
            // if true, cuase damage
            Vector3 direction = playerObj.transform.position - transform.position;
            direction.Normalize();
            playerObj.GetComponent<NavMeshAgent>().isStopped = true;
            playerObj.GetComponent<NavMeshAgent>().velocity = direction * pushBackForce;
            playerObj.GetComponent<NavMeshAgent>().ResetPath();

            if (transform.isFacingPlayer(playerObj.transform))
            {
                // play the dizzy animation when player got push back 
                playerObj.GetComponent<Animator>().SetTrigger("dizzy");
            }
            var targetData = playerObj.GetComponent<CharacterData>();         
            targetData.causeDamage(characterData, targetData);
        }
    }

    // Attacking animation event for Attack1
    public void throwRock()
    {    
            // creating the rock prefab at throwPosition.position 
            GameObject rock = Instantiate(rockPrefab, throwPosition.position, Quaternion.identity);
            //if (rock.GetComponent<RockController>().targetObj == null)
            //    rock.GetComponent<RockController>().targetObj = FindObjectOfType<PlayerController>().gameObject;
            // set the target of rock to be playerObj
            rock.GetComponent<RockController>().targetObj = playerObj;
       
    }

}
