using JetBrains.Annotations;
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
        private PlayerController player;
        private float axisY;
        private float axisX;
        private float lastX;
        private PlayerController opponent;
        private CombatSystem opponent2;

        // attack variables
        private bool hitable; // can you deal damage
        private int comboCount; // what stage of the combo you are on
        private int comboMax = 3; // max amount of hits in a combo -1 (for math)

        // attack cooldown variables
        private float comboResetDT; // delta time left until combo is reset
        private float attackDT; // delta time left between attacks in combo
        private float[] cds = { 0.3f, 0.4f, 0.8f, 2f, 1.1f, 1.1f }; // combo 1, 2, 3, 4/reset, up, down

        // stun variables
        private float stunTime = 1f; // duration of stun
        [NonSerialized] public float stunDT; // delta time left of stun

        // knockback variables
        private float kb = 1; // kbockback applied
        private float trueKB; // final knockback applied
        private float[] kbMult = {6.5f, 12.5f, 15f}; // combo 4, up, down

        // damage calculations
        private float damage = 5f; // base damage
        private float trueDamage; // actuall damage dealt
        private float[] damageMult = {0.5f, 0.3f, 0.7f, 1.5f, 1f, 1f}; // combo 1, 2, 3, 4, up, down

        // counter-attack variables
        [NonSerialized] public int counterTokens;
        private int counterTokenCap = 2;

        private bool actionWasPressed;

        public void Start()
        {
           player = GetComponent<PlayerController>();
        }

        public void Update()
        {
            axisY = ArcadeInput.Players[(int)playerID].AxisY;
            axisX = ArcadeInput.Players[(int)playerID].AxisX;

            // cooldown timers
            if (stunDT > 0) stunDT -= Time.deltaTime;
            if (attackDT > 0) attackDT -= Time.deltaTime;
            if (comboResetDT > 0) comboResetDT -= Time.deltaTime;

            // input switch | makes you have to re-press the button to do another action
            bool actionPressed = (ArcadeInput.Players[(int)playerID].Action1.Pressed);
            if (actionPressed && !actionWasPressed) Attack();
            actionWasPressed = actionPressed;

            if (comboResetDT <= 0) comboCount = 0;

            CounterAttack();

            if (axisX != 0) lastX = axisX;
        }

        private bool Cooldown()
        {
            bool cooldown; // is the player currently on a meaningful cooldown

            if (attackDT > 0) cooldown = true; // if you are on CD from doing an attack
            else cooldown = false; // if neither type of CD is active

            return cooldown;
        }

        public void Attack()
        {
            if (Stun()) return;

            // attack types
            if (axisY == 0) BaseAttack();
            if (axisY == 1) UpAttack();
            if (axisY == -1 && !player.isGrounded) DownAttack();
        }

        private void BaseAttack()
        {
            if (Cooldown()) return;

            trueDamage = damage * damageMult[comboCount];
            trueKB = kb * lastX * kbMult[0];
            Combo();
        }

        private void Combo()
        {
            bool finisher = false;

            attackDT = cds[comboCount];
            comboResetDT = cds[3];

            if (comboCount < comboMax) comboCount++;
            else { comboCount = 0; finisher = true; }

            if (!hitable) return;

            opponent2.stunDT = stunTime;
            opponent.playerHealth -= trueDamage;

            if (!finisher) return;
            finisher = false;
            opponent.rb2d.velocity = new Vector2(opponent.rb2d.velocity.x + trueKB, opponent.rb2d.velocity.y);
            opponent2.counterTokens++;
        }

        private void UpAttack()
        {
            trueDamage = damage * damageMult[4]; // damage multiplied by attack amount
            trueKB = kb * kbMult[1]; // damage multiplied by knockback

            if (Cooldown()) return; // check if the player is on cooldown or not

            player.rb2d.velocity = new Vector2(player.rb2d.velocity.x, 0); // resets y velocity when you attack

            // cooldowns & combo updates
            attackDT = cds[4];
            comboResetDT = cds[3];
            comboCount = 3;

            if (!hitable) return; // only does the following if actually hitting the enemy

            opponent2.stunDT = stunTime;
            opponent.playerHealth -= trueDamage;

            // makes both player go up into the air
            player.rb2d.velocity = new Vector2(player.rb2d.velocity.x, player.rb2d.velocity.y + trueKB);
            opponent.rb2d.velocity = new Vector2(opponent.rb2d.velocity.x, opponent.rb2d.velocity.y + trueKB);

        }

        private void DownAttack()
        {
            trueDamage = damage * damageMult[5];
            trueKB = kb * kbMult[2];

            if (Cooldown()) return;

            player.rb2d.velocity = new Vector2(player.rb2d.velocity.x, player.rb2d.velocity.y - trueKB);

            attackDT = cds[5];
            comboResetDT = cds[3];
            comboCount = 2;

            if (!hitable) return;

            opponent2.stunDT = stunTime;
            opponent.playerHealth -= trueDamage;

            opponent.rb2d.velocity = new Vector2(opponent.rb2d.velocity.x, opponent.rb2d.velocity.y - trueKB);
        }

        public bool Stun()
        {
            bool stunned;

            if (stunDT > 0) stunned = true;
            else stunned = false;

            return stunned;
        }

        private void CounterAttack()
        {
            if (counterTokens >= counterTokenCap && Stun())
            {
                if (ArcadeInput.Players[(int)playerID].Action1.Pressed)
                {
                    comboCount = 3;
                    BaseAttack();
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D Collision)
        {
            if (opponent == null) opponent = Collision.GetComponent<PlayerController>();
            if (opponent2 == null) opponent2 = Collision.GetComponent<CombatSystem>();
            
            if (opponent != null) hitable = true;
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.GetComponent<PlayerController>() == opponent) hitable = false;
        }
    }
}