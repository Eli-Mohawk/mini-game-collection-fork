using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MiniGameCollection.Games2026.Team02
{
    public class PortalLogic : MiniGameBehaviour
    {
        [SerializeField] private Transform targetLocation;

        [SerializeField] private PlayerController player1;
        [SerializeField] private PlayerController player2;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            // sets player to whichever player collides with the portal
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            if (player != null && player.portalCooldown <= 0) // makes it only work if off cooldown and there is actually a player colliding
            {
                // teleports the player to the chosen portals position, freezes them beifly and starts the cooldown for both
                player.portalCooldown = player.portalCooldownMax;
                player.rb2d.position = targetLocation.position;
                player.freeze = true;
                player.freezeLength = player.freezeMax;
            }
        }
    }
}
