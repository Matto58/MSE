namespace Mattodev.MSE;

class Program {
	public static int Main(string[] args) {
		Console.WriteLine("MSE");
		GameManager game = new();
		game.AddDefaultPieces();
		Console.WriteLine(game.ToFen());
		game.ApplyMove(new(4, 6, 4, 4));
		Console.WriteLine(game.ToFen());

		return 0;
	}
}