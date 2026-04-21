import { useEffect, useState } from "react";

function App() {
    const [data, setData] = useState<any>();

    useEffect(() => {
        fetch("http://localhost:5015")
            .then((response) => {
                return response.json();
            })
            .then((jsonData) => {
                console.log("Data diterima:", jsonData);
                setData(jsonData);
            })
            .catch((error) => console.error("Error fetching data:", error));
    }, []);

    return (
        <>
            <h1>Halo {data?.name}</h1>
        </>
    );
}

export default App;
