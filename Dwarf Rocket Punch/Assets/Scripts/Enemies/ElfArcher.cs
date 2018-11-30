using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElfArcher : Elf {
    [SerializeField] private float detectionAngle = 45f;

    private float detectionAngleInRads;
    private float detectionCos;
    private Transform playerPosition;
    private float elfPlayerDot;

    protected override void Start() {
        base.Start();
        playerPosition = transform.Find("Dwarf");
        detectionAngleInRads = detectionAngle * Mathf.Deg2Rad;
        detectionCos = Mathf.Cos(detectionAngleInRads);
    }

    protected override void Update() {
        base.Update();
        elfPlayerDot = Vector2.Dot(transform.position, playerPosition.position);
        if (detectionCos > elfPlayerDot) {
            Debug.Log("I see you");
            state = State.ATTACKING;
        }

        if (state == State.ATTACKING) {
            AttackMode();
        }
    }
    private void AttackMode() {
        Debug.Log("I'm attacking");
    }
}
