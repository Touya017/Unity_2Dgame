using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Toko : MonoBehaviour {

    Rigidbody2D rb2d;
    Animator anim;

    public LayerMask groundLayer;
    public float runSpeed = 5.0f;
    public float jumpPower = 500.0f;

    // 弾コンポーネント格納用
    public GameObject bullet;
    public float bulletSpeed = 8.0f;

    // 地面との接地判定用コンポーネント格納用
    protected Transform GroundCheck_L;
    protected Transform GroundCheck_C;
    protected Transform GroundCheck_R;

    // 格闘判定用コンポーネント格納用
    //public GameObject fight;

    // 体力の設定
    public int hp = 4;

    // 格闘攻撃の攻撃力設定
    //public int fightPower = 2;

    // 地面との接地判定用
    bool IsGrounded;

    // しゃがみ判定用
    bool IsCrouch;

    // ダメージを受けた判定用
    //bool IsDamage;

    void Awake()
    {
        GroundCheck_L = GameObject.Find("GroundCheck_L").GetComponentInParent<Transform>();
        GroundCheck_C = GameObject.Find("GroundCheck_C").GetComponentInParent<Transform>();
        GroundCheck_R = GameObject.Find("GroundCheck_R").GetComponentInParent<Transform>();
    }

	// Use this for initialization
	void Start () {
        rb2d = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }
	
	// Update is called once per frame
	void Update () {

        // カメラの設定
        Camera camera = GameObject.Find("Main Camera").GetComponent<Camera>();
        camera.transform.position = transform.position;
        camera.transform.position += new Vector3(0.0f, 0.2f, -10.0f);

        // 地面判定獲得用
        // 1つ目：開始点、2つ目：終点、3つ目：判定する為のレイヤー指定
        IsGrounded = Physics2D.Linecast(transform.position, GroundCheck_C.position, groundLayer);

        // 地面に接地している、しゃがんでいない、かつジャンプキーが押されたらジャンプ移行
        if(IsGrounded && Input.GetKeyDown(KeyCode.Space) && anim.GetBool("IsCrouch") == false)
        {
            Jump();
        }
        JumpAnim();

        // 走りモーション移行
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            anim.SetBool("IsRun", true);
        }
        else anim.SetBool("IsRun", false);

        // 射撃モーション移行
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            anim.SetBool("IsFire", true);
        }
        else anim.SetBool("IsFire", false);

        // 格闘モーション移行
        if (Input.GetKey(KeyCode.F))
        {
            anim.SetBool("IsPunch", true);
        }
        else anim.SetBool("IsPunch", false);

        // しゃがみモーション移行
        if (IsGrounded && Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
        {
            anim.SetBool("IsCrouch", true);
        }
        else anim.SetBool("IsCrouch", false);

       
        // 走り移動制御(左)
        if((Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) && anim.GetBool("IsCrouch") == false)
        {
            rb2d.velocity = new Vector2(-runSpeed, rb2d.velocity.y);
            transform.localScale = new Vector3(-4.0f, 4.0f, 0.0f);
        }
        // 走り移動制御(右)
        if ((Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) && anim.GetBool("IsCrouch") == false)
        {
            rb2d.velocity = new Vector2(runSpeed, rb2d.velocity.y);
            transform.localScale = new Vector3(4.0f, 4.0f, 0.0f);
        }
    }
    void Jump()
    {
        // ジャンプアニメーション開始
        anim.SetTrigger("IsJump");
        rb2d.AddForce(Vector2.up * jumpPower);
        // 地面から離れているのでIsGroundedをfalseに
        IsGrounded = false;
    }
    void JumpAnim()
    {
        // velY 上方向にかかる速度係数　上はプラス、下はマイナス
        float velY = rb2d.velocity.y;
        anim.SetFloat("velY", velY);
        anim.SetBool("IsGround", IsGrounded);
    }

    // 射撃処理
    void Fire()
    {
        GameObject go = Instantiate(bullet);
        Debug.Log(go.activeInHierarchy);
        go.transform.position = transform.position;
        go.transform.position += new Vector3(1.5f * transform.localScale.x/4, 0.5f, 0.0f);
        go.GetComponent<Rigidbody2D>().velocity = new Vector2(bulletSpeed * transform.localScale.x, 0.0f);
        go.transform.localScale = new Vector3(2.0f * transform.localScale.x / 4, 2.0f, 1.0f);
    }
    // 格闘攻撃が実行されたとき
    void Fight()
    {
        transform.FindChild("Toko_Arm_Hit").gameObject.SetActive(true);
    }

    // 格闘攻撃の終了
    void FightFinish()
    {
        transform.FindChild("Toko_Arm_Hit").gameObject.SetActive(false);
        Debug.Log("Active false");
    }

    // 何かに触れた時の処理
    void OnCollisionEnter2D(Collision2D E_other)
    {
        if(E_other.gameObject.tag == "Enemy")
        {
            if(hp > 0)
            {
                anim.SetTrigger("IsDamage");
                transform.localScale = new Vector3(transform.localScale.x, 4.0f, 0.0f);
            }
            else if(hp == 0)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
    }
}
