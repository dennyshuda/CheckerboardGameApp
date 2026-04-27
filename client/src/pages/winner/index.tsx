import { useMutation, useQuery } from "@tanstack/react-query";
import type { GameStateResponse } from "@/interfaces/game-state.response";
import { getGameState } from "@/lib/axios";
import { useNavigate } from "react-router";
import axios from "axios";
import { API_BASE } from "@/pages/games";

export default function WinnerPage() {
    const navigate = useNavigate();
    const { data } = useQuery<GameStateResponse>({
        queryKey: ["gameState"],
        queryFn: getGameState,
    });

    const resetMutation = useMutation({
        mutationFn: () => axios.post(`${API_BASE}/api/v1/game/reset`),
        onSuccess: () => {
            navigate("/");
        },
    });

    return (
        <main className="min-h-screen bg-checkered flex items-center justify-center p-6">
            <div className="flex flex-col items-center text-center animate-bounce-in">
                <section className="relative mb-8">
                    <div className="w-48 h-48 bg-yellow-400 border-4 border-slate-900 rounded-full flex items-center justify-center shadow-bold">
                        <span className="text-7xl">🏆</span>
                    </div>
                    <div className="absolute -top-4 -right-4 w-12 h-12 bg-red-600 border-4 border-slate-900 rounded-full"></div>
                    <div className="absolute -bottom-4 -left-4 w-12 h-12 bg-indigo-600 border-4 border-slate-900 rounded-full flex items-center justify-center">
                        <span className="text-white text-xs">👑</span>
                    </div>
                </section>

                <span className="text-red-600 font-black text-2xl uppercase tracking-[0.2em] mb-2">
                    Victory!
                </span>
                <h2 className="text-7xl font-black text-slate-900 mb-4">
                    {data?.status.winner === "Black" ? "Red" : "Blue"} Wins!
                </h2>
                <p className="text-slate-500 font-medium text-xl mb-12">
                    An incredible match and a well-deserved win.
                </p>

                <button
                    onClick={() => resetMutation.mutate()}
                    className="bg-indigo-600 text-white font-black text-xl py-5 px-16 rounded-2xl shadow-button hover:bg-indigo-700 transition-all flex items-center gap-3"
                >
                    PLAY AGAIN <span>↺</span>
                </button>
            </div>
        </main>
    );
}
