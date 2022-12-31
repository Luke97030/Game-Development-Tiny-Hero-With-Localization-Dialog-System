using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RockController : MonoBehaviour
{
    public enum RockHit { Player, Enemy, Nothing }
    public RockHit rockHitStatus;
    private Rigidbody rb;

    // rock damage when hitting player or enemy 
    public int damage = 10;

    [Header("Basic Settings")]
    public float force;
    public GameObject targetObj;
    private Vector3 direction;

    public GameObject rockPartitionEffect;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        // to skip the if condition in the FixedUpdate() 
        rb.velocity = Vector3.one;
        // when the Rock is created, the target must be player first
        rockHitStatus = RockHit.Player;
        // when the rock be created, call the throwTowardTarget function
        throwTowardTarget();
        
    }

    // Using FixedUpdate for physical game object
    private void FixedUpdate()
    {
        // rb.velocity.sqrMagnitude < 1 means the rock stop
        if (rb.velocity.sqrMagnitude < 1f)
        {
            rockHitStatus = RockHit.Nothing;
        }
        // get the length of Vector3(rb.velocity)
        //Debug.Log(rb.velocity.sqrMagnitude);
    }


    public void throwTowardTarget()
    {
        if (targetObj == null)
            targetObj = FindObjectOfType<PlayerController>().gameObject;
        // get the direction 
        // adding (0,1,0), we do not want the rock fall down so fast 
        direction = (targetObj.transform.position - transform.position + Vector3.up).normalized;
        rb.AddForce(direction * force, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision collider)
    {
        switch (rockHitStatus)
        {
            case RockHit.Player:
                if (collider.gameObject.CompareTag("Player"))
                {
                    // the collider belongs to player
                    collider.gameObject.GetComponent<NavMeshAgent>().isStopped = true;
                    collider.gameObject.GetComponent<NavMeshAgent>().velocity = direction * force;
                    // this will prevent the player running back to the position where he got hit
                    collider.gameObject.GetComponent<NavMeshAgent>().ResetPath();
                    collider.gameObject.GetComponent<Animator>().SetTrigger("dizzy");
                    collider.gameObject.GetComponent<CharacterData>().causeDamage(damage, collider.gameObject.GetComponent<CharacterData>());

                    rockHitStatus = RockHit.Nothing;
                }
                break;
            case RockHit.Enemy:
                // if the rock hit the Golem 
                if (collider.gameObject.GetComponent<EnemyGolemController>())
                {
                    var otherStats = collider.gameObject.GetComponent<CharacterData>();
                    otherStats.causeDamage(damage, otherStats);
                    Instantiate(rockPartitionEffect, transform.position, Quaternion.identity);
                    Destroy(gameObject);
                }
                break;
            //case RockHit.Nothing:
            //    rockHit = RockHit.Nothing;
            //    break;
        }
    }
}
