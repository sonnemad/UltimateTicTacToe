using System;

namespace UltimateTicTacToe
{
	public class HumanConsolePlayer : ITicTacToePlayer
	{
		public HumanConsolePlayer(Player player)
		{
			Player = player;
		}

		private Player Player { get; }

		Player ITicTacToePlayer.GetPlayer()
		{
			return Player;
		}

        private Taunt _taunt;
        public void SetTauntDelegate(Taunt taunt){
            _taunt = taunt;
        }

		UltimateTicTacToeMove ITicTacToePlayer.TakeTurn(UltimateTicTacToeBoard ticTacToeBoard)
		{
			Console.Clear();

			Console.WriteLine(ticTacToeBoard.ToString());
			Console.WriteLine();
			if (ticTacToeBoard.LastMove != null)
			{
				Console.WriteLine($"{ticTacToeBoard.LastMove.Player} made the move [{BoardToCell(ticTacToeBoard.LastMove.OuterCell)}:{BoardToCell(ticTacToeBoard.LastMove.InnerCell)}]");
				Console.WriteLine("");
			}
			Console.WriteLine($"Player {Player}'s, Please make your move!");
			byte outerCell;
			if (ticTacToeBoard.RequiredOuterCell == null)
			{
                Console.WriteLine("You get to play anywhere, choose wisely!");
				Console.Write("Outer Cell 1-9: ");
				byte.TryParse(Console.ReadLine(), out outerCell);
				outerCell = CellToBoard(outerCell);
				Console.WriteLine();
			}
			else
			{
				outerCell = ticTacToeBoard.RequiredOuterCell.Value;
			}

            var msg = "You are now playing in the ";
            switch(outerCell){
                case 0:
                    msg += "upper left cell";
                    break;
                case 1:
                    msg += "upper middle cell";
                    break;
                case 2:
                    msg += "upper right cell";
                    break;
                case 3:
                    msg += "middle left cell";
                    break;
                case 4:
                    msg += "center cell";
                    break;
                case 5:
                    msg += "middle right cell";
                    break;
                case 6:
                    msg += "lower left cell";
                    break;
                case 7:
                    msg += "lower center cell";
                    break;
                case 8:
                    msg += "lower right cell";
                    break;
                default:
                    msg += "DEFAULT ERROR";
		            break;
            }
			Console.WriteLine();
			Console.WriteLine(msg);
			Console.WriteLine();

			Console.Write("Inner Cell 1-9: ");
			byte innerCell;
			byte.TryParse(Console.ReadLine(), out innerCell);
			innerCell = CellToBoard(innerCell);

			ticTacToeBoard.MakeMove(Player, outerCell, innerCell);
			Console.Clear();
			Console.WriteLine(ticTacToeBoard.ToString());

			return new UltimateTicTacToeMove(Player, outerCell, innerCell);
		}

		private byte CellToBoard(byte cell)
		{
			if (cell >= 7)
			{
				return (byte) (cell - 7);
			}
			if (cell <= 3)
			{
				return (byte) (cell + 5);
			}
			return (byte) (cell - 1);
		}

		private byte BoardToCell(byte cell)
		{
			if (cell <= 2)
			{
				return (byte)(cell + 7);
			}
			if (cell >= 6)
			{
				return (byte)(cell - 5);
			}
			return (byte)(cell + 1);
		}
	}


	public class SimpleAIPlayer : ITicTacToePlayer
	{
		public SimpleAIPlayer(Player player)
		{
			Player = player;
		}

		private Player Player { get; }

		Player ITicTacToePlayer.GetPlayer()
		{
			return Player;
		}

        private Taunt _taunt;
		public void SetTauntDelegate(Taunt taunt)
		{
			_taunt = taunt;
        }

		UltimateTicTacToeMove ITicTacToePlayer.TakeTurn(UltimateTicTacToeBoard ticTacToeBoard)
		{
			byte outerCell;
			if (ticTacToeBoard.RequiredOuterCell == null)
			{
				var validCells = ticTacToeBoard.GetValidOuterCells();
				outerCell = validCells[new Random().Next(0, validCells.Count)];
			}
			else
			{
				outerCell = ticTacToeBoard.RequiredOuterCell.Value;
			}

			var validInnerCells = ticTacToeBoard.GetValidInnerCells(outerCell);
			var innerCell = validInnerCells[new Random().Next(0, validInnerCells.Count)];

			return new UltimateTicTacToeMove(Player, outerCell, innerCell);
		}
	}

	public delegate void Taunt(string msg);
	internal interface ITicTacToePlayer
	{
		Player GetPlayer();
		UltimateTicTacToeMove TakeTurn(UltimateTicTacToeBoard ticTacToeBoard);
        void SetTauntDelegate(Taunt taunt);
	}
}