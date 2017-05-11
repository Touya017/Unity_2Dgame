using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    // Inspector表示
    public int bulletSpeed = 10;
    Rigidbody2D rb2DB;

	// Use this for initialization
	void Start () {
        rb2DB = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
        rb2DB.velocity = transform.up.normalized * bulletSpeed;
	}
}
