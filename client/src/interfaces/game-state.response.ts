export type Role = "Troop" | "King";
export type Color = "White" | "Black";

export interface Piece {
    color: Color;
    role: Role;
}

export interface Point {
    x: number;
    y: number;
}

export interface Square {
    point: Point;
    piece: Piece | null;
}

export interface Board {
    rows: number;
    cols: number;
    squares: Square[];
}

export interface Status {
    currentPlayer: string;
    gameStatus: string;
    winner: string | null;
}

export interface GameStateResponse {
    board: Board;
    status: Status;
}

export interface MoveRequest {
    from: Point;
    to: Point;
}

export type HighlightedMove = {
    from: Point;
    valid: {
        to: Point;
        enemyCaptured: Point | null;
    }[];
};
