using UnityEngine;

namespace MiniGameCollection.Games2026.Team06
{
    public class PlayerController : MiniGameBehaviour
    {
        [field: SerializeField] public PlayerID PlayerID { get; private set; }
        [field: SerializeField] public Rigidbody2D Rigidbody2D { get; private set; }
        [field: SerializeField] public bool CanPlay { get; private set; } = false;
        [field: SerializeField] public float MoveSpeed { get; private set; } = 5f;



        void Update()
        {
            if (!CanPlay) return;

           ArcadeInput.Player input = ArcadeInput.Player1;
            if (PlayerID == PlayerID.Player2)
                input = ArcadeInput.Player2;

            Vector2 stick = input.Joystick8Way;
            Rigidbody2D.velocity = stick * MoveSpeed;

        }


        private void OnValidate()
        {
            if (Rigidbody2D == null)
                Rigidbody2D = GetComponent<Rigidbody2D>();
        }

        protected override void OnGameStart()
        {
            CanPlay = true;
        }

        protected override void OnGameEnd()
        {
            CanPlay = false;
            Rigidbody2D.velocity = Vector2.zero;
        }
    }
}
