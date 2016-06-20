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
        if (c.tag == "Enemy")
        {
            c.transform.position = GameObject.Find("Uni").GetComponent<Enemy>().transform.position;
            c.GetComponent<Rigidbody2D>().AddForce(new Vector2(300.0f * (Toko_master.transform.localScale.x / 4), 200.0f));
        }
    }
}
