using UnityEngine;
using System.Collections;

public class EnemyColliderCheck : MonoBehaviour {

    // プレイヤーコンポーネント格納
    GameObject player;

    // 敵の攻撃力
    public int eAttack = 1;

    void Awake()
    {
        player = GameObject.Find("Toko");
    }

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    // Uniがプレイヤーに触れた時の処理
    void OnCollisionEnter2D(Collision2D P_other)
    {
        if (P_other.gameObject.tag == "Player")
        {
            player.GetComponent<Rigidbody2D>().AddForce(new Vector2(-100.0f * (player.transform.localScale.x / 1.4f), 400.0f));
            player.GetComponent<Toko>().hp = player.GetComponent<Toko>().hp - eAttack;
        }
    }
}
