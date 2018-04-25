using System.Collections;
using UnityEngine;

public class PlayerMain : MonoBehaviour {

    // キャッシュ
    PlayerController playerCtrl;

    // 基本機能の実装
    void Awake()
    {
        playerCtrl = GetComponent<PlayerController>();
    }

    // Use this for initialization
    void Start () {
        // 操作可能か？
        if (!playerCtrl.activeSts)
        {
            return;
        }

        // パッド処理
        float joyNv = Input.GetAxis("Horizontal");
        playerCtrl.ActionMove(joyNv);

        // ジャンプ
        if (Input.GetButtonDown("Jump"))
        {
            playerCtrl.ActionJump();
        }
	}
}
