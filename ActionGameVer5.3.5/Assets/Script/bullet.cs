using UnityEngine;
using System.Collections;

public class bullet : MonoBehaviour {

	// Use this for initialization
	void Start () {
        DestroyObject(this.gameObject, 2.0f);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    // 弾が何かに当たったら判定をする
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag != "Player" && other.tag != "Ground")
        {
            other.transform.localScale += new Vector3(-1.0f, -1.0f, 1.0f);
            if(other.transform.localScale.x <= 0.0f)
            {
                Destroy(other.gameObject);
            }
        }
        Destroy(this.gameObject);
    }
}
