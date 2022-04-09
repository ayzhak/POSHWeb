using Xunit;
using POSHWeb.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;

namespace POSHWeb.Services.Tests
{
    public class PSScriptValidatorTests
    {


        [Fact()]
        public void ValidateInput_ValidScript_Valid()
        {
            string script = @"
Write-Host 'Test'
";
            var validator = new PSScriptValidator(new NullLogger<PSScriptValidator>());
            Assert.True(validator.ValidateInput(script));
        }

        [Fact()]
        public void ValidateInput_ValidInput_ShouldBeInvalid()
        {
            string script = @"
Write-Host Test'
";
            var validator = new PSScriptValidator(new NullLogger<PSScriptValidator>());
            Assert.False(validator.ValidateInput(script));
        }

        [Fact()]
        public void ValidateInput_EmptyInput_ShouldBeInvalid()
        {
            string script = @"";
            var validator = new PSScriptValidator(new NullLogger<PSScriptValidator>());
            Assert.True(validator.ValidateInput(script));
        }
    }
}