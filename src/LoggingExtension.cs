using System;
using Microsoft.Extensions.Logging;
using System.Reflection;
using Unity.Builder;
using Unity.Extension;
using Unity.Policy;

namespace Unity.Microsoft.Logging
{
    public class LoggingExtension : UnityContainerExtension,
                                    IBuildPlanPolicy
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
        {
            LoggerFactory = new LoggerFactory();
        }

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
            Context.Policies.Set(typeof(ILogger),   string.Empty, typeof(IBuildPlanPolicy), this);
            Context.Policies.Set(typeof(ILogger),   string.Empty, typeof(ResolveDelegateFactory), (ResolveDelegateFactory)GetResolver);
            Context.Policies.Set(typeof(ILogger<>), string.Empty, typeof(ResolveDelegateFactory), (ResolveDelegateFactory)GetResolver);
        }

        #endregion


        #region IBuildPlanPolicy


        public void BuildUp(ref BuilderContext context)
        {
            context.Existing = null == context.DeclaringType
                             ? LoggerFactory.CreateLogger(context.Name ?? string.Empty)
                             : LoggerFactory.CreateLogger(context.DeclaringType);
            context.BuildComplete = true;
        }

        #endregion


        #region IResolveDelegateFactory


        public ResolveDelegate<BuilderContext> GetResolver(ref BuilderContext context)
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
