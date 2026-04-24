/* eslint-disable @typescript-eslint/no-explicit-any */
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import axios from "axios";
import { useEffect, useRef, useState } from "react";
import type {
    GameStateResponse,
    Point,
    Square,
} from "../interfaces/game-state.response";

import { RotateCcw, Zap, Trophy, ArrowLeft } from "lucide-react";
import { getGameState, getValidMoves, movePiece } from "@/lib/axios";
import { cn } from "@/utils/cn";

const Checkerboard = () => {
    const queryClient = useQueryClient();

    const [selectedSquare, setSelectedSquare] = useState<Square | null>(null);
    const [highlightedMoves, setHighlightedMoves] = useState<Point[]>([]);
    const victoryAudio = useRef<HTMLAudioElement | null>(null);

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

    const setupDemoMutation = useMutation({
        mutationFn: () =>
            axios.post("http://localhost:5015/api/v1/game/setup-demo"),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ["gameState"] });
            resetSelection();
            console.log("Demo mode active!");
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

    useEffect(() => {
        if (data?.status.winner) {
            victoryAudio.current = new Audio("/windah.mp3");

            victoryAudio.current.play().catch((error) => {
                console.error(
                    "Autoplay diblokir oleh browser, butuh interaksi user dulu:",
                    error,
                );
            });
        }

        return () => {
            if (victoryAudio.current) {
                victoryAudio.current.pause();
                victoryAudio.current = null;
            }
        };
    }, [data?.status.winner]);

    if (isLoading)
        return <div className="text-white p-10">Loading Game...</div>;

    if (isError || !data)
        return <div className="text-red-500 p-10">Error loading API</div>;

    return (
        <div className="relative flex flex-col items-center gap-6 p-6 bg-slate-950 min-h-screen text-slate-100 overflow-hidden">
            <div className="absolute top-[-10%] right-[-10%] w-[30%] h-[30%] bg-amber-600/5 blur-[100px] rounded-full pointer-events-none"></div>
            <div className="absolute bottom-[-10%] left-[-10%] w-[30%] h-[30%] bg-blue-600/5 blur-[100px] rounded-full pointer-events-none"></div>

            <div className="z-10 w-full max-w-2xl flex justify-between items-center mb-2">
                <button className="flex items-center gap-2 text-slate-400 hover:text-white transition-colors text-sm font-medium group">
                    <ArrowLeft
                        size={18}
                        className="group-hover:-translate-x-1 transition-transform"
                    />
                    KEMBALI KE LOBBY
                </button>

                <div className="flex gap-3">
                    <button
                        onClick={() => setupDemoMutation.mutate()}
                        className="flex items-center gap-2 bg-slate-900 hover:bg-slate-800 text-purple-400 border border-purple-500/30 px-4 py-2 rounded-xl text-xs font-bold transition-all"
                    >
                        <Zap size={14} fill="currentColor" />
                        DEMO MODE
                    </button>
                    <button
                        onClick={() => resetMutation.mutate()}
                        className="flex items-center gap-2 bg-slate-900 hover:bg-slate-800 text-amber-500 border border-amber-500/30 px-4 py-2 rounded-xl text-xs font-bold transition-all"
                    >
                        <RotateCcw size={14} />
                        RESET
                    </button>
                </div>
            </div>

            <div className="z-10 animate-in fade-in zoom-in duration-500">
                <div
                    className={cn(
                        "px-10 py-3 rounded-2xl font-black tracking-[0.2em] border-b-4 transition-all shadow-2xl flex items-center gap-4",
                        data.status.currentPlayer === "White"
                            ? "bg-slate-50 text-slate-900 border-slate-300"
                            : "bg-slate-800 text-white border-slate-950",
                    )}
                >
                    <div
                        className={cn(
                            "w-4 h-4 rounded-full animate-pulse",
                            data.status.currentPlayer === "White"
                                ? "bg-amber-500"
                                : "bg-blue-500",
                        )}
                    />
                    {data.status.currentPlayer.toUpperCase()}'S TURN
                </div>
            </div>

            <div className="z-10 p-3 bg-amber-950/50 rounded-[2.5rem] shadow-[0_20px_50px_rgba(0,0,0,0.5)] border-8 border-amber-900/50 backdrop-blur-md animate-in slide-in-from-bottom-10 duration-700">
                <div className="grid grid-cols-8 rounded-xl overflow-hidden border-4 border-amber-950">
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
                                    "relative w-12 h-12 sm:w-20 sm:h-20 flex items-center justify-center cursor-pointer transition-all duration-200",
                                    isDark ? "bg-[#5d4037]" : "bg-[#d7ccc8]", // Warna kayu yang lebih deep/modern
                                    isSelected &&
                                        "bg-amber-500/40 ring-4 ring-inset ring-amber-400 z-10",
                                    !isSelected && "hover:brightness-110",
                                )}
                            >
                                {/* Hint Langkah Legal */}
                                {isDestination && (
                                    <div className="absolute w-5 h-5 bg-emerald-500/60 rounded-full border-2 border-emerald-300 shadow-[0_0_15px_rgba(16,185,129,0.5)] animate-pulse z-20" />
                                )}

                                {/* Render Bidak */}
                                {square.piece && (
                                    <div
                                        className={cn(
                                            "w-9 h-9 sm:w-16 sm:h-16 rounded-full shadow-2xl flex items-center justify-center border-b-[6px] transition-all duration-300",
                                            square.piece.color === "White"
                                                ? "bg-white"
                                                : "bg-linear-to-b from-slate-700 to-slate-900 border-black",
                                            isSelected &&
                                                "scale-110 -translate-y-2 shadow-[0_10px_20px_rgba(0,0,0,0.6)]",
                                        )}
                                    >
                                        {square.piece.role === "King" && (
                                            <span className="text-xl sm:text-4xl drop-shadow-md select-none">
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

            {/* Modal Menang (Sama seperti sebelumnya tapi disesuaikan dikit) */}
            {data.status.winner && (
                <div className="fixed inset-0 z-100 flex items-center justify-center bg-slate-950/90 backdrop-blur-md animate-in fade-in duration-500">
                    <div className="bg-slate-900 p-12 rounded-[3rem] border-2 border-amber-500/50 shadow-[0_0_80px_rgba(245,158,11,0.2)] text-center transform animate-in zoom-in-95 duration-300">
                        <Trophy
                            size={80}
                            className="mx-auto text-amber-500 mb-6 drop-shadow-glow"
                        />
                        <h2 className="text-5xl font-black text-white mb-2 italic tracking-tighter">
                            VICTORY!
                        </h2>
                        <img
                            className="mx-auto h-32 my-4 rounded-xl opacity-80"
                            src="/oke.gif"
                            alt="Winner"
                        />
                        <div className="my-6">
                            <p className="text-slate-400 font-medium uppercase tracking-[0.3em] mb-2">
                                Pemenang
                            </p>
                            <div
                                className={cn(
                                    "text-6xl font-black italic",
                                    data.status.winner === "White"
                                        ? "text-white"
                                        : "text-amber-500",
                                )}
                            >
                                {data.status.winner.toUpperCase()}
                            </div>
                        </div>
                        <button
                            onClick={() => resetMutation.mutate()}
                            className="mt-6 px-12 py-5 bg-amber-600 hover:bg-amber-500 text-white font-black rounded-2xl transition-all hover:scale-105 active:scale-95 shadow-xl shadow-amber-900/40 uppercase tracking-widest"
                        >
                            Main Lagi
                        </button>
                    </div>
                </div>
            )}
        </div>
    );
};

export default Checkerboard;
