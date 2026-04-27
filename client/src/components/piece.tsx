import type { Square } from "@/interfaces/game-state.response";
import { CrownIcon } from "lucide-react";
import { cn } from "@/utils/cn";

export function Piece({
    square,
    isSelected,
}: {
    square: Square;
    isSelected: boolean;
}) {
    if (!square?.piece) return null;

    return (
        <div
            className={cn(
                "w-12 h-12 rounded-full flex items-center border-4 justify-center shadow-[0px_8px_0px_-2px_rgba(0,0,0,0.3)]",
                square.piece.color === "White"
                    ? "bg-indigo-600 border-black"
                    : "bg-red-600 border-black",
                isSelected &&
                    "scale-110 -translate-y-1 shadow-[0px_8px_0px_-2px_rgba(0,0,0,0.3)]",
            )}
        >
            {square.piece.role === "King" && (
                <span className="text-xl sm:text-4xl drop-shadow-md select-none">
                    <CrownIcon className="text-yellow-500" />
                </span>
            )}
        </div>
    );
}
