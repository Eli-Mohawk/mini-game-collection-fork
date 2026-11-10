using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// dynamic health bar
/// jump
/// attack chain
/// attack type (up/down/right/left)
/// attack hitboxes
/// game timer


namespace MiniGameCollection.Games2026.Team02
{
    public class GameMaster : MiniGameBehaviour
    {
        [SerializeField] private PlayerController player1;
        [SerializeField] private PlayerController player2;

        [SerializeField] private TextMeshProUGUI player1Health;
        [SerializeField] private TextMeshProUGUI player2Health;

        public void Update()
        {
            player1Health.text = player1.playerHealth.ToString();
            player2Health.text = player2.playerHealth.ToString();
        }
    }
}
