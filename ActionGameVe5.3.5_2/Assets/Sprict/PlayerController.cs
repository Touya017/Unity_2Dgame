using UnityEngine;
using System.Collections;

public class PlayerController : BaseCharacterController {

    // アニメーションのハッシュ名
    public readonly static int ANISTS_Idle = Animator.StringToHash("Base Layer.Toko_Idle");
    public readonly static int ANISTS_Walk = Animator.StringToHash("Base Layer.Toko_Walk");
    public readonly static int ANISTS_Run = Animator.StringToHash("Base Layer.Toko_Run");
    public readonly static int ANISTS_Jump = Animator.StringToHash("Base Layer.Toko_Jump");
    public readonly static int ANISTS_Punch = Animator.StringToHash("Base Layer.Toko_Punch");
    public readonly static int ANISTS_Fire = Animator.StringToHash("Base Layer.Toko_Fire");
    public readonly static int ANISTS_Dead = Animator.StringToHash("Base Layer.Toko_Dead");

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

        if(stateInfo.shortNameHash == ANISTS_Punch || stateInfo.shortNameHash == ANISTS_Fire)
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
        if (Input.GetButtonDown("Fire2"))
        {
            atkInputEnable = true;
        }
    }

    public void SetNextAttack(string name)
    {
        if(atkInputNow == true)
        {
            atkInputNow = false;
            animator.Play(name);
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
                    rb2D.velocity = new Vector2(rb2D.velocity.x, 10.0f);
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
        if(stateInfo.shortNameHash == ANISTS_Idle || stateInfo.shortNameHash == ANISTS_Walk || 
            stateInfo.shortNameHash == ANISTS_Run || 
            stateInfo.shortNameHash == ANISTS_Jump || stateInfo.shortNameHash != ANISTS_Punch)
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
        if(stateInfo.shortNameHash != ANISTS_Fire)
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
}
