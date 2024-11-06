using BotFW_CvSharp_01;
using BotFW_CvSharp_01.CvThings;
using BotFW_CvSharp_01.GlobalStructs;
using OpenCvSharp;
using PoeFlasks3.GameClinet;

namespace PoeFlasks3.BotLogic
{
    public static class NumbersFinder
    {
        private struct FindedNumber
        {
            public int X_loc;
            public NumbersTamplates Number;
            public string Name;
        }


        private const int FLASKS_COUNT = 5;


        private static readonly Dictionary<AcceptPoeResolutions, int> EachFlaskWidth = new()
        {
            { AcceptPoeResolutions.x_983, 41 },
            { AcceptPoeResolutions.x_1050, 43 },
            { AcceptPoeResolutions.x_1080, 46 }
        };

        private static readonly Dictionary<AcceptPoeResolutions, int> EachFlaskLineWidth = new()
        {
            { AcceptPoeResolutions.x_983, 31 },
            { AcceptPoeResolutions.x_1050, 33 },
            { AcceptPoeResolutions.x_1080, 34 }
        };

        private static readonly Scalar screenNumbersMaskLower = new(191, 209, 197);
        private static readonly Scalar screenNumbersMaskUpper = new(256, 256, 256);

        private static readonly Scalar screenFlaskStateMaskLower = new(100, 100, 100);
        private static readonly Scalar screenFlaskStateMaskUpper = new(255, 255, 255);

        private static readonly Point numbersOffset = new(50, 17);

        public static GrabedData GrabData(
            ScreensForNumbersFinder screens, 
            Dictionary<AcceptPoeResolutions, Dictionary<NumbersTamplates, LoadedTemplate<NumbersTamplates>>>? allNumbers, 
            AcceptPoeResolutions poeClinetResolution)
        {
            var numbers = allNumbers[poeClinetResolution];
            var maskedScreens = ApplyMask(screens);
            screens.Dispose();
            GrabedData result = new();


            var findFlags = TryFindSlahes(maskedScreens, numbers, out ResLoc hpSlash, out ResLoc mpSlash, out ResLoc esSlash, out bool ESCoveredHP);

            // HP
            if (findFlags.HP_isFind && TryFindNumbers(maskedScreens.HPArea, hpSlash, poeClinetResolution, out int currentHP, out int maxHP))
                result.HP = new() { Current = currentHP, Max = maxHP };
            // MP
            if (findFlags.MP_isFind && TryFindNumbers(maskedScreens.MPArea, mpSlash, poeClinetResolution, out int currentMP, out int maxMP))
                result.MP = new() { Current = currentMP, Max = maxMP };
            // ES
            if (findFlags.ES_isFind && TryFindNumbers(ESCoveredHP ? maskedScreens.HPArea : maskedScreens.MPArea, esSlash, poeClinetResolution, out int currentES, out int maxES))
                result.ES = new() { Current = currentES, Max = maxES };

            // FLasksState
            var flasksStates = FindFlasksState(maskedScreens, poeClinetResolution);


            result.FlasksState = flasksStates;
            result.FindedFlags = findFlags;
            return result;
        }


        private static FindFlags TryFindSlahes(ScreensForNumbersFinder screens, Dictionary<NumbersTamplates, LoadedTemplate<NumbersTamplates>> numbers,
            out ResLoc hpSlash, out ResLoc mpSlash, out ResLoc esSlash, out bool EsCoveredHP)
        {
            var findedFlags = new FindFlags();


            var hpAreaSlashes = BotFW.MatchTemplate(screens.HPArea, numbers[NumbersTamplates.slash]);
            var mpAreaSlashes = BotFW.MatchTemplate(screens.MPArea, numbers[NumbersTamplates.slash]);


            // HP
            findedFlags.HP_isFind = hpAreaSlashes.IsFind;
            if (hpAreaSlashes.IsFind)
                hpSlash = GetTopMostSlash(hpAreaSlashes);
            else
                hpSlash = new();


            // MP
            findedFlags.MP_isFind = mpAreaSlashes.IsFind;
            if (mpAreaSlashes.IsFind)
                mpSlash = GetTopMostSlash(mpAreaSlashes);
            else
                mpSlash = new();


            // ES
            if ((hpAreaSlashes.IsFind || mpAreaSlashes.IsFind) && (hpAreaSlashes.TotalResults == 2 || mpAreaSlashes.TotalResults == 2))
            {
                EsCoveredHP = hpAreaSlashes.TotalResults == 2;
                findedFlags.ES_isFind = true;
                esSlash = EsCoveredHP ? GetBottomMostSlash(hpAreaSlashes) : GetBottomMostSlash(mpAreaSlashes);
            }
            else
            {
                esSlash = new();
                EsCoveredHP = false;
            }


            return findedFlags;
        }

        private static bool TryFindNumbers(Mat screen, ResLoc slashArea, AcceptPoeResolutions poeClinetResolution,
            out int number_current, out int number_max)
        {
            var numbersRECT_current = GetRECTForNumbersLication(slashArea, false);
            var numbersRECT_max = GetRECTForNumbersLication(slashArea, true);
            if (screen.Empty() || screen.Width < 1 || screen.Height < 1)
            {
                number_current = 0;
                number_max = 0;
                return false;
            }

            using Mat screenArea_current = new(screen, numbersRECT_current.ToRect());
            using Mat screenArea_max = new(screen, numbersRECT_max.ToRect());

            var numbersLocations_current = FindAllNumbersLocations(screenArea_current, poeClinetResolution);
            var numbersLocations_max = FindAllNumbersLocations(screenArea_max, poeClinetResolution);

            number_current = GetNumber(numbersLocations_current);
            number_max = GetNumber(numbersLocations_max);

            return true;
        }

        private static int GetNumber(List<FindedNumber> findedNumbersLocations, string tempRes = "")
        {
            int minX = int.MaxValue;
            FindedNumber minNumber = new();

            foreach (var item in findedNumbersLocations)
            {
                if (item.X_loc < minX)
                {
                    minX = item.X_loc;
                    minNumber = item;
                }
            }

            if (findedNumbersLocations.Count > 0)
            {
                tempRes += minNumber.Name;
                findedNumbersLocations.Remove(minNumber);
                return GetNumber(findedNumbersLocations, tempRes);
            }
            else
                return string.IsNullOrEmpty(tempRes) ? -1 : int.Parse(tempRes);
        }

        private static List<FindedNumber> FindAllNumbersLocations(Mat screen, AcceptPoeResolutions poeClinetResolution)
        {
            List<FindedNumber> result = new();
            foreach (var numberImage in BotResourseLoader.Numbers[poeClinetResolution])
            {
                if (numberImage.Key == NumbersTamplates.slash)
                    continue;
                var r = BotFW.MatchTemplate(screen, numberImage.Value);

                if (r.IsFind)
                {
                    foreach (var thing in r.Res_loc)
                    {
                        var findedNumber = new FindedNumber() { X_loc = thing.Loc.X, Number = numberImage.Key, Name = numberImage.Value.Name };
                        result.Add(findedNumber);
                    }
                }    
            }

            return result;
        }


        private static RECT GetRECTForNumbersLication(ResLoc slashLoc, bool forMax)
        {
            // hp/mp/es look like:   life:   123 / 1234
            //                         current^   max^
            // forMax mean for max or current

            int AdditionalOffsetFor_X = 5; 
            int AdditionalOffsetFor_Y = -2;
            RECT result = new() { y = slashLoc.Loc.Y + AdditionalOffsetFor_Y,  w = numbersOffset.X, h = numbersOffset.Y };

            if (forMax)
                result.x = slashLoc.Loc.X + AdditionalOffsetFor_X;
            else
                result.x = slashLoc.Loc.X - numbersOffset.X;

            if (result.x < 0)
                result.x = 0;
            if (result.y < 0)
                result.y = 0;

            return result;
        }

        private static FlasksStates FindFlasksState(ScreensForNumbersFinder screens, AcceptPoeResolutions poeClientResolution)
        {
            var pixels = GetFlaskStatePixels(screens.FlasksStateArea);
            var absoluteFlasksState = GetAbsoluteFlaksStates(pixels, poeClientResolution);
            var percentFlasksState = GetPercentFlasksState(absoluteFlasksState, poeClientResolution);

            return new(percentFlasksState);
        }

        private static byte[] GetFlaskStatePixels(Mat flaskSateAreaScreen)
        {
            if (flaskSateAreaScreen.GetArray(out byte[] bytes))
                return bytes;
            else
                throw new Exception("Cannot asses to bytes!");
        }

        private static int[] GetAbsoluteFlaksStates(byte[] pixels, AcceptPoeResolutions poeClientResolution)
        {
            int flask = 0;
            var result = new int[FLASKS_COUNT];

            for (int i = 0; i < pixels.Length; i++)
            {
                int pixel = pixels[i];
                if (pixel != 0)
                {
                    if (i != 0)
                        flask = i / EachFlaskWidth[poeClientResolution];

                    result[flask]++;
                }
            }

            return result;
        }

        private static float[] GetPercentFlasksState(int[] absoluteFlasksState, AcceptPoeResolutions poeClientResolution)
        {
            var result = new float[absoluteFlasksState.Length];
            for (int i = 0; i < absoluteFlasksState.Length; i++)
            {
                double percent = absoluteFlasksState[i] / (double)EachFlaskLineWidth[poeClientResolution];
                result[i] = (float)Math.Round(percent, 2);
            }

            return result;
        }

        private static ScreensForNumbersFinder ApplyMask(ScreensForNumbersFinder screens)
        {
            Mat hpAreaMasked = new();
            Mat mpAreaMasked = new();
            Mat flaskStateAreaMasked = new();

            BotFW.GetMask(screens.HPArea, hpAreaMasked, screenNumbersMaskLower, screenNumbersMaskUpper);
            BotFW.GetMask(screens.MPArea, mpAreaMasked, screenNumbersMaskLower, screenNumbersMaskUpper);
            BotFW.GetMask(screens.FlasksStateArea, flaskStateAreaMasked, screenFlaskStateMaskLower, screenFlaskStateMaskUpper);

            return new ScreensForNumbersFinder() { HPArea = hpAreaMasked, MPArea = mpAreaMasked, FlasksStateArea = flaskStateAreaMasked };
        }


        private static ResLoc GetTopMostSlash(MT_result findedSlashes)
        {
            int minY_index = 0;
            int minY_value = int.MaxValue;
            for (int i = 0; i < findedSlashes.Res_loc.Length; i++)
            {
                if (findedSlashes.Res_loc[i].Loc.Y < minY_value)
                {
                    minY_value = findedSlashes.Res_loc[i].Loc.Y;
                    minY_index = i;
                }
            }

            return findedSlashes.Res_loc[minY_index];
        }

        private static ResLoc GetBottomMostSlash(MT_result findedSlashes)
        {
            int minY_index = 0;
            int minY_value = 0;
            for (int i = 0; i < findedSlashes.Res_loc.Length; i++)
            {
                if (findedSlashes.Res_loc[i].Loc.Y > minY_value)
                {
                    minY_value = findedSlashes.Res_loc[i].Loc.Y;
                    minY_index = i;
                }
            }

            return findedSlashes.Res_loc[minY_index];
        }
    }
}
