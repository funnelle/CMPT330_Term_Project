using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElfArcher : Elf {
    [SerializeField] private float detectionAngle = 45f;

    private float detectionAngleInRads;
    private float detectionCos;
    private float elfPlayerDot;
    private Vector2 rayDirection;

    public Transform playerPosition;

    protected override void Start() {
        base.Start();
        detectionAngleInRads = detectionAngle * Mathf.Deg2Rad;
        detectionCos = Mathf.Cos(detectionAngleInRads);
    }

    protected override void Update() {
        base.Update();
        rayDirection = playerPosition.position - transform.position;
       
        RaycastHit2D hit = Physics2D.Raycast(transform.position, rayDirection);
        Debug.DrawRay(transform.position, rayDirection, Color.red);
        elfPlayerDot = Vector2.Dot(transform.position, playerPosition.position);

        if (detectionCos > elfPlayerDot) {
            Debug.Log("I see you");
            Debug.Log(hit.collider.name);
            Debug.Log(playerPosition.name);
            if (hit.collider.name != playerPosition.parent.name) {
                Debug.Log("Why you behind a wall, boo you suck");
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
