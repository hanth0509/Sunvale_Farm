using UnityEngine;

namespace GameDateTime
{
    public enum Season
    {
        Spring,
        Summer,
        Fall,
        Winter
    }

    [System.Serializable]
    public class GameTime
    {
        public const int MAX_DAY = 3;
        public const int MAX_HOUR = 24;
        public const int MAX_MINUTE = 60;

        [Tooltip("Current season of the game time")]
        [SerializeField] private Season _currentSeason;
        [SerializeField] private int _year;
        [SerializeField] private int _day;
        [SerializeField] private int _hour;
        [SerializeField] private int _minute;

        public Season CurrentSeason
        {
            get { return _currentSeason; }
        }
        public int Year
        {
            get { return _year; }
        }
        public int Day
        {
            get { return _day; }
        }
        public int Hour
        {
            get { return _hour; }
        }
        public int Minute
        {
            get { return _minute; }
        }

        public GameTime(int year, Season season, int day, int hour, int minute)
        {
            _year = year;
            _currentSeason = season;
            _day = day; 
            _hour = hour;
            _minute = minute;
        }

        public void Reset()
        {
            // Still in same day
            if(_hour >= 6 && _hour <= 24)
            {
                _day++;
            }

            _hour = 5;
            _minute = 59;
        }

        public void IncreaseTime()
        {
            _minute++;
            if(_minute >= MAX_MINUTE)
            {
                _minute = 0;
                _hour++;
            }

            if(_hour >= MAX_HOUR)
            {
                _hour = 0;
                _day++;
            }

            if(_day > MAX_DAY)
            {
                _day = 1;

                if(_currentSeason == Season.Winter)
                {
                    _year++;
                    _currentSeason = Season.Spring;
                }
                else
                {
                    _currentSeason++;
                }
            }
        }

        public string TimeString()
        {
            // Từ 12 giờ trưa trở đi dùng PM.
            string period = _hour < 12 ? "AM" : "PM";

            // Đổi giờ 24h sang 12h: 0 giờ và 12 giờ đều hiển thị là 12.
            int displayHour = _hour % 12;
            if (displayHour == 0)
            {
                displayHour = 12;
            }

            return $"{displayHour:00}:{_minute:00} {period}";
        }

        public string Preview()
        {
            return "Year " + _year + " Day" + _day + " " + _currentSeason.ToString() + " " + TimeString();
        }

        #region Convertion
        public static int HoursToMinutes(int hours)
        {
            return hours * MAX_MINUTE;
        }
        public static int DayToHours(int days)
        {
            return days * MAX_HOUR;
        }
        public static int SeasonsToDays(Season season)
        {
            int s = (int)season;
            return s * MAX_DAY;
        }
        #endregion
    }
}
