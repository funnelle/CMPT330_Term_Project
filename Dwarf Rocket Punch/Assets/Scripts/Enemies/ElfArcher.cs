﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElfArcher : Elf {
    protected override void Update() {
        base.Update();
        if (state == State.ATTACKING) {
            AttackMode();
        }
    }

    private void AttackMode() {
        Debug.Log("I'm attacking, ARRGGGG");
    }
}
