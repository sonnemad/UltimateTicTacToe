using System;
using System.IO;
using Newtonsoft.Json;

namespace UltimateTicTacToe
{
	internal class Program
	{
		private static TimeSpan _playtime = new TimeSpan(0, 0, 0);
		private static TimeSpan _totalPlaytime = new TimeSpan(0, 0, 0);
		private static int _xWins = 0;
		private static int _oWins = 0;
		private static int _draws = 0;

		private static void Main()
		{
			bool playAgain = true;
			while (playAgain)
			{
				PlayGame();

				Console.WriteLine("Would you like to play again (y/n) ?");
				var response = Console.ReadLine();
				playAgain = response?.StartsWith("y", StringComparison.InvariantCultureIgnoreCase) == true;
			}
		}

		private static void PlayGame() { 
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

			var startTime = DateTime.UtcNow;

			string[] player1LastTaunt = {""};
			Taunt p1Taunt = delegate(string msg)
			{
				if (player1LastTaunt[0].Equals(msg)) return;
				player1LastTaunt[0] = msg;
	            Console.WriteLine(msg);
            };
            player1.SetTauntDelegate(p1Taunt);
			string[] player2LastTaunt = {""};
			Taunt p2Taunt = delegate (string msg)
			{
				if (player2LastTaunt[0].Equals(msg)) return;
				player2LastTaunt[0] = msg;
				Console.WriteLine(msg);
			};
			player2.SetTauntDelegate(p2Taunt);

			var playerTurn = Player.X;
			while (board.State == GameState.Open)
			{
				player1LastTaunt[0] = "";
				player2LastTaunt[0] = "";
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
						Console.WriteLine(GetCurrentGameStats(DateTime.UtcNow.Subtract(startTime)));
						Console.WriteLine();
						Console.WriteLine(board.ToString());
					}
				}
				playerTurn = playerTurn == Player.X ? Player.O : Player.X;
			}

			var gameTime = DateTime.UtcNow.Subtract(startTime);
			_playtime = _playtime.Add(gameTime);
			_totalPlaytime = _totalPlaytime.Add(gameTime);

			string stateStr = null;
			switch (board.State)
			{
				case GameState.Xwin:
					_xWins++;
					stateStr = "Player X Wins!";
					break;
				case GameState.Owin:
					_oWins++;
					stateStr = "Player O Wins!";
					break;
				case GameState.Open:
					break;
				case GameState.Draw:
					_draws++;
					stateStr = "DRAW!";
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			Console.Clear();
			Console.WriteLine(GetGameStats());
			Console.WriteLine();
			Console.WriteLine(board.ToString());
			Console.WriteLine("Game has Ended: " + stateStr);

			WriteMoves(board);
		}

		private static void WriteMoves(UltimateTicTacToeBoard board)
		{
			//open file stream
			string fileName = DateTime.UtcNow.ToString("yyyymmddhhmm") + ".json";
			using (StreamWriter file = File.CreateText($"./{fileName}"))
			{
				JsonSerializer serializer = new JsonSerializer();
				//serialize object directly into file stream
				serializer.Serialize(file, board.Moves);
			}
		}

		private static string GetCurrentGameStats(TimeSpan time)
		{
			return $"Playtime {time.ToString(@"mm\:ss")}  Total Playtime {_totalPlaytime.Add(time).ToString(@"hh\:mm\:ss")}   Records: X:{_xWins} O:{_oWins} Draw:{_draws}";
		}

		private static string GetGameStats()
		{
			return $"Total Playtime {_totalPlaytime.ToString(@"hh\:mm\:ss")}  Last Game {_playtime.ToString(@"mm\:ss")}   Records: X:{_xWins} O:{_oWins} Draw:{_draws}";
		}
	}
}