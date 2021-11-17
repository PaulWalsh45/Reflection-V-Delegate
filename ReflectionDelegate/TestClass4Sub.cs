using System;
using System.Collections.Generic;
using System.Text;

namespace ReflectionDelegate
{
    public class TestClass4Sub
    {
        [CompareIgnore]
        public Guid Id { get; set; }

        public String TestClass4SubName { get; set; }

        public List<TestClass4NestedSub> TestClass4NestedSubs { get; set; }
    }
}
