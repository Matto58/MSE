namespace Mattodev.MSE;

public class GameManager {
	public Piece[] pieces;

	public GameManager() {
		pieces = new Piece[64];
	}

	public void AddDefaultPieces() {
		for (int y = 0; y < 8; y++) {
			for (int x = 0; x < 8; x++) {
				pieces[y*8+x] = y switch {
					0 => Piece.Black | DefaultPieceFromX(x),
					1 => Piece.Black | Piece.Pawn,
					6 => Piece.Pawn,
					7 => DefaultPieceFromX(x),
					_ => Piece.None
				};
			}
		}
	}
	public string ToFen() {
		string fen = "";
		for (int y = 0; y < 8; y++) {
			int blanks = 0;
			for (int x = 0; x < 8; x++) {
				Piece active = pieces[y*8+x];
				if (active == Piece.None) {
					blanks++;
					continue;
				}
				if (blanks != 0) {
					fen += blanks.ToString();
					blanks = 0;
				}
				fen += PieceToFenLetter(active);
			}
			if (blanks != 0) fen += blanks.ToString();
			fen += "/";
		}
		return fen[..^1];
	}
	public string ToVisualisation() {
		string vis = "";
		for (int y = 0; y < 8; y++) {
			for (int x = 0; x < 8; x++)
				vis += pieces[y*8+x] == Piece.None ? ' ' : PieceToFenLetter(pieces[y*8+x]);
			vis += "\n";
		}
		return vis;
	}
	public bool ApplyMove(Move move) {
		pieces[move.to.y*8+move.to.x] = pieces[move.from.y*8+move.from.x];
		pieces[move.from.y*8+move.from.x] = Piece.None;
		return false;
	}

	public static Piece DefaultPieceFromX(int x)
		=> x switch {
			0 => Piece.Rook,
			1 => Piece.Knight,
			2 => Piece.Bishop,
			3 => Piece.Queen,
			4 => Piece.King,
			5 => Piece.Bishop,
			6 => Piece.Knight,
			7 => Piece.Rook,
			_ => throw new ArgumentException("Invalid X (must be 0 <= x <= 7)")
		};
	public static char PieceToFenLetter(Piece piece) {
		char letter = (piece & (Piece)0b0111) switch {
			Piece.Pawn => 'p',
			Piece.Bishop => 'b',
			Piece.Knight => 'n',
			Piece.Rook => 'r',
			Piece.Queen => 'q',
			Piece.King => 'k',
			_ => throw new ArgumentException("Invalid piece entered (cannot be a blank square)")
		};
		return (piece & Piece.Black) == Piece.Black ? letter : (char)(letter - 0b100000);
	}
}

public enum Piece : ushort {
	None = 0b0000,
	Pawn = 0b0001,
	Bishop = 0b0010,
	Knight = 0b0011,
	Rook = 0b0100,
	Queen = 0b0101,
	King = 0b0110,
	Black = 0b1000
}

public class Move {
	public (int x, int y) from, to;
	public Move(int x1, int y1, int x2, int y2) {
		Console.WriteLine($"Move..ctor: ({x1} {y1} {x2} {y2})");
		if (!SquareWithinBoardBounds(x1, y1) || !SquareWithinBoardBounds(x2, y2))
			throw new ArgumentException("Square out of bounds");
		from = (x1, y1);
		to = (x2, y2);
	}

	public static bool SquareWithinBoardBounds(int x, int y) => 0 <= x && x <= 7 && 0 <= y && y <= 7;
}