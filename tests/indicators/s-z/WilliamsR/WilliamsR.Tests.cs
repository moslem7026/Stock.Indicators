﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests
{
    [TestClass]
    public class WilliamsR : TestBase
    {

        [TestMethod]
        public void Standard()
        {

            List<WilliamsResult> results = quotes.GetWilliamsR(14)
                .ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is quotes
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(489, results.Where(x => x.WilliamsR != null).Count());

            // sample values
            WilliamsResult r1 = results[343];
            Assert.AreEqual(-19.8211m, Math.Round((decimal)r1.WilliamsR, 4));

            WilliamsResult r2 = results[501];
            Assert.AreEqual(-52.0121m, Math.Round((decimal)r2.WilliamsR, 4));
        }

        [TestMethod]
        public void BadData()
        {
            IEnumerable<WilliamsResult> r = Indicator.GetWilliamsR(badQuotes, 20);
            Assert.AreEqual(502, r.Count());
        }

        [TestMethod]
        public void Removed()
        {
            List<WilliamsResult> results = quotes.GetWilliamsR(14)
                .RemoveWarmupPeriods()
                .ToList();

            // assertions
            Assert.AreEqual(502 - 13, results.Count);

            WilliamsResult last = results.LastOrDefault();
            Assert.AreEqual(-52.0121m, Math.Round((decimal)last.WilliamsR, 4));
        }

        [TestMethod]
        public void Exceptions()
        {
            // bad lookback period
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetWilliamsR(quotes, 0));

            // insufficient quotes
            Assert.ThrowsException<BadQuotesException>(() =>
                Indicator.GetWilliamsR(TestData.GetDefault(29), 30));
        }
    }
}
