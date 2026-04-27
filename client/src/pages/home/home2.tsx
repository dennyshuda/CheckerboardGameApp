import { useMutation } from "@tanstack/react-query";
import axios from "axios";
import { Trophy } from "lucide-react";
import { useState } from "react";
import { useNavigate } from "react-router";

export default function HomePage2() {
    const navigate = useNavigate();
    const [whitePlayer, setWhitePlayer] = useState("");
    const [blackPlayer, setBlackPlayer] = useState("");

    const startMutation = useMutation({
        mutationFn: async () => {
            return await axios.post("http://localhost:5015/api/v1/game/start", {
                playerWhiteName: whitePlayer,
                playerBlackName: blackPlayer,
            });
        },
        onSuccess: () => {
            navigate("/game");
        },
        onError: () => {
            alert(
                "Failed to start the game. Please ensure the server is active.",
            );
        },
    });

    return (
        <main className="min-h-screen bg-checkered flex items-center justify-center p-6">
            <div className="bg-white border-4 border-slate-900 rounded-4xl p-12 max-w-2xl w-full shadow-bold">
                <div className="flex flex-col items-center gap-4">
                    <Trophy className="w-24 h-24 text-indigo-600" />

                    <h1 className="text-5xl font-black text-slate-900">
                        Checkerboard
                    </h1>

                    <p className="text-slate-500 text-lg mb-10 font-medium">
                        Enter your player details to begin the match.
                    </p>
                </div>

                <div className="grid grid-cols-1 md:grid-cols-2 gap-6 mb-10">
                    <section className="space-y-2">
                        <label className="text-sm font-bold text-slate-900 uppercase tracking-wider">
                            Player 1 Name
                        </label>

                        <div className="relative">
                            <input
                                type="text"
                                value={whitePlayer}
                                onChange={(e) => setWhitePlayer(e.target.value)}
                                placeholder="Enter name..."
                                className="w-full bg-indigo-50 border-2 border-slate-900 rounded-2xl py-4 px-12 font-bold text-slate-900 focus:outline-none focus:ring-4 focus:ring-indigo-100 transition-all"
                            />
                            <span className="absolute left-4 top-1/2 -translate-y-1/2 text-indigo-600 font-bold">
                                ●
                            </span>
                        </div>
                    </section>

                    <section className="space-y-2">
                        <label className="text-sm font-bold text-slate-900 uppercase tracking-wider">
                            Player 2 Name
                        </label>
                        <div className="relative">
                            <input
                                type="text"
                                value={blackPlayer}
                                onChange={(e) => setBlackPlayer(e.target.value)}
                                placeholder="Enter name..."
                                className="w-full bg-indigo-50 border-2 border-slate-900 rounded-2xl py-4 px-12 font-bold text-slate-900 focus:outline-none focus:ring-4 focus:ring-indigo-100 transition-all"
                            />
                            <span className="absolute left-4 top-1/2 -translate-y-1/2 text-red-600 font-bold">
                                ●
                            </span>
                        </div>
                    </section>
                </div>

                <button
                    onClick={() => startMutation.mutate()}
                    disabled={
                        startMutation.isPending || !whitePlayer || !blackPlayer
                    }
                    className="w-full bg-indigo-600 text-white font-black text-2xl py-6 rounded-2xl shadow-button flex items-center justify-center gap-3 transition-all hover:bg-indigo-700"
                >
                    Start Game
                    <span className="text-3xl">▶</span>
                </button>
            </div>
        </main>
    );
}
