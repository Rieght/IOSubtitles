using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.IO;
using BepInEx.Configuration;

namespace IOSubtitles
{
    [BepInEx.BepInPlugin(_guid, "IOSubtitles", "1.0.1")]
    public class Hook : BaseUnityPlugin
    {
        private const string _guid = "rieght.insultorder.iosubtitles";

        public static SubtitlesConfig subtitlesConfig = new SubtitlesConfig();

        public Hook()
        {
            SceneManager.sceneLoaded += SubtitlesHook.SceneLoaded;
            SubtitlesHook.Init();
            var harmony = new Harmony(_guid);
            harmony.PatchAll(typeof(SubtitlesHook));

            this.AddConfigs();
        }

        public void AddConfigs()
        {
            subtitlesConfig.FontSize = Config.Bind<int>(new ConfigDefinition("Config", "Font Size"), 16);
        }

        public class SubtitlesConfig
        {
            public ConfigEntry<int> FontSize { get; set; }
        }
    }

    public static class SubtitlesHook
    {
        private static ManualLogSource _logger = BepInEx.Logging.Logger.CreateLogSource("iosubtitles");

        private static UILabel _subtitlesLabel;
        private static GameObject _subtitleTextbox;

        private static FH_AnimeController _AnimeControllerFH;
        private static AudioSource _audiosourceNormal;
        private static AudioSource _audiosourceUX;
        private static AudioSource _audiosourceEjac;
        private static AudioSource _audiosourceEjac2;

        private static Dictionary<string, List<List<string>>> _subtitles = new Dictionary<string, List<List<string>>>();
        private static float _subtitleTime = 0f;
        private static string _subitleId;
        private static int _subtitleLineNr = 0;

        public static void Init()
        {
            string json = File.ReadAllText("BepInEx/plugins/subtitles.json", System.Text.Encoding.UTF8);
            _subtitles = JSONDeserializer.Deserialize(json);
        }

        [HarmonyPatch(typeof(FH_AnimeController), "Update")]
        [HarmonyPostfix]
        public static void PlayClipFH()
        {
            // Return if AudioSources are not loaded.
            if (GameObject.Find("HS_Head").GetComponents<AudioSource>().Length == 0) return;

            // Init Audiosources
            if (_audiosourceNormal == null) _audiosourceNormal = GameObject.Find("HS_Head").GetComponents<AudioSource>()[12];
            if (_audiosourceUX == null) _audiosourceUX = GameObject.Find("HS_Head").GetComponents<AudioSource>()[13];
            if (_audiosourceEjac == null) _audiosourceEjac = GameObject.Find("HS_Head").GetComponents<AudioSource>()[14];
            if (_audiosourceEjac2 == null) _audiosourceEjac2 = GameObject.Find("HS_Head").GetComponents<AudioSource>()[15];

            // Play Subtitles
            if (GameClass.UraVoice && _AnimeControllerFH.ClipName[12] != null && _subtitles.ContainsKey(_AnimeControllerFH.ClipName[12]))
                PlaySubtitle(_AnimeControllerFH.ClipName[12], _audiosourceNormal.time);
            else if (_AnimeControllerFH.ClipName[13] != null && _subtitles.ContainsKey(_AnimeControllerFH.ClipName[13]))
                PlaySubtitle(_AnimeControllerFH.ClipName[13], _audiosourceUX.time);
            else if (_AnimeControllerFH.ClipName[14] != null && _subtitles.ContainsKey(_AnimeControllerFH.ClipName[14]))
                PlaySubtitle(_AnimeControllerFH.ClipName[14], _audiosourceEjac.time);
            else if (_AnimeControllerFH.ClipName[15] != null && _subtitles.ContainsKey(_AnimeControllerFH.ClipName[15]))
                PlaySubtitle(_AnimeControllerFH.ClipName[15], _audiosourceEjac2.time);
            else
                _subtitlesLabel.text = string.Empty;

            // Set Subtitles-Textbox-Position to bottom center
            _subtitleTextbox.transform.position = new Vector3(0, -0.75f, 0);
        }

        private static void PlaySubtitle(string clipname, float audiotime)
        {
            // Swap subtitles on new audioclip
            if (_subitleId != clipname)
            {
                _logger.LogDebug(clipname);
                _subitleId = clipname;
                _subtitleLineNr = 0;
                _subtitleTime = float.Parse(_subtitles[_subitleId][_subtitleLineNr][0]);
                _subtitlesLabel.text = _subtitles[_subitleId][_subtitleLineNr][1];
                return;
            }

            // Swap subtitle-line when current subtitle-line is finished
            if (audiotime >= _subtitleTime)
            {
                _subtitleLineNr++;
                // Return if there are no subtitle-lines left and deactivate the subtitle-line-swapping-process
                if (_subtitles[_subitleId].Count <= _subtitleLineNr)
                {
                    _subtitleTime = float.MaxValue;
                    return;
                }
                _subtitleTime = float.Parse(_subtitles[_subitleId][_subtitleLineNr][0]);
                _subtitlesLabel.text = _subtitles[_subitleId][_subtitleLineNr][1];
                _subtitlesLabel.MakePixelPerfect();
            }
        }

        // Add Subtitle TextBox to Scene
        public static void SceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            if (scene.name == "FH")
            {
                // Init Subtitle-UI-Object
                _subtitleTextbox = new GameObject("Subtitles_Text");
                _subtitlesLabel = _subtitleTextbox.AddComponent<UILabel>();

                // Init AnimeController
                _AnimeControllerFH = GameObject.Find("MainSystem").GetComponent<FH_AnimeController>();

                // Copy values from "Label_FPS" into Subtitle-UI-Object
                UILabel fpsLabel = GameObject.Find("Label_FPS").GetComponent<UILabel>();
                _subtitlesLabel.ambigiousFont = fpsLabel.ambigiousFont;
                _subtitlesLabel.applyGradient = fpsLabel.applyGradient;
                _subtitlesLabel.bitmapFont = fpsLabel.bitmapFont;
                _subtitlesLabel.alignment = fpsLabel.alignment;
                _subtitlesLabel.alpha = fpsLabel.alpha;
                _subtitlesLabel.border = fpsLabel.border;
                _subtitlesLabel.color = fpsLabel.color;
                _subtitlesLabel.depth = fpsLabel.depth;
                _subtitlesLabel.drawRegion = fpsLabel.drawRegion;
                _subtitlesLabel.effectColor = fpsLabel.effectColor;
                _subtitlesLabel.effectDistance = fpsLabel.effectDistance;
                _subtitlesLabel.effectStyle = fpsLabel.effectStyle;
                _subtitlesLabel.floatSpacingX = fpsLabel.floatSpacingX;
                _subtitlesLabel.floatSpacingY = fpsLabel.floatSpacingY;
                _subtitlesLabel.useFloatSpacing = fpsLabel.useFloatSpacing;
                _subtitlesLabel.fontSize = Hook.subtitlesConfig.FontSize.Value;
                _subtitlesLabel.fontStyle = fpsLabel.fontStyle;
                _subtitlesLabel.height = fpsLabel.height;
                _subtitlesLabel.material = fpsLabel.material;
                _subtitlesLabel.width = fpsLabel.width;
                _subtitlesLabel.enabled = true;
                _subtitlesLabel.multiLine = true;
            }
        }
    }
}