using UnityEngine;
using System.Collections;

public class StageObject_DogPile : MonoBehaviour {

    // 敵キャラクター格納
    public GameObject[] enemyList;
    // 削除用
    public GameObject[] destroyObjectList;

	// Use this for initialization
	void Start () {
        InvokeRepeating("CheckEnemy", 0.0f, 1.0f);
	}
	
    void CheckEnemy()
    {
        // 登録されている敵リストから敵の生存情報を確認
        bool flag = true;
        foreach(GameObject enemy in enemyList)
        {
            if(enemy != null)
            {
                flag = false;
            }
        }
        // すべての敵が倒されているか
        if (flag)
        {
            // 登録されている破壊物リストのオブジェクトを削除
            foreach(GameObject destroyObject in destroyObjectList)
            {
                Destroy(destroyObject, 1.0f);
            }
            CancelInvoke("CheckEnemy");
        }
    }
}
