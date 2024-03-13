using BotFW_CvSharp_01.GameClientThings;
using BotFW_CvSharp_01.GlobalStructs;
using Microsoft.Win32;
using System.Drawing;
using static BotFW_CvSharp_01.GameClientThings.Game;

namespace PoeFlasks3.GameClinet
{
    public enum AcceptPoeResolutions
    {
        x_983,
        x_1050,
        x_1080,
    }
    public class PoeClinet
    {
        private const string POE_CLIENT_WINDOW_NAME = "Path of Exile";

        private const string LOG_LOCAL_PATH = "logs\\Client.txt";
        private const string POE_INSTALL_PATH__REGISTRY_KEY = "SOFTWARE\\GrindingGearGames\\Path of Exile";
        private const string POE_INSTALL_PATH__REGISTRY_VALUE = "InstallLocation";

        public static readonly Dictionary<AcceptPoeResolutions, WindowResolution> ACCEPT_SCREEN_RES = new()
        {
            { AcceptPoeResolutions.x_983,  new (1280, 983) },
            { AcceptPoeResolutions.x_1050, new (1920, 1050) },
            { AcceptPoeResolutions.x_1080, new (1920, 1080) }
        };

        private static readonly Dictionary<AcceptPoeResolutions, RECT> FLASKS_SCREEN_AREA = new()
        {
            { AcceptPoeResolutions.x_983, new() },   // (!)
            { AcceptPoeResolutions.x_1050, new(292, 949, 217, 100) }, // (!)
            { AcceptPoeResolutions.x_1080, new(307, 974, 228, 104) }, // (!)
        };


        public Game Window;
        public bool ScreenResolutionIsAccept { get => ScreenResIsAccept(); }
        public AcceptPoeResolutions? Resolution { get => GetScreenResolution(); }


        private readonly bool DEBUG;


        public PoeClinet(bool debug)
        {
            DEBUG = debug;

            Window = new Game(POE_CLIENT_WINDOW_NAME, DEBUG);
        }

        public bool TryGetFlasksScreen(out Bitmap screen)
        {
            var screenRes = GetScreenResolution();
            if (screenRes != null && Window.IsFinded && ScreenResolutionIsAccept && !Window.IsMinimized)
            {
                var screenRECT = FLASKS_SCREEN_AREA[screenRes.Value];
                //screen = new Bitmap(screenRECT.w, screenRECT.h, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                screen = new Bitmap(screenRECT.w, screenRECT.h, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                Window.GetScreen(screen, screenRECT, mode: GetScreenMode.background);
                //screen.Save("test1.png");
                return true;
            }

            screen = new(1, 1, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            return false;
        }

        private AcceptPoeResolutions? GetScreenResolution()
        {
            foreach (var res in ACCEPT_SCREEN_RES)
            {
                if (res.Value == Window.Resolution)
                    return res.Key;
            }

            return null;
        }

        private bool ScreenResIsAccept()
        {
            if (DEBUG)
                return true;

            foreach (var screenRes in ACCEPT_SCREEN_RES)
            {
                if (Window.WindowRect == screenRes.Value)
                    return true;
            }

            return false;
        }

        public bool TryGetPoeLogFolderFromRegistry(out string logPath)
        {
            logPath = "";
            if (TryGetPoeInstallPath(out string poeInstallPath))
            {
                logPath =  poeInstallPath + LOG_LOCAL_PATH;
                if (File.Exists(logPath))
                    return true;
            }


            return false;
        }

        private bool TryGetPoeInstallPath(out string path)
        {
            path = "";
            using var key = Registry.CurrentUser.OpenSubKey(POE_INSTALL_PATH__REGISTRY_KEY, false);
            if (key != null)
            {
                var result = key.GetValue(POE_INSTALL_PATH__REGISTRY_VALUE);
                if (result != null)
                {
                    path = (string)result;
                    return true;
                }
            }

            return false;
        }
    }
}
