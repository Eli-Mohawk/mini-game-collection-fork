using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace MiniGameCollection.Games2026.Team02
{
    public class GameMaster : MiniGameBehaviour
    {
        [SerializeField] private PlayerController playerp1;
        [SerializeField] private CombatSystem playerc1;
        [SerializeField] private PlayerController playerp2;
        [SerializeField] private CombatSystem playerc2;

        [SerializeField] private TextMeshProUGUI player1Health;
        [SerializeField] private TextMeshProUGUI player1Tokens;
        [SerializeField] private TextMeshProUGUI player2Health;
        [SerializeField] private TextMeshProUGUI player2Tokens;

        public void Update()
        {
            // displays player health as a string on screen
            player1Health.text = playerp1.playerHealth.ToString();
            player1Tokens.text = playerc1.counterTokens.ToString();
            player2Health.text = playerp2.playerHealth.ToString();
            player2Tokens.text = playerc2.counterTokens.ToString();
        }
    }
}
