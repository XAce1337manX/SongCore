using HarmonyLib;
using SongCore.Utilities;

namespace SongCore.HarmonyPatches
{
    [HarmonyPatch(typeof(StandardLevelScenesTransitionSetupDataSO))]
    [HarmonyPatch(nameof(StandardLevelScenesTransitionSetupDataSO.Init), MethodType.Normal)]
    internal class SceneTransitionPatch
    {
        private static void Prefix(ref IDifficultyBeatmap difficultyBeatmap, ref ColorScheme? overrideColorScheme)
        {
            if (!Plugin.Configuration.CustomSongNoteColors && !Plugin.Configuration.CustomSongEnvironmentColors && !Plugin.Configuration.CustomSongObstacleColors)
            {
                return;
            }

            var songData = Collections.RetrieveDifficultyData(difficultyBeatmap);
            if (songData == null)
            {
                return;
            }

            if (songData._colorLeft == null && songData._colorRight == null && songData._envColorLeft == null && songData._envColorRight == null && songData._obstacleColor == null && songData._envColorLeftBoost == null && songData._envColorRightBoost == null)
            {
                return;
            }

            var environmentInfoSO = difficultyBeatmap.GetEnvironmentInfo();
            var fallbackScheme = overrideColorScheme ?? new ColorScheme(environmentInfoSO.colorScheme);

            if (Plugin.Configuration.CustomSongNoteColors) Logging.Logger.Info("Custom Song Note Colors On");
            if (Plugin.Configuration.CustomSongEnvironmentColors) Logging.Logger.Info("Custom Song Environment Colors On");
            if (Plugin.Configuration.CustomSongObstacleColors) Logging.Logger.Info("Custom Song Obstacle Colors On");

            var saberLeft = (songData._colorLeft == null || !Plugin.Configuration.CustomSongNoteColors)
                ? fallbackScheme.saberAColor
                : Utils.ColorFromMapColor(songData._colorLeft);
            var saberRight = (songData._colorRight == null || !Plugin.Configuration.CustomSongNoteColors)
                ? fallbackScheme.saberBColor
                : Utils.ColorFromMapColor(songData._colorRight);
            var envLeft = (songData._envColorLeft == null || !Plugin.Configuration.CustomSongEnvironmentColors)
                ? fallbackScheme.environmentColor0
                : Utils.ColorFromMapColor(songData._envColorLeft);
            var envRight = (songData._envColorRight == null || !Plugin.Configuration.CustomSongEnvironmentColors)
                ? fallbackScheme.environmentColor1
                : Utils.ColorFromMapColor(songData._envColorRight);
            var envLeftBoost = (songData._envColorLeftBoost == null || !Plugin.Configuration.CustomSongEnvironmentColors)
                ? fallbackScheme.environmentColor0Boost
                : Utils.ColorFromMapColor(songData._envColorLeftBoost);
            var envRightBoost = (songData._envColorRightBoost == null|| !Plugin.Configuration.CustomSongEnvironmentColors)
                ? fallbackScheme.environmentColor1Boost
                : Utils.ColorFromMapColor(songData._envColorRightBoost);
            var obstacle = (songData._obstacleColor == null || !Plugin.Configuration.CustomSongObstacleColors)
                ? fallbackScheme.obstaclesColor
                : Utils.ColorFromMapColor(songData._obstacleColor);
            overrideColorScheme = new ColorScheme("SongCoreMapColorScheme", "SongCore Map Color Scheme", true, "SongCore Map Color Scheme", false, saberLeft, saberRight, envLeft,
                envRight, true, envLeftBoost, envRightBoost, obstacle);
        }
    }
}