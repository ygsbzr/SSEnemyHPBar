
using EnemyHPBar;
using HarmonyLib;
using MonoMod.Utils;

namespace SSEnemyHPBar
{
    internal static class Patches
    {
        [HarmonyPatch(typeof(HealthManager),nameof(HealthManager.OnEnable))]
        [HarmonyPostfix]
        public static void OnEnableEnemy(HealthManager __instance)
        {
            GameObject enemy = __instance.gameObject;
            if (enemy.GetComponent<DisableHPBar>() != null)
            {
                return;
            }


            HealthManager hm = enemy.GetComponent<HealthManager>();

            if (hm == null)
            {
                return;
            }

            EnemyDeathEffects ede = enemy.GetComponent<EnemyDeathEffects>();


            bool isBoss = hm.hp >= 200;
            if (enemy.GetComponent<BossMarker>() is BossMarker marker)
            {
                isBoss = marker.isBoss;
            }

            if (!isBoss)
            {
                HPBar hpbar = hm.gameObject.GetComponent<HPBar>();
                if (hpbar != null || hm.hp >= 7000 || hm.isDead)
                {
                    return;
                }

                hm.gameObject.AddComponent<HPBar>();
                SSEnemyHPBarPlugin.Logger.LogDebug($@"Added HP bar to {enemy.name}");
            }
            else
            {
                BossHPBar bossHpBar = hm.gameObject.GetComponent<BossHPBar>();
                if (bossHpBar != null || hm.hp >= 7000 || hm.isDead)
                {
                    return;
                }

                hm.gameObject.AddComponent<BossHPBar>();
                SSEnemyHPBarPlugin.Logger.LogDebug($@"Added Boss HP bar to {enemy.name}");
            }

        }

        [HarmonyPatch]
        public static class EnemyDeathPatch
        {
            static MethodBase TargetMethod()
            {
                Type[] argumentTypes = new Type[] { typeof(float?), typeof(AttackTypes), typeof(NailElements), typeof(GameObject), typeof(float), typeof(bool), typeof(Action<Transform>), typeof(bool).MakeByRefType(), typeof(GameObject).MakeByRefType() };

                
                return AccessTools.Method(
                    typeof(EnemyDeathEffects),
                    nameof(EnemyDeathEffects.ReceiveDeathEvent), 
                    argumentTypes
                );
            }
            static bool Prefix(EnemyDeathEffects __instance)
            {
                var enemyDeathEffects = __instance;
                SSEnemyHPBarPlugin.Logger.LogDebug($@"Enemy {enemyDeathEffects.gameObject.name} dead");
                if (enemyDeathEffects.gameObject.GetComponent<HPBar>() != null)
                {
                    if (enemyDeathEffects.gameObject.GetComponent<HPBar>().oldHP == 0)
                    {
                        var placeHolder = new GameObject();
                        placeHolder.transform.position = enemyDeathEffects.gameObject.transform.position;
                        HealthManager phhm = placeHolder.AddComponent<HealthManager>();
                        HPBar phhp = placeHolder.AddComponent<HPBar>();
                        phhp.currHP = 18;
                        phhp.maxHP = 18;
                        phhm.hp = 0;
                    }
                    else
                    {
                        var placeHolder = new GameObject();
                        placeHolder.transform.position = enemyDeathEffects.gameObject.transform.position;
                        HealthManager phhm = placeHolder.AddComponent<HealthManager>();
                        HPBar phhp = placeHolder.AddComponent<HPBar>();
                        placeHolder.AddComponent<EnemyDeathEffects>();
                        phhp.currHP = enemyDeathEffects.gameObject.GetComponent<HPBar>().oldHP;
                        phhp.maxHP = enemyDeathEffects.gameObject.GetComponent<HPBar>().maxHP;
                        phhm.hp = 0;
                    }
                }
                return true;
            }
        }



    }
}
