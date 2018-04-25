using System.Collections;
using UnityEngine;

public class BaseCharacterController : MonoBehaviour {

    // 外部パラメータInspector表示
    public Vector2 velocityMin = new Vector2(-100.0f, -100.0f);
    public Vector2 velocityMax = new Vector2(+100.0f, +50.0f);

    // 外部パラメータ
    [System.NonSerialized] public float hpMax = 10.0f;                      // 最大体力設定
    [System.NonSerialized] public float hp = 10.0f;                         // 現在の体力
    [System.NonSerialized] public float dir = 1.0f;                         // 向き設定
    [System.NonSerialized] public float speed = 6.0f;                       // 移動速度
    [System.NonSerialized] public float basScaleX = 1.0f;                   // スプライトスケール
    [System.NonSerialized] public bool activeSts = false;                   // 操作可能確認フラグ
    [System.NonSerialized] public bool jumped = false;                      // ジャンプ判定用フラグ
    [System.NonSerialized] public bool grounded = false;                    // 地面チェック用フラグ
    [System.NonSerialized] public bool groundedPrev = false;                // 直前の地面チェック格納用フラグ

    // キャッシュ
    [System.NonSerialized] public Animator animator;                        // アニメーターコンポーネント
    Rigidbody2D rb2DB;                                                      // リジッドボディ取得
    protected Transform groundCheck_L;                                      // 地面チェック用座標
    protected Transform groundCheck_C;
    protected Transform groundCheck_R;

    // 内部パラメータ
    protected float speedVx = 0.0f;
    protected float speedVxAddPower = 0.0f;
    protected float gravityScale = 10.0f;                                   // ゲーム中に使用する重力設定
    protected float jumpStartTime = 0.0f;

    protected GameObject groundCheck_OnRoadObject;
    protected GameObject groundCheck_OnMoveObject;
    protected GameObject groundCheck_OnEnemyObject;

    // 基本機能の実装
    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();                                // アニメーターコンポーネントの取得
        rb2DB = GetComponent<Rigidbody2D>();                                // リジッドボディ取得
        groundCheck_L = transform.Find("GroundCheck_L");                    // 地面判定用左側
        groundCheck_C = transform.Find("GroundCheck_C");                    // 地面判定用中央
        groundCheck_R = transform.Find("GroundCheck_R");                    // 地面判定用右側

        dir = (transform.localScale.x > 0.0f) ? 1 : -1;                     // プレイヤーの向きの取得
        basScaleX = transform.localScale.x * dir;                           // プレイヤーの向き設定
        transform.localScale = 
            new Vector3(basScaleX, transform.localScale.y, transform.localScale.z); // キャラクターの向きと大きさの設定

        activeSts = true;
        gravityScale = rb2DB.gravityScale;
    }

	// Use this for initialization
	protected virtual void Start () {
		
	}

    protected virtual void Update()
    {

    }

    // Update is called once per frame
    protected virtual void FixedUpdate() {
        // 落下チェック
        if (transform.position.y < -30.0f)
        {
            Dead(false);                                                    // 死亡
        }

        // 地面チェック
        groundedPrev = grounded;                                            // 直前の地面チェックフラグを格納
        grounded = false;                                                   // 地面チェックの初期化

        groundCheck_OnRoadObject = null;                                    // 道路オブジェクトチェック用の初期化
        groundCheck_OnMoveObject = null;                                    // 動くオブジェクトチェック用の初期化
        groundCheck_OnEnemyObject = null;                                   // 敵オブジェクトチェック用の初期化

        Collider2D[][] groundCheckCollider = new Collider2D[3][];           // 地面チェック用オブジェクト格納構造体
        groundCheckCollider[0] = Physics2D.OverlapPointAll(groundCheck_L.position);
        groundCheckCollider[1] = Physics2D.OverlapPointAll(groundCheck_C.position);
        groundCheckCollider[2] = Physics2D.OverlapPointAll(groundCheck_R.position);

        foreach (Collider2D[] groundCheckList in groundCheckCollider) {
            foreach (Collider2D groundCheck in groundCheckList) {
                if (groundCheck != null) {
                    if (!groundCheck.isTrigger) {
                        grounded = true;
                        if (groundCheck.tag == "Road")
                        {
                            groundCheck_OnRoadObject = groundCheck.gameObject;
                        } else if (groundCheck.tag == "MoveObject")
                        {
                            groundCheck_OnMoveObject = groundCheck.gameObject;
                        } else if (groundCheck.tag == "Enemy")
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

        // Velocityの値をチェック
        float vx = Mathf.Clamp(rb2DB.velocity.x, velocityMin.x, velocityMax.x); // xの値の範囲をチェック
        float vy = Mathf.Clamp(rb2DB.velocity.y, velocityMin.y, velocityMax.y); // yの値の範囲をチェック
        rb2DB.velocity = new Vector2(vx, vy);
    }

    protected virtual void FixedUpdateCharacter()
    {

    }

    // 基本アクション
    public virtual void ActionMove(float n)
    {
        if(n != 0.0f)
        {
            dir = Mathf.Sign(n);                                                // 向きの値にnの符号を代入：左ならマイナス右ならプラス
            speedVx = speed * n;                                                // 移動量と向きを代入
            animator.SetTrigger("Run");                                         // 走るアニメーションを開始
        }
        else
        {
            speedVx = 0;                                                        // 移動量と向きに０を代入
            animator.SetTrigger("Idle");                                        // アイドリング状態に戻す
        }
    }

    // その他
    public virtual void Dead(bool gameOver)                                     // 死亡判定用
    {
        if (!activeSts)                                                         // 操作可能状態か
        {
            return;
        }
        activeSts = false;                                                      // 操作不可能に変更
        animator.SetTrigger("Dead");
    }

    public virtual bool SetHP(float _hp,float _hpMax)                           // 体力の初期化
    {
        hp = _hp;
        hpMax = _hpMax;
        return (hp <= 0);
    }
}
