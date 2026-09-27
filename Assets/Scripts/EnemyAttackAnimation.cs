using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackAnimation : MonoBehaviour
{
    // 

    public EnemyMoveController enemyMoveController;
    void Start()
    {
        
    }


    public void PlayerDamaged()
    {
        if (enemyMoveController != null)
        {
            enemyMoveController.ReducePlayerLife();
        }
        else
        {
            Debug.LogError("EnemyMoveController Ç™å©Ç¬Ç©ÇËÇ‹ÇπÇÒÅI");
        }
    }
}
