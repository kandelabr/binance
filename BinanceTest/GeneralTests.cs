using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace BinanceTest
{
    [TestClass]
    public class GeneralTests
    {
        [TestMethod]
        public void TestTimeStamp()
        {
            var timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        }

        [TestMethod]
        public void GetScale()
        {
            var decimals = new string[] { "10.00", "1.00", "1", "0.1", "0.01", "0.001" };
            var decimalPlaces = new byte[] { 0, 0, 0, 1, 2, 3 };
            for (int i = 0; i < decimals.Length; i++)
            {
                double x = double.Parse(decimals[i]);
                int[] bits = decimal.GetBits((decimal)x);
                byte scale = (byte)((bits[3] >> 16) & 0x7F);


                Assert.AreEqual(decimalPlaces[i], scale, $"Expected {decimalPlaces[i]} got {scale}");
            }
        }

        [TestMethod]
        public void DecimalRound()
        {
            var decimals = new decimal[] { (decimal)10.10, (decimal)1.02, (decimal)1, (decimal)0.1, (decimal)0.11, (decimal)0.19, (decimal)0.19, (decimal)0.123, (decimal)0.1239, (decimal)0.1234 };
            var decimalPlaces = new byte[] { 1, 2, 0, 0, 2, 1, 2, 3, 3, 4 };
            var finalDecimals = new decimal[] { (decimal)10.1, (decimal)1.02, (decimal)1, (decimal)0, (decimal)0.11, (decimal)0.1, (decimal)0.19, (decimal)0.123, (decimal)0.123, (decimal)0.1234 };
            for (int i = 0; i < decimals.Length; i++)
            {
                decimal d = Math.Floor(decimals[i] * (decimal)Math.Pow(10, decimalPlaces[i])) / (decimal)Math.Pow(10, decimalPlaces[i]);
                Assert.AreEqual(finalDecimals[i], d, $"Expected {finalDecimals[i]} got {d}");
            }
        }
    }
}
