using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethod
{
    // cos(60) = 0.5
    private const float angular = 0.5f;
    // creating an extension method for enemy 
    // only when the Vector3.Dot >=cos(60) 0.5f, then it will cause damage to player 
    // Extension the Transform propery to adding more feature
    // the function can be used by transform.isFacingPlayer(playerObj.transform) in EnemyController
    public static bool isFacingPlayer(this Transform transform, Transform playerObj)
    {
        // get the direction between player and enemy
        Vector3 toPlayer = playerObj.position - transform.position;
        toPlayer.Normalize();

        //  Vector3.Dot() will return -1,1,0
        // -1 means the player is behind the enemy
        // 1 means the enemy is facing the player
        // 0 means tghe player is at lhs or rhs of the enemy
        // passing the enemy facing Vector3, 
        float docValue = Vector3.Dot(transform.forward, toPlayer);
       
        // docValue need to be > 0.5f
        if (docValue >= angular)
            return true;
        else 
            return false;
    }
}
