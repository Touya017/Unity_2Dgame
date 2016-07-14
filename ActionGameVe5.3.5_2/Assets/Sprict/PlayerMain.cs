using UnityEngine;
using System.Collections;

public class PlayerMain : MonoBehaviour {

    //プレイヤーコントローラー
    PlayerController playerCtrl;

    // 
    void Awake()
    {
        playerCtrl = GetComponent<PlayerController>();
    }
	
    // Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        // 操作可能状態か確認
        if (!playerCtrl.activeSts)
        {
            return;
        }

        // GamePad処理
        float joyMv = Input.GetAxis("Lstick H");
        float joyCrouch = Input.GetAxis("Lstick V");
        playerCtrl.ActionMove(joyMv);
        playerCtrl.ActionCrouch(joyCrouch);
        //Debug.Log(joyMv);

        // ジャンプ
        if (Input.GetButtonDown("Jump"))
        {
            playerCtrl.ActionJump();
            return;
        }
        // 格闘攻撃
        if (Input.GetButtonDown("Fire1"))
        {
            playerCtrl.ActionFight();
        }

        // 射撃攻撃
        if (Input.GetButtonDown("Fire2"))
        {
            playerCtrl.ActionFire();
        }
	}
}
