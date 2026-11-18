using HarmonyLib;
using MelonLoader;
using UnityEngine;

[assembly: MelonInfo(typeof(OverkillTracker.Core), "OverkillTracker", "1.0.0", "alydev", null)]
[assembly: MelonGame("Finn Schuuring", "King of the Bridge")]

namespace OverkillTracker;

public class Core : MelonMod
{
    public override void OnInitializeMelon()
    {
        LoggerInstance.Msg("Initialized.");
    }

    [HarmonyPatch(typeof(ChessMatchManager), nameof(ChessMatchManager.CompleteMove))]
    public static class CompleteMoveWatcher
    {
        private static int _lastWhitePiecesRemaining = 16;
        public static void Postfix(ChessMatchManager __instance)
        {
            var count = __instance.whitePieces.Count;
            if (count < _lastWhitePiecesRemaining)
            {
                _lastWhitePiecesRemaining = count;
                if (_lastWhitePiecesRemaining >= 12)
                    Melon<Core>.Logger.Msg($"Remaining: {_lastWhitePiecesRemaining}");
                else if (_lastWhitePiecesRemaining >= 10)
                    Melon<Core>.Logger.Warning($"Remaining: {_lastWhitePiecesRemaining}");
                else
                    Melon<Core>.Logger.Warning($"Lost chance at Overkill");
            }
        }

        internal static void Reset()
        {
            _lastWhitePiecesRemaining = 16;
        }
    }

    [HarmonyPatch(typeof(ChessMatchManager), nameof(ChessMatchManager.SetupChessMatch))]
    public static class SetupMatchWatcher
    {
        public static void Postfix()
        {
            Melon<Core>.Logger.Msg("Resetting.");
            CompleteMoveWatcher.Reset();
        }
    }
}
