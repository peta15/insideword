using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SparrowRanker
{
    static class MovingAverage
    {
        static private int _hours;
        static private double[] _weightArray;
        static private double[] _cumulativeWeightArray;

        static public bool Initialise(int hours)
        {
            if (hours < 1)
            {
                throw new Exception("Hours in moving average must be greater than 0");
            }

            _hours = hours;
            _weightArray = new double[_hours];
            _cumulativeWeightArray = new double[_hours];
            _weightArray[0] = 1;
            _cumulativeWeightArray[0] = 1;
            for (int i = 1; i < _hours; i++)
            {
                _weightArray[i] = (double)(_hours - i) / (double)_hours;
                _cumulativeWeightArray[i] = _cumulativeWeightArray[i - 1] * _weightArray[i];
            }

            return true;
        }

        static public bool HasIncremented(DateTime aDateTime)
        {
            return DateTime.UtcNow.Subtract(aDateTime).TotalHours > 1.0;
        }

        static public bool IsValidDate(DateTime aDateTime)
        {
            return DateTime.UtcNow.Subtract(aDateTime).TotalHours < _hours;
        }

        static public double Weight(DateTime aDateTime)
        {
            int diff = (int)(DateTime.UtcNow.Subtract(aDateTime).TotalHours);
            return (diff < _hours) ? _weightArray[diff] : 0.0;
        }

        static public double CumWeight(DateTime aDateTime)
        {
            int diff = (int)(DateTime.UtcNow.Subtract(aDateTime).TotalHours);
            return (diff < _hours) ? _cumulativeWeightArray[diff] : 0.0;
        }

        static public double CumWeight(DateTime baseDate, DateTime startDateTime, DateTime endDateTime)
        {
            int startDiff = (int)(startDateTime.Subtract(baseDate).TotalHours);
            if (startDiff < 0) startDiff = 0;
            else if (startDiff > _hours) startDiff = _hours;

            int endDiff = (int)(endDateTime.Subtract(baseDate).TotalHours);
            if (endDiff < 0) startDiff = 0;
            else if (endDiff > _hours) startDiff = _hours;

            double cumWeight = 1;
            if (startDiff == 0)
            {
                cumWeight = _cumulativeWeightArray[endDiff];
            }
            else
            {
                for (int index = startDiff; index < endDiff; index++)
                {
                    cumWeight *= _weightArray[index];
                }
            }
            return cumWeight;
        }
    }
}
