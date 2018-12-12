using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class inherits from Elf and is used to create a specific class of elf.
/// In this case, an archer.
/// </summary>
/// 
/// Author: Evan Funnell    EVF
/// 
public class ElfArcher : Elf {

    protected override void Start() {
        base.Start();
    }

    /// <summary>
    /// Update function inherits from base class update, and adds a new state check to perform actions 
    /// needed to attack the player
    /// </summary>
    /// 
    /// 2018-12-05  EVF     Initial State
    /// 2018-12-08  EPM     Added Arm Animator
    /// 
    protected override void Update() {
        base.Update();
        if (state == State.ATTACKING) {
            AttackMode();
        }
        else {
            armAnimator.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

    /// <summary>
    /// Fires an arrow at the player
    /// </summary>
    /// 
    /// 2018-12-08  EPM     Added Animations and arrow shooting
    /// 
    private void AttackMode() {
        mainAnimator.Play("Elf_archer_attack");
        Vector3 delta = playerPosition.position - transform.position;
        //rotate the animator to face the player
        float theta = Mathf.Atan2(delta.y, delta.x);
        armAnimator.transform.localRotation = Quaternion.Euler(0, 0, theta * Mathf.Rad2Deg + 90);
        armAnimator.Play("Elf_archer_arm_attack");
        if (!arrowParticle.isPlaying) arrowParticle.Play();
    }
}
