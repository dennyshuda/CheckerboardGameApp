import { getGameState } from "@/lib/axios";
import { cn } from "@/utils/cn";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import axios from "axios";
import type { GameStateResponse } from "interfaces/game-state.response";
import { RotateCcw, Trophy } from "lucide-react";
import { useLocation, useNavigate } from "react-router";

const API_BASE = import.meta.env.VITE_API_URL || "http://localhost:5015";

export default function WinnerPage() {
    const navigate = useNavigate();
    const queryClient = useQueryClient();
    const location = useLocation();
    const stateData = location.state as GameStateResponse;

    const { data } = useQuery({
        queryKey: ["gameState"],
        queryFn: getGameState,
        enabled: !stateData,
    });

    const resetMutation = useMutation({
        mutationFn: () => axios.post(`${API_BASE}/api/v1/game/reset`),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ["gameState"] });
            navigate("/");
        },
    });

    const displayData = stateData || data;
    const winner = displayData?.status.winner;

    return (
        <div className="min-h-screen bg-[#0c0a09] flex flex-col items-center justify-center p-6 text-stone-100 font-serif relative overflow-hidden">
            <div className="absolute top-[-10%] right-[-10%] w-[40%] h-[40%] bg-amber-900/20 blur-[120px] rounded-full pointer-events-none"></div>
            <div className="absolute bottom-[-10%] left-[-10%] w-[40%] h-[40%] bg-orange-950/10 blur-[120px] rounded-full pointer-events-none"></div>

            <div className="z-10 flex flex-col items-center max-w-lg w-full text-center animate-in fade-in slide-in-from-bottom-8 duration-1000">
                <div className="relative mb-8">
                    <div className="absolute inset-0 bg-amber-500/20 blur-3xl rounded-full scale-150"></div>
                    <div className="relative bg-stone-900 border-4 border-amber-600 p-8 rounded-full shadow-2xl shadow-amber-900/20">
                        <Trophy
                            size={80}
                            className="text-amber-500 drop-shadow-lg"
                        />
                    </div>
                </div>

                <h1 className="text-4xl font-black tracking-widest uppercase text-stone-300 mb-2">
                    Victory!
                </h1>

                <div className="mb-12">
                    <p className="text-stone-500 uppercase text-xs font-bold tracking-[0.4em] mb-4">
                        The Grandmaster of the Match
                    </p>
                    <h2
                        className={cn(
                            "text-6xl font-extrabold tracking-tight drop-shadow-2xl italic",
                            winner === "White"
                                ? "text-stone-100"
                                : "text-amber-600",
                        )}
                    >
                        {winner}
                    </h2>
                </div>

                <div className="bg-stone-900/50 backdrop-blur-md p-8 rounded-3xl border border-stone-800 w-full space-y-4 shadow-2xl">
                    <button
                        onClick={() => resetMutation.mutate()}
                        disabled={resetMutation.isPending}
                        className="w-full flex items-center justify-center gap-3 p-5 bg-amber-700 hover:bg-amber-600 text-stone-100 transition-all rounded-xl font-bold text-lg shadow-xl border-b-4 border-amber-900 active:border-b-0 active:translate-y-1"
                    >
                        <RotateCcw size={20} />
                        <span>Rematch</span>
                    </button>
                </div>

                <div className="mt-12 flex items-center gap-8 opacity-30 grayscale italic text-sm text-stone-500">
                    <div className="h-px w-16 bg-stone-700"></div>
                    <span>A Legend is Born</span>
                    <div className="h-px w-16 bg-stone-700"></div>
                </div>
            </div>
        </div>
    );
}
