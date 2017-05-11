using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : BaseCharacterController {

    // アニメーションのハッシュ名
    public readonly static int ANISTS_Idle = Animator.StringToHash("Base Layer.Player_Idle");               // 待機モーション
    public readonly static int ANISTS_Walk = Animator.StringToHash("Basa Layer.Player_Walk");               // 歩き
    public readonly static int ANISTS_Run = Animator.StringToHash("Base Layer.Player_Run");                 // 走り
    public readonly static int ANISTS_Jump = Animator.StringToHash("Base Layer.Player_Jump");               // ジャンプ
    public readonly static int ANISTS_ATTACK_A = Animator.StringToHash("Base Layer.Player_ATK_A");          // 攻撃A
    public readonly static int ANISTS_ATTACK_B = Animator.StringToHash("Base Layer.Player_ATK_B");          // 攻撃B
    public readonly static int ANISTS_ATTACK_C = Animator.StringToHash("Base Layer.Player_ATK_C");          // 攻撃C
    public readonly static int ANISTS_ATTACKJUMP_A = Animator.StringToHash("Base Layer.Player_ATKJUMP_A");  // ジャンプ攻撃A
    public readonly static int ANISTS_ATTACKJUMP_B = Animator.StringToHash("Base Layer.Player_ATKJUMP_B");  // ジャンプ攻撃B
    public readonly static int ANISTS_DEAD = Animator.StringToHash("Base Layer.Player_Dead");               // 死亡

    // Inspector表示
    public float initHpMax = 20.0f;                         // 体力の初期化
    [Range(0.1f, 100.0f)] public float initSpeed = 12.0f;   // 値の範囲と速度の初期化

    // 内部パラメータ
    int jumpCount = 0;                                      // ジャンプ回数
    bool breakEnable = true;                                // 停止処理
    float groundFriction = 0.0f;                            // 地面摩擦
    Rigidbody2D rb2DP;

    // 基本機能の実装
    protected override void Awake()
    {
        base.Awake();

        // パラメータ初期化
        speed = initSpeed;                                              // 速度の初期化
        SetHP(initHpMax, initHpMax);                                    // 最大体力の初期化
        rb2DP = GetComponent<Rigidbody2D>();                            // リジッドボディの初期化
    }

    protected override void FixedUpdateCharacter()
    {
        // 現在のアニメーションステートを取得
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        // 接地チェック
        if (jumped)                                                     // ジャンプ中か？
        {
            if ((grounded && !groundedPrev) ||
                (grounded && Time.fixedTime > jumpStartTime + 1.0f))    // 地面に設置してるか、ジャンプしてから1フレーム以降か
            {
                animator.SetTrigger("Idle");                            // 待機モーションに移行
                jumped = false;                                         // ジャンプフラグをOffへ
                jumpCount = 0;                                          // ジャンプカウントを0へ
            }
        }
        else                                                            // ジャンプ中でない
        {
            jumpCount = 0;                                              // ジャンプカウントを0へ
        }
        if(stateInfo.fullPathHash == ANISTS_ATTACK_C || 
            stateInfo.fullPathHash == ANISTS_ATTACKJUMP_A ||
            stateInfo.fullPathHash == ANISTS_ATTACKJUMP_B)
        {
            // 移動停止
            speedVx = 0;
        }

        // キャラの方向
        transform.localScale = new Vector3(basScaleX * dir, transform.localScale.y, transform.localScale.z);

        // ジャンプ中の横移動減速
        if (jumped && !grounded)
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

        // カメラ位置
        Camera.main.transform.position = transform.position - Vector3.forward;
    }

    // 基本アクション(移動)
    public override void ActionMove(float n)
    {
        if (!activeSts)
        {
            return;
        }

        // 初期化
        float dirOld = dir;                                         // 過去の向きを代入
        breakEnable = false;

        // アニメーション指定
        float moveSpeed = Mathf.Clamp(Mathf.Abs(n), -1.0f, +1.0f);  // 絶対値を返す
        animator.SetFloat("MoveSpeed", moveSpeed);                   // 移動速度でアニメータースピードを変える
        // animator.speed = 1.0f + moveSpeed;                       // 

        // 移動チェック
        if (n != 0.0f)
        {
            // 移動
            dir = Mathf.Sign(n);
            moveSpeed = (moveSpeed < 0.5f) ? (moveSpeed * (1.0f / 0.5f)) : 1.0f;
            speedVx = initSpeed * moveSpeed * dir;
        } else
        {
            // 移動停止
            breakEnable = true;
        }

        // その場振り向きチェック
        if (dirOld != dir)
        {
            breakEnable = true;
        }
    }

    // ジャンプ処理
    public void ActionJump()
    {
        switch (jumpCount)
        {
            case 0:
                if (grounded)
                {
                    animator.SetTrigger("Jump");
                    rb2DP.velocity = Vector2.up * 30.0f;
                    jumpStartTime = Time.fixedTime;
                    jumped = true;
                    jumpCount++;
                }
                break;
            case 1:
                if (!grounded)
                {
                    animator.Play("Player_Jump", 0, 0.0f);
                    rb2DP.velocity = new Vector2(rb2DP.velocity.x, 20.0f);
                    jumped = true;
                    jumpCount++;
                }
                break;
        }
    }

    // 攻撃処理
    public void ActionAttack()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if(stateInfo.fullPathHash != ANISTS_ATTACK_A)
        {
            animator.SetTrigger("Attack_A");
        }
        else
        {
            animator.SetTrigger("Attack_B");
        }
    }
}