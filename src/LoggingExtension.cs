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
    public class LoggingExtension : UnityContainerExtension, IBuildPlanCreatorPolicy
    {
        #region Fields

        private readonly MethodInfo _createLoggerMethod;

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
            _createLoggerMethod = LoggerFactory.GetType().GetTypeInfo().GetDeclaredMethods(nameof(ILoggerFactory.CreateLogger))
                                               .First(m => m.IsGenericMethod);
        }


        #endregion


        #region Public Members

        public ILoggerFactory LoggerFactory { get; } = new LoggerFactory();

        #endregion


        #region Implementation

        protected override void Initialize()
        {
            Context.Policies.Set<IBuildPlanCreatorPolicy>(this, typeof(ILogger<>));
        }

        IBuildPlanPolicy IBuildPlanCreatorPolicy.CreatePlan(IBuilderContext context, INamedType buildKey)
        {
            var itemType = (context ?? throw new ArgumentNullException(nameof(context))).BuildKey
                                                                                        .Type
                                                                                        .GetTypeInfo()
                                                                                        .GenericTypeArguments
                                                                                        .First();
            var buildMethod = _createLoggerMethod.MakeGenericMethod(itemType)
                                                 .CreateDelegate(typeof(DynamicBuildPlanMethod));

            return new DynamicMethodBuildPlan((DynamicBuildPlanMethod)buildMethod);
        }

        #endregion


        #region Nested Types

        public delegate void DynamicBuildPlanMethod(IBuilderContext context);

        private class DynamicMethodBuildPlan : IBuildPlanPolicy
        {
            private readonly DynamicBuildPlanMethod _buildMethod;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="buildMethod"></param>
            public DynamicMethodBuildPlan(DynamicBuildPlanMethod buildMethod)
            {
                _buildMethod = buildMethod;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="context"></param>
            public void BuildUp(IBuilderContext context)
            {
                _buildMethod(context);
            }
        }
        
        #endregion
    }
}
