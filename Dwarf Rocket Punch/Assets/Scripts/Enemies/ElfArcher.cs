using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElfArcher : Elf {
    [SerializeField] private float detectionAngle = 90f;

    private float elfPlayerDot;
    private Vector2 direction;

    public Transform playerPosition;

    protected override void Update() {
        base.Update();
        direction = playerPosition.position - transform.position;
        elfPlayerDot = Vector2.Angle(direction, transform.right);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction);
        Debug.DrawRay(transform.position, direction, Color.red);

        if (elfPlayerDot < detectionAngle * 0.5f) {
            Debug.Log("I see you");
            Debug.Log(hit.collider.name);
            Debug.Log(playerPosition.name);
            if (hit.collider.name != playerPosition.parent.name) {
                Debug.Log("Why you behind a wall, boo you suck");
                state = State.PATROLLING;
            }
            else {
                state = State.ATTACKING;
            }
        }

        if (state == State.ATTACKING) {
            AttackMode();
        }
    }

    private void AttackMode() {
        Debug.Log("I'm attacking, ARRGGGG");
    }
}
