import { createBrowserRouter } from "react-router";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import HomePage from "@/pages/home";
import GamePage from "@/pages/game";

const queryClient = new QueryClient();

export const router = createBrowserRouter([
    {
        path: "/",
        element: <HomePage />,
    },
    {
        path: "/game",
        element: (
            <QueryClientProvider client={queryClient}>
                <GamePage />
            </QueryClientProvider>
        ),
    },
]);
