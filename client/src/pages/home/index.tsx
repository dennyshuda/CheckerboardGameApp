/* eslint-disable @typescript-eslint/no-unused-vars */
import { useMutation, useQuery } from "@tanstack/react-query";
import axios from "axios";
import { Loader2, Play } from "lucide-react";
import { useState } from "react";
import { useNavigate } from "react-router";

export default function HomePage() {
    const navigate = useNavigate();
    const [whitePlayer, setWhitePlayer] = useState("");
    const [blackPlayer, setBlackPlayer] = useState("");

    const { data, isError } = useQuery({
        queryKey: ["serverStatus"],
        queryFn: async () => {
            const response = await axios.get(
                "http://localhost:5015/api/v1/game/status",
            );
            return response.data;
        },
        retry: false,
        refetchInterval: 5000,
    });

    const startMutation = useMutation({
        mutationFn: async () => {
            return await axios.post("http://localhost:5015/api/v1/game/start", {
                playerWhiteName: whitePlayer,
                playerBlackName: blackPlayer,
            });
        },
        onSuccess: () => {
            navigate("/games");
        },
        onError: () => {
            alert(
                "Failed to start the game. Please ensure the server is active.",
            );
        },
    });

    return (
        <div className="min-h-screen bg-[#0c0a09] flex flex-col items-center justify-center p-6 text-stone-100 font-serif relative overflow-hidden">
            <div className="absolute top-[-10%] right-[-10%] w-[40%] h-[40%] bg-amber-900/20 blur-[120px] rounded-full pointer-events-none"></div>
            <div className="absolute bottom-[-10%] left-[-10%] w-[40%] h-[40%] bg-orange-950/10 blur-[120px] rounded-full pointer-events-none"></div>

            <div className="z-10 flex flex-col items-center max-w-lg w-full">
                <div className="mb-16 text-center">
                    <h1 className="text-5xl font-extrabold tracking-tight text-stone-100">
                        Checkerboard<span className="text-amber-500">Game</span>
                    </h1>
                </div>

                <div className="bg-stone-900/50 backdrop-blur-md p-8 rounded-3xl shadow-2xl shadow-black/50 border border-stone-800 w-full space-y-6 animate-in fade-in zoom-in duration-700">
                    <div className="space-y-4">
                        <div className="space-y-2">
                            <label className="text-xs font-bold uppercase tracking-wider text-stone-500 ml-1">
                                White Player
                            </label>
                            <input
                                type="text"
                                value={whitePlayer}
                                onChange={(e) => setWhitePlayer(e.target.value)}
                                placeholder="Enter name..."
                                className="w-full bg-stone-950 border border-stone-800 focus:border-amber-500 outline-none p-4 rounded-xl text-stone-100 transition-all placeholder:text-stone-700"
                            />
                        </div>
                        <div className="space-y-2">
                            <label className="text-xs font-bold uppercase tracking-wider text-stone-500 ml-1">
                                Black Player
                            </label>
                            <input
                                type="text"
                                value={blackPlayer}
                                onChange={(e) => setBlackPlayer(e.target.value)}
                                placeholder="Enter name..."
                                className="w-full bg-stone-950 border border-stone-800 focus:border-stone-400 outline-none p-4 rounded-xl text-stone-100 transition-all placeholder:text-stone-700"
                            />
                        </div>
                    </div>

                    <button
                        onClick={() => startMutation.mutate()}
                        disabled={
                            startMutation.isPending ||
                            !whitePlayer ||
                            !blackPlayer ||
                            isError
                        }
                        className="w-full flex items-center justify-center gap-2 p-4 bg-amber-700 hover:bg-amber-600 text-stone-100 disabled:bg-stone-800 disabled:text-stone-600 transition-all rounded-xl font-bold text-lg shadow-xl shadow-black/20 border-b-4 border-amber-900 active:border-b-0 active:translate-y-0.5"
                    >
                        {startMutation.isPending ? (
                            <Loader2 className="animate-spin size-5" />
                        ) : (
                            <Play size={20} className="fill-current" />
                        )}
                        <span>
                            {startMutation.isPending
                                ? "Starting..."
                                : "Start Match"}
                        </span>
                    </button>
                </div>

                <div className="mt-12 w-full flex justify-between items-center px-4">
                    <div className="flex items-center gap-2">
                        <div
                            className={`w-2 h-2 rounded-full ${isError ? "bg-red-500" : "bg-emerald-500 animate-pulse"}`}
                        ></div>
                        <span className="text-[10px] font-bold uppercase tracking-widest text-stone-500">
                            {isError ? "Server Offline" : "System Ready"}
                        </span>
                    </div>

                    <span className="text-[10px] font-bold uppercase tracking-widest text-stone-700">
                        v2.0.0 Stable
                    </span>
                </div>
            </div>
        </div>
    );
}
