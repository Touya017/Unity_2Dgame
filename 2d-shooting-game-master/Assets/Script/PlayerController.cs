using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    // SpaceShipコンポーネント
    Spaceship spaceship;

   	// コルーチン処理
	IEnumerator Start () {

        // SpaceShipコンポーネント取得
        spaceship = GetComponent<Spaceship>();

        while (true)
        {
            // 弾を生成
            spaceship.Shot(transform);
                        
            // shotDelay分待つ
            yield return new WaitForSeconds(spaceship.shotDelay);
        }
	}
	
	// 毎フレーム処理
	void Update () {
        // キーボード操作：左右
        float x = Input.GetAxisRaw("Horizontal");

        // キーボード操作：上下
        float y = Input.GetAxisRaw("Vertical");

        // 移動する向きを求める
        Vector2 direction = new Vector2(x,y).normalized;

        // 移動する向きとスピードを算出
        spaceship.Move(direction);
	}
}
