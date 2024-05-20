using System;
using System.Collections.Generic;

namespace congestion.calculator
{
    public class CongestionTaxCalculator
    {
        private readonly List<TollFeeRule> _tollFeeRules;

        public CongestionTaxCalculator(string configFilePath)
        {
            var config = TollFeeConfig.LoadFromFile(configFilePath);
            _tollFeeRules = config.TollFees;
        }

        private const int MaxDailyFee = 60;

        public int GetTax(IVehicle vehicle, DateTime[] dates)
        {
            if (dates.Length == 0 || IsTollFreeVehicle(vehicle))
            {
                return 0;
            }

            Array.Sort(dates);

            DateTime intervalStart = dates[0];
            int totalFee = 0;
            int highestFeeInInterval = GetTollFee(intervalStart, vehicle);

            for (int i = 1; i < dates.Length; i++)
            {
                int nextFee = GetTollFee(dates[i], vehicle);
                TimeSpan diff = dates[i] - intervalStart;

                if (diff.TotalMinutes <= 60)
                {
                    highestFeeInInterval = Math.Max(highestFeeInInterval, nextFee);
                }
                else
                {
                    totalFee += highestFeeInInterval;
                    highestFeeInInterval = nextFee;
                    intervalStart = dates[i];
                }
            }

            totalFee += highestFeeInInterval;

            return Math.Min(totalFee, MaxDailyFee);
        }

        private bool IsTollFreeVehicle(IVehicle vehicle)
        {
            if (vehicle == null) return false;

            string vehicleType = vehicle.GetVehicleType();
            return vehicleType switch
            {
                "Motorbike" => true,
                "Tractor" => true,
                "Emergency" => true,
                "Diplomat" => true,
                "Foreign" => true,
                "Military" => true,
                _ => false,
            };
        }

        public int GetTollFee(DateTime date, IVehicle vehicle)
        {
            if (IsTollFreeDate(date) || IsTollFreeVehicle(vehicle))
            {
                return 0;
            }

            var timeOfDay = date.TimeOfDay;

            foreach (var rule in _tollFeeRules)
            {
                if (timeOfDay >= rule.StartTimeSpan && timeOfDay <= rule.EndTimeSpan)
                {
                    return rule.Fee;
                }
            }

            return 0;
        }

        private bool IsTollFreeDate(DateTime date)
        {
            int year = date.Year;
            int month = date.Month;
            int day = date.Day;

            if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
            {
                return true;
            }

            if (year == 2013)
            {
                return month switch
                {
                    1 when day == 1 => true,
                    3 when day == 28 || day == 29 => true,
                    4 when day == 1 || day == 30 => true,
                    5 when day == 1 || day == 8 || day == 9 => true,
                    6 when day == 5 || day == 6 || day == 21 => true,
                    7 => true,
                    11 when day == 1 => true,
                    12 when day == 24 || day == 25 || day == 26 || day == 31 => true,
                    _ => false,
                };
            }

            return false;
        }
    }

}
