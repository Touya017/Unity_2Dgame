using UnityEngine;
using System.Collections;

public class Toko_fight : MonoBehaviour {

    GameObject Toko_master;

    // プレイヤーキャラクターのコンポーネント獲得
    void Awake()
    {
        Toko_master = GameObject.Find("Toko");
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    // 格闘攻撃が命中した場合の処理
    void OnTriggerEnter2D(Collider2D c)
    {
        if (c.tag == "Boss")
        {
            c.transform.position = GameObject.Find("Jack").GetComponent<Jack>().transform.position;
            c.GetComponent<Rigidbody2D>().AddForce(new Vector2(400.0f*(Toko_master.transform.localScale.x/4), 500.0f));
        }
        if (c.tag == "EnemyBody")
        {
            // Enemy親の座標獲得
            GameObject EnemyV = GameObject.Find("Uni");
            // Enemy親の座標をAddForceで移動させる
            EnemyV.GetComponent<Rigidbody2D>().AddForce(new Vector2(100.0f * (Toko_master.transform.localScale.x / 1.4f), 400.0f));
        }
    }
}
