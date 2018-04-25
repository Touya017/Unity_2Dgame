using System.Collections;
using UnityEngine;

public class PlayerController : BaseCharacterController {

    // 外部パラメータ(Inspector表示)
    public float initHpMax = 20.0f;                                         // プレイヤー体力
    [Range(0.1f, 100.0f)] public float initSpeed = 12.0f;                   // ランダム数値

    // 内部パラメータ
    Rigidbody2D rb2DP;                                                      // リジッドボディ取得
    int jumpCount = 0;
    bool breakEnabled = true;
    float groundFriction = 0.0f;

    // 基本機能の実装
    protected override void Awake()
    {
        base.Awake();                                                       // BaseCharacterControllerのAwake処理を呼び出す

        // パラメータ初期化
        speed = initSpeed;                                                  // speedにinitSpeedの値を代入
        SetHP(initHpMax, initHpMax);                                        // プレイヤーの体力初期化
        rb2DP = GetComponent<Rigidbody2D>();
    }

    protected override void FixedUpdateCharacter()
    {
        // 着地チェック
        if (jumped){                                                        // ジャンプ中か？
            // 地面に着地しているかつ直前に着地していない
            if((grounded && !groundedPrev) || (grounded && Time.fixedTime > jumpStartTime + 1.0f))
            {
                animator.SetTrigger("Idle");
                jumped = false;
                jumpCount = 0;
            }
        }
        if (!jumped){
            jumpCount = 0;
            rb2DP.gravityScale = gravityScale;

        }

        // キャラクターの向き
        transform.localScale = new Vector3(basScaleX * dir, transform.localScale.y, transform.localScale.z);

        // ジャンプ中の横移動減速
        if(jumped && !grounded){
            if (breakEnabled){
                breakEnabled = false;
                speedVx *= 0.9f;

            }
        }

        // 移動停止(減速)処理
        if (breakEnabled)
        {
            speedVx *= groundFriction;
        }

        // カメラ位置
        Camera.main.transform.position = transform.position - Vector3.forward;

    }


    // 基本アクション

    // 移動
    public override void ActionMove(float n)
    {
        if (!activeSts)
        {
            return;
        }

        // 初期化
        float dirOld = dir;                                                         // 向き情報を前回の向きに代入
        breakEnabled = false;

        // アニメーション設定
        float moveSpeed = Mathf.Clamp(Mathf.Abs(n), -1.0f, +1.0f);                  // 
        animator.SetFloat("MoveSpeed", moveSpeed);

        // 移動チェック
        if(n != 0.0f)
        {
            // 移動
            dir = Mathf.Sign(n);                                                    // 引数の符号を返す
            moveSpeed = (moveSpeed < 0.5f) ? (moveSpeed * (1.0f / 0.5f)) : 1.0f;
            speedVx = initSpeed * moveSpeed * dir;
        }
        else
        {
            // 移動停止
            breakEnabled = true;
        }

        // その場振り向きチェック
        if(dirOld != dir)
        {
            breakEnabled = true;
        }
    }

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
                    animator.SetTrigger("Jump");
                    rb2DP.velocity = new Vector2(rb2DP.velocity.x, 20.0f);
                    jumped = true;
                    jumpCount++;
                }
                break;
        }
        JumpAnim();
    }

    void JumpAnim()
    {
        // velY：y方向へかかる加速単位、上に行くとプラス、下に行くとマイナス
        float velY = rb2DP.velocity.y;
        // Animatorへパラメータを送る
        animator.SetFloat("velY", velY);
        animator.SetBool("grounded", grounded);
    }
}
