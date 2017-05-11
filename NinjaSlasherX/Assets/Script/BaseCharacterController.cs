using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseCharacterController : MonoBehaviour {
    // Inspector表示
    public Vector2 velocityMin = new Vector2(-100.0f, -100.0f);
    public Vector2 velocityMax = new Vector2(+100.0f, +100.0f);

    // 外部パラメータ
    [System.NonSerialized] public float hpMax = 10.0f;
    [System.NonSerialized] public float hp = 10.0f;
    [System.NonSerialized] public float dir = 1.0f;
    [System.NonSerialized] public float speed = 6.0f;
    [System.NonSerialized] public float basScaleX = 1.0f;
    [System.NonSerialized] public bool activeSts = false;
    [System.NonSerialized] public bool jumped = false;
    [System.NonSerialized] public bool grounded = false;
    [System.NonSerialized] public bool groundedPrev = false;

    // キャッシュ
    [System.NonSerialized] public Animator animator;
    Rigidbody2D rb2DB;
    protected Transform groundCheck_L;
    protected Transform groundCheck_C;
    protected Transform groundCheck_R;

    // 内部パラメータ
    protected float speedVx = 0.0f;
    protected float speedVxAddPower = 0.0f;
    protected float gravityScale = 10.0f;
    protected float jumpStartTime = 0.0f;

    protected GameObject groundCheck_OnRoadObject;
    protected GameObject groundCheck_OnMoveObject;
    protected GameObject groundCheck_OnEnemyObject;

    // 実行前呼び出し
    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();                //  アニメーター
        rb2DB = GetComponent<Rigidbody2D>();                //  キャラクター用のリジッドボディ
        groundCheck_L = transform.Find("GroundCheck_L");
        groundCheck_C = transform.Find("GroundCheck_C");
        groundCheck_R = transform.Find("GroundCheck_R");

        dir = (transform.localScale.x > 0.0f) ? 1 : -1;
        basScaleX = transform.localScale.x * dir;
        transform.localScale = new Vector3(basScaleX, transform.localScale.y, transform.localScale.z);

        activeSts = true;
        gravityScale = rb2DB.gravityScale;
    }
	// Use this for initialization
	protected virtual void Start () {
		
	}
	
	// Update is called once per frame
	protected virtual void Update () {
		
	}

    protected virtual void FixedUpdate()    // 確定フレーム処理
    {
        // 落下チェック
        if(transform.position.y < -30.0f)
        {
            Dead(false);                    // 死亡
        }

        // 地面チェック
        groundedPrev = grounded;
        grounded = false;

        groundCheck_OnRoadObject = null;
        groundCheck_OnMoveObject = null;
        groundCheck_OnEnemyObject = null;

        Collider2D[][] groundCheckCollider = new Collider2D[3][];                   // 地面チェック用の配列作成
        groundCheckCollider[0] = Physics2D.OverlapPointAll(groundCheck_L.position); // 地面チェック用の左側
        groundCheckCollider[1] = Physics2D.OverlapPointAll(groundCheck_C.position); // 地面チェック用の中央
        groundCheckCollider[2] = Physics2D.OverlapPointAll(groundCheck_R.position); // 地面チェック用の右側

        foreach(Collider2D[] groundCheckList in groundCheckCollider){               // コライダーチェック
            foreach(Collider2D groundCheck in groundCheckList){
                if(groundCheck != null){
                    if (!groundCheck.isTrigger){
                        grounded = true;
                        if (groundCheck.tag == "Road") {                            // 接触しているのが道の場合
                            groundCheck_OnRoadObject = groundCheck.gameObject;
                        }else
                        if(groundCheck.tag == "MoveObject") {                       // 接触しているのが動く物の場合
                            groundCheck_OnMoveObject = groundCheck.gameObject;
                        }else
                        if(groundCheck.tag == "Enemy")                              // 接触しているのが敵の場合
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
        rb2DB.velocity = new Vector2(speedVx, rb2DB.velocity.y);

        // velocityの値をチェック
        float vx = Mathf.Clamp(rb2DB.velocity.x, velocityMin.x, velocityMax.x);
        float vy = Mathf.Clamp(rb2DB.velocity.y, velocityMin.y, velocityMax.y);
        rb2DB.velocity = new Vector2(vx, vy);
    }

    protected virtual void FixedUpdateCharacter()
    {
    }
    // 基本アクション
    public virtual void ActionMove(float n)
    {
      if(n != 0.0f)                                 // 移動値が0.0fでない場合：移動している
        {
            dir = Mathf.Sign(n);                    // 向きに移動値の符号を代入：＋かー
            speedVx = speed * n;                    // 移動の値に速度と符号をかけて
            animator.SetTrigger("Run");             // 走るアニメーションを再生
        }else{
            speedVx = 0;                            // 移動値を0に
            animator.SetTrigger("Idle");            // 待機アニメーションを再生
    　　}
    }

    // その他：死亡処理
    public virtual void Dead(bool gameOver)
    {
        if (!activeSts)                     // プレイヤーキャラが操作可能か？
        {
            return;
        }
        activeSts = false;
        animator.SetTrigger("Dead");
    }

    // その他：体力管理
    public virtual bool SetHP(float _hp,float _hpMax)
    {
        hp = _hp;                           // 体力を格納
        hpMax = _hpMax;                     // 最大体力を格納
        return (hp <= 0);                   // 体力が０以下になったら戻り値を返す
    }
}
