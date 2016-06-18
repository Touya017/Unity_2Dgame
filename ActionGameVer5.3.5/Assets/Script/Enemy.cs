using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Enemy : MonoBehaviour {

    // 行動時間
    public float actionTime = 1.0f;
    // 行動内容
    public Vector2 addforce = new Vector2(0, 1500.0f);
    public Vector2 lowaddforce = new Vector2(0, 1500.0f);
    
    // 敵の攻撃力
    public int eAttack = 1;
    // 敵の行動管理用時間
    float startTime;
    // 画面内に映っているかを管理
    bool isRendered = false;

	// Use this for initialization
	void Start () {
        startTime = Time.time;
	}

    void OnWillRenderObject()
    {
        isRendered = true;
    }
	
	// Update is called once per frame
	void Update () {
        if(isRendered && Time.time >= startTime + actionTime)
        {
            if(GameObject.Find("Toko").transform.position.x > transform.position.x)
            {
                GetComponent<Rigidbody2D>().AddForce(lowaddforce);
                startTime = Time.time;
                Debug.Log("Enemy Run");
            }
            else if(GameObject.Find("Toko").transform.position.x < transform.position.x)
            {
                GetComponent<Rigidbody2D>().AddForce(addforce);
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
