﻿using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerController : BaseCharacterController {

    //-------------------------------------------------//
    // アニメーションのハッシュ名                        //
    //-------------------------------------------------//
    public readonly static int ANISTS_Idle = Animator.StringToHash("Base Layer.Toko_Idle");             // アイドル状態
    public readonly static int ANISTS_Walk = Animator.StringToHash("Base Layer.Toko_Walk");             // 歩き
    public readonly static int ANISTS_Run = Animator.StringToHash("Base Layer.Toko_Run");               // 走り
    public readonly static int ANISTS_Jump = Animator.StringToHash("Base Layer.Toko_Jump");             // ジャンプ
    public readonly static int ANISTS_Punch = Animator.StringToHash("Base Layer.Toko_Punch");           // 掌底
    public readonly static int ANISTS_QuickDraw = Animator.StringToHash("Base Layer.Toko_QuickDraw");   // クイックドロー
    public readonly static int ANISTS_Fire = Animator.StringToHash("Base Layer.Toko_Fire");             // 射撃
    public readonly static int ANISTS_Dead = Animator.StringToHash("Base Layer.Toko_Dead");             // 死亡

    // セーブデータパラメータ
    public static float nowHpMax = 0;
    public static float nowHp = 0;
    public static int score = 0;

    // Inspector表示部分
    public float initHpMax = 20.0f;
    [Range(0.1f, 100.0f)]
    public float initSpeed = 12.0f;

    // 内部パラメータ
    int jumpCount = 0;

    // コンボ入力用管理変数
    volatile bool atkInputEnable = false;
    volatile bool atkInputNow = false;

    bool breakEnable = true;
    float groundFriction = 0.0f;

    //------------------------------------------------//
    // 敵の行動有効範囲の領域設定                       //
    // ・カメラの範囲に付随                            //
    // ・この範囲に敵が入ると動作フラグが有効になる      //
    //------------------------------------------------//
    [System.NonSerialized]   public Vector3 enemyActiveZonePointA;
    [System.NonSerialized]   public Vector3 enemyActiveZonePointB;

    // サポート関数(Enemy側からの参照用)
    // プレイヤーゲームオブジェクト
    public static GameObject GetGameObject()
    {
        return GameObject.FindGameObjectWithTag("Player");
    }
    // プレイヤーの座標獲得
    public static Transform GetTransform()
    {
        return GameObject.FindGameObjectWithTag("Player").transform;
    }
    // プレイヤーコントローラーの獲得
    public static PlayerController GetController()
    {
        return GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }
    // プレイヤーのアニメーター獲得
    public static Animator GetAnimator()
    {
        return GameObject.FindGameObjectWithTag("Player").GetComponent<Animator>();
    }

    // BaseCharacterControllerのAwake処理を呼ぶ
    protected override void Awake()
    {
        base.Awake();

        // パラメーター初期化
        speed = initSpeed;
        SetHP(initHpMax, initHpMax);

        // アクティブゾーンをBoxCollider2Dから取得
        BoxCollider2D boxCol2D = transform.Find("Collider_EnemyActiveZone").GetComponent<BoxCollider2D>();
        enemyActiveZonePointA = new Vector3(boxCol2D.offset.x - boxCol2D.size.x / 2.0f,
            boxCol2D.offset.y - boxCol2D.size.y / 2.0f); 
        enemyActiveZonePointB = new Vector3(boxCol2D.offset.x + boxCol2D.size.x / 2.0f,
            boxCol2D.offset.y + boxCol2D.size.y / 2.0f);
        boxCol2D.transform.gameObject.SetActive(false);
    }

    // キャラクター個別の処理
    protected override void FixedUpdateCharacter()
    {
        // 現在のステートを取得
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        // 着地チェック
        if (jumped)
        {
            if((grounded && !groundedPrev) || (grounded && Time.fixedTime > jumpStartTime + 1.0f))
            {
                animator.SetTrigger("Idle");
                jumped = false;
                jumpCount = 0;
            }
        }
        if (!jumped)
        {
            jumpCount = 0;
        }

        if(stateInfo.fullPathHash == ANISTS_Punch || stateInfo.fullPathHash == ANISTS_Fire)
        {
            // 攻撃中は移動停止
            speedVx = 0;
        }

        // キャラクターの方向
        transform.localScale = new Vector3(basScaleX * dir, transform.localScale.y, transform.localScale.z);

        // ジャンプ中の横移動減速
        if(jumped && !grounded)
        {
            if (breakEnable)
            {
                breakEnable = false;
                speedVx *= 0.9f;
            }
        }

        // 移動停止(減速)処理
        if (breakEnable)
        {
            speedVx *= groundFriction;
        }

        // カメラ
        Camera.main.transform.position = transform.position - Vector3.forward;
    }

    public void EnableAttackInput()
    {
        atkInputEnable = true;
    }

    public void SetNextAttack(string name)
    {
        if (atkInputNow == true)
        {
            atkInputNow = false;
            animator.Play(name);
            Debug.Log("QuickDrow");
        }
    }

    // 基本アクション
    public override void ActionMove(float n)
    {
        if (!activeSts)
        {
            return;
        }
        if (animator.GetBool("Crouch"))
        {
            return;
        }

        // 初期化
        float dirOld = dir;
        breakEnable = false;

        // アニメーション指定
        float moveSpeed = Mathf.Clamp(Mathf.Abs(n), -1.0f, 1.0f);
        animator.SetFloat("MoveSpeed", moveSpeed);

        // 移動チェック
        if(n != 0.0f)
        {
            // 移動処理
            dir = Mathf.Sign(n);
            moveSpeed = (moveSpeed < 0.5f) ? (moveSpeed * (1.0f / 0.5f)) : 1.0f;
            speedVx = initSpeed * moveSpeed * dirOld;
        }
        else
        {
            // 移動停止
            breakEnable = true;
        }

        // その場振り向きチェック
        if(dirOld != dir)
        {
            breakEnable = true;
        }
    }
    public void ActionJump()
    {
        if (animator.GetBool("Crouch"))
        {
            return;
        }
        switch (jumpCount)
        {
            case 0:
                if (grounded)
                {
                    animator.SetTrigger("Jump");
                    rb2D.velocity = Vector2.up * 15.0f;
                    jumpStartTime = Time.fixedTime;
                    jumped = true;
                    jumpCount++;
                }
                break;
            case 1:
                if (!grounded)
                {
                    animator.Play("Toko_Jump", 0, 0.0f);
                    rb2D.velocity = new Vector2(rb2D.velocity.x, 15.0f);
                    jumped = true;
                    jumpCount++;

                }
                break;
        }
    }

    // 格闘攻撃
    public void ActionFight()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if(stateInfo.fullPathHash == ANISTS_Idle || stateInfo.fullPathHash == ANISTS_Walk || 
            stateInfo.fullPathHash == ANISTS_Run || stateInfo.fullPathHash == ANISTS_Jump)
        {
            animator.SetTrigger("Punch");
        }
        else
        {
            if (atkInputEnable)
            {
                atkInputEnable = false;
                atkInputNow = true;
            }
        }
    }

    // 射撃攻撃
    public void ActionFire()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.fullPathHash == ANISTS_Idle || stateInfo.fullPathHash == ANISTS_Walk ||
            stateInfo.fullPathHash == ANISTS_Run || stateInfo.fullPathHash == ANISTS_Jump)
        {
            animator.SetTrigger("Fire");
        }
        return;
    }

    // しゃがみモーション
    public void ActionCrouch(float v)
    {
        if (!activeSts)
        {
            return;
        }
        if (grounded && (v > 0))
        {
            animator.SetBool("Crouch", true);
        }
        else animator.SetBool("Crouch", false);
    }

    // ダメージ処理
    public void ActionDamage(float damage)
    {
        if (!activeSts)
        {
            return;
        }

        animator.SetTrigger("DMG_A");
        rb2D.AddForce(new Vector2(-80.0f * (basScaleX * dir), 300.0f));
        speedVx = 0;
        rb2D.gravityScale = gravityScale;

        if (jumped)
        {
            damage *= 1.5f;
        }

        if(SetHP(hp - damage, hpMax))
        {
            Dead(true); // 死亡
        }
    }

    // 死亡判定処理
    public override void Dead(bool gameover)
    {
        // 死亡処理を行うか
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if(!activeSts || stateInfo.fullPathHash == ANISTS_Dead)
        {
            return;
        }
        base.Dead(gameover);
        SetHP(0, hpMax);
        Invoke("GameOver", 10.0f);
    }

    // GameOver処理
    public void GameOver()
    {
        PlayerController.score = 0;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public override bool SetHP(float _hp, float _hpMax)
    {
        if(_hp > _hpMax)
        {
            _hp = _hpMax;
        }

        nowHp = hp;
        nowHpMax = _hpMax;
        return base.SetHP(_hp, _hpMax);
    }
}
