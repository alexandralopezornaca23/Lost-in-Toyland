using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttackAnimation : MonoBehaviour
{
    public void FireBulletFromParent()
    {
        BossController bossController = GetComponentInParent<BossController>();

        if (bossController != null)
        {
            bossController.FireBullet();
        }
    }
}
