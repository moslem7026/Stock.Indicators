using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // AWESOME OSCILLATOR
        /// <include file='./info.xml' path='indicator/*' />
        /// 
        public static IEnumerable<AwesomeResult> GetAwesome<TQuote>(
            this IEnumerable<TQuote> quotes,
            int fastPeriods = 5,
            int slowPeriods = 34)
            where TQuote : IQuote
        {

            // sort quotes
            List<TQuote> quotesList = quotes.Sort();

            // check parameter arguments
            ValidateAwesome(quotes, fastPeriods, slowPeriods);

            // initialize
            int size = quotesList.Count;
            List<AwesomeResult> results = new();
            double[] pr = new double[size]; // median price

            // roll through quotes
            for (int i = 0; i < size; i++)
            {
                TQuote q = quotesList[i];
                pr[i] = (double)(q.High + q.Low) / 2;
                int index = i + 1;

                AwesomeResult r = new()
                {
                    Date = q.Date
                };

                if (index >= slowPeriods)
                {
                    double sumSlow = 0;
                    double sumFast = 0;

                    for (int p = index - slowPeriods; p < index; p++)
                    {
                        sumSlow += pr[p];

                        if (p >= index - fastPeriods)
                        {
                            sumFast += pr[p];
                        }
                    }

                    r.Oscillator = (sumFast / fastPeriods) - (sumSlow / slowPeriods);
                    r.Normalized = (pr[i] != 0) ? 100 * r.Oscillator / pr[i] : null;
                }

                results.Add(r);
            }

            return results;
        }


        // remove recommended periods
        /// <include file='../../_common/Results/info.xml' path='info/type[@name="Prune"]/*' />
        ///
        public static IEnumerable<AwesomeResult> RemoveWarmupPeriods(
            this IEnumerable<AwesomeResult> results)
        {
            int removePeriods = results
                .ToList()
                .FindIndex(x => x.Oscillator != null);

            return results.Remove(removePeriods);
        }


        // parameter validation
        private static void ValidateAwesome<TQuote>(
            IEnumerable<TQuote> quotes,
            int fastPeriods,
            int slowPeriods)
            where TQuote : IQuote
        {

            // check parameter arguments
            if (fastPeriods <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(slowPeriods), slowPeriods,
                    "Fast periods must be greater than 0 for Awesome Oscillator.");
            }

            if (slowPeriods <= fastPeriods)
            {
                throw new ArgumentOutOfRangeException(nameof(slowPeriods), slowPeriods,
                    "Slow periods must be larger than Fast Periods for Awesome Oscillator.");
            }

            // check quotes
            int qtyHistory = quotes.Count();
            int minHistory = slowPeriods;
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient quotes provided for Awesome Oscillator.  " +
                    string.Format(EnglishCulture,
                    "You provided {0} periods of quotes when at least {1} are required.",
                    qtyHistory, minHistory);

                throw new BadQuotesException(nameof(quotes), message);
            }
        }
    }
}
