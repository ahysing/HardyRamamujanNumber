using System.Collections.Generic;
using System.Numerics;

namespace Hysing.HardyRamamujanNumber
{
    public class ProductPairs
    {
        public ProductPairs(BigInteger product, List<Pair> pairs)
        {
            Product = product;
            Pairs = pairs;
        }

        public BigInteger Product { get; }

        public List<Pair> Pairs { get; }
    }
}