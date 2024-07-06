namespace Mattodev.MSE;

class Program {
	public static int Main(string[] args) {
		Console.WriteLine("MSE");
		Board game = new();
		Move[] movesToApplyForTesting = [
			Move.FromSquares("e2e4"),
			Move.FromSquares("e7e5"),
			Move.FromSquares("b1c3"),
			Move.FromSquares("g8f6"),
			Move.FromSquares("f2f4"),
		];
		game.AddDefaultPieces();
		
		foreach (Move move in movesToApplyForTesting) {
			game.ApplyMove(move);
			Console.WriteLine(game.ToVisualisation());
			Console.WriteLine($"Eval:\t{EngineEval.TotalEvaluate(ref game)}");
			Console.WriteLine("(press any key to continue)");
			Console.ReadKey(true);
		}

		return 0;
	}
}