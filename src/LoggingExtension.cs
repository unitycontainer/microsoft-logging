using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Reflection;
using Unity.Attributes;
using Unity.Builder;
using Unity.Extension;
using Unity.Policy;

namespace Unity.Microsoft.Logging
{
    public class LoggingExtension : UnityContainerExtension,
                                    IBuildPlanCreatorPolicy,
                                    IBuildPlanPolicy
    {
        #region Fields

        private readonly MethodInfo _createLoggerMethod = typeof(LoggingExtension).GetTypeInfo()
                                                                                  .GetDeclaredMethod(nameof(CreateLogger));

        #endregion


        #region Constructors

        [InjectionConstructor]
        public LoggingExtension()
            : this(new LoggerFactory())
        {
        }

        public LoggingExtension(ILoggerFactory loggerFactory)
        {
            LoggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
        }


        #endregion


        #region Public Members

        public ILoggerFactory LoggerFactory { get; } = new LoggerFactory();

        #endregion


        #region IBuildPlanPolicy


        public void BuildUp(IBuilderContext context)
        {
            context.Existing = null == context.ParentContext
                             ? LoggerFactory.CreateLogger(context.OriginalBuildKey.Name ?? string.Empty)
                             : LoggerFactory.CreateLogger(context.ParentContext.BuildKey.Type);
            context.BuildComplete = true;
        }

        #endregion


        #region IBuildPlanCreatorPolicy

        IBuildPlanPolicy IBuildPlanCreatorPolicy.CreatePlan(IBuilderContext context, INamedType buildKey)
        {
            var info = (context ?? throw new ArgumentNullException(nameof(context))).BuildKey
                                                                                    .Type
                                                                                    .GetTypeInfo();
            if (!info.IsGenericType) return this;

            var buildMethod = _createLoggerMethod.MakeGenericMethod(info.GenericTypeArguments.First())
                                                 .CreateDelegate(typeof(DynamicBuildPlanMethod));

            return new DynamicMethodBuildPlan((DynamicBuildPlanMethod)buildMethod, LoggerFactory);
        }

        #endregion


        #region Implementation

        private static void CreateLogger<T>(IBuilderContext context, ILoggerFactory loggerFactory)
        {
            context.Existing = loggerFactory.CreateLogger<T>();
            context.BuildComplete = true;
        }

        protected override void Initialize()
        {
            Context.Policies.Set(typeof(ILogger), null, typeof(IBuildPlanPolicy), this);
            Context.Policies.Set<IBuildPlanCreatorPolicy>(this, typeof(ILogger));
            Context.Policies.Set<IBuildPlanCreatorPolicy>(this, typeof(ILogger<>));
        }

        private delegate void DynamicBuildPlanMethod(IBuilderContext context, ILoggerFactory loggerFactory);

        private class DynamicMethodBuildPlan : IBuildPlanPolicy
        {
            private readonly DynamicBuildPlanMethod _buildMethod;
            private readonly ILoggerFactory _loggerFactory;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="buildMethod"></param>
            public DynamicMethodBuildPlan(DynamicBuildPlanMethod buildMethod, 
                                          ILoggerFactory loggerFactory)
            {
                _buildMethod = buildMethod;
                _loggerFactory = loggerFactory;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="context"></param>
            public void BuildUp(IBuilderContext context)
            {
                _buildMethod(context, _loggerFactory);
            }
        }

        #endregion
    }
}
