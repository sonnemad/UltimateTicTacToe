﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace UltimateTicTacToe
{
	class MinMaxAIPlayer : ITicTacToePlayer
	{
		private Player Player { get; }
		private GameTree _gameTree;

		public MinMaxAIPlayer(Player player)
		{
			Player = player;
		}

		public Player GetPlayer()
		{
			return Player;
		}

		public UltimateTicTacToeMove TakeTurn(UltimateTicTacToeBoard ticTacToeBoard)
		{
			Console.WriteLine("Planning My Move...");

			Debug.WriteLine($"There were {_gameTree?.GetNodeCount() ?? 0} nodes in the tree prior to this move.");
			GameTree currentTree = _gameTree?.GetNode(ticTacToeBoard);
			_gameTree = currentTree ?? new GameTree(data: ticTacToeBoard.Clone());
			Debug.WriteLine($"Tree Triming reduced it to {_gameTree.GetNodeCount()} nodes.");
			Debug.WriteLine($"Memory Used {GC.GetTotalMemory(false)}");
			GC.Collect();
			GC.WaitForPendingFinalizers();
			Debug.WriteLine($"Memory Used {GC.GetTotalMemory(false)}");
			PopulateTree();
			Debug.WriteLine($"Memory Used {GC.GetTotalMemory(false)}");
			Debug.WriteLine($"Tree population increased it to {_gameTree.GetNodeCount()} nodes.");
			var move = MinMax();
			return move;
		}

		private void PopulateTree()
		{
			//Only allow to run for 5 seconds before stopping the population
			DateTime endAfter = DateTime.UtcNow.AddSeconds(5);
			while(DateTime.UtcNow < endAfter)
			{
				AddDepthToGameTree();
			}
		}

		private void AddDepthToGameTree()
		{
			var gameBoardLeafs = _gameTree.GetLeafNodes();
			var nodes = 0;

			foreach (var gameTreeLeaf in gameBoardLeafs)
			{
				//Don't Generate more leafs for completetd games
				if (gameTreeLeaf.Data.State != GameState.Open)
					continue;
				var validOuterCells = gameTreeLeaf.Data.GetValidOuterCells();
				foreach (var outerCell in validOuterCells)
				{
					var validInnerCells = gameTreeLeaf.Data.GetValidInnerCells(outerCell);

					foreach (var innerCell in validInnerCells)
					{
						var gameBoard = gameTreeLeaf.Data.Clone();
						var valid = gameBoard.MakeMove(gameBoard.CurrentPlayer, outerCell, innerCell);
						if (!valid) continue;
						nodes++;
						gameTreeLeaf.AddChild(gameBoard);
					}
				}
			}
			Debug.WriteLine($"{Player} Created {nodes} new nodes within the tree");
		}

		private UltimateTicTacToeMove MinMax()
		{
			var minMax = AlphaBeta(_gameTree, short.MinValue + 10, short.MaxValue - 10, Player == Player.X);
			var lowScore = _gameTree.GetLeafNodes().Min(node => node.Score);
			var highScore = _gameTree.GetLeafNodes().Max(node => node.Score);

			if ((Player == Player.X && highScore >= 5000) || (Player == Player.O && lowScore <= -5000))
			{
				Console.WriteLine("I can see a way for me to WIN!!!");
			}

			var possibleMoves = _gameTree.Children.Where(child => child.Score == minMax);

			// ReSharper disable PossibleMultipleEnumeration
			var index = new Random().Next(0, possibleMoves.Count());
			return possibleMoves.ElementAt(index).Data.LastMove;
			// ReSharper restore PossibleMultipleEnumeration
		}

		private static short AlphaBeta(GameTree node, short alpha, short beta, bool maximizingPlayer)
		{
			if (node.Children.Count == 0)
			{
				return ScoreNode(node);
			}

			if (maximizingPlayer)
			{
				var value = short.MinValue;
				foreach (var child in node.Children)
				{
					value = Math.Max(value, AlphaBeta(child, alpha, beta, false));
					alpha = Math.Max(alpha, value);
					if (beta < alpha)
						break;
				}
				node.Score = value;
				return value;
			}
			else
			{
				var value = short.MaxValue;
				foreach (var child in node.Children)
				{
					value = Math.Min(value, AlphaBeta(child, alpha, beta, true));
					beta = Math.Min(beta, value);
					if (beta < alpha)
						break;
				}
				node.Score = value;
				return value;
			}
		}

		private static short ScoreNode(GameTree leaf)
		{
			short score = 0;
			switch (leaf.Data.State)
			{
				case GameState.Xwin:
					score = 10000;
					break;
				case GameState.Owin:
					score = -10000;
					break;
				case GameState.Draw:
					score = 0;
					break;
				case GameState.Open:

					var xWins = (short) (CountWins(Player.X, leaf.Data) * 10);
					var oWins = (short) (CountWins(Player.O, leaf.Data) * 10);
					score = (short)(xWins - oWins);

					score = leaf.Data.GameBoard.Aggregate(score, (current, board) => (short) (current + CalculateScore(board)));
					break;
			}
			leaf.Score = score;
			return score;
		}

		private static short CountWins(Player player, UltimateTicTacToeBoard board)
		{
			var wins = (short)(CanWin(player, board, new[] { 0, 1, 2 }) ? 1 : 0);
			wins += (short)(CanWin(player, board, new[] { 3, 4, 5 }) ? 1 : 0);
			wins += (short)(CanWin(player, board, new[] { 6, 7, 8 }) ? 1 : 0);
			wins += (short)(CanWin(player, board, new[] { 0, 3, 6 }) ? 1 : 0);
			wins += (short)(CanWin(player, board, new[] { 1, 4, 7 }) ? 1 : 0);
			wins += (short)(CanWin(player, board, new[] { 2, 5, 8 }) ? 1 : 0);
			wins += (short)(CanWin(player, board, new[] { 0, 4, 8 }) ? 1 : 0);
			wins += (short)(CanWin(player, board, new[] { 2, 4, 6 }) ? 1 : 0);
			return wins;
		}

		private static bool CanWin(Player player, UltimateTicTacToeBoard board, IEnumerable<int> cells)
		{
			var winState = player == Player.X ? GameState.Xwin : GameState.Owin;
			return cells.All(cell => board.GameBoard.ElementAt(cell).State == GameState.Open || board.GameBoard.ElementAt(cell).State == winState);
		}

		private static short CalculateScore(TicTacToeBoard board)
		{
			short xWins = 0;
			short oWins = 0;
			if (board.State == GameState.Xwin)
				xWins = 100;
			else if (board.State == GameState.Owin)
				oWins = 100;
			else if (board.State == GameState.Open)
			{
				xWins = CountWins(Player.X, board);
				oWins = CountWins(Player.O, board);
			}
			return (short)(xWins - oWins);
		}

		private static short CountWins(Player player, TicTacToeBoard board)
		{
			var wins = (short)(CanWin(player, board, new[] { 0, 1, 2 }) ? 1 : 0);
			wins += (short)(CanWin(player, board, new[] { 3, 4, 5 }) ? 1 : 0);
			wins += (short)(CanWin(player, board, new[] { 6, 7, 8 }) ? 1 : 0);
			wins += (short)(CanWin(player, board, new[] { 0, 3, 6 }) ? 1 : 0);
			wins += (short)(CanWin(player, board, new[] { 1, 4, 7 }) ? 1 : 0);
			wins += (short)(CanWin(player, board, new[] { 2, 5, 8 }) ? 1 : 0);
			wins += (short)(CanWin(player, board, new[] { 0, 4, 8 }) ? 1 : 0);
			wins += (short)(CanWin(player, board, new[] { 2, 4, 6 }) ? 1 : 0);
			return wins;
		}

		private static bool CanWin(Player player, TicTacToeBoard board, IEnumerable<int> cells)
		{
			return cells.All(cell => board.CellValue((byte) cell) == Player.Empty || board.CellValue((byte) cell) == player);
		}
	}

	internal class GameTree
	{
		public List<GameTree> Children { get; }
		public UltimateTicTacToeBoard Data { get; }
		public short Score { get; set; }

		public GameTree(UltimateTicTacToeBoard data)
		{
			Data = data;
			Children = new List<GameTree>();
		}

		public void AddChild(UltimateTicTacToeBoard data)
		{
			Children.Add(new GameTree(data));
		}

		public GameTree GetNode(UltimateTicTacToeBoard data)
		{
			if (Data.Equals(data))
				return this;
			if (Children == null || Children.Count < 1) return null;
			var nodes = new List<GameTree>();
			nodes.AddRange(Children);

			for (var i = 0; i < nodes.Count; i++)
			{
				if (nodes[i].Data.ToString().Equals(data.ToString()))
					return nodes[i];
				if (nodes[i].Children != null && nodes[i].Children.Count >= 1)
					nodes.AddRange(nodes[i].Children);
			}
			return null;
		}

		internal int GetNodeCount()
		{
			var nodeCount = 1;
			if (Children == null || Children.Count == 0)
			{
				return nodeCount;
			}
			foreach (var child in Children)
			{
				nodeCount += child.GetNodeCount();
			}
			return nodeCount;
		}

		internal List<GameTree> GetLeafNodes()
		{
			var leafNodes = new List<GameTree>();
			if (Children == null || Children.Count == 0)
			{
				leafNodes.Add(this);
			}
			else
			{
				leafNodes.AddRange(GetLeafNodes(Children));
			}
			return leafNodes;
		}

		private List<GameTree> GetLeafNodes(List<GameTree> children)
		{
			var leafNodes = new List<GameTree>();
			foreach (var childNode in children)
			{
				if (childNode.Children == null || childNode.Children.Count == 0)
				{
					leafNodes.Add(childNode);
				}
				else
				{
					leafNodes.AddRange(GetLeafNodes(childNode.Children));
				}
			}
			return leafNodes;
		}
	}
}