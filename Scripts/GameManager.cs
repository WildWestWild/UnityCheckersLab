using Checkers;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class GameManager: MonoBehaviour, IGameManager
    {
        [SerializeField] private Text VictoryPhrase;
        private DeskGenerator _deskGenerator;
        private float WHITE_X_VICTORY_LINE = 41f;
        private float BLACK_X_VICTORY_LINE = -29f;
        public bool IsWhiteMove { get; private set; } = true;
        public bool IsStopGame { get; private set; }
        public int Rows { get;  private set; }

        public void Awake()
        {
            VictoryPhrase.enabled = false;
        }

        public void Init(DeskGenerator deskGenerator, int rows)
        {
            _deskGenerator = deskGenerator;
            Rows = rows;
        }

        /// <summary>
        /// Проверка условия победы
        /// </summary>
        public void CheckVictory(ChipComponent chip)
        {
            if (IsWhiteMove && chip.Pair.GetCoordinate().Y == ( Rows - 1))
            {
                ShowVictory();
            }

            if (!IsWhiteMove && chip.Pair.GetCoordinate().Y == 0)
            {
                ShowVictory();
            }
        }

        public void ShowVictory()
        {
            VictoryPhrase.text = IsWhiteMove
                ? "Белые победили!"
                : "Черные одержали победу!";
            VictoryPhrase.enabled = true;
            IsStopGame = true;
        }

        /// <summary>
        /// Передать ход другому
        /// </summary>
        public void PassMove()
        {
            IsWhiteMove = !IsWhiteMove;
            _deskGenerator.InitiateTurnDesk();
        }
    }
}