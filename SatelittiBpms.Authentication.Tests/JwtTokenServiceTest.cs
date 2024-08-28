using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using SatelittiBpms.Authentication.Models;
using SatelittiBpms.Authentication.Services;
using SatelittiBpms.Options.Models;
using System;

namespace SatelittiBpms.Authentication.Tests
{
    public class JwtTokenServiceTest
    {
        Mock<IOptions<AuthenticationOptions>> mockOptions;

        [SetUp]
        public void Init()
        {
            mockOptions = new Mock<IOptions<AuthenticationOptions>>();           
        }

        [Test]
        public void ensureThatThrowsWhenParameterIsNull()
        {
            JwtTokenService jwtTokenService = new JwtTokenService(mockOptions.Object);
            Assert.Throws(Is.TypeOf<ArgumentNullException>().And.Message.EqualTo("Value cannot be null. (Parameter 'parameters')"),
                () => jwtTokenService.GenerateToken(null));
        }

        [Test]
        public void ensureThatThrowsWhenSecretKeyIsNull()
        {
            GenerateTokenParameters tokenParams = new GenerateTokenParameters();
            mockOptions.SetupGet(x => x.Value).Returns(new AuthenticationOptions()
            {
                SecretKey = null
            });

            JwtTokenService jwtTokenService = new JwtTokenService(mockOptions.Object);
            Assert.Throws(Is.TypeOf<ArgumentNullException>().And.Message.EqualTo("Value cannot be null. (Parameter 'SecretKey')"),
                () => jwtTokenService.GenerateToken(tokenParams));
        }


        [Test]
        public void ensure()
        {
            GenerateTokenParameters tokenParams = new GenerateTokenParameters();
            tokenParams.Role = "tokenRole";
            tokenParams.Name = "tokenName";
            tokenParams.Sid = "tokenSid";
            tokenParams.Email = "tokenEmail";
            tokenParams.SuiteToken = "tokenSuiteToken";
            tokenParams.Timezone = 2;
            tokenParams.TokenLifetimeInMinutes = 22;
            
            mockOptions.SetupGet(x => x.Value).Returns(new AuthenticationOptions()
            {
                SecretKey = "aaaaaaaaaaaaaaaa1111111222!@#$%&"
            });

            JwtTokenService jwtTokenService = new JwtTokenService(mockOptions.Object);

            var result = jwtTokenService.GenerateToken(tokenParams);

            Assert.IsNotNull(result);
            Assert.AreEqual(1320, result.ExpiresIn);
            Assert.IsNotEmpty(result.Token);
        }
    }
}
