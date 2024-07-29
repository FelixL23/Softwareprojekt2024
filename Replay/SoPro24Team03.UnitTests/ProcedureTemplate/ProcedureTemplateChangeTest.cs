using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using Moq;
using Newtonsoft.Json;
using SoPro24Team03.Controllers;
using SoPro24Team03.Data;
using SoPro24Team03.Models;
using SoPro24Team03.ViewModels;

namespace SoPro24Team03.UnitTests;

public class ProcedureTemplateChangeTest
{
    // Made by Philip
    [Fact]
    public void changeProcedureWithRunningTaskInsts()
    {
        //Arrange
        var user = new Models.User()
        {
            Id = 1,
            UserName = "Testnutzer",
            FirstName = "Test",
            LastName = "Nutzer",
            DateOfBirth = DateAndTime.Now.AddYears(-20),
            ContractStart = DateAndTime.Now.AddYears(-2),
            ContractType = ContractType.permanent,
            EmailAddress = "testnutzer@testmailprovider.tld",
            PasswordHash = ""
        };

        var dep = new Department()
        {
            Id = 1,
            Name = "Abteilung"
        };

        var tasks = new List<TaskTemplate>()
        {
            new TaskTemplate()
            {
                Id = 1,
                Name = "Testaufgabe 1",
                IsArchived = false,
                DueType = DueType.asap,
                TaskResponsible = new TaskResponsible(TaskRespType.proc_resp)
            },
            new TaskTemplate()
            {
                Id = 2,
                Name = "Testaufgabe 2",
                IsArchived = false,
                DueType = DueType.asap,
                TaskResponsible = new TaskResponsible(TaskRespType.proc_resp)
            },
            new TaskTemplate()
            {
                Id = 3,
                Name = "Testaufgabe 3",
                IsArchived = false,
                DueType = DueType.asap,
                TaskResponsible = new TaskResponsible(TaskRespType.proc_resp)
            }
        };

        var procedureTemplate = new ProcedureTemplate
        {
            Id = 1,
            Name = "Beispiel-Vorlage",
            IsArchived = false,
            TaskTemplates = tasks,
            OrderedTaskTemplateIds = "1,2,3"
        };

        var procedure = procedureTemplate.CreateProcedure(
            "Beispiel-Vorlage", 
            DateTime.Now.AddDays(7), 
            user.Id, 
            user.Id, 
            ContractType.permanent, 
            dep.Id);

        var procedureCopy = DeepCopy(procedure);

        //Act
        procedureTemplate.Name = "anderer Name";
        procedureTemplate.OrderedTaskTemplateIds = "1,2";

        
        //Assert
        var obj1Str = JsonConvert.SerializeObject(procedure);
        var obj2Str = JsonConvert.SerializeObject(procedureCopy);
        Assert.Equal(obj1Str, obj2Str);
    }

    // Made by Philip
    private Procedure DeepCopy(Procedure input)
    {
        var newProcedure = new Procedure
        {
            Id = input.Id,
            Name = input.Name,
            TargetDate = input.TargetDate,
            IsArchived = input.IsArchived,
            NumTasksDone = input.NumTasksDone,
            NumTasksTotal = input.NumTasksTotal,
            CreatedContractType = input.CreatedContractType,
            OrderedTaskInstIds = input.OrderedTaskInstIds,
            TemplateId = input.TemplateId,
            Template = input.Template,
            FutureDepartmentId = input.FutureDepartmentId,
            FutureDepartment = input.FutureDepartment,
            RespId = input.RespId,
            Resp = input.Resp,
            ReferId = input.ReferId,
            Refer = input.Refer
        };

        foreach (var taskInst in input.TaskInsts)
        {
            var newTaskInst = new TaskInst
            {
                Id = taskInst.Id,
                Name = taskInst.Name,
                TargetDate = taskInst.TargetDate,
                IsArchived = taskInst.IsArchived,
                Status = taskInst.Status,
                Notes = taskInst.Notes.Select(note => new Note
                {
                    Id = note.Id,
                    CreationDate = note.CreationDate,
                    VisibleToOthers = note.VisibleToOthers,
                    Title = note.Title,
                    Text = note.Text,
                    AuthorId = note.AuthorId,
                    Author = note.Author,
                    TaskInstId = note.TaskInstId,
                    TaskInst = note.TaskInst 
                }).ToList(),
                ProcedureId = taskInst.ProcedureId,
                Procedure = taskInst.Procedure,
                TemplateId = taskInst.TemplateId,
                Template = taskInst.Template,
                RespId = taskInst.RespId,
                Resp = taskInst.Resp
            };

            newProcedure.TaskInsts.Add(newTaskInst);
        }

        return newProcedure;
    }
}