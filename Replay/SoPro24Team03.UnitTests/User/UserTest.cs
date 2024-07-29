using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit.Abstractions;
using SoPro24Team03.Models;

namespace SoPro24Team03.UnitTests.User
{
    public class UserTest
    {
        private readonly ITestOutputHelper _output;

        public UserTest(ITestOutputHelper output)
        {
            _output = output;
        }

        // Made by Daniel Albert
        [Theory]
        [InlineData("abc123")]
        [InlineData(" ")]
        [InlineData("")]
        public void testPassword_success(string pswdStr)
        {
            // Arange
            var testUser = new Models.User();
            testUser.PasswordHash = Models.User.HashPassword(pswdStr);

            // Act
            bool result = testUser.ValidatePassword(pswdStr);

            // Assert
            Assert.True(result);
        }

        // Made by Daniel Albert
        [Theory]
        [InlineData("abc123")]
        [InlineData(" ")]
        [InlineData("")]
        public void testPassword_fail(string pswdStr)
        {
            // Arange
            var testUser = new Models.User();
            testUser.PasswordHash = Models.User.HashPassword("sdASf5Rgg43qQVN($%8Q($V%§))");

            // Act
            bool result = testUser.ValidatePassword(pswdStr);

            // Assert
            Assert.False(result);
        }

        // Made by Daniel Albert
        [Theory]
        [InlineData("abc123")]
        [InlineData(" ")]
        public void testPassword_empy_fail(string pswdStr)
        {
            // Arange
            var testUser = new Models.User();
            testUser.PasswordHash = Models.User.HashPassword("");

            // Act
            bool result = testUser.ValidatePassword(pswdStr);

            // Assert
            Assert.False(result);
        }
        
    }
}