using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    private NavMeshAgent agent;
    // control the movement animation based on the speed
    private Animator playerAnimator;

    private GameObject enemyObj;
    private float attackCD;

    private CharacterData characterData;

    // Death bool
    private bool isDead;

    // adding the audio for play hit 
    public AudioSource hitAudioSource;

    // flag to control the player movement
    //public bool canMove;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        playerAnimator = GetComponent<Animator>();
        characterData = GetComponent<CharacterData>();
    }

    private void OnEnable()
    {

        // Add/subscript moveToTarget function into MouseEvent listener
        // move to a point on the ground
        MouseManager.singletonInstance.OnMouseClickedGround += moveToTarget;
        MouseManager.singletonInstance.OnMouseClickedEnemy += moveToAttackableTarget;

        // GameManager
        GameManager.singletonInstance.rigisterPlayer(characterData);
    }

    private void Start()
    {

        //// Add/subscript moveToTarget function into MouseEvent listener
        //// move to a point on the ground
        //MouseManager.singletonInstance.OnMouseClickedGround += moveToTarget;
        //MouseManager.singletonInstance.OnMouseClickedEnemy += moveToAttackableTarget;

        //// GameManager
        //GameManager.singletonInstance.rigisterPlayer(characterData);
        DataManager.singletonInstance.loadPlayerData();
        //// restore the currentHealth 
        //characterData.MaxHealth = 100;
        //characterData.CurrentHealth = characterData.MaxHealth;


        ////characterData.characterDataSO.currentExp = 0; 
        //characterData.characterDataSO.level = 1;
        //characterData.characterDataSO.currentExp = 0;
        //characterData.characterDataSO.levelUpNeededExp = 50;
        //canMove = true;
    }


    // when the playController got destoried
    private void OnDisable()
    {
        if (MouseManager.isSingletonCreated == false)
            return;
        MouseManager.singletonInstance.OnMouseClickedGround -= moveToTarget;
        MouseManager.singletonInstance.OnMouseClickedEnemy -= moveToAttackableTarget;

    }

    private void Update()
    {
        // player die, play Death animation
        if (characterData.CurrentHealth == 0)
        {
            isDead = true;
        }

        // if player is dead, notify all the enemies and then them will execute the code in endGameNotify() function
        if (isDead)
        {
            GameManager.singletonInstance.notifyObservers();
        }
        // update the speed for running animation
        changeAnimation();
        
        // CD refreshing  
        attackCD -= Time.deltaTime;
    }

    // move to any place on Ground
    public void moveToTarget(Vector3 targetV3)
    {
        //if (canMove)
        //{
            if (isDead) return;
            // allow player to move again after he attack the enemy
            agent.isStopped = false;
            // stop allCoroutines (moveToAttackableTargetCoroutine)
            StopAllCoroutines();
            agent.destination = targetV3;
        //}
    }


    private void changeAnimation()
    {
        // agent.velocity.sqrMagnitude will return a float value
        playerAnimator.SetFloat("speed", agent.velocity.sqrMagnitude);
        playerAnimator.SetBool("isCritical", characterData.isCritical);
        playerAnimator.SetBool("isDead", isDead); 
    }

    private void moveToAttackableTarget(GameObject enemyTarget)
    {
        //if (canMove)
        //{
            if (isDead) return;
            if (enemyTarget != null)
            {
                enemyObj = enemyTarget;
                // 
                StartCoroutine(moveToAttackableTargetCoroutine());

            }
        //}
    }

    IEnumerator moveToAttackableTargetCoroutine()
    {
        agent.isStopped = false;
        // turn around to toward the enemy 
        transform.LookAt(enemyObj.transform);
        // we need to keep track the distance between player and enemy, only the distance reach characterData.AttackRange, then we can attack
        // Otherwise, keep moving
        while (Vector3.Distance(enemyObj.transform.position, transform.position) > characterData.AttackRange)
        {
            // cannot attack due to distance, so moving toward enemy
            agent.destination = enemyObj.transform.position;
            yield return null;
        }

        // having enough distance, stop the player's Nav agent attack
        agent.isStopped = true;

        // Attack 
        if (attackCD < 0)
        {
            // reset attackCD
            attackCD = characterData.Cd;
            // check if isCritical or not 
            characterData.isCritical = Random.value < characterData.CriticalChance;
            playerAnimator.SetTrigger("attack");
        }
    }

    // Animation Even Trigger
    private void attackHit()
    {
            // play hit audio when play attacks
            hitAudioSource.Play();
            // if player attack the attackable (rock)
            if (enemyObj.CompareTag("Attackable"))
            {
                //  if player attack the attackable (rock) and the rock is on the Ground
                if (enemyObj.GetComponent<RockController>() && enemyObj.GetComponent<RockController>().rockHitStatus == RockController.RockHit.Nothing)
                {
                    // change the rockHitStatus to RockController.RockHit.Enemy when player attack the rock
                    enemyObj.GetComponent<RockController>().rockHitStatus = RockController.RockHit.Enemy;
                    // when hit back the default velocity < 1, so the rock status will be Nothing based on the status in the FixUpdated function in the Rockcontroller 
                    // so we assign value which = 1, then the rock will hit the enemy and destory itself
                    enemyObj.GetComponent<Rigidbody>().velocity = Vector3.one;
                    enemyObj.GetComponent<Rigidbody>().AddForce(transform.forward * 20, ForceMode.Impulse);
                }
            }
            else
            {
                var targetData = enemyObj.GetComponent<CharacterData>();
                targetData.causeDamage(characterData, targetData);
            }

        

    }

    
}
