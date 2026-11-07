namespace GameDateTime
{
    public interface ITimeChecker
    {
        void ClockUpdate(GameTime gameTime);
        void NewDay(GameTime gameTime);
    }
}
