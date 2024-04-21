using BotFW_CvSharp_01;
using PoeFlasks3.SettingsThings;
using System.ComponentModel.DataAnnotations;

namespace PoeFlasks3.BotLogic
{
    public struct CurMax
    {
        public int Current;
        public int Max;
        public readonly float Precent { get => (float)Current / Max; }

        public override string ToString()
        {
            return $"{Current} / {Max}";
        }
    }

    public struct MaxValueBuffer
    {
        public int Max { get => GetMax(); set => SetMax(value); }
        private int _max;
        private Dictionary<int, int> Buffer;

        public void Reset()
        {
            Buffer?.Clear();
        }

        private int GetMax()
        {
            if (TryGetMaxCountValue(out int value))
                return value;
            else
                return _max;
        }

        private void SetMax(int value)
        {
            if (Buffer == null)
                Buffer = new();

            if (Buffer.ContainsKey(value))
            {
                Buffer[value]++;
                //Log.Write($"Value: {value}, Count: {Buffer[value]}");
            }
            else
                Buffer.Add(value, 0);

            _max = value;
        }

        private bool TryGetMaxCountValue(out int value)
        {
            if (Buffer == null)
                Buffer = new();

            int maxCount = 0;
            value = 0;
            foreach (var ValueCountPair in Buffer)
            {
                if (ValueCountPair.Value > maxCount)
                {
                    maxCount = ValueCountPair.Value;
                    value = ValueCountPair.Key;
                }
            }
            if (maxCount > 0)
            {
                //Log.Write($"Value: {value}, Count: {Buffer[value]}");
                return true;
            }

            return false;
        }
    }

    public struct FlasksStates
    {
        public Dictionary<FlaskSlot, float> States;

        public FlasksStates(float[] percentStates)
        {
            if (percentStates.Length != 5)
                throw new Exception("Lenght of percentStates must be 5!");

            States = new Dictionary<FlaskSlot, float> 
            {
                { FlaskSlot.Slot1, percentStates[0] },
                { FlaskSlot.Slot2, percentStates[1] },
                { FlaskSlot.Slot3, percentStates[2] },
                { FlaskSlot.Slot4, percentStates[3] },
                { FlaskSlot.Slot5, percentStates[4] },
            };
        }

        public override string ToString()
        {
            return $"{States[FlaskSlot.Slot1]}, {States[FlaskSlot.Slot2]}, {States[FlaskSlot.Slot3]}, {States[FlaskSlot.Slot4]}, {States[FlaskSlot.Slot5]},";
        }
    }

    public struct GrabedData
    {
        public CurMax HP;
        public CurMax MP;
        public CurMax ES;

        public FlasksStates FlasksState;

        public FindFlags FindedFlags;

        private static GrabedDataMaxValuesBuffer MaxNumbersBuffer = new();


        public static void ResetMaxNumbers()
        {
            MaxNumbersBuffer.Reset();
        }

        public void UpdateMaxNumbers()
        {
            MaxNumbersBuffer.Update(this);

            HP.Max = MaxNumbersBuffer.MaxHP.Max;
            MP.Max = MaxNumbersBuffer.MaxMP.Max;
            ES.Max = MaxNumbersBuffer.MaxES.Max;
        }


        public override string ToString()
        {
            string hpString = FindedFlags.HP_isFind ? $"hp: {HP}," : "hp: N/A,";
            string mpString = FindedFlags.MP_isFind ? $"mp: {MP}," : "mp: N/A,";
            string esString = FindedFlags.ES_isFind ? $"es: {ES}," : "es: N/A,";

            return $"{hpString} {mpString} {esString} states: {FlasksState}";
        }
    }

    public struct GrabedDataMaxValuesBuffer
    {
        public MaxValueBuffer MaxHP;
        public MaxValueBuffer MaxMP;
        public MaxValueBuffer MaxES;

        public void Update(GrabedData? data)
        {
            if (data != null)
            {
                if (data.Value.FindedFlags.HP_isFind)
                    MaxHP.Max = data.Value.HP.Max;
                if (data.Value.FindedFlags.MP_isFind)
                    MaxMP.Max = data.Value.MP.Max;
                if (data.Value.FindedFlags.ES_isFind)
                    MaxES.Max = data.Value.ES.Max;
            }
        }

        public void Reset()
        {
            MaxHP.Reset();
            MaxMP.Reset();
            MaxES.Reset();
        }
    }

    public struct FindFlags
    {
        public bool HP_isFind;
        public bool MP_isFind;
        public bool ES_isFind;

        public readonly bool All_isFind { get => HP_isFind && MP_isFind && ES_isFind; }
        public readonly bool Any_isFind { get => HP_isFind || MP_isFind || ES_isFind; }
    }
}
