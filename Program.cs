namespace Mattodev.MSE;

class Program {
	public static int Main(string[] args) {
		Console.WriteLine("MSE");
		Engine.DebugMode = false;
		Engine.LessVerboseDebugMode = false;
		Engine engine = new(null, false);
		while (!engine.gameOver) {
			(Move move, double eval) = engine.Ponder(4)!.Value;
			//Console.WriteLine($"Top move: {move} (eval={eval})");
			engine.board.ApplyMove(move);
			Console.WriteLine(engine.board.ToVisualisation());
			Console.Write("Your move?");
			// FIXME: relies entirely on trust of the human playing to not take the king or anything
			string userMove = Console.ReadLine() ?? "";
			engine.board.ApplyMove(Move.FromSquares(userMove));
		}

		return 0;
	}
}