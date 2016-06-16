using UnityEngine;
using System.Collections;

public class Jack : MonoBehaviour {

    GameObject player;

    // ゲームオブジェクトにプレイヤー情報を記憶
    void Awake()
    {
        player = GameObject.Find("Toko");
    }

    // Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if(player.transform.position.x > transform.position.x)
        {
            transform.localScale = new Vector3(4.0f, 4.0f, 0.0f);
        }
        else transform.localScale = new Vector3(-4.0f, 4.0f, 0.0f);
	}
}
