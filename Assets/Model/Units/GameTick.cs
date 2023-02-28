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

        public static Time ToTime(int tick)
        {
            return ToTime((float)tick);
        }

        public static Time ToTime(float tickF)
        {
            return new Time((float)tickF / TICKS_PER_YEAR, Time.UnitType.Years);
        }
        

    }
}