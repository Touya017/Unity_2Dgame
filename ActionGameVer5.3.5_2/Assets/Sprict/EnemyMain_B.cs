using UnityEngine;
using System.Collections;

public class EnemyMain_B : EnemyMain {

    // Inspector表示
    public int aiIfRUNTOPLAYER = 20;
    public int aiIfJUMPTOPLAYER = 30;
    public int aiIfESCAPE = 10;

    public int damageAttack_B = 3;

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
                    SetAIState(ENEMYAISTS.RUNTOPLAYER, 3.0f);
                }
                else if (n < aiIfRUNTOPLAYER + aiIfJUMPTOPLAYER)
                {
                    SetAIState(ENEMYAISTS.JUMPTOPLAYER, 2.0f);
                }
                else if (n < aiIfRUNTOPLAYER + aiIfJUMPTOPLAYER + aiIfESCAPE)
                {
                    SetAIState(ENEMYAISTS.ESCAPE, Random.Range(2.0f, 5.0f));
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

            case ENEMYAISTS.RUNTOPLAYER: // 近づく処理
                if (GetDistanePlayerY() > 3.0f)
                {
                    SetAIState(ENEMYAISTS.JUMPTOPLAYER, 1.5f);
                }
                if (!enemyCtrl.ActionMoveToNear(player, 2.0f))
                {
                    Attack_B();
                }
                break;

            case ENEMYAISTS.JUMPTOPLAYER: // ジャンプで近づく
                if (GetDistanePlayer() < 2.5f && IsChangeDistanePlayer(0.5f))
                {
                    Attack_B();
                    break;
                }
                enemyCtrl.ActionJump();
                enemyCtrl.ActionMoveToNear(player, 0.1f);
                SetAIState(ENEMYAISTS.FREEZ, 1.5f);
                break;

            case ENEMYAISTS.ESCAPE: // 遠ざかる
                if (!enemyCtrl.ActionMoveToFar(player, 4.0f))
                {
                    SetAIState(ENEMYAISTS.ACTIONSELECT, 1.0f);
                }
                break;
        }
    }

    // アクション処理
    void Attack_B()
    {
        enemyCtrl.ActionLookup(player, 0.1f);
        enemyCtrl.ActionMove(0.0f);
        enemyCtrl.ActionAttack("Attack_B", damageAttack_B);
        SetAIState(ENEMYAISTS.WAIT, 3.0f);
    }
}
