using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// Adding some logic for Grunt's skill attack
public class EnemyChestMonsterController : EnemyController
{
    // adding the push back the play animation logic
    [Header("Skill Attack Info")]
    public float pushBackForce = 15;

    public void pushBack()
    {
        if (playerObj != null)
        {
            // Look at player before push
            //transform.LookAt(playerObj.transform);

            // using player's transform - enemyGrunt's transform to get the direction
            Vector3 direction = playerObj.transform.position - transform.position;
            // get the opposite direction
            direction.Normalize();

            // stop the player's nav mash agent
            playerObj.GetComponent<NavMeshAgent>().isStopped = true;
            // push back the player
            playerObj.GetComponent<NavMeshAgent>().velocity = direction * pushBackForce;
            playerObj.GetComponent<NavMeshAgent>().ResetPath();

            if (transform.isFacingPlayer(playerObj.transform))
            {
                // play the dizzy animation when player got push back 
                playerObj.GetComponent<Animator>().SetTrigger("dizzy");
            }

        }
    }

    override protected void performAttack()
    {
        // look at the target(player)
        transform.LookAt(playerObj.transform);

        if (playerInEnemyAttackRange())
        {
            //enemyAnimator.ResetTrigger("skillAttack");
            // normal attack animation
            enemyAnimator.SetTrigger("basicAttack");
        }
        else if (playerInEnemySkillRange())
        {
            //enemyAnimator.ResetTrigger("basicAttack");
            // skill attack animation
            enemyAnimator.SetTrigger("skillAttack");
        }    
    }
}

