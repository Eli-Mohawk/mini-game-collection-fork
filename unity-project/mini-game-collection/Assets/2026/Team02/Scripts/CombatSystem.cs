using MiniGameCollection.Games2025.Team00;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

namespace MiniGameCollection.Games2026.Team02
{
    public class CombatSystem : MiniGameBehaviour
    {
        // external variables
        public PlayerID playerID;
        PlayerController player;
        float axisY;
        PlayerController opponent;

        // attack variables
        private bool hitable; // can you deal damage
        private int comboCount; // what stage of the combo you are on
        private int comboMax = 3; // max amount of hits in a combo -1 (for math)

        // attack cooldown variables
        private float attackResetDT; // delta time left until you can attack after finishing combo
        private float comboResetDT; // delta time left until combo is reset
        private float attackDT; // delta time left between attacks in combo
        [SerializeField] private float[] cds = { 0.5f, 0.5f, 0.7f, 2f, 0.5f, 0.6f }; // combo 1, 2, 3, 4/reset, up, down

        // stun variables
        [SerializeField] private float stunTime; // duration of stun
        private float stunDT; // delta time left of stun
        public bool stunned = false; // are they stunned

        // knockback variables
        [SerializeField] private float kb = 5; // kbockback applied to opponent
        private float trueKB; // final knockback applied
        [SerializeField] private float[] kbMult = {0.5f, 0.3f, 0.8f, 1.4f, 4f, 5f}; // combo 1, 2, 3, 4, up, down

        // damage calculations
        [SerializeField] private float damage = 5f; // base damage
        private float trueDamage; // actuall damage dealt
        [SerializeField] private float[] damageMult = {0.6f, 0.4f, 0.8f, 1.1f, 0.9f, 1f}; // combo 1, 2, 3, 4, up, down

        private bool actionWasPressed;

        public void Start()
        {
           player = GetComponent<PlayerController>();
        }

        public void Update()
        {
            axisY = ArcadeInput.Players[(int)playerID].AxisY;

            // cooldown timers
            if (stunDT > 0) stunDT -= Time.deltaTime;
            if (attackDT > 0) attackDT -= Time.deltaTime;
            if (attackResetDT > 0) attackResetDT -= Time.deltaTime;
            if (comboResetDT > 0) comboResetDT -= Time.deltaTime;

            // input switch | makes you have to re-press the button to do another action
            bool actionPressed = (ArcadeInput.Players[(int)playerID].Action1.Pressed);
            if (actionPressed && !actionWasPressed) Attack();
            actionWasPressed = actionPressed;

            Debug.Log(hitable);
        }

        private bool Cooldown()
        {
            bool cooldown; // is the player currently on a meaningful cooldown

            if (attackDT > 0) cooldown = true; // if you are on CD from doing an attack
            else if (attackResetDT > 0) cooldown = true; // if you are on CD from finishing a combo
            else cooldown = false; // if neither type of CD is active

            return cooldown;
        }

        public void Attack()
        {
            if (stunned) return;

            // attack types
            if (axisY == 0) BaseAttack();
            if (axisY == 1) UpAttack();
            if (axisY == -1 && !player.isGrounded) DownAttack();
        }

        private void BaseAttack()
        {
            if (Cooldown()) return;

            trueDamage = damage * damageMult[comboCount];
            trueKB = kb * kbMult[comboCount];
            Combo();
        }

        private void Combo()
        {
            attackDT = cds[comboCount];
            comboResetDT = cds[3];

            if (comboCount < comboMax) comboCount++;
            if (comboCount >= comboMax) comboCount = 0;
            if (comboResetDT == 0) comboCount = 0;

            if (!hitable) return;

            opponent.playerHealth -= trueDamage;
            opponent.rb2d.velocity = new Vector2(opponent.rb2d.velocity.x + trueKB, opponent.rb2d.velocity.y);
        }

        private void UpAttack()
        {
            trueDamage = damage * damageMult[4]; // damage multiplied by attack amount
            trueKB = kb * kbMult[4]; // damage multiplied by knockback

            if (Cooldown()) return; // check if the player is on cooldown or not

            player.rb2d.velocity = new Vector2(player.rb2d.velocity.x, 0); // resets y velocity when you attack

            // cooldowns & combo updates
            attackDT = cds[4];
            comboResetDT = cds[3];
            comboCount = 3;

            if (!hitable) return; // only does the following if actually hitting the enemy
            
            opponent.playerHealth -= trueDamage;

            // makes both player go up into the air
            player.rb2d.velocity = new Vector2(player.rb2d.velocity.x, player.rb2d.velocity.y + trueKB);
            opponent.rb2d.velocity = new Vector2(opponent.rb2d.velocity.x, opponent.rb2d.velocity.y + trueKB);

        }

        private void DownAttack()
        {
            trueDamage = damage * damageMult[5];
            trueKB = kb * kbMult[5];

            if (Cooldown()) return;

            player.rb2d.velocity = new Vector2(player.rb2d.velocity.x, player.rb2d.velocity.y - trueKB);

            attackDT = cds[5];
            comboResetDT = cds[3];
            comboCount = 2;

            if (!hitable) return;

            opponent.playerHealth -= trueDamage;

            opponent.rb2d.velocity = new Vector2(opponent.rb2d.velocity.x, opponent.rb2d.velocity.y - trueKB);
        }

        private void Stun()
        {
            
        }

        private void OnTriggerEnter2D(Collider2D Collision)
        {
            if (opponent == null) opponent = Collision.GetComponent<PlayerController>();
            
            if (opponent != null) hitable = true;
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.GetComponent<PlayerController>() == opponent) hitable = false;
        }
    }
}