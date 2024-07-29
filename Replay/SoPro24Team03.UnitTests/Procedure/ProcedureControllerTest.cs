using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using Moq;
using SoPro24Team03.Controllers;
using SoPro24Team03.Data;
using SoPro24Team03.Models;
using SoPro24Team03.ViewModels;

namespace SoPro24Team03.UnitTests;

public class ProcedureControllerTest
{
    /*//pvb
    [Fact]
    public async void renameProcedure()
    {
        // Arrange
        var procRepoMock = new Mock<IProcedureRepository>();
        procRepoMock.Setup(repo => repo.ToList())
            .ReturnsAsync(GetProcedures());

        var userRepoMock = new Mock<IUserRepository>();
        userRepoMock.Setup(repo => repo.ToList())
            .ReturnsAsync(GetUsers());

        var procTemplRepoMock = new Mock<IProcedureTemplateRepository>();
        procTemplRepoMock.Setup(repo => repo.ToList())
            .ReturnsAsync(GetProcedureTemplates());

        var depRepoMock = new Mock<IDepartmentRepository>();
        depRepoMock.Setup(repo => repo.ToList())
            .ReturnsAsync(GetDepartments());

        var controller = new ProcedureController(procRepoMock.Object, 
                                            userRepoMock.Object, 
                                            procTemplRepoMock.Object, 
                                            depRepoMock.Object);

        // Act
        var create = await controller.Create(1);

        // Assert
        //Assert.True(result);
    }

    //pvb
    private List<Procedure> GetProcedures()
    {
        var procedures = new List<Procedure>()
        {
            new Procedure()
            {
                Name = "Test-Procedure"
            }
        };
        return procedures;
    }

    //pvb
    private List<User> GetUsers()
    {
        var users = new List<User>()
        {
            new User() 
            {
                UserName = "Testnutzer",
                FirstName = "Test",
                LastName = "Nutzer",
                DateOfBirth = DateAndTime.Now.AddYears(-20),
                ContractStart = DateAndTime.Now.AddYears(-2),
                ContractType = ContractType.permanent,
                EmailAddress = "testnutzer@testmailprovider.tld",
                PasswordHash = ""
            }
        };
        return users;
    }

    //pvb
    private List<ProcedureTemplate> GetProcedureTemplates()
    {
        var procTempls = new List<ProcedureTemplate>()
        {
            new ProcedureTemplate()
            {
                Id = 1,
                Name = "Testvorlage",
                IsArchived = false,
                TaskTemplates = new List<TaskTemplate>()
                {
                    new TaskTemplate()
                    {
                        Name = "Testaufgabe",
                        IsArchived = false,
                        DueType = DueType.before_2weeks
                    }
                }
            }
        };

        return procTempls;
    }

    //pvb
    private List<Department> GetDepartments()
    {
        var departments = new List<Department>()
        {
            new Department()
            {
                Name = "Testabteilung"
            }
        };

        return departments;
    }*/

}