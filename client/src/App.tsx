import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import Checkerboard from "./components/checkerboard";

function App() {
    const queryClient = new QueryClient();

    return (
        <QueryClientProvider client={queryClient}>
            <Checkerboard />
        </QueryClientProvider>
    );
}

export default App;
