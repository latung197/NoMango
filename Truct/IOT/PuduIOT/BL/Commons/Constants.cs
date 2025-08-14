namespace PuduIOT.BL.Commons
{
    public static class Constants
    {
        public const string CONDITIONER_TYPE = "1";

        public const string ELECTRIC_CABINET_TYPE = "3";

        public const string COMPRESSOR_TYPE = "2";

        public const string CONDITIONER_CONSUMPTION = "trn_air_conditioner_consumption";

        public const string COMPRESSOR_CONSUMPTION = "trn_air_compressor_consumption";

        public const string ELECTRIC_CABINET_CONSUMPTION = "trn_electric_cabinet_consumption";

        public const string CONDITIONER_REAL_TIME = "trn_air_conditioner_real_time";

        public const string COMPRESSOR_REAL_TIME = "trn_air_compressor_real_time";

        public const string ELECTRIC_CABINET_REAL_TIME = "trn_electric_cabinet_real_time";

        public const string DASHBOARD = "Dashboard";

        public const string MINUTE_FORMAT = "HH24:MI";

        public const string HOUR_FORMAT = "DD HH24:00";

        public const string DAY_FORMAT = "YYYY/MM/DD";

        public const string MONTH_FORMAT = "YYYY/MM";

        public const string YEAR_FORMAT = "YYYY";

        public const string JP_CULTURE = "ja-JP";

        public const string VN_CULTURE = "vi-VN";

        public const string ELECTRIC_COST = "ElectricCost";

        public enum Dashboard
        {
            Line = 0,
            Pac = 1,
            Air = 2,

        }

    }
}
