using Microsoft.Extensions.Logging;
using System;
using System.Reflection;
using System.Security;
using Unity.Builder;
using Unity.Extension;
using Unity.Policy;
using Unity.Resolution;

namespace Unity.Microsoft.Logging
{
    [SecuritySafeCritical]
    public class LoggingExtension : UnityContainerExtension
    {
        #region Fields

        private delegate object GenericLoggerFactory(ILoggerFactory factory);
        private static readonly MethodInfo CreateLoggerMethod = 
            typeof(LoggingExtension).GetTypeInfo()
                .GetDeclaredMethod(nameof(CreateLogger));

        #endregion


        #region Constructors

        [InjectionConstructor]
        public LoggingExtension()
            : this(new LoggerFactory())
        { }

        public LoggingExtension(ILoggerFactory factory)
        {
            LoggerFactory = factory ?? new LoggerFactory();
        }


        #endregion


        #region Public Members

        public ILoggerFactory LoggerFactory { get; }

        #endregion


        #region UnityContainerExtension

        protected override void Initialize()
        {
            Context.Policies.Set(typeof(ILoggerFactory),   UnityContainer.All, typeof(ResolveDelegateFactory), (ResolveDelegateFactory)GetFactoryResolver);
            Context.Policies.Set(typeof(ILogger), UnityContainer.All, typeof(ResolveDelegateFactory), (ResolveDelegateFactory)GetResolver);
            Context.Policies.Set(typeof(ILogger<>), UnityContainer.All, typeof(ResolveDelegateFactory), (ResolveDelegateFactory)GetResolverGeneric);
        }

        #endregion


        #region IResolveDelegateFactory

        public ResolveDelegate<BuilderContext> GetFactoryResolver(ref BuilderContext context)
        {
            return ((ref BuilderContext c) =>
            {
                return LoggerFactory;
            });
        }

        public ResolveDelegate<BuilderContext> GetResolver(ref BuilderContext context)
        {
            return ((ref BuilderContext c) =>
            {
                Type declaringType = c.DeclaringType;

                return null == declaringType
                ? LoggerFactory.CreateLogger(c.Name ?? UnityContainer.All)
                : LoggerFactory.CreateLogger(declaringType);
            });
        }

        public ResolveDelegate<BuilderContext> GetResolverGeneric(ref BuilderContext context)
        {
            var itemType = context.Type.GetTypeInfo().GenericTypeArguments[0];
            var buildMethod = (GenericLoggerFactory)CreateLoggerMethod.MakeGenericMethod(itemType)
                                                                      .CreateDelegate(typeof(GenericLoggerFactory));
            return ((ref BuilderContext c) =>
            {
                c.Existing = buildMethod(LoggerFactory);
                return c.Existing;
            });
        }

        #endregion


        #region Implementation

        private static object CreateLogger<TElement>(ILoggerFactory factory)
        {
            return factory.CreateLogger<TElement>();
        }

        #endregion
    }
}
