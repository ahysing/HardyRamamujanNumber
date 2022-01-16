using System.Numerics;
using Xunit;

namespace Hysing.HarduRamamujanNumber.Tests
{
    public class HardyRamamujanComputer
    {
        [Fact]
        public void Test_QubeRootRoundDown_InputIs64()
        {
            BigInteger result = HardyRamamujanNumber.HardyRamamujanComputer.QubeRootRoundDown(new BigInteger(64));
            Assert.Equal(new BigInteger(4), result);
        }
        
        [Fact]
        public void Test_QubeRootRoundDown_InputIs125()
        {
            BigInteger result = HardyRamamujanNumber.HardyRamamujanComputer.QubeRootRoundDown(new BigInteger(125));
            Assert.Equal(new BigInteger(5), result);
        }
        
        [Fact]
        public void Test_QubeRootRoundDown_InputIs29()
        {
            BigInteger result = HardyRamamujanNumber.HardyRamamujanComputer.QubeRootRoundDown(new BigInteger(29));
            Assert.Equal(new BigInteger(3), result);
        }
        
        [Fact]
        public void Test_Partition()
        {
            var result = HardyRamamujanNumber.HardyRamamujanComputer.Partition(new BigInteger(1), new BigInteger(6963472309248));
            Assert.NotEmpty(result);
        }
        
        [Fact]
        public void Test_Partition_InputIsTwoFullPartitionsWithRemains()
        {
            var result = HardyRamamujanNumber.HardyRamamujanComputer.Partition(new BigInteger(0), new BigInteger(4294967183));
            Assert.Equal(3, result.Count);
        }
        
        [Fact]
        public void Test_Partition_InputIsTwoFullPartitions()
        {
            var result = HardyRamamujanNumber.HardyRamamujanComputer.Partition(new BigInteger(0), new BigInteger(4294967182));
            Assert.Equal(2, result.Count);
        }
    }
}