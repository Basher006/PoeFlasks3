using BotFW_CvSharp_01.GlobalStructs;
using OpenCvSharp;
using PoeFlasks3.GameClinet;

namespace PoeFlasks3.BotLogic
{
    public struct ScreensForNumbersFinder : IDisposable
    {
        public Mat HPArea;
        //public RECT HPAreaRECT; 
        public Mat MPArea;
        //public Mat MPAreaRECT;
        public Mat FlasksStateArea;

        public void Dispose()
        {
            HPArea.Dispose();
            MPArea.Dispose();
            FlasksStateArea.Dispose();
        }
    }

    public static class ScreenSliser
    {
        private static readonly Dictionary<AcceptPoeResolutions, RECT> Screen_HPAreas = new()
        {
            { AcceptPoeResolutions.x_983, new RECT() }, // (!)
            { AcceptPoeResolutions.x_1050, new RECT(3, 760, 258, 83) },
            { AcceptPoeResolutions.x_1080, new RECT(3, 775, 258, 83) }
        };

        private static readonly Dictionary<AcceptPoeResolutions, RECT> Screen_MPAreas = new()
        {
            { AcceptPoeResolutions.x_983, new RECT() }, // (!)
            { AcceptPoeResolutions.x_1050, new RECT(1660, 760, 258, 83) },
            { AcceptPoeResolutions.x_1080, new RECT(1660, 775, 258, 83) } 
        };

        private static readonly Dictionary<AcceptPoeResolutions, RECT> Screen_flaskStateAreas = new()
        {
            { AcceptPoeResolutions.x_983, new RECT(0, 0, 205, 1) }, // (!)
            { AcceptPoeResolutions.x_1050, new RECT(296, 1042, 220, 1) },
            { AcceptPoeResolutions.x_1080, new RECT(312, 1073, 230, 1) }
        };



        public static ScreensForNumbersFinder Slise(Mat screen, AcceptPoeResolutions PoeWindowResolution)
        {
            //var hparea = new Mat(screen, Screen_HPAreas[PoeWindowResolution].ToRect());
            //hparea.SaveImage("hparea.png");

            //var mparea = new Mat(screen, Screen_MPAreas[PoeWindowResolution].ToRect());
            //mparea.SaveImage("mparea.png");

            //var flasksStateArea = new Mat(screen, Screen_flaskStateAreas[PoeWindowResolution].ToRect());
            //flasksStateArea.SaveImage("flasksStateArea.png");

            //Console.WriteLine();

            return new ScreensForNumbersFinder
            {
                HPArea = new Mat(screen, Screen_HPAreas[PoeWindowResolution].ToRect()),
                MPArea = new Mat(screen, Screen_MPAreas[PoeWindowResolution].ToRect()),
                FlasksStateArea = new Mat(screen, Screen_flaskStateAreas[PoeWindowResolution].ToRect())
            };
        }
    }
}
