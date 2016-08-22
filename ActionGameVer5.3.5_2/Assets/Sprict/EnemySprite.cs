using UnityEngine;
using System.Collections;

public class EnemySprite : MonoBehaviour
{
    EnemyMain enemyMain;

    void Awake()
    {
        // EnemyMainの取得
        enemyMain = GetComponentInParent<EnemyMain>();
    }

    void OnWillRenderObject()
    {
        if (Camera.current.tag == "MainCamera")
        {
            // 処理
            enemyMain.cameraEnabled = true;
        }
    }
}