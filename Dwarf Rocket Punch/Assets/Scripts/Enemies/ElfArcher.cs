using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElfArcher : Elf {

    protected override void Start() {
        base.Start();
    }

    protected override void Update() {
        base.Update();
        if (state == State.ATTACKING) {
            AttackMode();
        }
        else {
            armAnimator.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

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
