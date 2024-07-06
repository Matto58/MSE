namespace Mattodev.MSE;

public class EngineEval {
	public static double TotalEvaluate(Board board) {
		return
			evalCenterPawns(board, false) - evalCenterPawns(board, true)
			+ evalTargetedMat(board, false) - evalTargetedMat(board, true)
			+ evalPieceDev(board, false) - evalPieceDev(board, true)
			+ evalKingSafety(board, false) - evalKingSafety(board, true)
			+ evalCountMaterial(board, false) - evalCountMaterial(board, true);
	}
	
	private static double evalTargetedMat(Board board, bool forBlack) {
		return 0.0;
	}

	private static double evalCenterPawns(Board board, bool forBlack) {
		double eval = 0.0;
		Piece color = forBlack ? Piece.Black : Piece.None;
		for (int y = 0; y < 8; y++)
			for (int x = 0; x <	8; x++)
				if ((board.pieces[y*8+x] & (Piece)0b111) == Piece.Pawn && (board.pieces[y*8+x] & Piece.Black) == color)
					eval += (-dist(x, y, 3.5, 3.5)) + (evalCenterPawnsXWeight(x) * evalCenterPawnsYWeight(y));
		if (Engine.DebugMode) Console.WriteLine($"evalCenterPawns\tforBlack={forBlack}\teval={eval}");
		return eval;
	}
	private static double evalCenterPawnsXWeight(int x)
		=> x switch {
			0 => 0.4,
			1 => 0.1,
			2 => 1.0,
			3 => 1.5,
			4 => 1.5,
			5 => 1.0,
			6 => 0.1,
			7 => 0.4,
			_ => throw new ArgumentException("Invalid X value")
		};
	private static double evalCenterPawnsYWeight(int y)
		=> y switch {
			1 => 0.4,
			2 => 1.5,
			3 => 1.75,
			4 => 1.5,
			5 => 1.75,
			6 => 2.0,
			_ => throw new ArgumentException("Invalid Y value")
		};

	private static double evalPieceDev(Board board, bool forBlack) {
		double eval = 0.0;
		Piece color = forBlack ? Piece.Black : Piece.None;
		for (int y = 0; y < 8; y++)
			for (int x = 0; x <	8; x++)
				if (((board.pieces[y*8+x] & (Piece)0b111) == Piece.Bishop || (board.pieces[y*8+x] & (Piece)0b111) == Piece.Knight)
					&& (board.pieces[y*8+x] & Piece.Black) == color)
					eval += dist(x, y, 3.5, 3.5) * evalCenterPawnsXWeight(x) * evalCenterPawnsXWeight(y);

		if (Engine.DebugMode) Console.WriteLine($"evalPieceDev\tforBlack={forBlack}\teval={eval}");
		return eval;
	}

	private static double evalKingSafety(Board board, bool forBlack) {
		int x = 0, y = 0;
		bool foundKing = false;
		for (int x2 = 0; x2 < 8 && !foundKing; x2++)
			for (int y2 = 0; y2 < 8 && !foundKing; y2++)
				if ((board.pieces[y2*8+x2] & Piece.King) == Piece.King
					&& (board.pieces[y2*8+x2] & Piece.Black) == Piece.Black == forBlack) {
						x = x2; y = y2;
						foundKing = true;
					}
		
		if (!foundKing)
			throw new Exception("Board is missing king(s)");
		double eval = dist(x, y, 3.5, forBlack ? 7.0 : 0.0) * evalKingSafetyXWeight(x);
		if (Engine.DebugMode) Console.WriteLine($"evalKingSafety\tforBlack={forBlack}\teval={eval}");
		return eval;
	}
	private static double evalKingSafetyXWeight(int x)
		=> x switch {
			0 => 1,
			1 => 1.5,
			2 => 0.75,
			3 => 0.5,
			4 => 0.5,
			5 => 0.75,
			6 => 1.5,
			7 => 1,
			_ => throw new ArgumentException("Invalid X value")
		};

	private static double evalCountMaterial(Board board, bool forBlack) {
		double eval = 0.0;
		Piece color = forBlack ? Piece.Black : Piece.None;
		for (int y = 0; y < 8; y++)
			for (int x = 0; x <	8; x++)
				if ((board.pieces[y*8+x] & Piece.Black) == color && board.pieces[y*8+x] != Piece.None)
					eval += evalCountMaterialPieceToMat(board.pieces[y*8+x] & (Piece)0b111);
		if (Engine.DebugMode) Console.WriteLine($"evalCountMaterial\tforBlack={forBlack}\teval={eval}");
		return eval;
	}
	private static double evalCountMaterialPieceToMat(Piece p)
		=> p switch {
			Piece.Pawn => 1,
			Piece.Knight => 3,
			Piece.Bishop => 3.5,
			Piece.Rook => 5,
			Piece.Queen => 9,
			Piece.King => 1000.0,
			_ => throw new ArgumentException("Invalid piece entered")
		};

	private static double dist(double x, double y, double x2, double y2) {
		double x3 = x2-x, y3 = y2-y;
		return Math.Abs(Math.Sqrt(x3*x3 + y3*y3));
	}
}