using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElfWarrior : Elf {
    protected override void Update() {
        base.Update();
        AttackMode();
    }
    private void AttackMode() {
        Debug.Log("I'm attacking");
    }
}
