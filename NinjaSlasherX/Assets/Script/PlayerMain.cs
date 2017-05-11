using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMain : MonoBehaviour {

    // プレイヤーキャラクターのコントローラー呼び出し
    PlayerController playerCtrl;

    // 実行前の動作
    void Awake()
    {
        playerCtrl = GetComponent<PlayerController>();
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// 毎フレーム処理
	void Update () {
        // 操作可能か
        if (!playerCtrl.activeSts)
        {
            return;
        }

        // パッド処理
        float joyMv = Input.GetAxis("Horizontal");
        playerCtrl.ActionMove(joyMv);

        // ジャンプ処理
        if (Input.GetButtonDown("Jump"))
        {
            playerCtrl.ActionJump();
            return;
        }

        // 攻撃処理
        if(Input.GetButtonDown("Fire1") || Input.GetButtonDown("Fire2") ||
            Input.GetButtonDown("Fire3")){
            playerCtrl.ActionAttack();
        }
	}
}
