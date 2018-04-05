using System;

namespace UltimateTicTacToe
{
	internal class Program
	{
		private static void Main()
		{
			var board = new UltimateTicTacToeBoard();

			ITicTacToePlayer player1 = new HumanConsolePlayer(Player.X);
			ITicTacToePlayer player2 = new HumanConsolePlayer(Player.O);

			Console.WriteLine("Ultimate Tic Tac Toe!!!");

			bool correctPlayers = false;
			while (!correctPlayers)
			{
				Console.Write("How Many Players? ");
				int players;
				correctPlayers = int.TryParse(Console.ReadLine(), out players);
				if (correctPlayers)
				{
					if (players == 2)
					{
						player1 = new HumanConsolePlayer(Player.X);
						player2 = new HumanConsolePlayer(Player.O);
					} else if (players == 1)
					{
						player1 = new HumanConsolePlayer(Player.X);
						player2 = new MinMaxAIPlayer(Player.O);
					}
					else if (players == 0)
					{
						player1 = new MinMaxAIPlayer(Player.X);
						player2 = new MinMaxAIPlayer(Player.O);
					}
					else
					{
						correctPlayers = false;
					}
				}
				if (!correctPlayers)
				{
					Console.WriteLine("Wrong Pondo, try again!");
				}
			}

			var playerTurn = Player.X;
			while (board.State == GameState.Open)
			{
				var validMove = false;
				while (!validMove)
				{
					var move = playerTurn == Player.X ? player1.TakeTurn(board) : player2.TakeTurn(board);

					validMove = board.MakeMove(playerTurn, move.OuterCell, move.InnerCell);
					if (!validMove)
					{
						Console.WriteLine("Invalid selection, try again");
					}
					else
					{
						Console.WriteLine($"{board.LastMove.Player} made the move [{board.LastMove.OuterCell}:{board.LastMove.InnerCell}]");
						Console.WriteLine($"{board.State} : {board.GetValidOuterCells().Count}");
					}
				}
				playerTurn = playerTurn == Player.X ? Player.O : Player.X;
			}

			Console.Clear();
			Console.WriteLine(board.ToString());
			Console.WriteLine($"Game has Ended: {board.State}");
			Console.Write("Press any key to continue:");
			Console.ReadLine();
		}
	}
}