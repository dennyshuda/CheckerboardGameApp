import axios from "axios";

const API_URL = "http://localhost:5015/api/v1/game";

export const getGameState = () =>
    axios.get(`${API_URL}/state`).then((res) => res.data);

export const getValidMoves = (x: number, y: number) =>
    axios.get(`${API_URL}/valid-moves/${x}/${y}`).then((res) => res.data.valid);

export const movePiece = (moveData: any) =>
    axios.post(`${API_URL}/move`, moveData).then((res) => res.data);
