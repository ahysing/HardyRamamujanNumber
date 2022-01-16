using System;
using System.Diagnostics;
using Hysing.HardyRamamujanNumber;

var sw = new Stopwatch();
sw.Start();
ulong top = 366000;
ulong bottom = 38787;
uint combinations = 5;
var result = HardyRamamujanComputer.Compute(combinations, bottom: bottom, top: top);
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
else
{
    Console.WriteLine("Not Found");
}

Console.WriteLine($"Elapsed {sw.ElapsedMilliseconds} ms");