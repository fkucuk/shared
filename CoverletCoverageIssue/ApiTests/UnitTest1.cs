using CoverletIssueRaiser;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xunit;

namespace ApiTests
{
    public class UnitTest1
    {
        [Fact]
        public async Task Test1()
        {
            var factory = new TestWebApplicationFactory<Startup>();
            var client = factory.CreateClient();

            var respone = await client.GetAsync("api/values");

            Assert.Equal(HttpStatusCode.OK, respone.StatusCode);
        }


        [Fact]
        public async Task Test2()
        {
            var factory = new TestWebApplicationFactory<Startup>();
            var client = factory.CreateClient();

            var req = new SendMessageRequest{ Message = "a message"};

            var dd = JsonConvert.SerializeObject(req);
            var input = new StringContent(dd, Encoding.UTF8, "application/json");

            var respone = await client.PostAsync("api/values", input);

            Assert.Equal(HttpStatusCode.OK, respone.StatusCode);
        }
    }
}
