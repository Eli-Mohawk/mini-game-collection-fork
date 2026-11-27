using UnityEngine;

namespace MiniGameCollection.Games2026.Team04
{
    public class Team04RedBox : MiniGameBehaviour
    {
        private Rigidbody2D rb2d;
        private void OnValidate()
        {
            rb2d = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            if (transform.parent != null)
            {
                transform.localPosition = transform.parent.localPosition - transform.parent.localPosition;
            }
        }
        
        public void ThrowBox()
        {
            if (GetComponentInChildren<Team04RedBox>() != null && ArcadeInput.Player1.Action2.Pressed)
            {
                transform.parent = null;
                rb2d.velocity += Vector2.one;
            }
        }
    }
}
