using UnityEngine;
using System.Collections;

public enum ENEMYAISTS // 敵のAIステート
{
    ACTIONSELECT,       // アクション選択(思考)
    WAIT,               // 一定時間(止まって)待つ
    RUNTOPLAYER,        // 走ってプレイヤーに近づく
    JUMPTOPLAYER,       // ジャンプしてプレイヤーに近づく
    ESCAPE,             // プレイヤーから逃げる
    ATTACKONSIGHT,      // その場から移動せずに攻撃する(遠距離攻撃)
    FREEZ,              // 行動停止(移動処理は行う)
} 
public class EnemyMain : MonoBehaviour {
    // Inspector表示部分
    public int debug_SelectRandomAIState = -1;

    // 外部パラメータ
    [System.NonSerialized]  public ENEMYAISTS aiState = ENEMYAISTS.ACTIONSELECT;

    // キャッシュ
    protected EnemyController enemyCtrl;
    protected GameObject player;
    protected PlayerController playerCtrl;

    // 内部パラメータ
    protected float aiActionTimeLength = 0.0f;
    protected float aiActionTimeStart = 0.0f;
    protected float distanceToPlayer = 0.0f;
    protected float distanceToPlayerPrev = 0.0f;

    // 基本機能実装
    public virtual void Awake()
    {
        enemyCtrl = GetComponent<EnemyController>();
        player = PlayerController.GetGameObject();
        playerCtrl = player.GetComponent<PlayerController>();
    }

	// Use this for initialization
	public virtual void Start () {
	
	}
	
	// Update is called once per frame
	public virtual void Update () {
	
	}

    public virtual void FixedUpdate()
    {
        if (BeginEnemyCommonWork())
        {
            FixedUpdateAI();
            EndEnemyCommonWork();
        }
    }

    public virtual void FixedUpdateAI()
    {

    }

    // 基本AI動作処理
}
