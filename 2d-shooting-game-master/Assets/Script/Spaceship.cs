using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// リジッドボディの必須化
[RequireComponent(typeof(Rigidbody2D))]

public class Spaceship : MonoBehaviour {

    // Inspector表示用
    public float moveSpeed = 5;             // 移動用
    public int bulletSpeed = 10;            // 弾速用
    public float shotDelay;                 // 発射間隔用
    public GameObject bullet;               // 弾のプレハブ
    public bool CanShot;                    // 弾を撃つかどうかのスイッチ


    // 弾の生成
    public void Shot(Transform origin)      // 変数は弾(機体)の現在値
    {
        Instantiate(bullet, origin.position, origin.rotation);
    }

    // 機体の移動
    public void Move(Vector2 direction)     // 変数は機体の座標
    {
        GetComponent<Rigidbody2D>().velocity = direction * moveSpeed;
    }
}
