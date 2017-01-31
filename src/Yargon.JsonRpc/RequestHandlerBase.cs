using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Yargon.JsonRpc
{
    /// <summary>
    /// Base class for implementations of the <see cref="IRequestHandler"/> interface.
    /// </summary>
    public abstract class RequestHandlerBase : IRequestHandler
    {
        /// <inheritdoc />
        public abstract bool CanHandle(string method);

        /// <inheritdoc />
        public abstract JsonResponse Handle(JsonRequest request, JsonSerializer serializer);

        /// <summary>
        /// Serializes an object to a JSON token.
        /// </summary>
        /// <typeparam name="T">The type of object.</typeparam>
        /// <param name="obj">The object.</param>
        /// <param name="serializer">The serializer.</param>
        /// <returns>The JSON token.</returns>
        protected JToken Serialize<T>(T obj, JsonSerializer serializer)
        {
            #region Contract
            if (serializer == null)
                throw new ArgumentNullException(nameof(serializer));
            #endregion

            // TODO: Handle null?

            using (var writer = new JTokenWriter())
            {
                serializer.Serialize(writer, obj);
                return writer.Token;
            }
        }

        /// <summary>
        /// Deserializes a JSON token to an object.
        /// </summary>
        /// <typeparam name="T">The type of object.</typeparam>
        /// <param name="token">The token.</param>
        /// <param name="serializer">The serializer.</param>
        /// <returns>The deserialized object.</returns>
        protected T Deserialize<T>(JToken token, JsonSerializer serializer)
        {
            #region Contract
            if (serializer == null)
                throw new ArgumentNullException(nameof(serializer));
            #endregion

            // TODO: Handle null?

            using (var reader = token.CreateReader())
            {
                return serializer.Deserialize<T>(reader);
            }
        }
    }
}
