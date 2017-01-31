using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Yargon.JsonRpc
{
    /// <summary>
    /// Reflection utility functions.
    /// </summary>
    internal static class ReflectionUtils
    {

        /// <summary>
        /// Asserts that the argument type is compatible with the parameter.
        /// </summary>
        /// <param name="argument">The argument.</param>
        /// <param name="paramType">The parameter type.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <exception cref="InvalidParamsException">
        /// The argument type is not compatible with the parameter.
        /// </exception>
        public static void AssertTypeIsCompatible([CanBeNull] object argument, Type paramType, string paramName)
        {
            #region Contract
            if (paramName == null)
                throw new ArgumentNullException(nameof(paramName));
            if (paramType == null)
                throw new ArgumentNullException(nameof(paramType));
            #endregion

            if (argument == null && !IsNullableType(paramType))
            {
                throw new InvalidParamsException($"Parameter {paramName} must be a {paramType}, which does not accept null.");
            }
            if (argument != null && !argument.GetType().GetTypeInfo().IsAssignableFrom(paramType.GetTypeInfo()))
            {
                throw new InvalidParamsException($"Parameter {paramName} must be a {paramType}, got {argument.GetType()}.");
            }
        }

        /// <summary>
        /// Determines whether null is assignable to a type.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns><see langword="true"/> when it is nullable;
        /// otherwise, <see langword="false"/>.</returns>
        private static bool IsNullableType(Type type)
        {
            #region Contract
            Debug.Assert(type != null);
            #endregion

            // TODO: Not all types accept null.
            return true;
        }
    }
}
