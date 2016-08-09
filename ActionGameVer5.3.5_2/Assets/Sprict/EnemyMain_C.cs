using UnityEngine;
using System.Collections;

public class EnemyMain_C : EnemyMain {

    // Inspector表示
    public int aiIfRUNTOPLAYER = 20;
    public int aiIfJUMPTOPLAYER = 30;
    public int aiIfESCAPE = 10;

    public int damageAttack_A = 2;
    public int damageAttack_B = 2;
    public int damageAttack_C = 2;

    // AI思考処理
    public override void FixedUpdateAI()
    {
        // AIステート
        switch (aiState)
        {
            case ENEMYAISTS.ACTIONSELECT: //思考の起点
                // アクションの選択
                int n = SelectRandomAIState();
                if (n < aiIfRUNTOPLAYER)
                {
                    SetAIState(ENEMYAISTS.RUNTOPLAYER, 7.0f);
                }
                else if (n < aiIfRUNTOPLAYER + aiIfJUMPTOPLAYER)
                {
                    SetAIState(ENEMYAISTS.JUMPTOPLAYER, 5.0f);
                }
                else if (n < aiIfRUNTOPLAYER + aiIfJUMPTOPLAYER + aiIfESCAPE)
                {
                    SetAIState(ENEMYAISTS.ESCAPE, Random.Range(5.0f, 8.0f));
                }
                else
                {
                    SetAIState(ENEMYAISTS.WAIT, 1.0f + Random.Range(0.0f, 1.0f));
                }
                enemyCtrl.ActionMove(0.0f);
                break;

            case ENEMYAISTS.WAIT: // 休憩
                enemyCtrl.ActionLookup(player, 0.1f);
                enemyCtrl.ActionMove(0.0f);
                break;

            case ENEMYAISTS.RUNTOPLAYER: // 走って近づく処理
                if (GetDistanePlayerY() > 10.0f)
                {
                    SetAIState(ENEMYAISTS.JUMPTOPLAYER, 5.0f);
                }
                if (!enemyCtrl.ActionMoveToFar (player, 5.0f))
                {
                    Attack_A();
                }
                break;

            case ENEMYAISTS.JUMPTOPLAYER: // ジャンプで近づく
                if (GetDistanePlayer() < 7.0f && IsChangeDistanePlayer(5.5f))
                {
                    Attack_C();
                    break;
                }
                enemyCtrl.ActionJump();
                enemyCtrl.ActionMoveToNear(player, 3.5f);
                SetAIState(ENEMYAISTS.FREEZ, 1.5f);
                break;

            case ENEMYAISTS.ESCAPE: // 遠ざかる
                if (!enemyCtrl.ActionMoveToFar(player, 5.0f))
                {
                    SetAIState(ENEMYAISTS.ACTIONSELECT, 1.0f);
                }
                break;
        }
    }

    // アクション処理
    void Attack_A()
    {
        enemyCtrl.ActionLookup(player, 0.1f);
        enemyCtrl.ActionMove(0.0f);
        enemyCtrl.ActionAttack("Attack_A", damageAttack_A);
        SetAIState(ENEMYAISTS.WAIT, 3.0f);
    }

    void Attack_B()
    {
        enemyCtrl.ActionLookup(player, 0.1f);
        enemyCtrl.ActionMove(0.0f);
        enemyCtrl.ActionAttack("Attack_B", damageAttack_B);
        SetAIState(ENEMYAISTS.WAIT, 3.0f);
    }

    void Attack_C()
    {
        enemyCtrl.ActionLookup(player, 0.1f);
        enemyCtrl.ActionMove(0.0f);
        enemyCtrl.ActionAttack("Attack_C", damageAttack_C);
        SetAIState(ENEMYAISTS.WAIT, 3.0f);
    }
}