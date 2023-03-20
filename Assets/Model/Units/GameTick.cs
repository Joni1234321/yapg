namespace Bserg.Model.Units
{
    public static class GameTick
    {
        public const int  TICKS_PER_MONTH = 30, TICKS_PER_YEAR = TICKS_PER_MONTH * 12, TICKS_PER_QUARTER = TICKS_PER_MONTH * 3;

        public static int ToTick(Time time)
        {
            return (int)(time.To(Time.UnitType.Years) * TICKS_PER_YEAR);
        }

        public static float ToTickF(Time time) => (float)time.To(Time.UnitType.Years) * TICKS_PER_YEAR;
        public static Time ToTime(float tickF) => new Time(tickF / TICKS_PER_YEAR, Time.UnitType.Years);
        
        public static float TickAtNextEventF(int ticks, float tickPeriod, float tickOffset = 0)
        {
            if (tickPeriod == 0) 
                return ticks + 1;

            int eventNTimesBefore = (int)((ticks + 1 - tickOffset) / tickPeriod);
            float tickAtLastEvent =  eventNTimesBefore * tickPeriod + tickOffset;
            return tickPeriod + tickAtLastEvent;
        }

    }
}