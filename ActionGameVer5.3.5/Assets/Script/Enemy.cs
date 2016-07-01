using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Enemy : MonoBehaviour {

    // rigidbody格納用
    Rigidbody2D Erb2D;
    // 敵の行動管理用
    int EActionNum = 0;

    // 行動時間
    public float actionTime = 1.0f;
    // 行動内容
    public Vector2 addforce = new Vector2(60, 1500.0f);
    public Vector2 lowaddforce = new Vector2(-60, 1500.0f);
    
    // 敵の行動管理用時間
    float startTime;
    // 画面内に映っているかを管理
    bool isRendered = false;

    // プレイヤーコンポーネント格納
    GameObject player;

    void Awake()
    {
        player = GameObject.Find("Toko");
        Erb2D = GetComponentInParent<Rigidbody2D>();
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

    void OnWillRenderObject()
    {
        isRendered = true;
    }
	
	// Update is called once per frame
	void Update () {

        // プレイヤーと敵との位置関係で向きを変える
        if (player.transform.position.x > transform.position.x)
        {
            transform.localScale = new Vector3(-1.0f, 1.0f, 0.0f);
        }
        else transform.localScale = new Vector3(1.0f, 1.0f, 0.0f);
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
}
