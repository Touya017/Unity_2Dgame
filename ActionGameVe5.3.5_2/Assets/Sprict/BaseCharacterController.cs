using UnityEngine;
using System.Collections;

public class BaseCharacterController : MonoBehaviour {

    // 移動制限範囲
    public Vector2 velocityMin = new Vector2(-100.0f, -100.0f);
    public Vector2 velocityMax = new Vector2(+100.0f, +50.0f);

    // 体力設定
    [System.NonSerialized]  public float hpMax = 10.0f;
    [System.NonSerialized]  public float hp = 10.0f;
    // キャラクターの向き
    [System.NonSerialized]  public float dir = 1.0f;
    // 移動速度
    [System.NonSerialized]  public float speed = 6.0f;
    // 基本スケールサイズ
    [System.NonSerialized]  public float basScaleX = 1.0f;
    // 操作可能確認フラグ
    [System.NonSerialized]  public bool activeSts = false;
    // ジャンプ中確認フラグ
    [System.NonSerialized]  public bool jumped = false;
    // 地面接地確認フラグ
    [System.NonSerialized]  public bool grounded = false;
    [System.NonSerialized]  public bool groundedPrev = false;

    // キャッシュ用
    [System.NonSerialized]  public Animator animator;
    [System.NonSerialized]  public Rigidbody2D rb2D;
    protected Transform groundCheck_L;
    protected Transform groundCheck_C;
    protected Transform groundCheck_R;

    // 内部パラメータ
    protected float speedVx = 0.0f;
    protected float speedAddPower = 0.0f;
    protected float gravityScale = 4.0f;
    protected float jumpStartTime = 0.0f;

    protected GameObject groundCheck_OnRoadObject;
    protected GameObject groundCheck_OnMoveObject;
    protected GameObject groundCheck_OnEnemyObject;

    // 基本機能実装
    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
        rb2D = GetComponent<Rigidbody2D>();
        groundCheck_L = transform.Find("GroundCheck_L");
        groundCheck_C = transform.Find("GroundCheck_C");
        groundCheck_R = transform.Find("GroundCheck_R");

        dir = (transform.localScale.x > 0.0f) ? 1 : -1;
        basScaleX = transform.localScale.x * dir;
        transform.localScale = new Vector3(basScaleX, transform.localScale.y, transform.localScale.z);

        activeSts = true;
        gravityScale = rb2D.gravityScale;
    }



	// Use this for initialization
	protected virtual void Start () {
	
	}
	
	// Update is called once per frame
	protected virtual void Update () {
	
	}

    // 動作系処理
    protected virtual void FixedUpdate()
    {
        // 落下チェック
        if(transform.position.y < -30.0f)
        {
            Dead(false);
        }

        // 地面チェック
        groundedPrev = grounded;
        grounded = false;

        groundCheck_OnRoadObject = null;
        groundCheck_OnMoveObject = null;
        groundCheck_OnEnemyObject = null;

        Collider2D[][] groundCheckCollider = new Collider2D[3][];
        groundCheckCollider[0] = Physics2D.OverlapPointAll(groundCheck_L.position);
        groundCheckCollider[1] = Physics2D.OverlapPointAll(groundCheck_C.position);
        groundCheckCollider[2] = Physics2D.OverlapPointAll(groundCheck_R.position);

        foreach(Collider2D[] groundCheckList in groundCheckCollider)
        {
            foreach(Collider2D groundCheck in groundCheckList)
            {
                if(groundCheck != null)
                {
                    if (!groundCheck.isTrigger)
                    {
                        grounded = true;
                        if(groundCheck.tag == "Road")
                        {
                            groundCheck_OnRoadObject = groundCheck.gameObject;
                        }
                        else if(groundCheck.tag == "MoveObject")
                        {
                            groundCheck_OnMoveObject = groundCheck.gameObject;
                        }
                        else if(groundCheck.tag == "Enemy")
                        {
                            groundCheck_OnEnemyObject = groundCheck.gameObject;
                        }
                    }
                }
            }
        }
        // キャラクター個別の処理
        FixedUpdateCharacter();

        // 移動計算
        rb2D.velocity = new Vector2(speedVx, rb2D.velocity.y);

        // Velocityの値チェック
        float vx = Mathf.Clamp(rb2D.velocity.x, velocityMin.x, velocityMax.x);
        float vy = Mathf.Clamp(rb2D.velocity.y, velocityMin.y, velocityMax.y);

        rb2D.velocity = new Vector2(vx, vy);
    }

    // FixedUpdateCharacter関数
    protected virtual void FixedUpdateCharacter()
    {
        
    }
    // 移動処理
    public virtual void ActionMove(float n)
    {
        if(n != 0.0f)
        {
            // 正の値か、負の値を確認
            dir = Mathf.Sign(n);
            speedVx = speed * n;
            animator.SetTrigger("Run");
        }
        else
        {
            speedVx = 0;
            animator.SetTrigger("Idle");
        }
    }

    // 死亡時の処理
    public virtual void Dead(bool gameover)
    {
        if (activeSts)
        {
            return;
        }
        activeSts = false;
        animator.SetTrigger("Dead");
    }

    // 体力の減少処理
    public virtual bool SetHP(float _hp, float _hpMax)
    {
        hp = _hp;
        hpMax = _hpMax;
        return (hp <= 0);
    }
}
