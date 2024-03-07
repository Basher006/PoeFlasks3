﻿using BotFW_CvSharp_01;
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

    public static class BotResourseLoader
    {
        private static readonly ImreadModes ImgReadMode = ImreadModes.Grayscale;

        private static readonly Dictionary<NumbersTamplates, string> numbersFileNames = new()
        {
            { NumbersTamplates.one,     "1.png" },
            { NumbersTamplates.two,     "2.png" },
            { NumbersTamplates.three,    "3.png" },
            { NumbersTamplates.four,    "4.png" },
            { NumbersTamplates.five,    "5.png" },
            { NumbersTamplates.six,     "6.png" },
            { NumbersTamplates.seven,   "7.png" },
            { NumbersTamplates.eight,   "8.png" },
            { NumbersTamplates.nine,    "9.png" },
            { NumbersTamplates.zero,    "0.png" },
            { NumbersTamplates.slash,   "s.png" },
        };

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


        public static Dictionary<AcceptPoeResolutions, Dictionary<NumbersTamplates, LoadedTemplate<NumbersTamplates>>>? Numbers;

        //public static Mat testScreen = new Mat("imgs\\testScreens\\1050en.png", ImreadModes.Color);
        public static Mat testScreen = new Mat("imgs\\testScreens\\1050ru_nf.png", ImreadModes.Color);

        public static void Load()
        {
            _LoadNumbers();
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

            // 1080en_nf.png
            // 0.96

            // 1050
            // 0.96

            //using Mat maskedScreen = new Mat();
            //BotFW.GetMask(testScreen, maskedScreen, new Scalar(191, 209, 197), new Scalar(256, 256, 256));
            //maskedScreen.SaveImage("screen.png");
            ////var res = BotFW.MatchTemplate_ignoreBlack(maskedScreen, Numbers[AcceptPoeResolutions.x_1080][NumbersTamplates.one].Image, tr: 0.9f);
            //var resolution = AcceptPoeResolutions.x_1050;
            //float tr = 0.88f;
            //var res1 = BotFW.MatchTemplate(maskedScreen, Numbers[resolution][NumbersTamplates.one].Image, tr: tr);
            //var res2 = BotFW.MatchTemplate(maskedScreen, Numbers[resolution][NumbersTamplates.two].Image, tr: tr);
            //var res3 = BotFW.MatchTemplate(maskedScreen, Numbers[resolution][NumbersTamplates.three].Image, tr: tr);
            //var res4 = BotFW.MatchTemplate(maskedScreen, Numbers[resolution][NumbersTamplates.four].Image, tr: tr);
            //var res5 = BotFW.MatchTemplate(maskedScreen, Numbers[resolution][NumbersTamplates.five].Image, tr: tr);
            //var res6 = BotFW.MatchTemplate(maskedScreen, Numbers[resolution][NumbersTamplates.six].Image, tr: tr);
            //var res7 = BotFW.MatchTemplate(maskedScreen, Numbers[resolution][NumbersTamplates.seven].Image, tr: tr);
            //var res8 = BotFW.MatchTemplate(maskedScreen, Numbers[resolution][NumbersTamplates.eight].Image, tr: tr);
            //var res9 = BotFW.MatchTemplate(maskedScreen, Numbers[resolution][NumbersTamplates.nine].Image, tr: tr);
            //var res0 = BotFW.MatchTemplate(maskedScreen, Numbers[resolution][NumbersTamplates.zero].Image, tr: tr);
            //var ress = BotFW.MatchTemplate(maskedScreen, Numbers[resolution][NumbersTamplates.slash].Image, tr: tr);
            //res.ToBitmap().Save("test.png");

            //Console.WriteLine();
        }

        //private static readonly Dictionary<NumbersResolution, string> numbersDirectories = new()
        //{
        //    { NumbersResolution.x_983, "imgs\\Numbers\\983" },
        //    { NumbersResolution.x_1050, "imgs\\Numbers\\1050" },
        //    { NumbersResolution.x_1080, "imgs\\Numbers\\1080" },
        //};

        //public static Dictionary<NumbersResolution, Dictionary<NumbersTamplates, Mat>> LoadNumbers()
        //{
        //    var result = new Dictionary<NumbersResolution, Dictionary<NumbersTamplates, Mat>>();
        //    foreach (var dir in numbersDirectories)
        //    {
        //        result.Add(dir.Key, _loadNumbersFromDirectory(dir.Value));
        //    }

        //    return result;
        //}

        private static Dictionary<NumbersTamplates, Mat> _loadNumbersFromDirectory(string directory)
        {
            Dictionary<NumbersTamplates, Mat> result = new();
            if (Directory.Exists(directory))
            {
                foreach (var fileName in numbersFileNames)
                {
                    string filePath = directory + fileName.Value;
                    if (File.Exists(filePath))
                        result.Add(fileName.Key, new Mat(filePath, ImgReadMode));
                    else
                        throw new Exception("Cant load numbers file!");
                }
            }
            else
                throw new Exception("Cant find numbers directory!");

            return result;
        }
    }
}