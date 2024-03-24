using BotFW_CvSharp_01;
using BotFW_CvSharp_01.GlobalStructs;
using OpenCvSharp;
using PoeFlasks3.GameClinet;

namespace PoeFlasks3.BotLogic
{
    public enum NumbersTamplates
    {
        one,
        two,
        three,
        four,
        five,
        six,
        seven,
        eight,
        nine,
        zero,
        slash
    }

    public enum Langueges
    {
        RU,
        EN
    }

    public static class BotResourseLoader
    {

        public static Dictionary<AcceptPoeResolutions, Dictionary<NumbersTamplates, LoadedTemplate<NumbersTamplates>>>? Numbers { get; private set; }
        public static Dictionary<Langueges, string[]> LanguegesText { get; private set; }
        public static readonly Dictionary<string, Langueges> StrToLanguege = new()
        { { "RU", Langueges.RU }, { "EN", Langueges.EN } };



        private static readonly ImreadModes ImgReadMode = ImreadModes.Grayscale;

        private static readonly Dictionary<AcceptPoeResolutions, string> poeNumbersPathes = new() 
        { { AcceptPoeResolutions.x_983, "imgs\\Numbers\\983\\" }, { AcceptPoeResolutions.x_1050, "imgs\\Numbers\\1050\\" }, { AcceptPoeResolutions.x_1080, "imgs\\Numbers\\1080\\" } };

        private static readonly TemplateSetup LoadSetup = new() { GetOnlyMax = false, IgnoreBlack = false, Rect = new RECT(), Treshold = 0.9f };

        private static readonly List<TemplateLoadSetup<NumbersTamplates>> templateLoadSeup = new List<TemplateLoadSetup<NumbersTamplates>>()
        {
            new() { ImgReadMod = ImgReadMode, Name = "1", Path = "1.png", Setup = LoadSetup, Template = NumbersTamplates.one },
            new() { ImgReadMod = ImgReadMode, Name = "2", Path = "2.png", Setup = LoadSetup, Template = NumbersTamplates.two },
            new() { ImgReadMod = ImgReadMode, Name = "3", Path = "3.png", Setup = LoadSetup, Template = NumbersTamplates.three },
            new() { ImgReadMod = ImgReadMode, Name = "4", Path = "4.png", Setup = LoadSetup, Template = NumbersTamplates.four },
            new() { ImgReadMod = ImgReadMode, Name = "5", Path = "5.png", Setup = LoadSetup, Template = NumbersTamplates.five },
            new() { ImgReadMod = ImgReadMode, Name = "6", Path = "6.png", Setup = LoadSetup, Template = NumbersTamplates.six },
            new() { ImgReadMod = ImgReadMode, Name = "7", Path = "7.png", Setup = LoadSetup, Template = NumbersTamplates.seven },
            new() { ImgReadMod = ImgReadMode, Name = "8", Path = "8.png", Setup = LoadSetup, Template = NumbersTamplates.eight },
            new() { ImgReadMod = ImgReadMode, Name = "9", Path = "9.png", Setup = LoadSetup, Template = NumbersTamplates.nine },
            new() { ImgReadMod = ImgReadMode, Name = "0", Path = "0.png", Setup = LoadSetup, Template = NumbersTamplates.zero },
            new() { ImgReadMod = ImgReadMode, Name = "slash", Path = "s.png", Setup = LoadSetup, Template = NumbersTamplates.slash },
        };

        private static Dictionary<Langueges, string> LanguegesPathes = new()
        { { Langueges.RU, "Lang\\RU.txt" }, { Langueges.EN, "Lang\\EN.txt" } };

        public static void Load()
        {
            _LoadNumbers();
            _LoadLangueges();
        }

        private static void _LoadLangueges()
        {
            LanguegesText = new();
            foreach (Langueges lang in Enum.GetValues(typeof(Langueges)))
            {
                if (File.Exists(LanguegesPathes[lang]))
                    LanguegesText.Add(lang, File.ReadAllLines(LanguegesPathes[lang], System.Text.Encoding.UTF8));
                else
                {
                    LanguegesText.Add(lang, LoadDefaultLanguegeText());
                    Log.Write($"Cant load languege file: {LanguegesPathes[lang]}", Log.LogType.Error);
                }
            }
        }

        private static void _LoadNumbers()
        {
            Numbers = new();
            foreach (var item in poeNumbersPathes)
            {
                var _templateLoadSeup = templateLoadSeup.ToArray();
                for (int i = 0; i < templateLoadSeup.Count; i++)
                {
                    var temp = _templateLoadSeup[i];
                    temp.Path = poeNumbersPathes[item.Key] + temp.Path;
                    _templateLoadSeup[i] = temp;
                }
                Numbers.Add(item.Key, TemplateLoader.Load(_templateLoadSeup));
            }
        }

        private static string[] LoadDefaultLanguegeText()
        {
            return Array.Empty<string>(); // (!!!)
        }
    }
}
