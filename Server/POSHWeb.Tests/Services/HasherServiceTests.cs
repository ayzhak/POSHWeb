using Xunit;
using POSHWeb.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSHWeb.Services.Tests
{
    public class HasherServiceTests
    {
        [Fact()]
        public void Sha256_ValidContent_ShouldBeValid()
        {
            var service = new HasherService();
            string hash = service.Sha256("TEST");
            Assert.Equal("94ee059335e587e501cc4bf90613e0814f00a7b08bc7c648fd865a2af6a22cc2", hash);
        }

        [Fact()]
        public void Sha256_InvalidContent_ShouldFail()
        {
            var service = new HasherService();
            string hash = service.Sha256("test");
            Assert.NotEqual("94ee059335e587e501cc4bf90613e0814f00a7b08bc7c648fd865a2af6a22cc2", hash);
        }

        [Fact()]
        public void Sha256_EmptyInput_ShouldBeValid()
        {
            var service = new HasherService();
            string hash = service.Sha256("");
            Assert.Equal("e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855", hash);
        }
    }
}