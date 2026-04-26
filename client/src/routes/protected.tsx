import { useQuery } from "@tanstack/react-query";
import axios from "axios";

export const ProtectedRoute = ({ children }: { children: React.ReactNode }) => {
    const { data } = useQuery({
        queryKey: ["check"],
        queryFn: () =>
            axios
                .get("http://localhost:5015/api/v1/game/state")
                .then((res) => res),
    });

    console.log("ProtectedRoute data:", data);

    return <div>{children}</div>;
};
