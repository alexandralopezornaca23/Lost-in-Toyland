using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDeathAnimation : MonoBehaviour
{
    public void DieFromParent()
    {
        BossController bossController = GetComponentInParent<BossController>();

        if (bossController != null)
        {
            bossController.Die();
        }
    }
}
