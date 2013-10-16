using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using TestCase.SuperSecret;

namespace TestCase
{
    public enum TestEnum
    {
        One = 1,
        Two = 2
    }

    [TestFixture]
    public class TestCaseDemo
    {
        public const int One = 1;
        public const int Two = 1;
        public readonly StringList test;

        public TestCaseDemo()
        {
            test = new StringList(){MyStrings = new List<string>(){"2"}};
        }

        // Standard
        [TestCase(1, "1", One, TestEnum.One)]
        public void A_TestStandard(int toTest, string toTestString, int constant, TestEnum testEnum)
        {
            int myInt = int.Parse(toTestString);
            Assert.That(toTest == myInt);
            Assert.That(toTest == constant);
            Assert.That(toTest == (int)testEnum); 
        }

        // Results
        [TestCase(1, TestEnum.One, true)]
        [TestCase(2, TestEnum.Two, true)]
        public void B_TestWithBool(int toTest,  TestEnum testEnum, bool expectedResult)
        {
            Assert.That((toTest == (int)testEnum) == expectedResult);
        }

        [TestCase(1, TestEnum.One, Result=true)]
        [TestCase(2, TestEnum.Two, Result=true)]
        [TestCase(3, TestEnum.Two, Result=false)]
        public bool C_TestWithResult(int toTest, TestEnum testEnum)
        {
            return (toTest == (int)testEnum);
        }


        // Test Case with Expected Exception
        [TestCase(ExpectedException=(typeof(InvalidOperationException)))]
        public void D_WithExpectedException()
        {
            throw new InvalidOperationException("Bla!");
        }

        // TestCase with Expected Message

        [TestCase(ExpectedException=(typeof(InvalidOperationException)), ExpectedMessage="Bla!")]
        public void E_WithExpectedExceptionMessage()
        {
            throw new InvalidOperationException("Bla!");
        }


        // TestCase with Generator/TestCaseSource
        [Test, TestCaseSource("ExpectedTestData")]
        public void F_WithTestCaseSource(StringList testData)
        {
           Assert.That(testData != null); 
        }

        public static IEnumerable<StringList> ExpectedTestData()
        {
            for(int i = 0; i<3; i++)
            {
                var list = new List<string>() { Convert.ToString(i) };
                yield return new StringList(){ MyStrings = list};
            }
        }

        // More Complex?
        // http://nunit.org/?p=testCaseSource&r=2.5
        [Test, TestCaseSource("TestCases")]
        public int G_WithTestCaseSource(StringList testData, int denominator)
        {
            var myTestInt = Convert.ToInt32(testData.MyStrings[0]);
            return myTestInt / denominator;
        }

        // Use Test Case data to set up more complex input/outputs
        public static IEnumerable TestCases
        {
            get
            {
                var list0 = new List<string>() { Convert.ToString(0) };
                var list1 = new List<string>() { Convert.ToString(1) };
                var list2 = new List<string>() { Convert.ToString(2) };
                yield return new TestCaseData( new StringList() {MyStrings = list2}, 2).Returns(1);
                yield return new TestCaseData( new StringList() {MyStrings = list1}, 1).Returns(1);
                yield return new TestCaseData( new StringList() {MyStrings = list0}, 0)
                  .Throws(typeof(DivideByZeroException))
                  .SetName("DivideByZero")
                  .SetDescription("An exception is expected");
            }
        }  
        
        // Test Case with Convert.ChangeType()
        
        [TestCase("12/21/2013")]
        public void H_TestStringToDateTime(DateTime toTest)
        {
            var test = (DateTime)Convert.ChangeType("12/21/2013", typeof(DateTime));
            Assert.That(test == toTest);
        }

        [TestCase("1")]
        public void I_TestStringToInt(string toTest)
        {
            var test = (int)Convert.ChangeType("1", typeof(int));
            Assert.That(test == 1);
        }
        
        [TestCase("1")]
        public void I_TestStringToInt_B(int toTest)
        {
            Assert.That(toTest == 1);
        }

        // Why? SLIDE 1

        [TestCase(1)]
        public void J_TestEnumAndInt(int testEnum)
        {
            var test = (TestEnum)Convert.ChangeType(1, typeof(TestEnum));
            Assert.That(test == TestEnum.One);
        }

        [TestCase(1)]
        public void J_TestEnumAndIntB(TestEnum testEnum)
        {
            var test = (int)testEnum;
            Assert.That(testEnum == TestEnum.One);
        }
        
        // Why? See: SLIDE 2
        
        [Test, TestCaseSource("TestCases")]
        public int K_WithTestCaseSource_NoConversion(StringList testData, int denominator)
        {
            var myTestWithArray = (StringArray)Convert.ChangeType(testData, typeof(StringArray));

            var myTestInt = Convert.ToInt32(myTestWithArray.MyStrings[0]);
            return myTestInt / denominator;
        }

        [Test, TestCaseSource("TestCases")]
        public int K_WithTestCaseSource_WithConversion(StringArray testData, int denominator)
        {
            var myTestInt = Convert.ToInt32(testData.MyStrings[0]);
            return myTestInt / denominator;
        }

        [TestCase(ExpectedException=typeof(NotSupportedException))]
        public void L_TestConversionException()
        {
            var defaultBinder = Type.DefaultBinder;
            defaultBinder.ChangeType(test, typeof(StringArray), Thread.CurrentThread.CurrentCulture);
        }
    }
}
