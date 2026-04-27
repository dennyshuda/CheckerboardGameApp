import { getGameState, getValidMoves, movePiece } from "@/lib/axios";
import { cn } from "@/utils/cn";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import axios from "axios";
import { Piece } from "@/components/piece";
import type {
    GameStateResponse,
    HighlightedMove,
    Square,
} from "@/interfaces/game-state.response";
import { useCallback, useEffect, useState } from "react";
import { Link, useNavigate } from "react-router";

export const API_BASE = import.meta.env.VITE_API_URL || "http://localhost:5015";

export default function GamesPage() {
    const queryClient = useQueryClient();
    const navigate = useNavigate();

    const [selectedSquare, setSelectedSquare] = useState<Square | null>(null);
    const [highlightedMoves, setHighlightedMoves] =
        useState<HighlightedMove | null>();

    const { data, isLoading, isError } = useQuery<GameStateResponse>({
        queryKey: ["gameState"],
        queryFn: getGameState,
    });

    const mutation = useMutation({
        mutationFn: movePiece,
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ["gameState"] });
            resetSelection();
            setHighlightedMoves(null);
        },
        onError: (error: any) => {
            const msg = "Langkah ilegal!";
            alert(error.response?.data?.message || msg);
            resetSelection();
        },
    });

    const resetMutation = useMutation({
        mutationFn: () => axios.post(`${API_BASE}/api/v1/game/reset`),
        onSuccess: () => {
            navigate("/");
            resetSelection();
        },
    });

    const setupDemoMutation = useMutation({
        mutationFn: () => axios.post(`${API_BASE}/api/v1/game/demo`),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ["gameState"] });
            resetSelection();
        },
    });

    const resetSelection = () => {
        setSelectedSquare(null);
        setHighlightedMoves(null);
    };

    const handleSquareClick = useCallback(
        async (square: Square) => {
            if (!data) return;

            const { x, y } = square.point;

            const isTarget = highlightedMoves?.valid.some(
                (m) => m.to.x === x && m.to.y === y,
            );
            if (selectedSquare && isTarget) {
                mutation.mutate({
                    from: selectedSquare.point,
                    to: square.point,
                });
                return;
            }

            if (
                square.piece &&
                square.piece.color === data.status.currentPlayer
            ) {
                setSelectedSquare(square);
                try {
                    const moves = await getValidMoves(x, y);
                    setHighlightedMoves(moves);
                } catch (error) {
                    console.error("Failed to fetch valid moves", error);
                    setHighlightedMoves(null);
                }
            } else {
                resetSelection();
            }
        },
        [data, selectedSquare, highlightedMoves, mutation],
    );

    useEffect(() => {
        if (data?.status.winner) {
            setTimeout(() => {
                navigate("/winner", { state: data });
            }, 500);
        }
    }, [data, navigate]);

    if (isLoading)
        return <div className="text-white p-10">Loading Game...</div>;

    if (isError || !data)
        return <div className="text-red-500 p-10">Error loading API</div>;

    return (
        <div className="min-h-screen bg-slate-50 flex flex-col items-center justify-center p-8">
            <div
                className={cn(
                    "bg-indigo-600 mb-10 text-white font-bold py-4 px-10 rounded-2xl flex items-center gap-2 shadow-button",
                    data.status.currentPlayer === "Black"
                        ? "bg-red-600"
                        : "bg-indigo-600",
                )}
            >
                {data.status.currentPlayer === "Black" ? "Red" : "Blue"}'s Turn
            </div>

            <div className="fixed inset-0 pointer-events-none opacity-20 confetti-gradient" />
            <div className="bg-slate-900 p-4 rounded-[40px] shadown-bold mb-12">
                <div className="grid grid-cols-8 gap-1 bg-slate-200 border-8 border-slate-900 rounded-3xl overflow-hidden">
                    {data?.board.squares.map((square) => {
                        const { x, y } = square.point;
                        const isDark = (x + y) % 2 !== 0;
                        const isSelected =
                            selectedSquare?.point.x === x &&
                            selectedSquare?.point.y === y;
                        const isDestination =
                            highlightedMoves &&
                            highlightedMoves.valid.some(
                                (m) => m.to.x === x && m.to.y === y,
                            );
                        const isEnemyTarget = highlightedMoves?.valid.some(
                            (s) =>
                                s.enemyCaptured?.x === square.point.x &&
                                s.enemyCaptured?.y === square.point.y,
                        );

                        return (
                            <button
                                key={`${x}-${y}`}
                                onClick={() => handleSquareClick(square)}
                                aria-label={`Square at ${String.fromCharCode(97 + x)}${8 - y}`}
                                className={cn(
                                    "relative w-11 h-11 sm:w-20 sm:h-20 flex items-center justify-center transition-all duration-200 border-none p-0",
                                    isDark ? "bg-indigo-50" : "bg-white",
                                    isSelected &&
                                        "bg-amber-500/20 ring-4 ring-inset ring-amber-500 z-10",
                                    !isSelected && "hover:brightness-110",
                                    isEnemyTarget &&
                                        "ring-4 ring-inset ring-red-500 z-10",
                                    isDestination &&
                                        "ring-4 ring-inset ring-green-500 z-10",
                                )}
                            >
                                {isEnemyTarget && (
                                    <div className="absolute inset-0 bg-red-500/20 animate-pulse" />
                                )}

                                {isDestination && (
                                    <div className="absolute inset-0 bg-green-400 animate-pulse" />
                                )}

                                {square.piece && Piece({ square, isSelected })}
                            </button>
                        );
                    })}
                </div>
            </div>

            <div className="flex gap-6">
                <Link
                    to="/"
                    className="bg-slate-900 text-white font-bold py-4 px-10 rounded-2xl flex items-center gap-2 shadow-button hover:bg-slate-800 transition-colors"
                >
                    <span>🏠</span> Back to Home
                </Link>

                <button
                    onClick={() => resetMutation.mutate()}
                    className="bg-indigo-600 text-white font-bold py-4 px-10 rounded-2xl flex items-center gap-2 shadow-button hover:bg-indigo-700 transition-all"
                >
                    <span>↺</span> Reset Game
                </button>
            </div>
        </div>
    );
}
