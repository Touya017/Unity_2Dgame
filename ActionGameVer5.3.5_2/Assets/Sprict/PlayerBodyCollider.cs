using UnityEngine;
using System.Collections;

public class PlayerBodyCollider : MonoBehaviour {
    // プレイヤーコントローラー獲得
    PlayerController playerCtrl;

    void Awake()
    {
        playerCtrl = transform.parent.GetComponent<PlayerController>();
    }

    // 当たり判定チェック
    void OnTriggerEnter2D(Collider2D other)
    {
        // 触れたものを確認
        if(other.tag == "EnemyArm")
        {
            EnemyController enemyCtrl = other.GetComponentInParent<EnemyController>();

            if (enemyCtrl.attackEnable)
            {
                enemyCtrl.attackEnable = false;
                playerCtrl.dir = (playerCtrl.transform.position.x < enemyCtrl.transform.position.x) ? +1 : -1;
            }
        }
    }
}
