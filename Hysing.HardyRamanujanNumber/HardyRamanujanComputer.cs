using System;
using System.Collections.Generic;
using System.Numerics;

namespace Hysing.HardyRamamujanNumber
{
    public class HardyRamamujanComputer
    {
        public static BigInteger QubeRootRoundDown(BigInteger number)
        {
            BigInteger iterator = number / 2;
            while (iterator * iterator * iterator > number)
            {
                iterator = iterator / 2;
            }

            BigInteger itPlusOne = iterator + BigInteger.One;
            while (itPlusOne * itPlusOne * itPlusOne <= number)
            {
                iterator ++;
                itPlusOne = iterator + BigInteger.One;
            }
            
            return iterator;
        }
        
        private static Tuple<BigInteger[], ulong> ComputeAllCubes(BigInteger bottom, BigInteger top)
        {
            BigInteger delta = top - bottom;
            ulong size = (ulong)delta;
            var cubes = new BigInteger[size];
            for (ulong i = 0; i < size; i++)
            {
                BigInteger factor = bottom + new BigInteger(i);
                cubes[i] = factor * factor * factor;
            }
 
            return Tuple.Create(cubes, size);
        }
        
        private static SortedDictionary<BigInteger, List<Pair>> RemoveToosSmallProducts(SortedDictionary<BigInteger, List<Pair>> sums, BigInteger[] cubes, BigInteger bottom, ulong i)
        {
            bool remove = true;
            while (remove)
            {
                BigInteger smallestQubeAlongDiagonal = cubes[i] + cubes[i];
                var smallSums = sums.GetEnumerator();
                remove = smallSums.MoveNext();
                if (remove)
                {
                    var key = smallSums.Current.Key;
                    smallSums.Dispose();
                    remove = key < smallestQubeAlongDiagonal;
                    if (remove)
                    {
                        sums.Remove(key);
                    }
                }
            }
            
            return sums;
        }

        public static List<System.ValueTuple<BigInteger, BigInteger>> Partition(BigInteger bottom, BigInteger top)
        {
            
            List<System.ValueTuple<BigInteger, BigInteger>> partitions = new List<ValueTuple<BigInteger, BigInteger>>();
            BigInteger delta = top - bottom;
            // The array size is limited to a total of 4 billion elements, and to a maximum index of 0X7FEFFFFF in any
            // given dimension (0X7FFFFFC7 for byte arrays and arrays of single-byte structures).
            //
            // https://docs.microsoft.com/en-us/dotnet/api/system.array?view=net-6.0
            BigInteger next = bottom;
            while (delta > 0X7FFFFFC7)
            {
                BigInteger nextUpper = next + new BigInteger(0X7FFFFFC7);
                partitions.Add( ValueTuple.Create(next, nextUpper));
                next += 0X7FFFFFC7;
                delta -= 0X7FFFFFC7;
            }

            if (delta > BigInteger.Zero)
            {
                BigInteger nextUpper = next + delta;
                partitions.Add(ValueTuple.Create(next, nextUpper));    
            }
            
            return partitions;
        }

        private static ProductPairs? ComputeSubset(SortedDictionary<BigInteger, List<Pair>> sums, uint combinations, BigInteger bottom, BigInteger top)
        {
            ulong i = 0;
            ulong j = 0;
            var cubesWithSize = ComputeAllCubes(bottom, top);
            BigInteger[] cubes = cubesWithSize.Item1;
            ulong cubesSize = cubesWithSize.Item2;
            while (j < cubesSize && i < cubesSize)
            {
                ulong nextI;
                ulong nextJ;
                if (i == j)
                {
                    sums = RemoveToosSmallProducts(sums, cubes, bottom, i);
                    nextI = i + 1;
                    nextJ = j;
                }
                else
                {
                    nextI = i;
                    nextJ = j + 1;
                }
                
                while (i < cubesSize && j != ulong.MaxValue)
                {
                    BigInteger cubeOne = cubes[i];
                    BigInteger cubeTwo = cubes[j];
                    BigInteger sum = cubeOne + cubeTwo;
                    
                    BigInteger factorOne = new BigInteger(i) + bottom;
                    BigInteger factorTwo = new BigInteger(j) + bottom;
                    var pair = new Pair { I = factorOne, J = factorTwo };
                    if (sums.ContainsKey(sum))
                    {
                        var pairs = sums[sum];
                        pairs.Add(pair);
                        if (pairs.Count == combinations)
                        {
                            return new ProductPairs(sum, pairs);
                        }
                        sums[sum] = pairs;
                    }
                    else
                    {
                        var pairs = new List<Pair> { pair };
                        if (1 == combinations)
                        {
                            return new ProductPairs(sum, pairs);
                        }
                        sums[sum] = pairs;
                    }

                    i++;
                    j--;
                }

                i = nextI;
                j = nextJ;
            }

            return null;
        }

        public static ProductPairs? Compute(uint combinations, ulong bottom = 1, ulong top = 87539319)
        {
            var sums = new SortedDictionary<BigInteger, List<Pair>>();
            var partitions = Partition(bottom, top);
            foreach (var partition in partitions)
            {
                var result = ComputeSubset(sums, combinations, partition.Item1, partition.Item2);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }
    }
}