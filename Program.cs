namespace Mattodev.MSE;

class Program {
	public static int Main(string[] args) {
		Console.WriteLine("MSE");
		GameManager game = new();
		game.AddDefaultPieces();
		Console.WriteLine(game.ToFen());
		game.ApplyMove(Move.FromSquares("e2", "e4"));
		Console.WriteLine(game.ToFen());

		return 0;
	}
}