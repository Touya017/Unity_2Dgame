using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriangleSpawner : MonoBehaviour {

    GameObject Triangle;
    Rigidbody2D rb2D;

	// Use this for initialization
	void Start () {
        Triangle = FindObjectOfType<GameObject>();
        rb2D = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
        Triangle = Instantiate(Triangle);
        //Triangle.GetComponent<Transform>() = Vector2.up;
		
	}
}
