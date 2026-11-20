using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// to-do
/// lose/win mechanic | either opponent hits zero hp or the one with more hp when time runs out
/// fix attacks
/// combo breaker
/// dodge
/// change kb to stun type

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
            // displays player health as a string on screen
            player1Health.text = player1.playerHealth.ToString();
            player2Health.text = player2.playerHealth.ToString();
        }
    }
}
