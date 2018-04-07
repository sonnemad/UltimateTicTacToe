using System;
using System.Collections.Generic;
using System.IO;

namespace UltimateTicTacToe
{
	public class UltimateTicTacToeBoard
	{
		private readonly LinkedList<UltimateTicTacToeMove> _moves = new LinkedList<UltimateTicTacToeMove>();
		private readonly TicTacToeBoard[] _grid = new TicTacToeBoard[9];
		public byte? RequiredOuterCell
		{
			get
			{
				if (LastMove == null)
				{
					return null;
				}
				byte cell = LastMove.InnerCell;
				if (_grid[cell].State != GameState.Open)
				{
					return null;
				}
				return cell;
			}
		}

		public List<byte> GetValidOuterCells()
		{
			List<byte> cells = new List<byte>();
			if (RequiredOuterCell != null)
			{
				cells.Add((byte) RequiredOuterCell);
			}
			else
			{
				for (byte i = 0; i < 9; i++)
				{
					if (_grid[i].State == GameState.Open)
					{
						cells.Add(i);
					}
				}
			}
			return cells;
		}

		public List<byte> GetValidInnerCells(byte outerCell)
		{
				List<byte> cells = new List<byte>();
				for (byte i = 0; i < 9; i++)
				{
					if (_grid[outerCell].CellValue(i) == Player.Empty)
					{
						cells.Add(i);
					}
				}
				return cells;
		}

		public UltimateTicTacToeMove LastMove => _moves.Last?.Value;

		public UltimateTicTacToeMove[] Moves
		{
			get
			{
				UltimateTicTacToeMove[] moves = new UltimateTicTacToeMove[_moves.Count];
				_moves.CopyTo(moves,0);
				return moves;
			}
		}

		public TicTacToeMove[] WinningMove { get; private set; }

		public UltimateTicTacToeBoard Clone(bool cloneMoves = false)
		{
			var clonedBoard = new UltimateTicTacToeBoard();
			if (cloneMoves)
			{
				foreach (var move in _moves)
				{
					clonedBoard._moves.AddLast(move);
				}
			}
			else if(_moves.Count >= 1)
			{
				clonedBoard._moves.AddLast(_moves.Last.Value);
			}
			clonedBoard.WinningMove = WinningMove;
			clonedBoard.State = State;
			for (int i = 0; i < 9; i++)
			{
				clonedBoard._grid[i] = _grid[i].Clone();
			}
			return clonedBoard;
		}

		public TicTacToeBoard[] GameBoard => (TicTacToeBoard[])_grid.Clone();

		public GameState State { get; private set; }

		public Player CurrentPlayer => LastMove?.Player == Player.O ? Player.X : Player.O;

		public UltimateTicTacToeBoard()
		{
			for (int i = 0; i < 9; i++)
			{
				_grid[i] = new TicTacToeBoard();
			}
			State = GameState.Open;
			WinningMove = null;
		}

		public bool MakeMove(Player player, byte outerCell, byte innerCell)
		{
			if (outerCell > 8)
			{
				//				throw new ArgumentOutOfRangeException(nameof(outerCell), outerCell, "value must be 0-8;");
				Console.WriteLine("Outercell value out of range");
				return false;
			}
			if (innerCell > 8)
			{
				//				throw new ArgumentOutOfRangeException(nameof(innerCell), innerCell, "value must be 0-8;");
				Console.WriteLine("Innercell value out of range");
				return false;
			}
			if (RequiredOuterCell != null && RequiredOuterCell != outerCell)
			{
				return false;
			}
			if (MoveTaken(outerCell, innerCell))
			{
				return false;
			}
			var move = new UltimateTicTacToeMove(player, outerCell, innerCell);
			_moves.AddLast(move);
			_grid[outerCell].MakeMove(player, innerCell);
			CalculateGameState();
			return true;
		}

		private void CalculateGameState()
		{
			if (State != GameState.Open)
			{
				return;
			}

			//Check Horizontal
			if (_grid[0].State != GameState.Open && _grid[0].State != GameState.Draw && _grid[0].State.Equals(_grid[1].State) && _grid[0].State.Equals(_grid[2].State))
			{
				SetWinningMove(0, 1, 2, _grid[0].State);
			}
			else if (_grid[3].State != GameState.Open && _grid[3].State != GameState.Draw && _grid[3].State.Equals(_grid[4].State) && _grid[3].State.Equals(_grid[5].State))
			{
				SetWinningMove(3, 4, 5, _grid[3].State);
			}
			else if (_grid[6].State != GameState.Open && _grid[6].State != GameState.Draw && _grid[6].State.Equals(_grid[7].State) && _grid[6].State.Equals(_grid[8].State))
			{
				SetWinningMove(6, 7, 8, _grid[6].State);
			}

			//Check Vertical
			else if (_grid[0].State != GameState.Open && _grid[0].State != GameState.Draw && _grid[0].State.Equals(_grid[3].State) && _grid[0].State.Equals(_grid[6].State))
			{
				SetWinningMove(0, 3, 6, _grid[0].State);
			}
			else if (_grid[1].State != GameState.Open && _grid[1].State != GameState.Draw && _grid[1].State.Equals(_grid[4].State) && _grid[1].State.Equals(_grid[7].State))
			{
				SetWinningMove(1, 4, 7, _grid[1].State);
			}
			else if (_grid[2].State != GameState.Open && _grid[2].State != GameState.Draw && _grid[2].State.Equals(_grid[5].State) && _grid[2].State.Equals(_grid[8].State))
			{
				SetWinningMove(2, 5, 8, _grid[2].State);
			}

			//Check Diagonal
			else if (_grid[4].State != GameState.Open && _grid[4].State != GameState.Draw && _grid[4].State.Equals(_grid[0].State) && _grid[4].State.Equals(_grid[8].State))
			{
				SetWinningMove(0, 4, 8, _grid[4].State);
			}
			else if (_grid[4].State != GameState.Open && _grid[4].State != GameState.Draw && _grid[4].State.Equals(_grid[2].State) && _grid[4].State.Equals(_grid[6].State))
			{
				SetWinningMove(2, 4, 6, _grid[4].State);
			}

			if (State == GameState.Open)
			{
				State = GameState.Draw;
				foreach (var smallGame in _grid)
				{
					if (smallGame.State != GameState.Open) continue;
					State = GameState.Open;
					break;
				}
			}
		}

		private void SetWinningMove(byte m1, byte m2, byte m3, GameState state)
		{
			State = state;
			Player player = state == GameState.Xwin ? Player.X : Player.O;
			WinningMove = new TicTacToeMove[3];
			WinningMove[0] = new TicTacToeMove(player, m1);
			WinningMove[1] = new TicTacToeMove(player, m2);
			WinningMove[2] = new TicTacToeMove(player, m3);
		}

		public bool MoveTaken(byte outerCell, byte innerCell)
		{
			return _grid[outerCell].MoveTaken(innerCell);
		}

		public Player CellValue(byte outerCell, byte innerCell)
		{
			return _grid[outerCell].CellValue(innerCell);
		}

		public override string ToString()
		{
			StringWriter stringWriter = new StringWriter();

			stringWriter.Write(ToString(0));
			stringWriter.WriteLine("===========##===========##===========");
			stringWriter.Write(ToString(1));
			stringWriter.WriteLine("===========##===========##===========");
			stringWriter.Write(ToString(2));

			return stringWriter.ToString();
		}

		private string ToString(byte outerRow)
		{
			StringWriter stringWriter = new StringWriter();
			stringWriter.WriteLine("   |   |   ||   |   |   ||   |   |   ");
			stringWriter.WriteLine(ToString(outerRow, 0));
			stringWriter.WriteLine("   |   |   ||   |   |   ||   |   |   ");
			stringWriter.WriteLine("---+---+---++---+---+---++---+---+---");
			stringWriter.WriteLine("   |   |   ||   |   |   ||   |   |   ");
			stringWriter.WriteLine(ToString(outerRow, 1));
			stringWriter.WriteLine("   |   |   ||   |   |   ||   |   |   ");
			stringWriter.WriteLine("---+---+---++---+---+---++---+---+---");
			stringWriter.WriteLine("   |   |   ||   |   |   ||   |   |   ");
			stringWriter.WriteLine(ToString(outerRow, 2));
			stringWriter.WriteLine("   |   |   ||   |   |   ||   |   |   ");
			return stringWriter.ToString();
		}

		private string ToString(byte outerRow, byte innerRow)
		{
			byte outerCell = (byte) (outerRow*3);
			byte innerCell = (byte) (innerRow*3);

			string c0 = CellValue(outerCell, innerCell) == Player.Empty ? " " : CellValue(outerCell, innerCell).ToString();
			string c1 = CellValue(outerCell, (byte)(innerCell + 1)) == Player.Empty ? " " : CellValue(outerCell, (byte)(innerCell + 1)).ToString();
			string c2 = CellValue(outerCell, (byte) (innerCell + 2)) == Player.Empty ? " " : CellValue(outerCell, (byte)(innerCell + 2)).ToString();
			string c3 = CellValue((byte)(outerCell + 1), innerCell) == Player.Empty ? " " : CellValue((byte)(outerCell + 1), innerCell).ToString();
			string c4 = CellValue((byte)(outerCell + 1), (byte)(innerCell + 1)) == Player.Empty ? " " : CellValue((byte)(outerCell + 1), (byte)(innerCell + 1)).ToString();
			string c5 = CellValue((byte)(outerCell + 1), (byte)(innerCell + 2)) == Player.Empty ? " " : CellValue((byte)(outerCell + 1), (byte)(innerCell + 2)).ToString();
			string c6 = CellValue((byte)(outerCell + 2), innerCell) == Player.Empty ? " " : CellValue((byte)(outerCell + 2), innerCell).ToString();
			string c7 = CellValue((byte)(outerCell + 2), (byte)(innerCell + 1)) == Player.Empty ? " " : CellValue((byte)(outerCell + 2), (byte)(innerCell + 1)).ToString();
			string c8 = CellValue((byte)(outerCell + 2), (byte)(innerCell + 2)) == Player.Empty ? " " : CellValue((byte)(outerCell + 2), (byte)(innerCell + 2)).ToString();
			return
				$" {c0} | {c1} | {c2} || {c3} | {c4} | {c5} || {c6} | {c7} | {c8} ";
		}
	}

	public class UltimateTicTacToeMove
	{
		public byte OuterCell { get; private set; }
		private readonly TicTacToeMove _move;
		public Player Player => _move.Player;
		public byte InnerCell => _move.Cell;

		public UltimateTicTacToeMove(Player player, byte outerCell, byte innerCell)
		{
			OuterCell = outerCell;
			_move = new TicTacToeMove(player, innerCell);
		}
	}
}
