using System;

namespace WATP
{
    /// <summary>
    /// game utc time ¿ë
    /// </summary>
    public sealed class GameUTCTime
    {
        private double gameTime = (double)((DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds / 1000);
        private double standardTime = GetStandardTime();

        public long TimeToMs { get => (long)System.Math.Floor(gameTime * 1000); }              //ms
        public int TimeToSec { get => Convert.ToInt32(gameTime); }                             //sec
        public int TimeToTomorrow
        {
            get
            {
                var tomorrow = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(TimeToMs).AddDays(1);
                var tomorrowDateTime = new DateTime(tomorrow.Year, tomorrow.Month, tomorrow.Day, 0, 0, 0);
                return (int)(tomorrowDateTime - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
            }
        }

        public void Update()
        {
            gameTime = (long)System.Math.Round((gameTime + (GetStandardTime() - standardTime)) * 1000000) / 1000000;
            standardTime = GetStandardTime();
        }

        public void SetServerTime(long serverTime)
        {
            gameTime = Convert.ToDouble(serverTime) / 1000;
            standardTime = GetStandardTime();
        }

        public void ClearTime()
        {
            gameTime = 0;
            standardTime = 0;
        }

        private static double GetStandardTime()
        {
            return UnityEngine.Time.realtimeSinceStartupAsDouble;
        }
    }
}