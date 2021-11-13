using NUnit.Framework;


namespace ReflectionDelegate.Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestUsingCompare_Reflection()
        {
            //Arrange
            var object1 = new TestClass1()
            {
                FirstName = "Joe",
                LastName = "Bloggs",
                Age = 33,
                Property1 = 1,
                Property2 = 2,
                Property3 = 3,
                Property4 = 4,
                Property5 = 5,
                Property6 = 6,
                Property7 = 7,
                Property8 = 8,
                Property9 = 9,
                Property10 = 10
            };

            var object2 = new TestClass1()
            {
                FirstName = "Mary",
                LastName = "Black",
                Age = 25,
                Property1 = 11,
                Property2 = 21,
                Property3 = 31,
                Property4 = 41,
                Property5 = 51,
                Property6 = 61,
                Property7 = 71,
                Property8 = 81,
                Property9 = 91,
                Property10 = 101
            };

            //Act
            var differences = object1.Compare<TestClass1>(object2);

            
            //Assert
            Assert.AreEqual(13000000,differences.Count);
        }

        [Test]
        public void TestUsingCompare_Delegate()
        {
            //Arrange
            var object1 = new TestClass1()
            {
                FirstName = "Joe",
                LastName = "Bloggs",
                Age = 33,
                Property1 = 1,
                Property2 = 2,
                Property3 = 3,
                Property4 = 4,
                Property5 = 5,
                Property6 = 6,
                Property7 = 7,
                Property8 = 8,
                Property9 = 9,
                Property10 = 10
            };

            var object2 = new TestClass1()
            {
                FirstName = "Mary",
                LastName = "Black",
                Age = 25,
                Property1 = 11,
                Property2 = 21,
                Property3 = 31,
                Property4 = 41,
                Property5 = 51,
                Property6 = 61,
                Property7 = 71,
                Property8 = 81,
                Property9 = 91,
                Property10 = 101
            };

            //Act
            var differences = object1.CompareUsingDelegate<TestClass1>(object2);


            //Assert
            Assert.AreEqual(13000000, differences.Count);
        }

    }
}