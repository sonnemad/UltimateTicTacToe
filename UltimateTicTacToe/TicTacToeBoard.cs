using System.Collections.Generic;
using System.Linq;

namespace UltimateTicTacToe
{
	public class TicTacToeBoard
	{
		private readonly Player[] _grid = new Player[9];
		private readonly LinkedList<TicTacToeMove> _moves = new LinkedList<TicTacToeMove>();

		public TicTacToeBoard()
		{
			for (var i = 0; i < 9; i++)
			{
				_grid[i] = Player.Empty;
			}
			State = GameState.Open;
			WinningMove = null;
		}

		public Player[] GameBoard => (Player[]) _grid.Clone();

		public GameState State { get; private set; }
		public TicTacToeMove[] WinningMove { get; private set; }

		public bool MakeMove(Player player, byte cell)
		{
			if (cell > 8)
			{
//				throw new ArgumentOutOfRangeException(nameof(cell), cell, "value must be 0-8;");
				return false;
			}
			if (MoveTaken(cell))
			{
				return false;
			}
			_moves.AddLast(new TicTacToeMove(player, cell));
			_grid[cell] = player;
			CalculateGameState();
			return true;
		}

		private void CalculateGameState()
		{
			if (State != GameState.Open)
			{
				return;
			}

			if (!_grid.Any(cell => cell == Player.Empty))
			{
				//Last move was taken, game is either won by someone or a draw
				// set as draw here and it will be overwritten by winning move below
				State = GameState.Draw;
			}

			//Check Horizontal
			if (_grid[0] != Player.Empty && _grid[0].Equals(_grid[1]) && _grid[0].Equals(_grid[2]))
			{
				SetWinningMove(0, 1, 2, _grid[0]);
			}
			else if (_grid[3] != Player.Empty && _grid[3].Equals(_grid[4]) && _grid[3].Equals(_grid[5]))
			{
				SetWinningMove(3, 4, 5, _grid[3]);
			}
			else if (_grid[6] != Player.Empty && _grid[6].Equals(_grid[7]) && _grid[6].Equals(_grid[8]))
			{
				SetWinningMove(6, 7, 8, _grid[6]);
			}

			//Check Vertical
			else if (_grid[0] != Player.Empty && _grid[0].Equals(_grid[3]) && _grid[0].Equals(_grid[6]))
			{
				SetWinningMove(0, 3, 6, _grid[0]);
			}
			else if (_grid[1] != Player.Empty && _grid[1].Equals(_grid[4]) && _grid[1].Equals(_grid[7]))
			{
				SetWinningMove(1, 4, 7, _grid[1]);
			}
			else if (_grid[2] != Player.Empty && _grid[2].Equals(_grid[5]) && _grid[2].Equals(_grid[8]))
			{
				SetWinningMove(2, 5, 8, _grid[2]);
			}

			//Check Diagonal
			else if (_grid[4] != Player.Empty && _grid[4].Equals(_grid[0]) && _grid[4].Equals(_grid[8]))
			{
				SetWinningMove(0, 4, 8, _grid[4]);
			}
			else if (_grid[4] != Player.Empty && _grid[4].Equals(_grid[2]) && _grid[4].Equals(_grid[6]))
			{
				SetWinningMove(2, 4, 6, _grid[4]);
			}
		}

		internal TicTacToeBoard Clone(bool cloneMoves = false)
		{
			var clonedBoard = new TicTacToeBoard();
			if (cloneMoves)
			{
				foreach (var move in _moves)
				{
					clonedBoard._moves.AddLast(move);
				}
			}
			else if (_moves.Count >= 1)
			{
				clonedBoard._moves.AddLast(_moves.Last.Value);
			}
			clonedBoard.WinningMove = WinningMove;
			clonedBoard.State = State;
			_grid.CopyTo(clonedBoard._grid, 0);
			return clonedBoard;
		}

		private void SetWinningMove(byte m1, byte m2, byte m3, Player player)
		{
			State = player == Player.X ? GameState.Xwin : GameState.Owin;
			WinningMove = new TicTacToeMove[3];
			WinningMove[0] = new TicTacToeMove(player, m1);
			WinningMove[1] = new TicTacToeMove(player, m2);
			WinningMove[2] = new TicTacToeMove(player, m3);
		}

		public bool MoveTaken(byte move)
		{
			return _grid[move] != Player.Empty;
		}

		public Player CellValue(byte cell)
		{
			return _grid[cell];
		}
	}

	public class TicTacToeMove
	{
		public TicTacToeMove(Player player, byte cell)
		{
			Player = player;
			Cell = cell;
		}

		public Player Player { get; set; }
		public byte Cell { get; set; }
	}

	public enum Player
	{
		Empty,
		X,
		O
	}

	public enum GameState
	{
		Open,
		Draw,
		Xwin,
		Owin
	}
}