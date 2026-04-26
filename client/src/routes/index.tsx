import GamePage from "@/pages/game";
import HomePage from "@/pages/home";
import WinnerPage from "@/pages/winner";
import { createBrowserRouter } from "react-router";
import { ProtectedRoute } from "./protected";
import MainLayout from "@/components/main-layout";

export const router = createBrowserRouter([
    {
        element: <MainLayout />,
        children: [
            {
                path: "/",
                element: <HomePage />,
            },
            {
                path: "/game",
                element: (
                    <ProtectedRoute>
                        <GamePage />
                    </ProtectedRoute>
                ),
            },
            {
                path: "/winner",
                element: <WinnerPage />,
            },
        ],
    },
]);
