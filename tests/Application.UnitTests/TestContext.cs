using Moq;

namespace Application.UnitTests
{
    public class TestContext
    {

        public T CreateService<T>() where T : class
        {
            var m = new Mock<T>();

            return m.Object;
        }
    }
}
