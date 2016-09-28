using UnityEngine;
using System.Collections;

public enum ENEMYAISTS // 敵のAIステート
{
    ACTIONSELECT,       // アクション選択(思考)
    WAIT,               // 一定時間(止まって)待つ
    RUNTOPLAYER,        // 走ってプレイヤーに近づく
    JUMPTOPLAYER,       // ジャンプしてプレイヤーに近づく
    ESCAPE,             // プレイヤーから逃げる
    RETURNTODOGPILE,    // ドックパイルに戻る
    ATTACKONSIGHT,      // その場から移動せずに攻撃する(遠距離攻撃)
    FREEZ,              // 行動停止(移動処理は行う)
} 
public class EnemyMain : MonoBehaviour {
    // Inspector表示部分
    public bool cameraSwitch = true;
    public bool inActiveZoneSwitch = false;
    public bool combatAIOerder = true;
    public float dogPileReturnLength = 10.0f;
    public int debug_SelectRandomAIState = -1;
    Rigidbody2D Erb2D;

    // 外部パラメータ
    [System.NonSerialized]  public bool cameraEnabled = false;
    [System.NonSerialized]  public bool inActiveZone = false;
    [System.NonSerialized]  public ENEMYAISTS aiState = ENEMYAISTS.ACTIONSELECT;
    [System.NonSerialized]  public GameObject dogPile;

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
        Erb2D = GetComponent<Rigidbody2D>();
        enemyCtrl = GetComponent<EnemyController>();
        player = PlayerController.GetGameObject();
        playerCtrl = player.GetComponent<PlayerController>();
    }

	// Use this for initialization
	public virtual void Start () {
        // dogPile設置
        StageObject_DogPile[] dogPileList = GameObject.FindObjectsOfType<StageObject_DogPile>();
        foreach(StageObject_DogPile findDogPile in dogPileList)
        {
            foreach(GameObject go in findDogPile.enemyList)
            {
                if(gameObject == go)
                {
                    dogPile = findDogPile.gameObject;
                    break;
                }
            }
        }
	}

    // ジャンプトリガー設定
    void OnTriggerStay2D(Collider2D other)
    {
        // 状態チェック
        if (enemyCtrl.grounded && CheckAction())
        {
            if (other.name == "EnemyJumpTrigger_L")
            {
                if (enemyCtrl.ActionJump())
                {
                    enemyCtrl.ActionMove(-1.0f);
                }
            }
            else if (other.name == "EnemyJumpTrigger_R")
            {
                if (enemyCtrl.ActionJump())
                {
                    enemyCtrl.ActionMove(+1.0f);
                }
            }
            else if (other.name == "EnemyJumpTrigger")
            {
                enemyCtrl.ActionJump();
            }
        }
    }
	
	// Update is called once per frame
	public virtual void Update () {
        cameraEnabled = false;
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

        // アクティブゾーンに入っているか
        if (inActiveZoneSwitch)
        {
            inActiveZone = false;
            Vector3 vecA = player.transform.position + playerCtrl.enemyActiveZonePointA;
            Vector3 vecB = player.transform.position + playerCtrl.enemyActiveZonePointB;
            if(transform.position.x > vecA.x && transform.position.x < vecB.x &&
                transform.position.y > vecA.y && transform.position.y < vecB.y)
            {
                inActiveZone = true;
            }
        }

        // 空中は強制実行(空中設定敵)
        if (enemyCtrl.grounded)
        {
            // カメラ内にいるか
            if(cameraSwitch && !cameraEnabled && !inActiveZone)
            {
                // カメラに映っていない
                enemyCtrl.ActionMove(0.0f);
                enemyCtrl.cameraRendered = false;
                enemyCtrl.animator.enabled = false;
                Erb2D.Sleep();
                return false;
            }
        }
        enemyCtrl.animator.enabled = true;
        enemyCtrl.cameraRendered = true;

        // 状態チェック
        if (!CheckAction())
        {
            return false;
        }

        // ドッグパイル
        if(dogPile != null)
        {
            if(GetDistaneDogPile() > dogPileReturnLength)
            {
                aiState = ENEMYAISTS.RETURNTODOGPILE;
            }
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

    public bool IsChangeDistanePlayer(float l)
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

    public float GetDistaneDogPile()
    {
        return Vector3.Distance(transform.position, dogPile.transform.position);
    }
}
