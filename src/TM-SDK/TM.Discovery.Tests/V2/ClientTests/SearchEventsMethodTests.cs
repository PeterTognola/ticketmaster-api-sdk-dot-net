﻿using NSubstitute;
using Ploeh.AutoFixture;
using RestSharp;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using TM.Discovery.V2;
using TM.Discovery.V2.Models;
using Xunit;

namespace TM.Discovery.Tests.V2.ClientTests
{
    public class SearchEventsMethodTests : MethodTest
    {
        private readonly EventsClient _sut;
        private readonly SearchEventsResponse _searchEventsResponse;

        public SearchEventsMethodTests()
        {
            _searchEventsResponse = Fixture.Create<SearchEventsResponse>();
            Client
                   .ExecuteTaskAsync<SearchEventsResponse>(Arg.Any<IRestRequest>())
                   .Returns(new RestResponse<SearchEventsResponse>
                   {
                       Data = _searchEventsResponse,
                       StatusCode = HttpStatusCode.OK
                   });
            Client
                .ExecuteTaskAsync(Arg.Any<IRestRequest>())
                .Returns(new RestResponse
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = "{jobject: { this: 1 }}"
                });
            _sut = new EventsClient(Client, Config);
        }

        [Fact]
        public async Task CallSearchEventsAsync_ShouldReturnIRestResponse()
        {
            var response = await _sut.CallSearchEventsAsync(new SearchEventsRequest());
            Assert.NotNull(response);
            Assert.IsType<RestResponse>(response);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(response.Content);
        }

        [Theory]
        [InlineData(HttpStatusCode.Accepted)]
        [InlineData(HttpStatusCode.Ambiguous)]
        [InlineData(HttpStatusCode.BadGateway)]
        [InlineData(HttpStatusCode.BadRequest)]
        [InlineData(HttpStatusCode.Conflict)]
        [InlineData(HttpStatusCode.Created)]
        [InlineData(HttpStatusCode.InternalServerError)]
        [InlineData(HttpStatusCode.Continue)]
        [InlineData(HttpStatusCode.ExpectationFailed)]
        [InlineData(HttpStatusCode.GatewayTimeout)]
        [InlineData(HttpStatusCode.Gone)]
        [InlineData(HttpStatusCode.NonAuthoritativeInformation)]
        [InlineData(HttpStatusCode.NotAcceptable)]
        public async Task SearchEventsAsync_ShouldThrowException_WhenResponseCodeNotOk(HttpStatusCode statusCode)
        {
            Client
               .ExecuteTaskAsync<SearchEventsResponse>(Arg.Any<IRestRequest>())
                   .Returns(new RestResponse<SearchEventsResponse>
                   {
                       StatusCode = statusCode
                   });

            await Assert.ThrowsAnyAsync<InvalidDataException>(() => _sut.SearchEventsAsync(new SearchEventsRequest()));
        }

        [Fact]
        public async Task SearchEventsAsync_ShouldReturnParsedRespond_WhenStatusCodeIsHttpStatusCodeOK()
        {
            var result = await _sut.SearchEventsAsync(new SearchEventsRequest());

            Assert.NotNull(result);
            Assert.NotNull(result.Links);
            Assert.NotNull(result._embedded);
            Assert.NotNull(result.Page);
            Assert.Equal(_searchEventsResponse, result);
        }

        [Theory]
        [InlineData(SearchEventsQueryParameters.attractionId, "K8vZ91713eV")]
        [InlineData(SearchEventsQueryParameters.venueId, "KovZpZAEdFtJ")]
        [InlineData(SearchEventsQueryParameters.postalCode, "90069")]
        [InlineData(SearchEventsQueryParameters.latlong, "34.0928090,-118.3286610")]
        [InlineData(SearchEventsQueryParameters.radius, "25")]
        [InlineData(SearchEventsQueryParameters.unit, "miles")]
        [InlineData(SearchEventsQueryParameters.source, "ticketmaster")]
        [InlineData(SearchEventsQueryParameters.marketId, "27")]
        [InlineData(SearchEventsQueryParameters.startDateTime, "2017-01-01T00:00:00Z")]
        [InlineData(SearchEventsQueryParameters.endDateTime, "2017-01-01T00:00:00Z")]
        [InlineData(SearchEventsQueryParameters.size, "1")]
        [InlineData(SearchEventsQueryParameters.page, "1")]
        [InlineData(SearchEventsQueryParameters.geoPoint, "9q5cgq7tn")]
        public async Task SearchEventsAsync_ShouldBuildRequestWithQueryParameters(SearchEventsQueryParameters key, string value)
        {
            var request = new SearchEventsRequest();
            request.AddQueryParameter(new KeyValuePair<SearchEventsQueryParameters, string>(key, value));

            await _sut.SearchEventsAsync(request);

            await Client
                .Received()
                .ExecuteTaskAsync<SearchEventsResponse>(
                    Arg.Is<RestRequest>(
                        restRequest => restRequest.Parameters.Any(p => p.Name == key.ToString() && Equals(p.Value, value))));
        }
    }
}
