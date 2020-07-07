using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Unity;
using Unity.Exceptions;
using Unity.Microsoft.Logging;

namespace Microsoft.Logging.Tests
{
    [TestClass]
    public class LoggingFixture
    {
        private static IUnityContainer _container;

        [ClassInitialize]
        public static void Setup(TestContext context)
        {
        }

        [TestInitialize]
        public void TestSetup()
        {
            _container = new UnityContainer();
            _container.AddExtension(new LoggingExtension(NullLoggerFactory.Instance));
        }

        [TestMethod]
        public void microsoft_logging_factory()
        {
            var factory = _container.Configure<LoggingExtension>().LoggerFactory;

            Assert.IsNotNull(factory);
            Assert.IsInstanceOfType(factory, typeof(ILoggerFactory));
        }

        [TestMethod]
        public void microsoft_logging_factory_resolve_LoggerFactory()
        {
            Assert.IsNotNull(_container.Resolve<ILoggerFactory>());
        }

        [TestMethod]
        public void microsoft_logging_factory_CreateLogger_Category()
        {
            var factory = _container.Configure<LoggingExtension>().LoggerFactory;
            Assert.IsNotNull(_container.Resolve<ILogger>());
            Assert.IsNotNull(_container.Resolve<ILogger>("Test"));
        }

        [TestMethod]
        public void microsoft_logging_CreateLogger_Type()
        {
            var instance = _container.Resolve<LoggedType>();
            Assert.IsNotNull(instance);

            var logger = _container.Configure<LoggingExtension>()
                                   .LoggerFactory
                                   .CreateLogger(instance.GetType());

            Assert.AreEqual(logger.GetType(), instance.ResolvedLogger.GetType());
        }


        [TestMethod]
        public void microsoft_logging_CreateLogger_Generic()
        {
            var instance = _container.Resolve<LoggedType>();
            Assert.IsNotNull(instance);

            var logger = _container.Configure<LoggingExtension>()
                                   .LoggerFactory
                                   .CreateLogger<LoggedType>();

            Assert.AreNotEqual(logger.GetType(), instance.ResolvedLogger.GetType());
            Assert.AreEqual(   logger.GetType(), instance.ResolvedGenericLogger.GetType());
        }


        public class LoggedType
        {
            public LoggedType(ILogger log, ILogger<LoggedType> generic)
            {
                ResolvedLogger = log;
                ResolvedGenericLogger = generic;
            }

            public ILogger ResolvedLogger { get; }

            public ILogger<LoggedType> ResolvedGenericLogger { get; }

        }
    }
}
