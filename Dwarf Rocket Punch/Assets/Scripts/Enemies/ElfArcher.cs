using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElfArcher : Elf {

    public float arrowVelocity;
    public float attackDelay;
    public float arrowLifeDelay;
    public ElfArrow arrowPrefab;

    private Vector3 enemyDirection;
    private float timeSinceAttack;

    protected override void Start() {
        //arrows ignore elf
        //Physics2D.queriesStartInColliders = false;

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

        timeSinceAttack += Time.deltaTime;
    }

    private void AttackMode() {
        mainAnimator.Play("Elf_archer_attack");
        Vector3 delta = playerPosition.position - transform.position;
        //rotate the animator to face the player
        float theta = Mathf.Atan2(delta.y, delta.x);
        enemyDirection = new Vector3(0, 0, theta * Mathf.Rad2Deg + 90);

        armAnimator.transform.localRotation = Quaternion.Euler(enemyDirection);
        armAnimator.Play("Elf_archer_arm_attack");


        if (timeSinceAttack >= attackDelay) Attack();

    }

    private void Attack() {
        timeSinceAttack = 0;

        ElfArrow arrow = Instantiate(arrowPrefab, armAnimator.transform, false);
        arrow.arrowLifeDelay = arrowLifeDelay;

        arrow.transform.parent = null;

        Rigidbody2D arrowRB = arrow.GetComponent<Rigidbody2D>();

        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), arrow.GetComponent<Collider2D>());

        arrowRB.velocity = new Vector2(arrowVelocity, 0);



    }
}
