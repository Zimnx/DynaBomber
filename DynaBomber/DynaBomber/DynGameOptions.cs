using System;
using System.Linq;
using System.Text;

namespace DynaBomber
{
    /// <summary>
    /// Class with possible options for the game
    /// Useful when extending game (no need to pass additional parameters etc.)
    /// </summary>
    class DynGameOptions
    {
        public int boardSize { get; set; }
        public int playerNumber { get; set; }
        public int monsterNumber { get; set; }
        public string settingsFileName { get; set; }

        public DynGameOptions(int boardSize, int playerNumber, int monsterNumber)
        {
            this.boardSize = boardSize;
            this.playerNumber = playerNumber;
            this.monsterNumber = monsterNumber;
        }
    }
}