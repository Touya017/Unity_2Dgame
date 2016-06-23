using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Enemy : MonoBehaviour {

    // rigidbody格納用
    Rigidbody2D Erb2D;

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
        startTime = Time.time;
        player = GameObject.Find("Toko");
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
        StartCoroutine(Action());
	}

    void OnWillRenderObject()
    {
        isRendered = true;
    }
	
	// Update is called once per frame
	void Update () {
        //if (player.transform.position.x > transform.position.x)
        //{
            //transform.localScale = new Vector3(-1.0f * transform.localScale.x, 1.5f, 0.0f);
        //}
    }

    void EActionWalkJump()
    {
        if (player.transform.position.x < transform.position.x)
        {
            transform.localScale = new Vector3(1.0f * transform.localScale.x, 1.5f, 0.0f);
            if (isRendered && Time.time >= startTime + actionTime)
            {
                Erb2D.AddForce(lowaddforce);
                startTime = Time.time;
                Debug.Log("Enemy Run");
            }
        }
        else
        {
            transform.localScale = new Vector3(-1.0f * transform.localScale.x, 1.5f, 0.0f);
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
            P_other.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(-100.0f * (P_other.transform.localScale.x/4),400.0f));
            P_other.transform.GetComponent<Toko>().hp = P_other.transform.GetComponent<Toko>().hp - eAttack;
        }
    }
}
