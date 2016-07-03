using UnityEngine;
using System.Collections;

public class PlayerController : BaseCharacterController {
    // Inspector表示部分
    public float initHpMax = 20.0f;
    [Range(0.1f, 100.0f)]
    public float initSpeed = 12.0f;

    // 内部パラメータ
    int jumpCount = 0;
    bool breakEnable = true;
    float groundFriction = 0.0f;

    // BaseCharacterControllerのAwake処理を呼ぶ
    protected override void Awake()
    {
        base.Awake();

        // パラメーター初期化
        speed = initSpeed;
        SetHP(initHpMax, initHpMax);

    }

    // キャラクター個別の処理
    protected override void FixedUpdateCharacter()
    {
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
                    rb2D.velocity = new Vector2(rb2D.velocity.x, 10.0f);
                    jumped = true;
                    jumpCount++;

                }
                break;
        }
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
}
