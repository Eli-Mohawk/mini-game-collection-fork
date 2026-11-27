using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MiniGameCollection.Games2026.Team02
{
    public class PortalLogic : MiniGameBehaviour
    {
        [SerializeField] private Transform targetLocation;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            // sets player to whichever player collides with the portal
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            if (player != null && player.portalCooldown <= 0) // makes it only work if off cooldown and there is actually a player colliding
            {
                // teleports the player to the chosen portals position and starts the cooldown for both
                player.portalCooldown = player.portalCooldownMax;
                player.rb2d.position = new Vector2(targetLocation.position.x, targetLocation.position.y - 0.5f); // -0.5f smooths the movement after teleporting
                player.playerHealth -= 1; // stops players from over-abusing portals
            }
        }
    }
}
