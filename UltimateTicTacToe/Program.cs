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

			Console.SetWindowSize(80, 48);
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
					Console.WriteLine("No Pondo, try again!");
				}
			}

            Taunt myTaunt = delegate(string msg) { Console.WriteLine(msg); };
            player1.SetTauntDelegate(myTaunt);
            player2.SetTauntDelegate(myTaunt);

			var playerTurn = Player.X;
			while (board.State == GameState.Open)
			{
				var validMove = false;
				while (!validMove)
				{
					var move = playerTurn == Player.X ? player1.TakeTurn(board.Clone(cloneMoves: true)) : player2.TakeTurn(board.Clone(cloneMoves: true));

					validMove = board.MakeMove(playerTurn, move.OuterCell, move.InnerCell);
					if (!validMove)
					{
						Console.WriteLine("Invalid selection, try again");
					}
					else
					{
						Console.Clear();
						Console.WriteLine(board.ToString());
					}
				}
				playerTurn = playerTurn == Player.X ? Player.O : Player.X;
			}

			Console.Clear();
			Console.WriteLine(board.ToString());
            var stateStr = board.State == GameState.Draw ? "DRAW!" : board.State == GameState.Xwin ? "Player X Wins!" : "Player O Wins!";
			Console.WriteLine("Game has Ended: " + stateStr);
			Console.Write("Press any key to continue:");
			Console.ReadKey();
		}
	}
}