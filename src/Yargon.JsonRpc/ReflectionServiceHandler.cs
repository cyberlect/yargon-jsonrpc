using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Yargon.JsonRpc
{
    /// <summary>
    /// Request handler that uses reflection to discover the method a service instance supports.
    /// </summary>
    public sealed class ReflectionServiceHandler<T> : RequestHandlerBase
    {
        /// <summary>
        /// The supported method names (without the prefix).
        /// </summary>
        private readonly IReadOnlyDictionary<string, object> methods;

        /// <summary>
        /// Gets the method prefix to use for this service.
        /// </summary>
        /// <value>The method prefix; or an empty string when none is set.</value>
        public string MethodPrefix { get; }

        /// <summary>
        /// Gets the service instance.
        /// </summary>
        /// <value>The service instance.</value>
        public T Service { get; }

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ReflectionServiceHandler{T}"/> class.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="methodPrefix">The method prefix.</param>
        public ReflectionServiceHandler(T service, string methodPrefix)
        {
            #region Contract
            if (service == null)
                throw new ArgumentNullException(nameof(service));
            if (methodPrefix == null)
                throw new ArgumentNullException(nameof(methodPrefix));
            #endregion

            this.Service = service;
            this.MethodPrefix = methodPrefix;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReflectionServiceHandler{T}"/> class.
        /// </summary>
        /// <param name="service">The service.</param>
        public ReflectionServiceHandler(T service)
            : this(service, String.Empty)
        {
            // Nothing to do.
        }
        #endregion

        /// <inheritdoc />
        public override bool CanHandle(string method)
        {
            return method.StartsWith(this.MethodPrefix)
                && this.methods.ContainsKey(method.Substring(this.MethodPrefix.Length));
        }

        /// <inheritdoc />
        public override JsonResponse Handle(JsonRequest request, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        private void X(T service)
        {
            foreach (var method in typeof(T).GetTypeInfo().DeclaredMethods)
            {
                var methodAttr = method.GetCustomAttribute<JsonRpcMethod>();
                if (methodAttr == null)
                {
                    // The method has no attributes.
                    continue;
                }

                string methodName = methodAttr.MethodName ?? method.Name;

                // TODO: Parameters
            }
        }
    }
}
