using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Xunit.Abstractions;
using SoPro24Team03.Models;
using SoPro24Team03.Controllers;
using Moq;
using SoPro24Team03.Data;
using SoPro24Team03.ViewModels;


namespace SoPro24Team03.UnitTests.User
{
    public class UserControllerTest
    {
        private readonly ITestOutputHelper _output;

        Mock<IUserRepository> _userRepo;
        Mock<IRoleRepository> _roleRepo;
        Mock<IDepartmentRepository> _depRepo;
        Mock<ISessionService> _sessionService;
        List<Models.User> _userList;
        List<Role> _roleList;
        string _invalStr;

        
        // Made by Daniel Albert
        public UserControllerTest(ITestOutputHelper output)
        {
            _output = output;

            // setup mocks
            _userRepo = new Mock<IUserRepository>();
            _userList = new List<Models.User>() {
                new Models.User() {
                    Id = 3,
                    UserName = "mustmax",
                    FirstName = "Maximilian",
                    LastName = "Musterman",
                    DateOfBirth = new DateTime(2000, 1, 1),
                    ContractStart = new DateTime(2020, 1, 1),
                    ContractType = ContractType.werkstudent,
                    EmailAddress = "max.musterman@xyz.de",
                    PasswordHash = Models.User.HashPassword("Passwort"),
                    Roles = new List<Role> {new Role() {Id = 2, Name = "IT", IsAdmin = false}},
                    DepartmentId = 1,
                    isArchived = false,
                    isSuspended = false,
                    changedInitialPassword = true,
                    Notes = ""
                }
            };
            _invalStr = "";

            // userRepo.Update(user)
            _userRepo.Setup(repo => repo.Update(It.IsAny<Models.User>()))
                .Callback<Models.User>(u => {
                    _userList.RemoveAll(uu => uu.Id == u.Id);
                    _userList.Add(u);
                })
                .Returns(Task.CompletedTask);

            _roleRepo = new Mock<IRoleRepository>();
            _roleList = new List<Role>() {
                new Role() {Id = 1, Name = "Administratoren", IsAdmin = true},
                new Role() {Id = 2, Name = "IT", IsAdmin = false},
            };

            // roleRepo.Find(id)
            _roleRepo.Setup(repo => repo.Find(It.IsAny<int>()))
                .ReturnsAsync((int id) => {
                    return _roleList.Find(r => r.Id == id);
                });

            _depRepo = new Mock<IDepartmentRepository>();

            _sessionService = new Mock<ISessionService>();

            // sessionService.InvalidateUserSessions(id.ToString());
            _sessionService.Setup(service => service.InvalidateUserSessions(It.IsAny<string>()))
                .Callback((string s) => _invalStr = s);

            
        }

        // Made by Daniel Albert
        [Fact]
        public async void testEdit_success()
        {
            // Arange
            var myUser = _userList.Find(u => u.Id == 3);

            var controller = new UsermanagementController(_userRepo.Object, _roleRepo.Object, _depRepo.Object, _sessionService.Object);
            var vm = new UserEditViewModel() {
                UserName = myUser.UserName,
                FirstName = myUser.FirstName,
                LastName = myUser.LastName,
                PasswordHash = myUser.PasswordHash,
                DateOfBirth = new DateTime(2001, 1, 1),
                ContractStart = new DateTime(2021, 1, 1),
                ContractType = ContractType.permanent,
                SelectedRoleIds = new List<int>() {1, 2},
                SelectedDepartmentId = 2,
                isSuspended = myUser.isSuspended,
                isArchived = myUser.isArchived,
                EmailAddress = "max.musterman@domain.com",
                Notes = "test"
            };
            
            // Act
            var result = await controller.UserEdit(myUser.Id, vm);
            var newUser = _userList.Find(u => u.Id == 3);

            // Assert
            Assert.True(newUser != null);
            Assert.True(    // should not change
                newUser.UserName.Equals(myUser.UserName) &&
                newUser.FirstName.Equals(myUser.FirstName) &&
                newUser.LastName.Equals(myUser.LastName) &&
                newUser.PasswordHash.Equals(myUser.PasswordHash) &&
                newUser.isSuspended == myUser.isSuspended &&
                newUser.isArchived == myUser.isArchived
            );
            Assert.True(    // should change
                newUser.DateOfBirth.Equals(new DateTime(2001, 1, 1)) &&
                newUser.ContractStart.Equals(new DateTime(2021, 1, 1)) &&
                newUser.DepartmentId == 2 &&
                newUser.EmailAddress.Equals("max.musterman@domain.com") &&
                newUser.Notes.Equals("test")
            );
            Assert.True(    // should change
                newUser.Roles.Count() == 2 &&
                newUser.Roles.Any(r => (r.Id == 1) && r.Name.Equals("Administratoren")) &&
                newUser.Roles.Any(r => (r.Id == 2) && r.Name.Equals("IT"))
            );
            // no invalidation of session cookie
            _sessionService.Verify(s => s.InvalidateUserSessions(It.IsAny<string>()), Times.Never);
        }


        [Fact]
        public async void testEdit_fail()
        {
            // Arange
            var myUser = _userList.Find(u => u.Id == 3);

            var controller = new UsermanagementController(_userRepo.Object, _roleRepo.Object, _depRepo.Object, _sessionService.Object);
            var vm = new UserEditViewModel() {
                UserName = myUser.UserName,
                FirstName = myUser.FirstName,
                LastName = myUser.LastName,
                PasswordHash = myUser.PasswordHash,
                DateOfBirth = new DateTime(2001, 1, 1),
                ContractStart = new DateTime(2021, 1, 1),
                ContractType = ContractType.permanent,
                SelectedRoleIds = new List<int>() {42},     // bad roleId
                SelectedDepartmentId = 2,
                isSuspended = myUser.isSuspended,
                isArchived = myUser.isArchived,
                EmailAddress = "max.musterman@domain.com",
                Notes = "test"
            };
            
            // Act
            await Assert.ThrowsAsync<Exception>(() => controller.UserEdit(myUser.Id, vm));
            var newUser = _userList.Find(u => u.Id == 3);

            // Assert
            Assert.True(newUser != null);
            Assert.True(    // should not change
                newUser.UserName.Equals(myUser.UserName) &&
                newUser.FirstName.Equals(myUser.FirstName) &&
                newUser.LastName.Equals(myUser.LastName) &&
                newUser.PasswordHash.Equals(myUser.PasswordHash) &&
                newUser.isSuspended == myUser.isSuspended &&
                newUser.isArchived == myUser.isArchived
            );
            Assert.True(    // should not change
                newUser.DateOfBirth.Equals(myUser.DateOfBirth) &&
                newUser.ContractStart.Equals(myUser.ContractStart) &&
                newUser.DepartmentId == myUser.DepartmentId &&
                newUser.EmailAddress.Equals(myUser.EmailAddress) &&
                newUser.Notes.Equals(myUser.Notes)
            );
            Assert.True(    // should not change
                newUser.Roles.Count() == 1 &&
                newUser.Roles.Any(r => (r.Id == 2) && r.Name.Equals("IT"))
            );
            // no invalidation of session cookie
            _sessionService.Verify(s => s.InvalidateUserSessions(It.IsAny<string>()), Times.Never);
        }

        // Made by Daniel Albert
        [Fact]
        public async void testEdit_suspend()
        {
            // Arange
            var myUser = _userList.Find(u => u.Id == 3);

            var controller = new UsermanagementController(_userRepo.Object, _roleRepo.Object, _depRepo.Object, _sessionService.Object);
            var vm = new UserEditViewModel() {
                UserName = myUser.UserName,
                FirstName = myUser.FirstName,
                LastName = myUser.LastName,
                PasswordHash = myUser.PasswordHash,
                DateOfBirth = new DateTime(2001, 1, 1),
                ContractStart = new DateTime(2021, 1, 1),
                ContractType = ContractType.permanent,
                SelectedRoleIds = new List<int>() {1, 2},
                SelectedDepartmentId = 2,
                isSuspended = true,             // suspend this user
                isArchived = myUser.isArchived,
                EmailAddress = "max.musterman@domain.com",
                Notes = "test"
            };
            
            // Act
            var result = await controller.UserEdit(myUser.Id, vm);
            var newUser = _userList.Find(u => u.Id == 3);

            // Assert
            Assert.True(newUser != null);
            Assert.True(    // should not change
                newUser.UserName.Equals(myUser.UserName) &&
                newUser.FirstName.Equals(myUser.FirstName) &&
                newUser.LastName.Equals(myUser.LastName) &&
                newUser.PasswordHash.Equals(myUser.PasswordHash) &&
                newUser.isArchived == myUser.isArchived
            );
            Assert.True(    // should change
                newUser.DateOfBirth.Equals(new DateTime(2001, 1, 1)) &&
                newUser.ContractStart.Equals(new DateTime(2021, 1, 1)) &&
                newUser.DepartmentId == 2 &&
                newUser.EmailAddress.Equals("max.musterman@domain.com") &&
                newUser.Notes.Equals("test") &&
                newUser.isSuspended == true
            );
            Assert.True(    // should change
                newUser.Roles.Count() == 2 &&
                newUser.Roles.Any(r => (r.Id == 1) && r.Name.Equals("Administratoren")) &&
                newUser.Roles.Any(r => (r.Id == 2) && r.Name.Equals("IT"))
            );

            // Session Cookie invalidated?
            _sessionService.Verify(s => s.InvalidateUserSessions(It.IsAny<string>()), Times.Once());
            Assert.True(_invalStr.Equals(newUser.Id.ToString()));
        }

    }
}