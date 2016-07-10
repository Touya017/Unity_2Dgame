using UnityEngine;
using System.Collections;

public class EnemyController : BaseCharacterController {
    // Inspector表示部分
    public float initHpMax = 5.0f;
    public float initSpeed = 6.0f;
    public bool jumpActionEnabled = true;
    public Vector2 jumpPower = new Vector2(0.0f, 1500.0f);
    public int addScore = 500;

    // 外部パラメータ
    [System.NonSerialized]  public bool attackEnable = false;
    [System.NonSerialized]  public int attackDamege = 1;
    [System.NonSerialized]  public Vector2 attackNockBackVector = Vector3.zero;

    // アニメーションのハッシュ値
    public readonly static int ANISTS_Idle = Animator.StringToHash("Base Layer.Unitychan_Idle");
    public readonly static int ANISTS_Run = Animator.StringToHash("Base Layer.Unitychan_Run");
    public readonly static int ANISTS_Jump = Animator.StringToHash("Base Layer.Unitychan_Jump");
    public readonly static int ANISTS_Attack = Animator.StringToHash("Blade");
    public readonly static int ANISTS_DMG = Animator.StringToHash("Base Layer.Unitychan_DMG");
    public readonly static int ANISTS_Dead = Animator.StringToHash("Base Layer.Unitychan_Dead");

    // キャッシュ
    PlayerController playerCtrl;
    Animator playerAnim;
    Rigidbody2D Erb2D;

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
        if(jumpActionEnabled && grounded && !jumped)
        {
            animator.SetTrigger("Jump");
            Erb2D.AddForce(jumpPower);
            jumped = true;
            jumpStartTime = Time.fixedTime;
        }
        return jumped;
    }

    // 攻撃
    public void ActionAttack(string atkname, int damage)
    {
        attackEnable = true;
        attackDamege = damage;
        animator.SetTrigger(atkname);
    }

    // ダメージ判定
    public void ActionDamage()
    {
        int damage = 0;
        if(hp <= 0)
        {
            return;
        }
        if (superArmor)
        {
            animator.SetTrigger("SuperArmor");
        }

        AnimatorStateInfo stateInfo = playerAnim.GetCurrentAnimatorStateInfo(0);

        if(stateInfo.fullPathHash == PlayerController.ANISTS_QuickDraw)
        {
            damage = 2;
            if (!superArmor)
            {
                animator.SetTrigger("DMG");
                Erb2D.AddForce(new Vector2(60.0f, 200.0f));
                Debug.Log(string.Format(">>> DMG {0}", stateInfo.fullPathHash));
            }
            else
            {
                damage = 1;
                animator.SetTrigger("DMG");
                Debug.Log(string.Format(">>> DMG {0}", stateInfo.fullPathHash));
            }
        }
        if(SetHP(hp - damage, hpMax))
        {
            Dead(false);
            int addScoreV = ((int)((float)addScore * (playerCtrl.hp / playerCtrl.hpMax)));
            PlayerController.score += addScoreV;
        }
    }

    // 死亡処理
    public override void Dead(bool gameover)
    {
        base.Dead(gameover);
        Destroy(gameObject, 1.0f);
    }
}
