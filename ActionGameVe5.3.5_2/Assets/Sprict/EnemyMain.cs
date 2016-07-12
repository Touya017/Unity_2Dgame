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
    public bool BeginEnemyCommonWork()
    {
        // 生きているか
        if(enemyCtrl.hp <= 0)
        {
            return false;
        }

        enemyCtrl.animator.enabled = true;

        // 状態チェック
        if (!CheckAction())
        {
            return false;
        }
        return true;
    }

    public void EndEnemyCommonWork()
    {
        // アクションのリミット時間をチェック
        float time = Time.fixedTime - aiActionTimeStart;
        if(time > aiActionTimeLength)
        {
            aiState = ENEMYAISTS.ACTIONSELECT;
        }
    }

    // 敵の現在の状態を確認
    public bool CheckAction()
    {
        // 状態チェック
        AnimatorStateInfo stateInfo = enemyCtrl.animator.GetCurrentAnimatorStateInfo(0);

        if(stateInfo.tagHash == EnemyController.ANISTS_Attack ||
            stateInfo.tagHash == EnemyController.ANISTS_DMG ||
            stateInfo.tagHash == EnemyController.ANISTS_Dead)
        {
            return false;
        }
        return true;
    }

    // AIの状態をランダム数字から選ぶ
    public int SelectRandomAIState()
    {
#if UNITY_EDITOR
        if(debug_SelectRandomAIState >= 0)
        {
            return debug_SelectRandomAIState;
        }
#endif
        return Random.Range(0, 100 + 1);
    }

    // AIの状態を代入
    public void SetAIState(ENEMYAISTS sts, float t)
    {
        aiState = sts;
        aiActionTimeStart = Time.fixedTime;
        aiActionTimeLength = t;
    }

    // コンバットAIを獲得
    public virtual void SetCombatAIState(ENEMYAISTS sts)
    {
        aiState = sts;
        aiActionTimeStart = Time.fixedTime;
        enemyCtrl.ActionMove(0.0f);
    }

    // AIスクリプトサポート用関数
    public float GetDistanePlayer()
    {
        distanceToPlayerPrev = distanceToPlayer;
        distanceToPlayer = Vector3.Distance(transform.position, playerCtrl.transform.position);
        return distanceToPlayer;
    }

    public bool IsChangeDistanePlayerX(float l)
    {
        return (Mathf.Abs(distanceToPlayer - distanceToPlayerPrev) > 1);
    }

    public float GetDistanePlayerX()
    {
        Vector3 posA = transform.position;
        Vector3 posB = playerCtrl.transform.position;
        posA.y = 0; posA.z = 0;
        posB.y = 0; posB.z = 0;
        return Vector3.Distance(posA, posB);
    }

    public float GetDistanePlayerY()
    {
        Vector3 posA = transform.position;
        Vector3 posB = playerCtrl.transform.position;
        posA.x = 0; posA.z = 0;
        posB.x = 0; posB.z = 0;
        return Vector3.Distance(posA, posB);
    }
}
