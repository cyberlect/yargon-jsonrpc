using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Yargon.JsonRpc
{
    /// <summary>
    /// Tests the <see cref="DefaultJsonRpcService"/> class.
    /// </summary>
    [TestFixture]
    public sealed class DefaultJsonRpcServiceTests : IJsonRpcServiceTests
    {
        /// <inheritdoc />
        public override IJsonRpcService CreateNew()
        {
            return new DefaultJsonRpcService();
        }

        [Test]
        public void RequestForUnknownMethod_ReturnsMethodNotFoundError()
        {
            // Arrange
            var json = @"{
                jsonrpc: '2.0',
                method: 'foobar_foobaz',
                id: 1
            }";
            var rpcService = CreateNew();

            // Act
            string output = rpcService.Process(json);

            // Assert
            var result = JsonConvert.DeserializeObject<JsonResponse>(output);
            var expected = new JsonError(null, new JsonErrorObject(-32601, "Method not found"));
            Assert.That(result, Is.EqualTo(expected).Using<JsonResponse>(new JsonResponseComparer()));
        }
    }
}
