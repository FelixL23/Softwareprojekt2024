using Microsoft.AspNetCore.Mvc;
using Moq;
using SoPro24Team03.Controllers;
using SoPro24Team03.Data;
using SoPro24Team03.Models;
using SoPro24Team03.ViewModels;

namespace SoPro24Team03.UnitTests;

// Made by Fabio
public class TaskInstControllerTest
{
    [Theory]
    [InlineData(TaskRespType.role, 1, null, null, null)]
    [InlineData(TaskRespType.proc_refer, null, 1, 1, 3)]
    [InlineData(TaskRespType.proc_resp, null, 1, 2, 1)]
    [InlineData(TaskRespType.user, null, null, null, 2)]
    public void CreateTaskResponsible_WithTypes_ReturnsTrue(TaskRespType type, int? roleId, int? referId, int? respId, int? userId)
    {
        TaskResponsible taskResponsible = new TaskResponsible(
            type,
            roleId,
            referId,
            respId,
            userId
        );

        Assert.IsAssignableFrom<TaskResponsible>(taskResponsible);
    }

    [Fact]
    public void CreateTaskResponsible_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new TaskResponsible(
            TaskRespType.user,
            1,
            null,
            null,
            null
        ));
    }

    [Fact]
    public void CreateTaskInst_FromTaskTemplate_ReturnsTrue()
    {
        TaskResponsible tr = new TaskResponsible(
            TaskRespType.role,
            0,
            0,
            0,
            0
        );

        TaskTemplate taskTemplate = new TaskTemplate
        {
            Id = 1,
            Name = "SSH-Key erstellen",
            DueType = DueType.at_start,
            TaskResponsible = tr,
            TaskResponsibleId = 1
        };

        Procedure procedure = new Procedure
        {
            Id = 2,
            TargetDate = new DateTime(2024, 12, 24),
            ReferId = 1,
            RespId = 1
        };

        TaskInst newTaskInst = taskTemplate.CreateTaskInstance(procedure);

        TaskInst expectedTaskInst = new TaskInst
        {
            Name = taskTemplate.Name,
            ProcedureId = procedure.Id,
            TemplateId = taskTemplate.Id,
            TargetDate = procedure.TargetDate,
            Resp = tr
        };

        Assert.True(
            expectedTaskInst.Name == newTaskInst.Name &&
            expectedTaskInst.ProcedureId == newTaskInst.ProcedureId &&
            expectedTaskInst.TargetDate == newTaskInst.TargetDate &&
            expectedTaskInst.TemplateId == newTaskInst.TemplateId &&
            expectedTaskInst.Resp.TaskRespType == newTaskInst.Resp.TaskRespType &&
            expectedTaskInst.Resp.Role == newTaskInst.Resp.Role &&
            expectedTaskInst.Resp.User == newTaskInst.Resp.User
        );
    }

    [Fact]
    public void CreateTaskInst_FromTaskTemplateWithCustomDays_ReturnsTrue()
    {
        TaskResponsible tr = new TaskResponsible(
            TaskRespType.role,
            0,
            0,
            0,
            0
        );

        TaskTemplate taskTemplate = new TaskTemplate
        {
            Id = 1,
            Name = "SSH-Key erstellen",
            DueType = DueType.custom,
            CustomDays = -1,
            TaskResponsible = tr,
            TaskResponsibleId = 1
        };

        Procedure procedure = new Procedure
        {
            Id = 2,
            TargetDate = new DateTime(2024, 12, 24),
            ReferId = 1,
            RespId = 1
        };

        TaskInst newTaskInst = taskTemplate.CreateTaskInstance(procedure);

        TaskInst expectedTaskInst = new TaskInst
        {
            Name = taskTemplate.Name,
            ProcedureId = procedure.Id,
            TemplateId = taskTemplate.Id,
            TargetDate = procedure.TargetDate.AddDays(-1),
            Resp = tr
        };

        Assert.True(
            expectedTaskInst.Name == newTaskInst.Name &&
            expectedTaskInst.ProcedureId == newTaskInst.ProcedureId &&
            expectedTaskInst.TargetDate == newTaskInst.TargetDate &&
            expectedTaskInst.TemplateId == newTaskInst.TemplateId &&
            expectedTaskInst.Resp.TaskRespType == newTaskInst.Resp.TaskRespType &&
            expectedTaskInst.Resp.Role == newTaskInst.Resp.Role &&
            expectedTaskInst.Resp.User == newTaskInst.Resp.User
        );
    }

    //Made by Celina
    [Fact]
    public void isOverdue_StatusNotDone_TargetDateInPast_ReturnsTrue()
    {
        // Arrange
        var taskItem = new TaskInst
        {
            Status = CompletionStatus.backlog,
            TargetDate = DateTime.Now.AddDays(-1)
        };

        // Act
        var result = taskItem.isOverdue();

        // Assert
        Assert.True(result);
    }

    //Made by Celina
    [Fact]
    public void isOverdue_StatusNotDone_TargetDateInFuture_ReturnsFalse()
    {
        // Arrange
        var taskItem = new TaskInst
        {
            Status = CompletionStatus.backlog,
            TargetDate = DateTime.Now.AddDays(1)
        };

        // Act
        var result = taskItem.isOverdue();

        // Assert
        Assert.False(result);
    }

    //Made by Celina
    [Fact]
    public void isOverdue_StatusDone_TargetDateInPast_ReturnsFalse()
    {
        // Arrange
        var taskItem = new TaskInst
        {
            Status = CompletionStatus.done,
            TargetDate = DateTime.Now.AddDays(-1)
        };

        // Act
        var result = taskItem.isOverdue();

        // Assert
        Assert.False(result);
    }

    //Made by Celina
    [Fact]
    public void isOverdue_StatusDone_TargetDateInFuture_ReturnsFalse()
    {
        // Arrange
        var taskItem = new TaskInst
        {
            Status = CompletionStatus.done,
            TargetDate = DateTime.Now.AddDays(1)
        };

        // Act
        var result = taskItem.isOverdue();

        // Assert
        Assert.False(result);
    }

    //Made by Celina
    [Fact]
    public void isOverdue_StatusNotDone_TargetDateIsNow_ReturnsTrue()
    {
        // Arrange
        var taskItem = new TaskInst
        {
            Status = CompletionStatus.backlog,
            TargetDate = DateTime.Now
        };

        // Act
        var result = taskItem.isOverdue();

        // Assert
        Assert.True(result);
    }
}
