namespace PoeFlasks3.BotLogic.Flasks
{
    internal interface IUsable
    {
        public void Use();
        public bool Chek(GrabedData? grabedData, FlaskKeyUsedRecentlyCheker pauseCheker);
    }
}
