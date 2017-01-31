using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Yargon.JsonRpc
{
    partial class ReflectionServiceHandler<T>
    {
        /// <summary>
        /// Describes an RPC method parameter.
        /// </summary>
        internal sealed class RpcMethodParam
        {
            /// <summary>
            /// Gets the name of the parameter, as used in a JSON RPC request.
            /// </summary>
            /// <value>The name of the parameter.</value>
            public string Name { get; }

            /// <summary>
            /// Gets the type of the parameter.
            /// </summary>
            /// <value>The type of the parameter.</value>
            public Type Type { get; }

            /// <summary>
            /// Gets whether the parameter is optional.
            /// </summary>
            /// <value><see langword="true"/> when the parameter is optional;
            /// otherwise, <see langword="false"/>.</value>
            public bool IsOptional { get; }

            /// <summary>
            /// Gets the default value of the parameter.
            /// </summary>
            /// <value>The default value of the parameter, which may be <see langword="null"/>;
            /// or <see langword="null"/> when the parameter is not optional.</value>
            [CanBeNull]
            public object DefaultValue { get; }

            #region Constructors
            /// <summary>
            /// Initializes a new instance of the <see cref="RpcMethodParam"/> class.
            /// </summary>
            /// <param name="name">The name of the parameter.</param>
            /// <param name="type">The type of the parameter.</param>
            /// <param name="isOptional">Whether the parameter is optional.</param>
            /// <param name="defaultValue">The default value of the parameter; or <see langword="null"/> when it's not optional.</param>
            public RpcMethodParam(string name, Type type, bool isOptional, [CanBeNull] object defaultValue)
            {
                #region Contract
                if (name == null)
                    throw new ArgumentNullException(nameof(name));
                if (name == "")
                    throw new ArgumentException("The method name cannot be empty.", nameof(name));
                if (type == null)
                    throw new ArgumentNullException(nameof(type));
                if (!isOptional && defaultValue != null)
                    throw new ArgumentException("The default value must be null; or the parameter must be optional.", nameof(defaultValue));
                if (isOptional)
                    ReflectionUtils.AssertTypeIsCompatible(defaultValue, type, "default");
                #endregion

                this.Name = name;
                this.Type = type;
                this.IsOptional = isOptional;
                this.DefaultValue = defaultValue;
            }
            #endregion

            /// <inheritdoc />
            public override string ToString()
            {
                return $"{this.Type} {this.Name}" + (this.IsOptional ? $" = {this.DefaultValue}" : "");
            }
        }
    }
}
