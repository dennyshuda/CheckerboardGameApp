import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { useEffect, useState } from "react";
import type {
    GameStateResponse,
    Point,
    Square,
} from "../interfaces/game-state.response";
import { getGameState, getValidMoves, movePiece } from "../lib/axios";
import { cn } from "../utils/cn";
import axios from "axios";

const Checkerboard = () => {
    const queryClient = useQueryClient();

    const [selectedSquare, setSelectedSquare] = useState<Square | null>(null);
    const [highlightedMoves, setHighlightedMoves] = useState<Point[]>([]);

    const { data, isLoading, isError } = useQuery<GameStateResponse>({
        queryKey: ["gameState"],
        queryFn: getGameState,
    });

    const mutation = useMutation({
        mutationFn: movePiece,
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ["gameState"] });
            resetSelection();
        },
        onError: (error: any) => {
            const msg = error.response?.data?.message || "Langkah ilegal!";
            alert(msg);
            resetSelection();
        },
    });

    const resetMutation = useMutation({
        mutationFn: () => axios.post("http://localhost:5015/api/v1/game/reset"),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ["gameState"] });
            resetSelection();
        },
    });

    const resetSelection = () => {
        setSelectedSquare(null);
        setHighlightedMoves([]);
    };

    const handleSquareClick = async (square: Square) => {
        if (!data) return;

        const { x, y } = square.point;

        const isTarget = highlightedMoves.some((m) => m.x === x && m.y === y);
        if (selectedSquare && isTarget) {
            mutation.mutate({
                from: selectedSquare.point,
                to: square.point,
            });
            return;
        }

        if (square.piece && square.piece.color === data.status.currentPlayer) {
            setSelectedSquare(square);
            try {
                const moves = await getValidMoves(x, y);
                setHighlightedMoves(moves);
            } catch {
                setHighlightedMoves([]);
            }
        } else {
            resetSelection();
        }
    };

    console.log("higasj", highlightedMoves);

    // useEffect(() => {
    //     if (mutation.isSuccess) {
    //         const moveSound = new Audio("/win.mp3");
    //         moveSound.play().catch(() => {});
    //     }
    // }, [mutation.isSuccess]);

    if (isLoading)
        return <div className="text-white p-10">Loading Game...</div>;

    if (isError || !data)
        return <div className="text-red-500 p-10">Error loading API</div>;

    return (
        <div className="flex flex-col items-center gap-8 p-10 bg-slate-900 min-h-screen text-slate-100">
            <button
                onClick={() => resetMutation.mutate()}
                className="mt-4 px-10 py-4 bg-amber-600 hover:bg-amber-500 text-white font-bold rounded-xl transition-all hover:scale-105 active:scale-95 shadow-lg shadow-amber-900/40"
            >
                RESET
            </button>

            {data.status.winner && (
                <div className="fixed inset-0 z-50 flex items-center justify-center bg-slate-900/80 backdrop-blur-sm animate-in fade-in duration-500">
                    <div className="bg-slate-800 p-10 rounded-3xl border-2 border-amber-500 shadow-[0_0_50px_rgba(245,158,11,0.3)] text-center transform animate-in zoom-in-95 duration-300">
                        <h2 className="text-4xl font-black text-amber-500 mb-2">
                            PERMAINAN SELESAI
                        </h2>
                        <img
                            className="mx-auto"
                            src="/oke.gif"
                            alt="Loading..."
                        />
                        ;
                        <div className="my-6">
                            <p className="text-xl text-slate-300 mb-1">
                                Pemenangnya adalah:
                            </p>
                            <div
                                className={cn(
                                    "text-5xl font-extrabold uppercase tracking-widest drop-shadow-lg",
                                    data.status.winner === "White"
                                        ? "text-white"
                                        : "text-slate-400",
                                )}
                            >
                                {data.status.winner}
                            </div>
                        </div>
                        <button
                            onClick={() => resetMutation.mutate()}
                            className="mt-4 px-10 py-4 bg-amber-600 hover:bg-amber-500 text-white font-bold rounded-xl transition-all hover:scale-105 active:scale-95 shadow-lg shadow-amber-900/40"
                        >
                            MAIN LAGI
                        </button>
                    </div>
                </div>
            )}
            <div
                className={cn(
                    "px-8 py-3 rounded-full font-black tracking-widest border-2 transition-all",
                    data.status.currentPlayer === "White"
                        ? "bg-white text-slate-900 border-slate-300"
                        : "bg-slate-800 text-white border-slate-700",
                )}
            >
                {data.status.currentPlayer.toUpperCase()}'S TURN
            </div>
            <div className="p-4 bg-amber-950 rounded-lg shadow-[0_0_50px_rgba(0,0,0,0.7)] border-4 border-amber-900">
                <div className="grid grid-cols-8 border-2 border-amber-900">
                    {data.board.squares.map((square) => {
                        const { x, y } = square.point;
                        const isDark = (x + y) % 2 !== 0;
                        const isSelected =
                            selectedSquare?.point.x === x &&
                            selectedSquare?.point.y === y;
                        const isDestination = highlightedMoves.some(
                            (m) => m.x === x && m.y === y,
                        );

                        return (
                            <div
                                key={`${x}-${y}`}
                                onClick={() => handleSquareClick(square)}
                                className={cn(
                                    "relative w-14 h-14 sm:w-20 sm:h-20 flex items-center justify-center cursor-pointer transition-all duration-200",
                                    isDark ? "bg-[#7a5c41]" : "bg-[#f0d9b5]",
                                    isSelected &&
                                        "bg-yellow-500/40 ring-4 ring-inset ring-yellow-400 z-10",
                                    !isSelected && "hover:brightness-110",
                                )}
                            >
                                {isDestination && (
                                    <div className="absolute w-6 h-6 bg-emerald-500/50 rounded-full border-2 border-emerald-400 animate-pulse z-20" />
                                )}

                                {square.piece && (
                                    <div
                                        className={cn(
                                            "w-10 h-10 sm:w-16 sm:h-16 rounded-full shadow-2xl flex items-center justify-center border-b-4 transition-transform",
                                            square.piece.color === "White"
                                                ? "bg-slate-50 border-slate-300"
                                                : "bg-slate-900 border-black",
                                            isSelected &&
                                                "scale-110 -translate-y-2 shadow-black/50",
                                        )}
                                    >
                                        {square.piece.role === "King" && (
                                            <span className="text-xl sm:text-3xl drop-shadow-[0_2px_2px_rgba(0,0,0,0.5)]">
                                                👑
                                            </span>
                                        )}
                                    </div>
                                )}
                            </div>
                        );
                    })}
                </div>
            </div>
        </div>
    );
};

export default Checkerboard;
