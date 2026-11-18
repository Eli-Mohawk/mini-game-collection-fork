using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Key : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
            }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Key")
        {
            SceneManager.LoadScene("Level 2");
        }

        if (collision.gameObject.name == "Key 2")
        {
            SceneManager.LoadScene("Win");
        }
    }
}
//key's collision with the player, it takes the player to the next level