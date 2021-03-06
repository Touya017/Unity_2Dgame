﻿using UnityEngine;
using System.Collections;

public class EnemyController : BaseCharacterController {
    // Inspector表示部分
    public float initHpMax = 5.0f;                          // 初期体力
    public float initSpeed = 6.0f;                          // 初期移動速度
    public bool jumpActionEnabled = true;                   // ジャンプ行動の有無
    public Vector2 jumpPower = new Vector2(0.0f, 1500.0f);  // ジャンプ上昇速度
    public int addScore = 500;                              // 倒した時の得点

    // 外部パラメータ
    [System.NonSerialized]  public bool cameraRendered = false;                 // カメラ範囲内にいるか
    [System.NonSerialized]  public bool attackEnable = false;                   // 攻撃判定
    [System.NonSerialized]  public int attackDamege = 1;                        // 攻撃力
    [System.NonSerialized]  public Vector2 attackNockBackVector = Vector3.zero; // ノックバック距離

    // アニメーションのハッシュ値
    public readonly static int ANISTS_Idle = Animator.StringToHash("Base Layer.Unitychan_Idle");
    public readonly static int ANISTS_Run = Animator.StringToHash("Base Layer.Unitychan_Run");
    public readonly static int ANISTS_Jump = Animator.StringToHash("Base Layer.Unitychan_Jump");
    public readonly static int ANISTS_Attack = Animator.StringToHash("Blade");
    public readonly static int ANISTS_DMG = Animator.StringToHash("Base Layer.Unitychan_DMG");
    public readonly static int ANISTS_Dead = Animator.StringToHash("Base Layer.Unitychan_Dead");

    // キャッシュ
    PlayerController playerCtrl;                                                // プレイヤーの状態確認用
    Animator playerAnim;                                                        // プレイヤーのアニメーション確認用
    Rigidbody2D Erb2D;                                                          // 敵のリジッドボディ

    // Monobehaviour機能実装
    protected override void Awake()
    {
        base.Awake();

        Erb2D = GetComponent<Rigidbody2D>();
        playerCtrl = PlayerController.GetController();
        playerAnim = playerCtrl.GetComponent<Animator>();
        hpMax = initHpMax;
        hp = hpMax;
        speed = initSpeed;
    }
    // 敵個別の処理
    protected override void FixedUpdateCharacter()
    {
        // カメラ範囲内かチェック
        if (!cameraRendered)
        {
            return;
        }

        // ジャンプチェック
        if (jumped)
        {
            // 着地チェック(A:接地瞬間判定 B:接地と時間による判定)
            if((grounded && !groundedPrev) || (grounded && Time.fixedTime > jumpStartTime + 1.0f))
            {
                jumped = false;
            }
            if(Time.fixedTime > jumpStartTime + 1.0f)
            {
                if(Erb2D.gravityScale < gravityScale)
                {
                    Erb2D.gravityScale = gravityScale;
                }
            }
            else
            {
                Erb2D.gravityScale = gravityScale;
            }
        }
        // キャラの方向
        transform.localScale = new Vector3(basScaleX * dir, transform.localScale.y, transform.localScale.z);
    }

    // ジャンプ
    public bool ActionJump()
    {
        if(jumpActionEnabled && grounded && !jumped)            // ジャンプフラグONかつ地面に接地中かつジャンプ中でない時
        {
            animator.SetTrigger("Jump");
            Erb2D.AddForce(jumpPower);
            jumped = true;
            jumpStartTime = Time.fixedTime;
        }
        return jumped;                                          // ジャンプ中であれば返り値をジャンプで返す
    }

    // 攻撃
    public void ActionAttack(string atkname, int damage)        // 宣言したアニメーション名とダメージ値を含んで宣言する
    {
        attackEnable = true;
        attackDamege = damage;
        animator.SetTrigger(atkname);
    }

    //-----------------------------------------------------//
    //      ダメージ判定                                    //
    // ・スーパーアーマーフラグ確認                          //
    // ・プレイヤーの攻撃アニメーション確認                   //
    // ・受けるダメージの設定                                //
    //-----------------------------------------------------//
    public void ActionDamage()
    {
        int damage = 0;
        if(hp <= 0)
        {
            return;
        }
        if (superArmor)                                         // スーパーアーマーフラグの確認
        {
            animator.SetTrigger("SuperArmor");
        }

        AnimatorStateInfo stateInfo = playerAnim.GetCurrentAnimatorStateInfo(0);    // プレイヤーのアニメーションステータスにアイドル状態を格納(初期化)

        if(stateInfo.fullPathHash == PlayerController.ANISTS_QuickDraw)             // プレイヤーの攻撃がクイックドローの場合
        {
            damage = 3;
            if (!superArmor)
            {
                animator.SetTrigger("DMG_A");                                       // ダメージモーションの設定
                Erb2D.AddForce(new Vector2(-200.0f * (basScaleX * dir), 500.0f));   // 吹き飛ばされる距離と向き
                Debug.Log(string.Format(">>> DMG {0}", stateInfo.fullPathHash));    // デバッグ用コメント
            }
            else
            {
                damage = 2;
                animator.SetTrigger("DMG_A");
                Debug.Log(string.Format(">>> DMG {0}", stateInfo.fullPathHash));
            }
        }

        if (stateInfo.fullPathHash == PlayerController.ANISTS_Punch)                // プレイヤーの攻撃が掌底の場合
        {
            damage = 1;
            if (!superArmor)
            {
                animator.SetTrigger("DMG_A");
                Erb2D.AddForce(new Vector2(-150.0f * (basScaleX * dir), 500.0f));
                Debug.Log(string.Format(">>> DMG {0}", stateInfo.fullPathHash));
            }
            else
            {                                                                       // スーパーアーマー持ちの場合
                damage = 0;
                animator.SetTrigger("DMG_A");
                Erb2D.AddForce(new Vector2(-300.0f * (basScaleX * dir), 500.0f));
                Debug.Log(string.Format(">>> DMG {0}", stateInfo.fullPathHash));
            }
        }

        if (stateInfo.fullPathHash == PlayerController.ANISTS_Fire)
        {
            damage = 4;
            if (!superArmor)
            {
                animator.SetTrigger("DMG_A");
                Erb2D.AddForce(new Vector2(-250.0f * (basScaleX * dir), 500.0f));
                Debug.Log(string.Format(">>> DMG {0}", stateInfo.fullPathHash));
            }
            else
            {
                damage = 2;
                animator.SetTrigger("DMG_A");
                Debug.Log(string.Format(">>> DMG {0}", stateInfo.fullPathHash));
            }
        }

        if (SetHP(hp - damage, hpMax))
        {
            Dead(false);
            animator.SetTrigger("Dead");
            int addScoreV = ((int)((float)addScore * (playerCtrl.hp / playerCtrl.hpMax)));
            PlayerController.score += addScoreV;
        }
    }

    // 死亡処理
    public override void Dead(bool gameover)
    {
        base.Dead(gameover);
        Destroy(gameObject, 1.5f);
    }
}
