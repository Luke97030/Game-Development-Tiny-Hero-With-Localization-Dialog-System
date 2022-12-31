using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// adding enemy condition Enum
// 1. wandering 
// 2. chasing 
// 3. dead
public enum EnemyStates {
    STANDSTILL,
    WANDER, 
    CHASE, 
    DEAD
}

// require what game components enemy will have. Then when we drage the script into the enemy game object
// these componets will be automatically generate under the enemy game object. 
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(CharacterData))]
public class EnemyController : MonoBehaviour, IEndgameObserver
{
    private EnemyStates enemyState;
    private NavMeshAgent agent;
    private Collider enemyCollider;

    // enemy's targert => player
    // in order to use it in the child class, set the scope to protected
    protected GameObject playerObj;

    [Header("Basic Settings")]
    public float sightRadius;
    public bool isStandStill;
    // controll the speed of Nav mesh agent
    private float speed;

    // Variables for animation controller
    protected Animator enemyAnimator;
    private bool isWalk;
    private bool isBattleIdle;
    private bool isFollow;
    private bool isDead;
    private bool isPlayerDead;

    [Header("Wandering Settings")]
    public float wanderRange;
    private Vector3 enemyInitialPosition;
    private Quaternion enemyInitialRotation;
    private Vector3 wanderPoint;            // the random point enemy will walk to in the wanderRange
    public float wanderStagnantTime;
    private float remainingWanderStagnantTime;

    protected CharacterData characterData;
    private float lastAttackTime = 0.2f;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        enemyCollider = GetComponent<Collider>();
        // get the enemy initial speed
        speed = agent.speed;
        enemyAnimator = GetComponent<Animator>();
        // assign enemy initial position to a local variable 
        enemyInitialPosition = transform.position;
        enemyInitialRotation = transform.rotation;
        // will decreasing outOfChasingRangeRemainingTime in the updating function once the player is run out of the enemy's chasing range
        remainingWanderStagnantTime = wanderStagnantTime;
        characterData = GetComponent<CharacterData>();
    }

    private void Start()
    {
        // isStandStill == true, the enemy cannot perform patrolling 
        if (isStandStill)
        {
            enemyState = EnemyStates.STANDSTILL;
        }
        else
        {
            enemyState = EnemyStates.WANDER;
            // when initial, getting a random wander point, so in the Wander case of Update(), it know where to move to
            GetRandomWanderPoint();
        }

        // restore the currentHealth 
        characterData.CurrentHealth = characterData.MaxHealth;
        isPlayerDead = false;

        GameManager.singletonInstance.addToObserver(this);
    }

    // when enemy object be generated
    //private void OnEnable()
    //{
    //    GameManager.singleInstance.addToObserver(this);
    //}

    // when enemy object be destoried
    private void OnDisable()
    {
        // Only disable if there is a singletonInstance of GameManager exists
        if (GameManager.isSingletonCreated)
        {
            GameManager.singletonInstance.removeFromObserver(this);
        }
        else {
            return;
        }
     
    }

    private void Update()
    {
        // enemy die, play Death animation
        if (characterData.CurrentHealth == 0)
        {
            isDead = true;
        }

        // only change the enemy state and animtion when player is not dead 
        if(isPlayerDead == false)
        {
            changeStates();
            changeAnimation();
            lastAttackTime -= Time.deltaTime;
        }
      
    }

    private void changeStates()
    {
        // if enemy die, changing the enemy state to EnemyStates.DEAD;
        if (isDead)
        {
            enemyState = EnemyStates.DEAD;
        }
        else if (playInDetectRange())           // only detect player when the enemy is not dead 
        {
            enemyState = EnemyStates.CHASE;
            //Debug.Log("Player in range");
        }
        switch (enemyState)
        {
            // for standstill enemy logic
            case EnemyStates.STANDSTILL:
                standstillLogic();
                break;
            case EnemyStates.WANDER:
                wanderLogic();                          // if enemyState == EnemyStates.WANDER, perform Wander state
                break;
            case EnemyStates.CHASE:                     // if enemy detects player, change the state to CHASE
                chaseLogic();
                break;
            case EnemyStates.DEAD:
                deadLogic();
                break;
        }      
    }

    private void changeAnimation()
    {
        enemyAnimator.SetBool("isWalk", isWalk);
        enemyAnimator.SetBool("isBattleIdle", isBattleIdle);
        enemyAnimator.SetBool("isFollow", isFollow);
        enemyAnimator.SetBool("isCritical", characterData.isCritical);
        enemyAnimator.SetBool("isDead", isDead);
    }

    // player is in enemy's chasing/attcking range
    private bool playInDetectRange()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, sightRadius);
        for (int i = 0; i < colliders.Length; ++i)
        {
            if (colliders[i].CompareTag("Player"))
            {
                // assign the target (colliders[i].gameObject) to local player game object
                playerObj = colliders[i].gameObject;
                return true;
            }
        }
        // Otherwise, 
        playerObj = null;
        return false;
    }

    private void chaseLogic()
    {
        //isWalk = false;
        // before changing the animation to running, change it to battel idle first
        isBattleIdle = true;
        // keep the initial speed when chasing
        agent.speed = speed;
        // if play is in the chasing range, chasing action occur
        if (playInDetectRange())
        {
            isFollow = true;
            agent.isStopped = false;
            agent.destination = playerObj.transform.position;
        }
        else
        {
            // if play is out of the chasing range, go back to its original position
            isFollow = false;
            //isIdle = true;
            // if enemy can wander, 
            if (remainingWanderStagnantTime > 0)
            {
                agent.destination = transform.position;
                remainingWanderStagnantTime -= Time.deltaTime;
            }
            else if (isStandStill)
            {
                enemyState = EnemyStates.STANDSTILL;
            }
            else
            {
                
                enemyState = EnemyStates.WANDER;
                // restore the remainingWanderStagnantTime when enemy back to WANDER state
                remainingWanderStagnantTime = wanderStagnantTime;
            }
        }

        // attack action occur when player in attack range
        if (playerInEnemyAttackRange() || playerInEnemySkillRange())
        {
            // the enemy should stop following and stop the nav agent
            isFollow = false;
            agent.isStopped = true;

            // Attack
            if (lastAttackTime < 0)
            {
                lastAttackTime = characterData.Cd;

                // Critical check
                // if the Random.value (0.1~1) less than the characterData.CriticalChance (0.2), return ture, enemy perform critical attack
                characterData.isCritical = Random.value < characterData.CriticalChance;
                // perform attacking
                performAttack();
            }
        }
    }

    /***********************Attack Logic*******************************/
    protected bool playerInEnemyAttackRange()
    {
        if (playerObj != null)
            return Vector3.Distance(playerObj.transform.position, transform.position) <= characterData.AttackRange;
        else
            return false;
    }

    protected bool playerInEnemySkillRange()
    {
        if (playerObj != null)
            return Vector3.Distance(playerObj.transform.position, transform.position) <= characterData.SkillRange;
        else
            return false;
    }

    protected virtual void performAttack()
    {
        // look at the target(player)
        transform.LookAt(playerObj.transform);

        //if (playerInEnemyAttackRange())
        //{
        //    //enemyAnimator.ResetTrigger("skillAttack");
        //    // normal attack animation
        //    enemyAnimator.SetTrigger("basicAttack");
        //}

        //if (playerInEnemySkillRange())
        //{
        //    //enemyAnimator.ResetTrigger("basicAttack");
        //    // skill attack animation
        //    enemyAnimator.SetTrigger("skillAttack");
        //}
        
        if (playerInEnemySkillRange())
        {
            //enemyAnimator.ResetTrigger("basicAttack");
            // skill attack animation
            enemyAnimator.SetTrigger("skillAttack");
        }

        if (playerInEnemyAttackRange())
        {
            //enemyAnimator.ResetTrigger("skillAttack");
            // normal attack animation
            enemyAnimator.SetTrigger("basicAttack");
        }

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, sightRadius);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, wanderRange);
    }

    // Animation Even Trigger
    private void attackHit()
    {
        // need to check the playerObj, if its not null, then enemy perform attack
        // and 
        // need to check if the player is facing the back side of enemy. The attack wont cause damage
        if (playerObj != null && transform.isFacingPlayer(playerObj.transform))
        {
            var targetData = playerObj.GetComponent<CharacterData>();
            targetData.causeDamage(characterData, targetData);
        }

    }

    private void GetRandomWanderPoint()
    {
        remainingWanderStagnantTime = wanderStagnantTime;
        // do not change Y-axis value
        float randomX = Random.Range(-wanderRange, wanderRange);
        float randomZ = Random.Range(-wanderRange, wanderRange);
        Vector3 randomWanderPoint = new Vector3(enemyInitialPosition.x + randomX, transform.position.y, enemyInitialPosition.z + randomZ);

        NavMeshHit hit;
        // public static bool SamplePosition(Vector3 sourcePosition, out NavMeshHit hit, float maxDistance, int areaMask);
        // 1 means walkable navMesh area 
        if (NavMesh.SamplePosition(randomWanderPoint, out hit, wanderRange, 1))
        {
            // if the randomWanderPoint is walkable, hit.position == randomWanderPoint
            wanderPoint = hit.position;
        }
        else
        {
            // in wanderLogic(), because (Vector3.Distance(wanderPoint, transform.position) <= agent.stoppingDistance) => (0<=agent.stoppingDistance) so it will get a new random wander point again
            wanderPoint = transform.position;
        }
    }

    private void wanderLogic()
    {
        isBattleIdle = false;
        agent.speed = speed * 0.5f;
        if (Vector3.Distance(wanderPoint, transform.position) <= agent.stoppingDistance)      // if the distance between Wanderpoint and enemy position is <= 1 or 0.5 (agent.stoppingDistance). It means the enemy wandered to that wanderpoint 
        {
            isWalk = false;

            //Debug.Log("starting " + remainingWanderStagnantTime);
            if (remainingWanderStagnantTime > 0)
            {
                remainingWanderStagnantTime -= Time.deltaTime;
            }
            else
            {
                // wander to the new wanderPoint
                GetRandomWanderPoint();
            }
            //Debug.Log("endering " + remainingWanderStagnantTime);
        }
        else
        {
            isWalk = true;
            agent.destination = wanderPoint;
        }
    }

    /***standstillLogic for standstill state enemy**/
    private void standstillLogic()
    {
        //  set battel idle to false first
        isBattleIdle = false;
        //isWalk = false;

        // if the current enemy's position is not enemyInitialPosition
        if (transform.position != enemyInitialPosition)
        {
            // walk          
            isWalk = true;
            agent.isStopped = false;
            // agent move to the initial position
            agent.destination = enemyInitialPosition;

            // if statement is true means enemy already walked back to the initial position
            if (Vector3.SqrMagnitude(enemyInitialPosition - transform.position) < agent.stoppingDistance)
            {
                isWalk = false;
                // change the enemy face direction to the original direction 
                transform.rotation = Quaternion.Lerp(transform.rotation, enemyInitialRotation, 0.02f);
            }
        }
    }

    private void deadLogic()
    {
        // disable the enemy collider, so when the enemy dead, player cannot click it anymore
        enemyCollider.enabled = false;
        // disable the agent component (unselect the agent component)
        //agent.enabled = false;

        // throw error becase we add animation behaviour, we do not want to disable the agent but set the radius to 0
        agent.radius = 0f;
        Destroy(gameObject, 2f);

    }

    public void endGameNotify()
    {
        // execute enemy win animation 
        enemyAnimator.SetBool("isWin", true);    
        // stop the movement for all enemies
        // stop Nav mesh agent
        isBattleIdle = false;
        isWalk = false;
        playerObj = null;
        isPlayerDead = true;
    }
}
