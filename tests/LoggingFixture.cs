using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;
using Unity.Microsoft.Logging;

namespace Microsoft.Logging.Tests
{
    [TestClass]
    public class LoggingFixture
    {
        private static IUnityContainer _container;
        private LoggedType _instance;

        [ClassInitialize]
        public static void Setup(TestContext context)
        {
        }

        [TestInitialize]
        public void TestSetup()
        {
            _container = new UnityContainer();
            _container.AddNewExtension<LoggingExtension>();
        }

        [TestMethod]
        public void microsoft_logging_default_name()
        {
            Assert.IsNotNull(_container.Configure<LoggingExtension>().LoggerFactory);

            var ss = _container.Configure<LoggingExtension>().LoggerFactory.CreateLogger<LoggedType>();
            Assert.IsInstanceOfType(_container.Configure<LoggingExtension>().LoggerFactory,
                                    typeof(ILoggerFactory));
        }

        [TestMethod]
        public void microsoft_logging_can_resolve_test_type()
        {
            _instance = _container.Resolve<LoggedType>();
            Assert.IsNotNull(_instance);
            Assert.IsNotNull(_instance.ResolvedLogger);
        }


        [TestMethod]
        public void microsoft_logging_correct_type()
        {
        }

        [TestMethod]
        public void microsoft_logging_change_name()
        {
        }


        public class LoggedType
        {
            public LoggedType(ILogger<LoggedType> log)
            {
                ResolvedLogger = log;
            }

            public ILogger<LoggedType> ResolvedLogger { get; }
        }
    }
}
