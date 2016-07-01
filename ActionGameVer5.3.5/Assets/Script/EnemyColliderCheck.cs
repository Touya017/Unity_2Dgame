using UnityEngine;
using System.Collections;

public class EnemyColliderCheck : MonoBehaviour {

<<<<<<< HEAD
    // 敵の攻撃力
    public int eAttack = 1;

    // プレイヤーコンポーネント格納
    GameObject player;

    // プレイヤーコンポーネント記憶
=======
    // プレイヤーコンポーネント格納
    GameObject player;

    // 敵の攻撃力
    public int eAttack = 1;

>>>>>>> 23aa46af0fc4f4c75d5ca7d58ebb127c49e10803
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
<<<<<<< HEAD

=======
>>>>>>> 23aa46af0fc4f4c75d5ca7d58ebb127c49e10803
    // Uniがプレイヤーに触れた時の処理
    void OnCollisionEnter2D(Collision2D P_other)
    {
        if (P_other.gameObject.tag == "Player")
        {
            player.GetComponent<Rigidbody2D>().AddForce(new Vector2(-100.0f * (player.transform.localScale.x / 1.4f), 400.0f));
            player.GetComponent<Toko>().hp = player.GetComponent<Toko>().hp - eAttack;
<<<<<<< HEAD
            Debug.Log(player.GetComponent<Toko>().hp);
=======
>>>>>>> 23aa46af0fc4f4c75d5ca7d58ebb127c49e10803
        }
    }
}
