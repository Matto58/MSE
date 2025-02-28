using System.Diagnostics;

namespace Mattodev.MSE;

public class Engine {
	public static bool DebugMode = false;
    public static bool LessVerboseDebugMode = false;
	public bool playsAsBlack;
	public bool gameOver = false;
	public Board board;


    public Engine(Board? board, bool playsAsBlack) {
		if (board != null) this.board = board;
		else {
			this.board = new();
			this.board.AddDefaultPieces();
		}
		this.playsAsBlack = playsAsBlack;
	}

	public (Move move, double eval)? Ponder(int depth = 1) {
		List<Move> moves = GenerateLegalMoves();
		if (moves.Count == 0) {
			gameOver = true;
			return null;
		}
		List<(Move move, double eval)> evaledMoves = [];
		Board newBoard = new();
		Engine newEng;
		foreach (Move move in moves) {
			for (int i = 0; i < 64; i++) newBoard.pieces[i] = board.pieces[i];
			newEng = new(newBoard, !playsAsBlack);
			newBoard.ApplyMove(move);
			if (DebugMode) Console.WriteLine($"Ponder\tMove #{evaledMoves.Count+1}\t{move}...");
			evaledMoves.Add((move, EngineEval.TotalEvaluate(newBoard)));
			if (DebugMode) Console.WriteLine($"Ponder\tMove #{evaledMoves.Count}\t..evals at {evaledMoves[^1].eval}");

			if (depth > 1) {
				if (DebugMode) Console.WriteLine($"Ponder\tGoing deeper... (current depth={depth})");
				var newEvaled = newEng.Ponder(depth-1);
				if (newEvaled == null) continue;
				evaledMoves[^1] = (evaledMoves[^1].move, (evaledMoves[^1].eval + newEvaled.Value.eval)/2);
			}
		}
		evaledMoves.Sort((a, b) => (int)(b.eval*1000 - a.eval*1000));
		if (DebugMode || LessVerboseDebugMode) {
			Console.WriteLine($"Ponder\tTop: {evaledMoves[0].move}\tWorst: {evaledMoves[^1].move}\tDepth: {depth}");
			//Console.WriteLine($"Ponder\tPrevious position:\n{newBoard.ToVisualisation()}");
		}
		return evaledMoves[0];
	}

	public List<Move> GenerateLegalMoves() {
		List<Move> moves = [];
		if (DebugMode) Console.WriteLine($"GenerateLegalMoves\tEngine plays as black? {playsAsBlack}");
		for (int y = 0; y < 8; y++) {
			for (int x = 0; x < 8; x++) {
				bool black = (board.pieces[y*8+x] & Piece.Black) == Piece.Black;
				if (black != playsAsBlack) continue;
				Piece p = board.pieces[y*8+x] & (Piece)0b111;
				if (p == Piece.None) continue;

				switch (p) {
					case Piece.Pawn: {
						if (y == (black ? 0 : 7)) continue;
						int y2 = y + (black ? 1 : -1);
						if (y == (black ? 1 : 6)) {
							moves.AddRange([
								new(x, y, x, y2, Piece.Bishop),
								new(x, y, x, y2, Piece.Knight),
								new(x, y, x, y2, Piece.Rook),
								new(x, y, x, y2, Piece.Queen),
							]);
						}
						else moves.Add(new(x, y, x, y2));
						if (y == (black ? 6 : 1)) moves.Add(new(x, y, x, y2*2));
						break;
					}
					case Piece.Rook: {
						addRookMoves(x, y, ref moves);
						break;
					}
					case Piece.Bishop: {
						addBishopMoves(x, y, ref moves);
						break;
					}
					case Piece.Knight: {
						addKnightMoves(x, y, ref moves);
						break;
					}
					case Piece.Queen: {
						addRookMoves(x, y, ref moves);
						addBishopMoves(x, y, ref moves);
						break;
					}
				}
			}
		}
		return moves;
	}
	private bool validateRBMove(int x, int y, int x2, int y2, ref List<Move> moves) {			
		bool black = (board.pieces[y*8+x2] & Piece.Black) == Piece.Black;
		Piece p = board.pieces[y*8+x2] & (Piece)0b111;
		if (p != Piece.None) {
			if (black != playsAsBlack) moves.Add(new(x, y, x2, y2));
			return false;
		}
		moves.Add(new(x, y, x2, y2));
		return true;
	}

	// TODO: please simplify, this section is filled with copy-paste and i'd be surprised if this actually worked
	private void addRookMoves(int x, int y, ref List<Move> moves) {
		for (int x2 = x+1; x2 < 8 && validateRBMove(x, y, x2, y, ref moves); x2++);
		for (int x2 = x-1; x2 >= 0 && validateRBMove(x, y, x2, y, ref moves); x2--);
		for (int y2 = y+1; y2 < 8 && validateRBMove(x, y, x, y2, ref moves); y2++);
		for (int y2 = y-1; y2 >= 0 && validateRBMove(x, y, x, y2, ref moves); y2--);
	}
	private void addBishopMoves(int x, int y, ref List<Move> moves) {
		for (int i = 1; x+i < 8 && y+i < 8 && validateRBMove(x, y, x+i, y+i, ref moves); i++);
		for (int i = 1; x-i >= 0 && y-i >= 0 && validateRBMove(x, y, x-i, y-i, ref moves); i++);
		for (int i = 1; x+i < 8 && y-i >= 0 && validateRBMove(x, y, x+i, y-i, ref moves); i++);
		for (int i = 1; x+i >= 0 && y+i >= 0 && validateRBMove(x, y, x-i, y+i, ref moves); i--);
	}
	private void addKnightMoves(int x, int y, ref List<Move> moves) {
		addMoveIfWithinRange(x, y, x+1, y+2, ref moves);
		addMoveIfWithinRange(x, y, x-1, y+2, ref moves);
		addMoveIfWithinRange(x, y, x+1, y-2, ref moves);
		addMoveIfWithinRange(x, y, x-1, y-2, ref moves);
		addMoveIfWithinRange(x, y, x+2, y+1, ref moves);
		addMoveIfWithinRange(x, y, x-2, y+1, ref moves);
		addMoveIfWithinRange(x, y, x+2, y-1, ref moves);
		addMoveIfWithinRange(x, y, x-2, y-1, ref moves);
	}
	private void addMoveIfWithinRange(int x, int y, int x2, int y2, ref List<Move> moves) {
		if (Move.SquareWithinBoardBounds(x2, y2) && board.pieces[y*8+x] == Piece.None) moves.Add(new(x, y, x2, y2));
	}
}