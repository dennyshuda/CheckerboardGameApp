/* eslint-disable @typescript-eslint/no-explicit-any */
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import axios from "axios";
import { useCallback, useEffect, useState } from "react";

import { getGameState, getValidMoves, movePiece } from "@/lib/axios";
import { cn } from "@/utils/cn";
import { ArrowLeft, RotateCcw, Zap } from "lucide-react";
import { useNavigate } from "react-router";
import { Link } from "react-router";
import type {
    GameStateResponse,
    Point,
    Square,
} from "@/interfaces/game-state.response";

const API_BASE = import.meta.env.VITE_API_URL || "http://localhost:5015";

interface ApiErrorResponse {
    message: string;
}

type HighlightedMove = {
    from: Point;
    valid: {
        to: Point;
        enemyCaptured: Point | null;
    }[];
};

export default function GamePage() {
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
            const msg =
                (error.response?.data as ApiErrorResponse)?.message ||
                "Langkah ilegal!";
            alert(msg);
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
        <div className="relative flex flex-col items-center gap-8 p-6 bg-[#0c0a09] min-h-screen text-stone-100 font-serif overflow-hidden">
            <div className="absolute top-[-10%] right-[-10%] w-[40%] h-[40%] bg-amber-900/20 blur-[120px] rounded-full pointer-events-none"></div>
            <div className="absolute bottom-[-10%] left-[-10%] w-[40%] h-[40%] bg-orange-950/10 blur-[120px] rounded-full pointer-events-none"></div>

            <div className="z-10 w-full max-w-3xl flex justify-between items-center">
                <Link
                    to="/"
                    className="flex items-center gap-2 text-slate-400 hover:text-slate-900 transition-colors text-xs font-bold uppercase tracking-widest group"
                >
                    <ArrowLeft
                        size={16}
                        className="group-hover:-translate-x-1 transition-transform"
                    />
                    Back to Lobby
                </Link>

                <div className="flex gap-3">
                    <button
                        onClick={() => setupDemoMutation.mutate()}
                        className="flex items-center gap-2 bg-stone-900 hover:bg-stone-800 text-stone-300 border border-stone-800 px-4 py-2 rounded-xl text-xs font-bold transition-all shadow-sm active:scale-95"
                    >
                        <Zap size={14} className="text-slate-400" />
                        DEMO MODE
                    </button>
                    <button
                        onClick={() => resetMutation.mutate()}
                        className="flex items-center gap-2 bg-stone-900 hover:bg-stone-800 text-stone-300 border border-stone-800 px-4 py-2 rounded-xl text-xs font-bold transition-all shadow-sm active:scale-95"
                    >
                        <RotateCcw size={14} className="text-slate-400" />
                        RESET
                    </button>
                </div>
            </div>

            <div className="z-10 flex flex-col items-center gap-2">
                <span className="text-[10px] font-bold uppercase tracking-[0.3em] text-slate-400">
                    Current Turn
                </span>
                <div
                    className={cn(
                        "px-8 py-3 rounded-2xl font-black tracking-widest transition-all shadow-xl flex items-center gap-4 border",
                        data.status.currentPlayer === "White"
                            ? "bg-stone-100 text-stone-900 border-amber-200"
                            : "bg-[#2b1b19] text-stone-100 border-[#3e2723]",
                    )}
                >
                    <div
                        className={cn(
                            "w-4 h-4 rounded-full animate-pulse",
                            data.status.currentPlayer === "White"
                                ? "bg-amber-400 shadow-[0_0_10px_rgba(251,191,36,0.6)]"
                                : "bg-red-800 shadow-[0_0_10px_rgba(153,27,27,0.6)]",
                        )}
                    />
                    {data.status.currentPlayer.toUpperCase()}'S TURN
                </div>
            </div>

            <div className="z-10 flex flex-col items-center">
                <div className="flex px-12 mb-2 w-full justify-around text-[10px] font-bold text-stone-500 tracking-widest">
                    {["A", "B", "C", "D", "E", "F", "G", "H"].map((l) => (
                        <span key={l} className="w-11 sm:w-20 text-center">
                            {l}
                        </span>
                    ))}
                </div>

                <div className="flex items-center">
                    <div className="flex flex-col h-full justify-around mr-4 text-[10px] font-bold text-stone-500">
                        {[8, 7, 6, 5, 4, 3, 2, 1].map((n) => (
                            <span
                                key={n}
                                className="h-11 sm:h-20 flex items-center"
                            >
                                {n}
                            </span>
                        ))}
                    </div>

                    <div className="p-4 bg-[#3e2723] rounded-4xl shadow-[0_40px_80px_-15px_rgba(0,0,0,0.8)] border-8 border-[#2b1b19] animate-in slide-in-from-bottom-8 duration-700">
                        <div className="grid grid-cols-8 rounded-lg overflow-hidden border-2 border-[#1a110f]">
                            {data.board.squares.map((square) => {
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
                                const isEnemyTarget =
                                    highlightedMoves?.valid.some(
                                        (s) =>
                                            s.enemyCaptured?.x ===
                                                square.point.x &&
                                            s.enemyCaptured?.y ===
                                                square.point.y,
                                    );

                                return (
                                    <button
                                        key={`${x}-${y}`}
                                        onClick={() =>
                                            handleSquareClick(square)
                                        }
                                        aria-label={`Square at ${String.fromCharCode(97 + x)}${8 - y}`}
                                        className={cn(
                                            "relative w-11 h-11 sm:w-20 sm:h-20 flex items-center justify-center transition-all duration-200 border-none p-0",
                                            isDark
                                                ? "bg-[#5d4037]"
                                                : "bg-[#d7ccc8]",
                                            isSelected &&
                                                "bg-amber-500/20 ring-4 ring-inset ring-amber-500 z-10",
                                            !isSelected &&
                                                "hover:brightness-110",
                                            isEnemyTarget
                                                ? "ring-4 ring-inset ring-red-500 z-10"
                                                : "",
                                        )}
                                    >
                                        {isEnemyTarget && (
                                            <div className="absolute inset-0 bg-red-500/20 animate-pulse" />
                                        )}

                                        {isDestination && (
                                            <div className="absolute w-4 h-4 sm:w-6 sm:h-6 bg-slate-400/40 rounded-full border-2 border-slate-400 shadow-[0_0_15px_rgba(0,0,0,0.2)] animate-pulse z-20" />
                                        )}

                                        {square.piece && (
                                            <div
                                                className={cn(
                                                    "w-8 h-8 sm:w-16 sm:h-16 rounded-full shadow-[0_12px_20px_rgba(0,0,0,0.5)] flex items-center justify-center transition-all duration-300 border-b-4",
                                                    square.piece.color ===
                                                        "White"
                                                        ? "bg-stone-100 border-stone-300 text-stone-900"
                                                        : "bg-[#1a0f0e] border-black text-stone-200",
                                                    isSelected &&
                                                        "scale-110 -translate-y-2 shadow-2xl",
                                                )}
                                            >
                                                {square.piece.role ===
                                                    "King" && (
                                                    <span className="text-xl sm:text-4xl drop-shadow-md select-none">
                                                        👑
                                                    </span>
                                                )}
                                                <div
                                                    className={cn(
                                                        "absolute inset-1 rounded-full border opacity-20",
                                                        square.piece.color ===
                                                            "White"
                                                            ? "border-stone-900"
                                                            : "border-stone-100",
                                                    )}
                                                />
                                            </div>
                                        )}
                                    </button>
                                );
                            })}
                        </div>
                    </div>

                    <div className="flex flex-col h-full justify-around ml-4 text-[10px] font-bold text-stone-500">
                        {[8, 7, 6, 5, 4, 3, 2, 1].map((n) => (
                            <span
                                key={n}
                                className="h-11 sm:h-20 flex items-center"
                            >
                                {n}
                            </span>
                        ))}
                    </div>
                </div>

                <div className="flex px-12 mt-2 w-full justify-around text-[10px] font-bold text-stone-500 tracking-widest">
                    {["A", "B", "C", "D", "E", "F", "G", "H"].map((l) => (
                        <span key={l} className="w-11 sm:w-20 text-center">
                            {l}
                        </span>
                    ))}
                </div>
            </div>
        </div>
    );
}
