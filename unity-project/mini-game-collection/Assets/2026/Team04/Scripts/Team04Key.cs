using MiniGameCollection;
using MiniGameCollection.Games2026.Team04;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class Team04Key : MiniGameBehaviour
{
    public Scene targetScene;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            SceneManager.LoadScene(targetScene.name);
        }
    }
}
//key's collision with the player, it takes the player to the next level