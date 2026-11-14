using MiniGameCollection.Games2025.Team00;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Net;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace MiniGameCollection.Games2026.Team02
{
    public class PlayerController : MiniGameBehaviour
    {
        // movement & jump variables
        [SerializeField] private float moveSpeed;
        [SerializeField] private float jumpHeight;
        private bool isGrounded;

        public int playerHealth;

        // attack related variables
        [SerializeField] private int playerDamage;
        [SerializeField] private float attackCooldownMax;
        private float attackCooldown = 0;
        private bool isAttackable;

        // object related variables
        private PlayerController opponent;
        public PlayerID playerID;
        public Rigidbody2D rb2d;

        // portal cooldown variables
        public float portalCooldownMax;
        public float portalCooldown = 0;

        public bool freeze;
        public float freezeMax;
        public float freezeLength = 0;

        private bool lose;

        public void Start()
        {
            Physics2D.gravity = new Vector2(0, -9.81f); // sets the universal gravity to unity's default
        }

        public void Update()
        {
            PlayerControls();
            HealthSystem();
            #region Timers
            if (attackCooldown > 0) attackCooldown -= Time.deltaTime;
            if (portalCooldown > 0) portalCooldown -= Time.deltaTime;
            if (freezeLength > 0) freezeLength -= Time.deltaTime;
            #endregion
        }

        private void PlayerControls()
        {
            // freeze is in place to make a buffer after teleporting for better movement control
            if (!freeze) // not frozen is default
            {
                float axisX = ArcadeInput.Players[(int)playerID].AxisX; // gets player input for each player
                float movement = axisX * Time.deltaTime * moveSpeed;
                rb2d.velocity = new Vector2(axisX * moveSpeed, rb2d.velocity.y); // gives velocity based on movement direction & set speed

                
            }
            else if (freeze) // after using a portal you stop movement
            {
                rb2d.velocity = new Vector2(0, rb2d.velocity.y);
            }
            if (freezeLength <= 0)
            {
                freeze = false;
            }

            if (ArcadeInput.Players[(int)playerID].Action1.Pressed) CombatSystem();
            if (ArcadeInput.Players[(int)playerID].Action2.Pressed && isGrounded)
            {
                rb2d.velocity = new Vector2(rb2d.velocity.x, jumpHeight);
            }
        }

        // checks for collision with ground objects and enables jumps if colliding
        #region GroundDetection
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Respawn"))
            {
                Vector3 normal = collision.GetContact(0).normal; // gets the direction of the objects collision
                if (normal == Vector3.up) isGrounded = true; // if you collide with the top you are grounded
            }
        }
        private void OnCollisionExit2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Respawn"))
            {
                isGrounded = false;
            }
        }
        #endregion

        private void HealthSystem()
        {
            if (playerHealth <= 0)
            {
                lose = true;
            }
        }

        private void CombatSystem()
        {
            if (attackCooldown <= 0)
            {
                if (isAttackable) opponent.playerHealth -= playerDamage;
                attackCooldown = attackCooldownMax;
            }
        }

        // detects other objects with this script (aka players) an object that can be interacted with
        #region OpponentDetection
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
        #endregion

        private void OnValidate()
        {
            if (rb2d == null) rb2d = GetComponent<Rigidbody2D>();
        }
    }
}