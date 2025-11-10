using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitTestSample01.Services;

namespace XunitTestProject
{
    public class DepartmentsServiceUnitTest
    {
        [Fact]
        public void Initiate_Instance_With_Null_DbContext_Throw_Exception()
        {
            //Assert.Throws<ArgumentNullException>(() => new DepartmentsService(null));
        }
    }
}
