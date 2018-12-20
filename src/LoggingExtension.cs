using System;
using Microsoft.Extensions.Logging;
using System.Reflection;
using Unity.Builder;
using Unity.Extension;
using Unity.ObjectBuilder.BuildPlan.DynamicMethod;
using Unity.Policy;

namespace Unity.Microsoft.Logging
{
    public class LoggingExtension : UnityContainerExtension,
                                    IBuildPlanCreatorPolicy,
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
            Context.Policies.Set(typeof(ILogger), string.Empty, typeof(IBuildPlanPolicy), this);
            Context.Policies.Set(typeof(ILogger), string.Empty, typeof(IBuildPlanCreatorPolicy), this);
            Context.Policies.Set(typeof(ILogger<>), string.Empty, typeof(IBuildPlanCreatorPolicy), this);
        }

        #endregion


        #region IBuildPlanPolicy


        public void BuildUp(ref BuilderContext context)
        {
            context.Existing = null == context.Parent
                             ? LoggerFactory.CreateLogger(context.Registration.Name ?? string.Empty)
                             : LoggerFactory.CreateLogger(context.Parent.Type);
            context.BuildComplete = true;
        }

        #endregion


        #region IBuildPlanCreatorPolicy

        IBuildPlanPolicy IBuildPlanCreatorPolicy.CreatePlan(ref BuilderContext context, Type type, string name)
        {
            var itemType = context.Type.GetTypeInfo().GenericTypeArguments[0];
            var buildMethod = (GenericLoggerFactory)CreateLoggerMethod.MakeGenericMethod(itemType)
                                                                      .CreateDelegate(typeof(GenericLoggerFactory));

            return new DynamicMethodBuildPlan((ResolveDelegate<BuilderContext>)((ref BuilderContext c) =>
            {
                c.Existing = buildMethod(LoggerFactory);
                return c.Existing;
            }));
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
