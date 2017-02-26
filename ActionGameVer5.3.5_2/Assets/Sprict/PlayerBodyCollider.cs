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

            if (enemyCtrl.attackEnable)                     // 敵の攻撃が命中したら
            {
                enemyCtrl.attackEnable = false;
                playerCtrl.dir = (playerCtrl.transform.position.x < enemyCtrl.transform.position.x) ? +1 : -1;
                playerCtrl.ActionDamage(enemyCtrl.attackDamege);
            }
        }
        else if(other.tag == "EnemyArmBullet")
        {
            FireBullet fireBullet = other.transform.GetComponent<FireBullet>();
            if (fireBullet.attackEnable)
            {
                fireBullet.attackEnable = false;
                playerCtrl.dir = (playerCtrl.transform.position.x < fireBullet.transform.position.x) ? +1 : -1;
                playerCtrl.ActionDamage(fireBullet.attackDamage);
                Destroy(other.gameObject);
            }
        }
    }
}
