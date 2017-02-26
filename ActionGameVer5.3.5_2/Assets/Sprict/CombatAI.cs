using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CombatAI : MonoBehaviour
{
    // 外部パラメータ(Inspector表示)
    public int freeAIMax = 3;                           // 自由に攻撃させたい敵の数 
    public int blockAttackAIMax = 10;                   // 一度に管理する敵の数

    // コード記入
    void FixeUpdate()
    {
        var activeEnemyMainList = new List<EnemyMain>();

        // カメラに映っている敵を検索
        GameObject[] enemyList = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemyList == null)
        {
            return;
        }
        foreach (GameObject enemy in enemyList)
        {
            EnemyMain enemyMain = enemy.GetComponent<EnemyMain>();
            if (enemyMain != null)
            {
                if (enemyMain.combatAIOerder && enemyMain.cameraEnabled)       // 画面外の敵がカメラ範囲内に入った場合
                {
                    activeEnemyMainList.Add(enemyMain);
                }
            }
            else
            {
                Debug.LogWarning(string.Format("CombatAI : EnemyMain null : {0} {1}",
                    enemy.name, enemy.transform.position));
            }
        }

        // 攻撃する敵を抑制
        int i = 0;
        foreach (EnemyMain enemyMain in activeEnemyMainList)                   // 敵の数を行動できる敵のリストとして格納
        {
            if (i < freeAIMax)
            {
                // そのまま自由に行動させる
            }
            else if (i < freeAIMax + blockAttackAIMax)                         // i=敵の番号　i番目の敵が13番目よりも手前のとき
            {
                // 攻撃を抑制する
                if (enemyMain.aiState == ENEMYAISTS.RUNTOPLAYER)
                {
                    enemyMain.SetCombatAIState(ENEMYAISTS.WAIT);
                }
            }
            else
            {                                                                  // 14番目以降の敵の場合
                // 攻撃を停止する
                if (enemyMain.aiState != ENEMYAISTS.WAIT)
                {
                    enemyMain.SetCombatAIState(ENEMYAISTS.WAIT);
                }
            }
            i++;
        }
    }
}
