using UnityEngine;

namespace MiniGameCollection.Games2026.Team04
{
    public class Team04PlayerController : MiniGameBehaviour
    {
        public Team04PlayerID PlayerID;
        private Rigidbody2D rb;
        private float playerSpeed = 8f;
        private float jumpSpeed = 10f;
        private float groundCheckLength = 0.65f;
        public LayerMask groundLayer;
        public string direction = "right";

        void Update()
        {
            float axisX = ArcadeInput.Players[(int)PlayerID].AxisX;

            if (axisX < 0)
            {
                direction = "left";
            }
            else if (axisX > 0)
            {
                direction = "right";
            }

            rb.velocity = new Vector2(axisX * playerSpeed, rb.velocity.y);

            if (ArcadeInput.Players[(int)PlayerID].Action1.Pressed && GroundCheck())
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
            }

            if(ArcadeInput.Player1.Action2.Pressed == true && GetComponentInChildren<Team04RedBox>() != null) 
            {
                GetComponentInChildren<Team04RedBox>().ThrowBox();
            }

            if (ArcadeInput.Player2.Action2.Pressed == true && GetComponentInChildren<Team04BlueBox>() != null)
            {
                GetComponentInChildren<Team04BlueBox>().ThrowBox();
            }
        }

        private void OnValidate()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        bool GroundCheck()
        {
            bool check = Physics2D.Raycast(transform.position, Vector2.down,
                groundCheckLength, groundLayer);

            return check;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundCheckLength);
        }
    }
}
