namespace Mattodev.MSE;

public class Board {
	public Piece[] pieces;

	public Board() {
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
		string vis =
			"  \n" +
			" +--------+";
		for (int y = 0; y < 8; y++) {
			vis += $"{y}|";
			for (int x = 0; x < 8; x++)
				vis += pieces[y*8+x] == Piece.None ? ' ' : PieceToFenLetter(pieces[y*8+x]);
			vis += "|\n";
		}
		vis += " +--------+";
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
	public Piece? promotesTo;
	public Move(int x1, int y1, int x2, int y2, Piece? promotesTo = null) {
		//Console.WriteLine($"Move..ctor: ({x1} {y1} {x2} {y2})");
		if (!SquareWithinBoardBounds(x1, y1) || !SquareWithinBoardBounds(x2, y2))
			throw new ArgumentException("Square out of bounds");
		from = (x1, y1);
		to = (x2, y2);
		this.promotesTo = promotesTo;
	}
	public static Move FromSquares(string square1, string square2, Piece? promotesTo = null) {
		if (square1.Length != 2 || square2.Length != 2)
			throw new ArgumentException("Invalid square string size (must be 2, for column and row respectively)");
		return new(
			ColLetterToX(square1[0]), RowLetterToY(square1[1]),
			ColLetterToX(square2[0]), RowLetterToY(square2[1]),
			promotesTo
		);
	}
	public static Move FromSquares(string squares, Piece? promotesTo = null) {
		if (squares.Length != 4)
			throw new ArgumentException("Invalid squares string size (must be 4, for 2 columns and rows for each respective square)");
		return FromSquares(squares[0..2], squares[2..4], promotesTo);
	}

	public static bool SquareWithinBoardBounds(int x, int y) => 0 <= x && x <= 7 && 0 <= y && y <= 7;
	public static int ColLetterToX(char col)
		=> col >= 'a' && col <= 'h' ? col - 'a' : throw new ArgumentException("Invalid column letter (must be a-h)");
	public static char ColLetterFromX(int x)
		=> x >= 0 && x <= 7 ? (char)(x + 'a') : throw new ArgumentException("Invalid X (must be 0 <= x <= 7)");
	public static int RowLetterToY(char row)
		=> row >= '0' && row <= '7' ? 7-(row - '1') : throw new ArgumentException("Invalid row letter (must be 7-0)");
	public static char RowLetterFromY(int y)
		=> y >= 0 && y <= 7 ? (char)(y + '1') : throw new ArgumentException("Invalid Y (must be 0 <= y <= 7)");

	public override string ToString()
		=> $"{ColLetterFromX(from.x)}{RowLetterFromY(from.y)}{ColLetterFromX(to.x)}{RowLetterFromY(to.y)}";
}