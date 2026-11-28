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
        #region Variables
        // movement & jump variables
        private float moveSpeed = 8;
        private float jumpHeight = 15;
        [NonSerialized] public bool isGrounded;

        public float playerHealth;

        // object related variables
        public PlayerID playerID;
        public Rigidbody2D rb2d;
        [NonSerialized] public CombatSystem combatSystem;

        // portal cooldown variables
        [NonSerialized] public float portalCooldownMax = 0.1f;
        [NonSerialized] public float portalCooldown;

        private bool lose;
        #endregion
        [SerializeField] private Animator animator;

        private float xPosLastFrame;

        public void Start()
        {
            Physics2D.gravity = new Vector2(0, -9.81f); // sets the universal gravity to unity's default
            combatSystem = GetComponent<CombatSystem>();
        }

        public void Update()
        {
            PlayerControls();
            HealthSystem();
            if (portalCooldown > 0) portalCooldown -= Time.deltaTime;

        }
        

        private void PlayerControls()
        {
            float axisX = ArcadeInput.Players[(int)playerID].AxisX; // gets player input for each player
            float movement = axisX * Time.deltaTime * moveSpeed;

            if (combatSystem.Stun()) return;
            
            rb2d.velocity = new Vector2(axisX * moveSpeed, rb2d.velocity.y); // gives velocity based on movement direction & set speed
            
            if (ArcadeInput.Players[(int)playerID].Action2.Pressed && isGrounded)
            {
                rb2d.velocity = new Vector2(rb2d.velocity.x, jumpHeight);
            }

            if (axisX != 0)
            {
                animator.SetBool("P1Running", true);
                animator.SetBool("P2Running", true);
            }
            else 
            {
                animator.SetBool("P1Running", false);
                animator.SetBool("P2Running", false);
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

        private void OnValidate()
        {
            if (rb2d == null) rb2d = GetComponent<Rigidbody2D>();
        }
    }
}