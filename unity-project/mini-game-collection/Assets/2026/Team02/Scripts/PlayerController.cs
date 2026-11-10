using MiniGameCollection.Games2025.Team00;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MiniGameCollection.Games2026.Team02
{
    public class PlayerController : MiniGameBehaviour
    {
        [SerializeField] private float moveSpeed;
        [SerializeField] private float jumpHeight;
        private bool isGrounded;

        public int playerHealth;

        [SerializeField] private int playerDamage;
        [SerializeField] private float attackCooldownMax;
        private float cooldown = 0;
        private bool isAttackable;

        private PlayerController opponent;
        public PlayerID playerID;
        public Rigidbody2D rb2d;

        private bool lose;

        public void Update()
        {
            PlayerControls();
            HealthSystem();
            if (cooldown > 0) cooldown -= Time.deltaTime;
        }

        private void PlayerControls()
        {
            float axisX = ArcadeInput.Players[(int)playerID].AxisX;
            float movement = axisX * Time.deltaTime * moveSpeed;
            Vector3 newPositionX = transform.position + new Vector3(movement, 0, 0);
            rb2d.MovePosition(newPositionX);

            if (ArcadeInput.Players[(int)playerID].Action1.Pressed) CombatSystem();
            if (ArcadeInput.Players[(int)playerID].Action2.Pressed)
            {

            }
        }

        private void HealthSystem()
        {
            if (playerHealth <= 0)
            {
                lose = true;
            }
        }

        private void CombatSystem()
        {
            if (cooldown <= 0)
            {
                if (isAttackable) opponent.playerHealth -= playerDamage;
                cooldown = attackCooldownMax;
            }
        }

        private void OnTriggerEnter2D(Collider2D Collision)
        {
            opponent = Collision.GetComponent<PlayerController>();
            if (opponent != null) isAttackable = true;
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.GetComponent<PlayerController>() == opponent)
            {
                opponent = null;
                isAttackable = false;
            }
        }

        private void OnValidate()
        {
            if (rb2d == null) rb2d = GetComponent<Rigidbody2D>();
        }
    }
}