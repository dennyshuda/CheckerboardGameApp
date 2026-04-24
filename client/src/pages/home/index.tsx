import { Play, Settings, Trophy, Gamepad2 } from "lucide-react";
import { Link } from "react-router";

export default function HomePage() {
    return (
        <div className="min-h-screen bg-slate-950 flex flex-col items-center justify-center p-6 text-slate-100 overflow-hidden relative">
            {/* Efek Background Dekoratif */}
            <div className="absolute top-[-10%] left-[-10%] w-[40%] h-[40%] bg-amber-600/10 blur-[120px] rounded-full"></div>
            <div className="absolute bottom-[-10%] right-[-10%] w-[40%] h-[40%] bg-blue-600/10 blur-[120px] rounded-full"></div>

            <div className="z-10 flex flex-col items-center max-w-4xl w-full">
                {/* Logo & Judul */}
                <div className="mb-12 text-center animate-in fade-in slide-in-from-bottom-8 duration-700">
                    <div className="inline-flex p-4 rounded-3xl bg-slate-900 border border-slate-800 shadow-2xl mb-6">
                        <Gamepad2 size={64} className="text-amber-500" />
                    </div>
                    <h1 className="text-7xl font-black tracking-tighter bg-linear-to-br from-white to-slate-500 bg-clip-text text-transparent italic">
                        CHECKER<span className="text-amber-500">.IO</span>
                    </h1>
                    <p className="text-slate-400 mt-4 text-lg font-medium tracking-wide">
                        Master the board, capture the crown.
                    </p>
                </div>

                {/* Menu Utama */}
                <div className="grid grid-cols-1 md:grid-cols-2 gap-4 w-full max-w-2xl animate-in fade-in slide-in-from-bottom-12 duration-1000 delay-200">
                    {/* Tombol Play Utama */}
                    <Link
                        to="/game"
                        className="group relative col-span-1 md:col-span-2 flex items-center justify-center gap-3 p-6 bg-amber-600 hover:bg-amber-500 transition-all rounded-2xl border-b-4 border-amber-800 active:border-b-0 active:translate-y-1 overflow-hidden"
                    >
                        <Play className="fill-current group-hover:scale-110 transition-transform" />
                        <span className="text-2xl font-bold uppercase tracking-widest">
                            Mulai Permainan
                        </span>
                    </Link>

                    {/* Tombol Stats */}
                    <button className="flex items-center justify-center gap-3 p-4 bg-slate-900 hover:bg-slate-800 transition-all rounded-2xl border border-slate-800 group">
                        <Trophy
                            className="text-amber-500 group-hover:rotate-12 transition-transform"
                            size={20}
                        />
                        <span className="font-semibold uppercase tracking-wider text-sm text-slate-300">
                            Leaderboard
                        </span>
                    </button>

                    <button className="flex items-center justify-center gap-3 p-4 bg-slate-900 hover:bg-slate-800 transition-all rounded-2xl border border-slate-800 group">
                        <Settings
                            className="text-slate-500 group-hover:rotate-90 transition-transform duration-500"
                            size={20}
                        />
                        <span className="font-semibold uppercase tracking-wider text-sm text-slate-300">
                            Pengaturan
                        </span>
                    </button>
                </div>

                <div className="mt-24 flex gap-8 text-slate-600 font-mono text-xs uppercase tracking-widest">
                    <div className="flex flex-col items-center gap-1">
                        <span className="text-slate-400">Version</span>
                        <span>1.0.0 Web-Build</span>
                    </div>
                    <div className="flex flex-col items-center gap-1">
                        <span className="text-slate-400">Server Status</span>
                        <span className="text-emerald-500 flex items-center gap-1">
                            <span className="w-1.5 h-1.5 bg-emerald-500 rounded-full animate-pulse"></span>
                            Online
                        </span>
                    </div>
                </div>
            </div>
        </div>
    );
}
