using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;

namespace Yargon.JsonRpc
{
    partial class ReflectionServiceHandler<T>
    {
        /// <summary>
        /// Describes and calls an RPC method.
        /// </summary>
        internal sealed class RpcMethod
        {
            /// <summary>
            /// The method.
            /// </summary>
            private readonly MethodInfo method;

            /// <summary>
            /// Gets the method name, as used in a JSON RPC request.
            /// </summary>
            /// <value>The method name.</value>
            public string Name { get; }

            /// <summary>
            /// Gets the method's parameters.
            /// </summary>
            /// <value>The method's parameters.</value>
            public IReadOnlyList<RpcMethodParam> Parameters { get; }

            /// <summary>
            /// Gets the return type; or <see langword="null"/> when it returns nothing.
            /// </summary>
            /// <value>The return type; or <see langword="null"/> when it returns nothing.</value>
            [CanBeNull]
            public Type ReturnType { get; }

            /// <summary>
            /// Gets the minimum number of required arguments, when using positional arguments.
            /// </summary>
            /// <value>The minimum number of required arguments.</value>
            /// <remarks>
            /// If at least this number of positional arguments is provided,
            /// the remaining arguments are optional.
            /// </remarks>
            public int RequiredArgumentCount
                => this.Parameters.Count - this.Parameters.Reverse().TakeWhile(p => p.IsOptional).Count();

            #region Constructors
            /// <summary>
            /// Initializes a new instance of the <see cref="RpcMethod"/> class.
            /// </summary>
            /// <param name="name">The name of the method.</param>
            /// <param name="method">The method itself.</param>
            /// <param name="returnType">The return type.</param>
            /// <param name="parameters">The parameters.</param>
            public RpcMethod(string name, MethodInfo method, [CanBeNull] Type returnType, IReadOnlyList<RpcMethodParam> parameters)
            {
                #region Contract
                if (name == null)
                    throw new ArgumentNullException(nameof(name));
                if (name == "")
                    throw new ArgumentException("The method name cannot be empty.", nameof(name));
                if (method == null)
                    throw new ArgumentNullException(nameof(method));
                if (parameters == null)
                    throw new ArgumentNullException(nameof(parameters));
                #endregion

                this.Name = name;
                this.method = method;
                this.ReturnType = returnType;
                this.Parameters = parameters;
            }
            #endregion

            /// <summary>
            /// Calls the method with positional arguments.
            /// </summary>
            /// <param name="instance">The instance on which to call the method.</param>
            /// <param name="arguments">The positional arguments.</param>
            /// <returns>The return value of the method, which may be <see langword="null"/>;
            /// or <see langword="null"/> when the method doesn't return anything.</returns>
            [CanBeNull]
            public object Call(object instance, IReadOnlyList<object> arguments)
            {
                #region Contract
                if (instance == null)
                    throw new ArgumentNullException(nameof(instance));
                if (arguments == null)
                    throw new ArgumentNullException(nameof(arguments));
                #endregion

                var allArguments = GetAllArguments(arguments);
                return this.method.Invoke(instance, allArguments);
            }

            /// <summary>
            /// Calls the method with named arguments.
            /// </summary>
            /// <param name="instance">The instance on which to call the method.</param>
            /// <param name="arguments">The named arguments.</param>
            /// <returns>The return value of the method, which may be <see langword="null"/>;
            /// or <see langword="null"/> when the method doesn't return anything.</returns>
            [CanBeNull]
            public object Call(object instance, IReadOnlyDictionary<string, object> arguments)
            {
                #region Contract
                if (instance == null)
                    throw new ArgumentNullException(nameof(instance));
                if (arguments == null)
                    throw new ArgumentNullException(nameof(arguments));
                #endregion

                var allArguments = GetAllArguments(arguments);
                return this.method.Invoke(instance, allArguments);
            }

            /// <summary>
            /// Gets all arguments from the specified list of positional arguments and the default arguments.
            /// </summary>
            /// <param name="arguments">The positional arguments.</param>
            /// <returns>An array of all arguments.</returns>
            private object[] GetAllArguments(IReadOnlyList<object> arguments)
            {
                #region Contract
                if (arguments == null)
                    throw new ArgumentNullException(nameof(arguments));
                if (arguments.Count < this.RequiredArgumentCount)
                    throw new InvalidParamsException($"Expected at least {this.RequiredArgumentCount} arguments, got {arguments.Count}.");
                if (arguments.Count > this.Parameters.Count)
                    throw new InvalidParamsException($"Expected at most {this.Parameters.Count} arguments, got {arguments.Count}.");
                #endregion

                var allArguments = new object[this.Parameters.Count];

                Debug.Assert(arguments.Count <= allArguments.Length);
                for (int i = 0; i < arguments.Count; i++)
                {
                    var parameter = this.Parameters[i];
                    var argument = arguments[i];
                    ReflectionUtils.AssertTypeIsCompatible(argument, parameter.Type, parameter.Name);
                    allArguments[i] = arguments[i];
                }
                
                for (int i = arguments.Count; i < allArguments.Length; i++)
                {
                    var parameter = this.Parameters[i];
                    Debug.Assert(parameter.IsOptional);
                    allArguments[i] = parameter.DefaultValue;
                }

                return allArguments;
            }

            /// <summary>
            /// Gets all arguments from the specified dictionary of named arguments and the default arguments.
            /// </summary>
            /// <param name="arguments">The named arguments.</param>
            /// <returns>An array of all arguments.</returns>
            private object[] GetAllArguments(IReadOnlyDictionary<string, object> arguments)
            {
                // NOTE: This implementation ignores extra unused arguments.

                var allArguments = new object[this.Parameters.Count];
                for (int i = 0; i < this.Parameters.Count; i++)
                {
                    var parameter = this.Parameters[i];
                    object argument;
                    if (!arguments.TryGetValue(parameter.Name, out argument))
                    {
                        if (!parameter.IsOptional)
                            throw new InvalidParamsException($"Parameter {parameter.Name} is required and not specified.");
                        argument = parameter.DefaultValue;
                    }
                    ReflectionUtils.AssertTypeIsCompatible(argument, parameter.Type, parameter.Name);
                    allArguments[i] = argument;
                }
                return allArguments;
            }

            /// <inheritdoc />
            public override string ToString()
            {
                return $"{this.ReturnType?.ToString() ?? "void"} {this.Name}({String.Join(", ", this.Parameters)})";
            }
        }
    }
}
