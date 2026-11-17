using MiniGameCollection.Games2025.Team00;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
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
        private float attackCD; // cooldown from ending a combo until you can attack again
        private float attackResetDT; // delta time left until you can attack after finishing combo
        private bool hitable; // can you deal damage
        private int comboCount; // what stage of the combo you are on
        private int comboMax = 4; // max amount of hits in a combo
        private float comboResetCD = 2f; // cooldown after hitting with a combo attack until the combo is reset to 0
        private float comboResetDT; // delta time left until combo is reset

        // attack cooldown variables
        private float comboCD1; // cooldown after hit 1 in combo
        private float comboCD2; // cooldow after hit 2 in combo
        private float comboCD3; // cooldown after hit 3 in combo
        private float upCD = 0.4f; // cooldown after up attack
        private float downCD; // cooldown after down attack
        private float attackDT; // delta time left for attack cooldowns

        // stun variables
        [SerializeField] private float stunTime; // duration of stun
        private float stunDT; // delta time left of stun
        private bool stunned = false; // are they stunned

        // damage calculations
        private float damage = 5f; // base damage
        private float trueDamage; // actuall damage dealt
        private float comboMult1 = 0.6f; // combo hit 1 damage multiplier
        private float comboMult2 = 0.4f; // combo hit 2 damage multiplier
        private float comboMult3 = 0.8f; // combo hit 3 damage multiplier
        private float comboMult4 = 1.1f; // combo hit 4 damage multiplier
        private float upMult = 0.9f; // up attack damage multiplier
        private float downMult = 1f; // down attack damage multiplier

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
            if (axisY == -1 && !player.isGrounded) DownAttack();
            if (axisY == 1) UpAttack();
            if (axisY == 0) BaseAttack();
        }

        private void BaseAttack()
        {
            if (Cooldown()) return;

            if (comboCount == 0)
            {
                trueDamage = damage * comboMult1;

                attackCD = comboCD1;

                if (opponent == null) return;

                opponent.playerHealth -= trueDamage;

            }
            if (comboCount == 1)
            {
                trueDamage = damage * comboMult2;

                attackCD = comboCD2;

                if (opponent == null) return;

                opponent.playerHealth -= trueDamage;
            }
            if (comboCount == 2)
            {
                trueDamage = damage * comboMult3;

                attackCD = comboCD3;

                if (opponent == null) return;

                opponent.playerHealth -= trueDamage;
            }
            if (comboCount == 3)
            {
                trueDamage = damage * comboMult4;

                attackResetDT = attackCD;

                if (opponent == null) return;

                opponent.playerHealth -= trueDamage;
            }
        }

        private void DownAttack()
        {
            trueDamage = damage * downMult;

            if (Cooldown()) return;

            player.rb2d.velocity = new Vector2(player.rb2d.velocity.x, 0);
            player.rb2d.velocity = new Vector2(player.rb2d.velocity.x, player.rb2d.velocity.y - 16);

            attackDT = upCD;
            comboResetDT = comboResetCD;
            comboCount = 2;

            if (opponent == null) return;

            opponent.playerHealth -= trueDamage;

            opponent.rb2d.velocity = new Vector2(opponent.rb2d.velocity.x, opponent.rb2d.velocity.y - 16);
        }

        private void UpAttack()
        {
            trueDamage = damage * upMult; // damage multiplied by attack amount

            if (Cooldown()) return; // check if the player is on cooldown or not

            player.rb2d.velocity = new Vector2(player.rb2d.velocity.x, 0); // resets y velocity when you attack

            // cooldowns & combo updates
            attackDT = upCD;
            comboResetDT = comboResetCD;
            comboCount = 3;

            if (opponent == null) return; // only does the following if actually hitting the enemy
            
            opponent.playerHealth -= trueDamage;

            // makes both player go up into the air
            player.rb2d.velocity = new Vector2(player.rb2d.velocity.x, player.rb2d.velocity.y + 16);
            opponent.rb2d.velocity = new Vector2(opponent.rb2d.velocity.x, opponent.rb2d.velocity.y + 16);
            
        }

        private void Stun()
        {

        }

        private void OnTriggerEnter2D(Collider2D Collision)
        {
            opponent = Collision.GetComponent<PlayerController>();
            if (opponent != null) hitable = true;
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.GetComponent<PlayerController>() == opponent)
            {
                opponent = null;
                hitable = false;
            }
        }
    }
}