using PoeFlasks3.SettingsThings;

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

        public override string ToString()
        {
            string hpString = FindedFlags.HP_isFind ? $"hp: {HP}," : "hp: N/A,";
            string mpString = FindedFlags.MP_isFind ? $"mp: {MP}," : "mp: N/A,";
            string esString = FindedFlags.ES_isFind ? $"es: {ES}," : "es: N/A,";

            return $"{hpString} {mpString} {esString} states: {FlasksState}";
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
