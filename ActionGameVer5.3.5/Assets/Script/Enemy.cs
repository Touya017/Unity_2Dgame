using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Enemy : MonoBehaviour {

    // rigidbody格納用
    Rigidbody2D Erb2D;
    // Collider判定用の格納
    GameObject Ebody;
    // 敵の行動管理用
    int EActionNum = 0;

    // 行動時間
    public float actionTime = 1.0f;
    // 行動内容
    public Vector2 addforce = new Vector2(60, 1500.0f);
    public Vector2 lowaddforce = new Vector2(-60, 1500.0f);
    
    // 敵の攻撃力
    public int eAttack = 1;
    // 敵の行動管理用時間
    float startTime;
    // 画面内に映っているかを管理
    bool isRendered = false;

    // プレイヤーコンポーネント格納
    GameObject player;

    void Awake()
    {
        player = GameObject.Find("Toko");
        Ebody = GameObject.Find("UniColliderBody");
        Erb2D = GetComponent<Rigidbody2D>();
    }

    // 1秒ごとに敵にジャンプで接近させる
    IEnumerator Action()
    {
        while (true)
        {
            EActionWalkJump();
            yield return new WaitForSeconds(actionTime);
        }
    }

	// Use this for initialization
	void Start () {
        startTime = Time.time;
        StartCoroutine(Action());
        EActionNum = Random.Range(0,11);
	}

    void OnWillRenderObject(Collider2D e)
    {
        if(e.tag == "Enemy")
        {
            isRendered = true;
        }
    }
	
	// Update is called once per frame
	void Update () {

        // プレイヤーと敵との位置関係で向きを変える
        if (player.transform.position.x > transform.position.x)
        {
            transform.localScale = new Vector3(-1.0f, 1.5f, 0.0f);
        }
        else transform.localScale = new Vector3(1.0f, 1.5f, 0.0f);
    }

    // １秒毎にジャンプする処理
    void EActionWalkJump()
    {
        if (player.transform.position.x < transform.position.x)
        {
            if (isRendered && Time.time >= startTime + actionTime)
            {
                Erb2D.AddForce(lowaddforce);
                startTime = Time.time;
                Debug.Log("Enemy Run");
            }
        }
        else if(player.transform.position.x > transform.position.x)
        {
            if (isRendered && Time.time >= startTime + actionTime)
            {
                Erb2D.AddForce(addforce);
                startTime = Time.time;
                Debug.Log("Enemy Run");
            }
        }
    }

    // Uniがプレイヤーに触れた時の処理
    void OnCollisionEnter2D(Collision2D P_other)
    {
        if(P_other.gameObject.tag == "Player")
        {
            P_other.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(-100.0f * (P_other.transform.localScale.x/1.4f),400.0f));
            P_other.transform.GetComponent<Toko>().hp = P_other.transform.GetComponent<Toko>().hp - eAttack;
        }
    }
}
