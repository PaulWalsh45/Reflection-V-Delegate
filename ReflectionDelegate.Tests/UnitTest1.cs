using System.Collections.Generic;
using System.Dynamic;
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
            var differences = object1.Compare<TestClass1>(object2,1000000);

            
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
            var differences = object1.CompareUsingDelegate<TestClass1>(object2,1000000);


            //Assert
            Assert.AreEqual(13000000, differences.Count);
        }

        [Test]
        public void TestUsingCompare_IgnoreProperties()
        {
            //Arrange
            var ignoreProps = new List<string>
            {
                "Property1",
                "Property2",
                "Property3",
                "Property4",
                "Property5",
                "Property6",
                "Property7",
                "Property8",
                "Property9",
                "Property10",

            };

            var expectedDiffs = new List<string>
            {
                "TestClass1 difference - 'FirstName : Joe' != FirstName :'Mary'",
                "TestClass1 difference - 'LastName : Bloggs' != LastName :'Black'",
                "TestClass1 difference - 'Age : 33' != Age :'25'"
            };

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
            var differences = object1.CompareUsingDelegate<TestClass1>(object2,1, ignoreProps);


            //Assert
            Assert.AreEqual(3, differences.Count);
            Assert.AreEqual(expectedDiffs[0],differences[0]);
            Assert.AreEqual(expectedDiffs[1], differences[1]);
            Assert.AreEqual(expectedDiffs[2], differences[2]);
        }

        [Test]
        public void TestUsingCompare_PropertiesContainObjects()
        {
            //Arrange
            var object1 = new TestClass2()
            {
                FirstName = "Joe",
                SubClass = new TestSubClass
                {
                    Prop1 = "prop1",
                    Prop2 = "prop2",
                    Prop3 = 3

                },
                AnotherSubClass = new AnotherSubClass
                {
                    Prop11 = "prop11",
                    Prop12 = "prop12",
                    Prop13 = 13
                },
                LastName = "Bloggs"
            
            };
            
            var object2 = new TestClass2()
            {
                FirstName = "Mary",
                SubClass = new TestSubClass
                {
                    Prop1 = "prop11",
                    Prop2 = "prop12",
                    Prop3 = 13
                },
                AnotherSubClass = new AnotherSubClass
                {
                    Prop11 = "prop111",
                    Prop12 = "prop112",
                    Prop13 = 113
                },
                LastName = "Black"
               
            };

            //Act
            var differences = object1.CompareUsingDelegateSH(object2);


            //Assert
            Assert.AreEqual(8, differences.Count);
            
        }

        [Test]
        public void TestUsingCompare_PropertiesContainListedObjects()
        {
            //Arrange
            var object1 = new TestClass3()
            {
                FirstName = "Joe",
                SubClassList = new List<TestSubClass>()
                {
                    new TestSubClass
                    {
                        Prop1 = "prop1",
                        Prop2 = "prop2",
                        Prop3 = 3
                    },
                    new TestSubClass
                    {
                        Prop1 = "prop11",
                        Prop2 = "prop12",
                        Prop3 = 13
                    }
                }
             
            };

            var object2 = new TestClass3()
            {
                FirstName = "Mary",
                SubClassList = new List<TestSubClass>()
                {
                    new TestSubClass
                    {
                        Prop1 = "prop1x",
                        Prop2 = "prop12x",
                        Prop3 = 33
                    },
                    new TestSubClass
                    {
                        Prop1 = "prop11x",
                        Prop2 = "prop12x",
                        Prop3 = 133
                    }
                }

            };

            //Act
            //get object differences ignoring known properties of type List
            var differences = object1.CompareUsingDelegateSH1(object2,new List<string>{"SubClassList"});

            // iterate through the ignored properties above and call the compare for each
            for (int i = 0; i < object1.SubClassList.Count; i++)
            {
                differences.AddRange(object1.SubClassList[i].CompareUsingDelegateSH1(object2.SubClassList[i]));
            }
            
            //Assert
            Assert.AreEqual(7, differences.Count);

        }

        [Test]
        public void TestUsingCompare_PropertiesContainNestedListedObjects()
        {
            //Arrange
            var object1 = new TestClass4()
            {
                TestClass4Name = "Joe",
                TestClass4Subs = new List<TestClass4Sub>
                {
                    new TestClass4Sub
                    {
                        TestClass4SubName = "Mick",
                        TestClass4NestedSubs = new List<TestClass4NestedSub>
                        {
                            new TestClass4NestedSub
                            {
                                TestClass4NestedSubName = "Billy"
                            }
                        }
                    },
                    new TestClass4Sub
                    {
                        TestClass4SubName = "Paddy",
                        TestClass4NestedSubs = new List<TestClass4NestedSub>
                        {
                            new TestClass4NestedSub
                            {
                                TestClass4NestedSubName = "Tom"
                            }
                        }
                    }
                }

            };

            var object2 = new TestClass4()
            {
                TestClass4Name = "Sarah",
                TestClass4Subs = new List<TestClass4Sub>
                {
                    new TestClass4Sub
                    {
                        TestClass4SubName = "Ann",
                        TestClass4NestedSubs = new List<TestClass4NestedSub>
                        {
                            new TestClass4NestedSub
                            {
                                TestClass4NestedSubName = "Patricia"
                            }
                        }
                    },
                    new TestClass4Sub
                    {
                        TestClass4SubName = "Michele",
                        TestClass4NestedSubs = new List<TestClass4NestedSub>
                        {
                            new TestClass4NestedSub
                            {
                                TestClass4NestedSubName = "Lorraine"
                            }
                        }
                    }
                }

            };

            //Act
            //get object differences ignoring known properties of type List
            var differences = object1.CompareUsingDelegateSH1(object2);

            

            //Assert
            Assert.AreEqual(7, differences.Count);

        }

    }
}