using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Threading;

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

        private const ulong _cubeRootUlongMax = 2642245;
        private static Tuple<BigInteger[], ulong> ComputeAllCubes(BigInteger bottom, BigInteger top)
        {
            ulong bottomUl = (ulong)bottom;
            BigInteger delta = top - bottom;
            ulong size = (ulong)delta;
            
            var cubes = new BigInteger[size];
            if (top < _cubeRootUlongMax)
            {
                for (ulong i = 0; i < size; i++)
                {
                    ulong factor = bottomUl + i;
                    cubes[i] = factor * factor * factor;
                }
            }
            else if (bottom <= _cubeRootUlongMax)
            {
                ulong bottomIt = _cubeRootUlongMax - (ulong)bottom;
                for (ulong i = 0; i <= bottomIt; i++)
                {
                    ulong factor = bottomUl + i;
                    cubes[i] = factor * factor * factor;
                }

                for (ulong i = bottomIt + 1; i < size; i++)
                {
                    BigInteger factor = bottom + new BigInteger(i);
                    cubes[i] = factor * factor * factor;
                }
            }
            else
            {
                for (ulong i = 0; i < size; i++)
                {
                    BigInteger factor = bottom + new BigInteger(i);
                    cubes[i] = factor * factor * factor;
                }
            }

            return Tuple.Create(cubes, size);
        }
        
        private static void RemoveToosSmallProducts(ref SortedDictionary<BigInteger, List<Pair>> sums, BigInteger[] cubes, BigInteger bottom, ulong i)
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
        }

        static readonly BigInteger Portion = new BigInteger(10_000);
        public static List<ValueTuple<BigInteger, BigInteger>> Partition(BigInteger bottom, BigInteger top)
        {
            
            List<ValueTuple<BigInteger, BigInteger>> partitions = new List<ValueTuple<BigInteger, BigInteger>>();
            BigInteger delta = top - bottom;
            // The array size is limited to a total of 4 billion elements, and to a maximum index of 0X7FEFFFFF in any
            // given dimension (0X7FFFFFC7 for byte arrays and arrays of single-byte structures).
            //
            // https://docs.microsoft.com/en-us/dotnet/api/system.array?view=net-6.0
            BigInteger next = bottom;
            while (delta > Portion)
            {
                BigInteger nextUpper = next + Portion;
                partitions.Add( ValueTuple.Create(next, nextUpper));
                next += Portion;
                delta -= Portion;
            }

            if (delta > BigInteger.Zero)
            {
                BigInteger nextUpper = next + delta;
                partitions.Add(ValueTuple.Create(next, nextUpper));    
            }
            
            return partitions;
        }

        private static ProductPairs? ComputeSubset(uint combinations, BigInteger bottom, BigInteger top)
        {
            ulong i = 0;
            ulong j = 0;
            
            SetLowerBoundOfThisThread(bottom);

            var cubesWithSize = ComputeAllCubes(bottom, top);
            BigInteger[] cubes = cubesWithSize.Item1;
            ulong cubesSize = cubesWithSize.Item2;
            while (j < cubesSize && i < cubesSize)
            {
                ulong nextI;
                ulong nextJ;
                if (i == j)
                {
                    SetLowerBoundOfThisThread(new BigInteger(i) + bottom);
                    
                    var globalBottom = LowerBounds.Values.Min();
                    BigInteger smallestQubeAlongDiagonal = new BigInteger(2) * globalBottom * globalBottom * globalBottom;

                    bool remove = true;
                    lock (Sums)
                    {
                        while (remove)
                        {
                            var smallSums = Sums.GetEnumerator();
                            remove = smallSums.MoveNext();
                            if (remove)
                            {
                                var key = smallSums.Current.Key;
                                smallSums.Dispose();
                                remove = key < smallestQubeAlongDiagonal;
                                if (remove)
                                {
                                    Sums.Remove(key);
                                }
                            }
                        }
                    }

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
                    lock (Sums)
                    {
                        if (Sums.ContainsKey(sum))
                        {
                            var pairs = Sums[sum];
                            pairs.Add(pair);
                            if (pairs.Count == combinations)
                            {
                                RemoveLowerBoundOfThread();
                                return new ProductPairs(sum, pairs);
                            }
                            Sums[sum] = pairs;
                        }
                        else
                        {
                            var pairs = new List<Pair> { pair };
                            if (1 == combinations)
                            {
                                RemoveLowerBoundOfThread();
                                return new ProductPairs(sum, pairs);
                            }
                            Sums[sum] = pairs;
                        }
                    }

                    i++;
                    j--;
                }

                i = nextI;
                j = nextJ;
            }

            RemoveLowerBoundOfThread();
            return new ProductPairsNotFound();
        }

        private static void RemoveLowerBoundOfThread()
        {
            var myThread = Thread.CurrentThread;
            bool wasAdded;
            int attempts = 0;
            do
            {
                wasAdded = LowerBounds.TryRemove(myThread, out BigInteger _);
                attempts++;
            } while (!wasAdded && attempts < 100);
        }

        private static void SetLowerBoundOfThisThread(BigInteger i)
        {
            var myThread = Thread.CurrentThread;
            bool wasAdded;
            int attempts = 0;
            do
            {
                wasAdded = LowerBounds.TryAdd(myThread, i);
                attempts++;
            } while (!wasAdded && attempts < 100);
        }

        private static readonly ConcurrentDictionary<Thread, BigInteger> LowerBounds =
            new ConcurrentDictionary<Thread, BigInteger>();
        private static readonly SortedDictionary<BigInteger, List<Pair>> Sums = new SortedDictionary<BigInteger, List<Pair>>();
        public static ParallelQuery<ProductPairs> Compute(uint combinations, BigInteger bottom, BigInteger top)
        {
            lock (Sums)
            {
                Sums.Clear();
            }
            
            var partitions = Partition(bottom, top);
            var results = from partition in partitions.AsParallel()
                    .WithDegreeOfParallelism(Environment.ProcessorCount)
                select ComputeSubset(combinations, partition.Item1, partition.Item2);
            return results.Where(r => r.GetType() != typeof(ProductPairsNotFound));
        }
    }

    internal class ProductPairsNotFound : ProductPairs
    {
        public ProductPairsNotFound() : base(BigInteger.Zero, new List<Pair>())
        {
        }
    }
}