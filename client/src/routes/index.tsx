import GamePage from "@/pages/game";
import HomePage from "@/pages/home";
import WinnerPage from "@/pages/winner";
import { createBrowserRouter } from "react-router";
import { ProtectedRoute } from "./protected";
import MainLayout from "@/components/main-layout";
import GamesPage from "@/pages/games";
import HomePage2 from "@/pages/home/home2";

export const router = createBrowserRouter([
    {
        element: <MainLayout />,
        children: [
            {
                path: "/",
                element: <HomePage2 />,
            },
            {
                path: "/game",
                element: <GamesPage />,
            },
            {
                path: "/winner",
                element: <WinnerPage />,
            },
        ],
    },
]);
