namespace CheckerboardGameApp.Constants;

public static class GameConstants
{
    public const int BoardSize = 8;
    public const string ErrorGameNotStarted = "Permainan belum dimulai. Silakan mulai game terlebih dahulu.";
    public const string ErrorPlayerNamesRequired = "Nama kedua pemain harus diisi.";
    public const string ErrorPlayerNamesDuplicate = "Nama kedua pemain tidak boleh sama.";
    public const string ErrorResetGameFailed = "Gagal mereset game.";

    public const string SuccessGameStarted = "Game dimulai! Putih: {0}, Hitam: {1}";
    public const string SuccessGameReset = "Game telah di-reset ke posisi awal standar.";
    public const string SuccessDemoLoaded = "Papan demo berhasil dimuat.";
    public const string SuccessGameRunning = "Game sedang berjalan.";

    public const string LogGameStartFailed = "Gagal memulai game: Validasi pemain gagal.";
    public const string LogGetGameStateFailed = "Gagal mendapatkan status game: Permainan belum dimulai.";
    public const string LogValidMovesRequest = "Mendapatkan valid moves untuk posisi: ({0}, {1})";
    public const string LogMakeMoveRequest = "Menerima permintaan move: {0} -> {1} dari pemain: {2}";
}
