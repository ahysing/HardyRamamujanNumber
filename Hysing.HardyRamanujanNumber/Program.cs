using System;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using Hysing.HardyRamamujanNumber;

var sw = new Stopwatch();
sw.Start();
BigInteger bottom = new BigInteger(19095); // HardyRamamujanComputer.QubeRootRoundDown(BigInteger.Parse("28906284"));
BigInteger top = new BigInteger(365903); // new BigInteger(2919534755); // HardyRamamujanComputer.QubeRootRoundDown(BigInteger.Parse("24885189317885898975235988544"));
uint combinations = 5;
var results = HardyRamamujanComputer.Compute(combinations, bottom: bottom, top: top);
if (!Equals(results, Enumerable.Empty<ProductPairs>()))
{
    foreach (var result in results)
    {
        if (result != null)
        {
            Console.WriteLine(result.Product);
            foreach (var value in result.Pairs)
            {
                Console.Write(value.I);
                Console.Write("³ + ");
                Console.Write(value.J);
                Console.WriteLine("³");
            }
        }
    }
}
else
{
    Console.WriteLine("Not Found");
}

Console.WriteLine($"Elapsed {sw.ElapsedMilliseconds} ms");