using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests
{
    [TestClass]
    public class Cci : TestBase
    {

        [TestMethod]
        public void Standard()
        {

            List<CciResult> results = quotes.GetCci(20).ToList();

            // assertions

            // proper quantities
            // should always be the same number of results as there is quotes
            Assert.AreEqual(502, results.Count);
            Assert.AreEqual(483, results.Where(x => x.Cci != null).Count());

            // sample value
            CciResult r = results[501];
            Assert.AreEqual(-52.9946, Math.Round((double)r.Cci, 4));
        }

        [TestMethod]
        public void BadData()
        {
            IEnumerable<CciResult> r = Indicator.GetCci(badQuotes, 15);
            Assert.AreEqual(502, r.Count());
        }

        [TestMethod]
        public void Removed()
        {
            List<CciResult> results = quotes.GetCci(20)
                .RemoveWarmupPeriods()
                .ToList();

            // assertions
            Assert.AreEqual(502 - 19, results.Count);

            CciResult last = results.LastOrDefault();
            Assert.AreEqual(-52.9946, Math.Round((double)last.Cci, 4));
        }

        [TestMethod]
        public void Exceptions()
        {
            // bad lookback period
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                Indicator.GetCci(quotes, 0));

            // insufficient quotes
            Assert.ThrowsException<BadQuotesException>(() =>
                Indicator.GetCci(TestData.GetDefault(30), 30));
        }
    }
}
