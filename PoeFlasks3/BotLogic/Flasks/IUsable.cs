namespace PoeFlasks3.BotLogic.Flasks
{
    internal interface IUsable
    {
        public void UseAsync();
        public bool Chek(GrabedData? grabedData, FlaskKeyUsedRecentlyCheker pauseCheker);
    }
}
