using System;

namespace Area730.Notifications
{
    /// <summary>
    /// Class holds all required data for the notification
    /// </summary>
    public class Notification
    {
        #region Private

        private int     _id;
        private string  _smallIcon;
        private string  _largeIcon;
        private int     _defaults;
        private bool    _autoCancel;
        private string  _sound;
        private string  _ticker;
        private string  _title;
        private string  _body;
        private long[]  _vibratePattern;
        private long    _when;
        private long    _delay;
        private bool    _isRepeating;
        private long    _interval;
        private int     _number;
        private bool    _alertOnce;
        private string  _color;

        private string  _group;
        private string  _sortKey;

        #endregion

        #region Modifiers

        public int      ID              { get { return _id; } }
        public String   SmallIcon       { get { return _smallIcon; } }
        public String   LargeIcon       { get { return _largeIcon; } }
        public int      Defaults        { get { return _defaults; } }
        public bool     AutoCancel      { get { return _autoCancel; } }
        public String   Sound           { get { return _sound; } }
        public String   Ticker          { get { return _ticker; } }
        public String   Title           { get { return _title; } }
        public String   Body            { get { return _body; } }
        public long[]   VibratePattern  { get { return _vibratePattern; } }
        public long     When            { get { return _when; } }
        public long     Delay           { get { return _delay; } }
        public bool     IsRepeating     { get { return _isRepeating; } }
        public long     Interval        { get { return _interval; } }
        public int      Number          { get { return _number; } }
        public bool     AlertOnce       { get { return _alertOnce; } }
        public string   Color           { get { return _color; } }
        public string   Group           { get { return _group; } }
        public string   SortKey         { get { return _sortKey; } }

        #endregion


        public Notification ( 
            int id, 
            string smallIcon, 
            string largeIcon, 
            int defaults, 
            bool autoCancel, 
            string sound, 
            string ticker, 
            string title, 
            string body, 
            long [] vibroPattern, 
            long when, 
            long delay,
            bool repeating,
            long interval,
            int number,
            bool alertOnce,
            string color,
            string group, 
            string sortKey)
        {
            _id             = id;
            _smallIcon      = smallIcon;
            _largeIcon      = largeIcon;
            _defaults       = defaults;
            _autoCancel     = autoCancel;
            _sound          = sound;
            _ticker         = ticker;
            _title          = title;
            _body           = body;
            _vibratePattern = vibroPattern;
            _when           = when;
            _delay          = delay;
            _isRepeating    = repeating;
            _interval       = interval;
            _number         = number;
            _alertOnce      = alertOnce;
            _color          = color;
            _group          = group;
            _sortKey        = sortKey;
        }

        public override string ToString()
        {
            string res = "Notification: " + "\n"
                + "ID: " + _id + "\n"
                + ", number: " + _number
                + ", SmallIcon: " + _smallIcon + "\n"
                + ", LargeIcon: " + _largeIcon + "\n"
                + ", defaults: " + _defaults + "\n"
                + ", AutoCancel: " + _autoCancel + "\n"
                + ", Sound: " + _sound + "\n"
                + ", Ticker: " + _ticker + "\n"
                + ", Title: " + _title + "\n"
                + ", Body: " + _body + "\n";

                res += ", pattern: \n";
                if (_vibratePattern != null)
                {
                    foreach (long item in _vibratePattern)
                    {
                        res += "vibro: " + item + "\n";
                    }
                }
                

                res += ", When: " + _when + "\n"
                + ",delay: " + _delay + "\n"
                + ", isRepeating: " + _isRepeating + "\n"
                + ", alertOnce: " + _alertOnce + "\n"
                + ", interval: " + _interval + "\n"
                + ", color: " + _color + "\n"
                + ", group: " + _group + "\n"
                + ", sort key: " + _sortKey + "\n";

            return res;
        }


    }
}
