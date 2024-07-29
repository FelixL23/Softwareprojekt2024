using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using SoPro24Team03.Models;


namespace SoPro24Team03.UnitTests;

// Made by Felix
public class ProcedureTemplateTests
{
    [Fact]

    public void CreateProcedure_CreatesNewProcedureCorrectly()
    {
        // Arrange
        var procedureTemplate = new ProcedureTemplate
        {
            Id = 1,
            OrderedTaskTemplateIds = "11,12",
            TaskTemplates = new List<TaskTemplate>
            {
                new TaskTemplate
                {
                    Id = 11,
                    Name = "Task 111",
                    IsArchived = false,
                    ContractTypes = new List<ContractType> { ContractType.permanent },
                    TaskResponsibleId = 3,
                    DueType = DueType.at_start,
					TaskResponsible = new TaskResponsible(
						TaskRespType.proc_resp
					)

                },
                new TaskTemplate
                {
                    Id = 12,
                    Name = "Task 12",
                    IsArchived = false,
                    ContractTypes = new List<ContractType> { ContractType.werkstudent },
                    TaskResponsibleId = 4,
					TaskResponsible = new TaskResponsible(
						TaskRespType.proc_refer
					)
                }
            }
        };

        string name = "Test Procedure";
        DateTime targetDate = DateTime.Now.AddDays(10);
        int respId = 3;
        int referId = 5;
        ContractType? createdContractType = ContractType.permanent;
        int? futureDepartmentId = 1;

        // Act
        var result = procedureTemplate.CreateProcedure(name, targetDate, respId, referId, createdContractType, futureDepartmentId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(name, result.Name);
        Assert.Equal(targetDate, result.TargetDate);
        Assert.Equal(respId, result.RespId);
        Assert.Equal(referId, result.ReferId);
        Assert.Equal(createdContractType, result.CreatedContractType);
        Assert.Equal(futureDepartmentId, result.FutureDepartmentId);
        Assert.Equal(1, result.TaskInsts.Count);
        Assert.Equal(1, result.NumTasksTotal);
        Assert.Equal(0, result.NumTasksDone);

        var taskInstance1 = result.TaskInsts.FirstOrDefault(ti => ti.Name.Equals("Task 111"));

        Assert.NotNull(taskInstance1);
        Assert.Equal("Task 111", taskInstance1.Name);
        Assert.Equal(targetDate, taskInstance1.TargetDate);
        Assert.Equal(false, taskInstance1.IsArchived);
        Assert.Equal(CompletionStatus.backlog, taskInstance1.Status);
        Assert.Equal(TaskRespType.user, taskInstance1.Resp.TaskRespType);
        Assert.Equal(respId, taskInstance1.Resp.UserId);
        Assert.Equal(TaskRespType.user, taskInstance1.Resp.TaskRespType);
    }

        [Fact]
        public void CreateProcedure_HandlesInvalidInputCorrectly()
        {
            // Arrange
            var procedureTemplate = new ProcedureTemplate
            {
                Id = 1,
                OrderedTaskTemplateIds = "11,12,13",
                TaskTemplates = new List<TaskTemplate>
                {
                    new TaskTemplate
                    {
                        Id = 11,
                        Name = "Task 111",
                        IsArchived = false,
                        ContractTypes = new List<ContractType> { ContractType.permanent },
                        TaskResponsibleId = 3,
                        TaskResponsible = new TaskResponsible(
                            TaskRespType.proc_resp
                        )
                    },
                    new TaskTemplate
                    {
                        Id = 12,
                        Name = "Task 12",
                        IsArchived = false,
                        ContractTypes = new List<ContractType> { ContractType.werkstudent },
                        TaskResponsibleId = 4,
                        TaskResponsible = new TaskResponsible(
                            TaskRespType.proc_refer
                        )
                    }
                }
            };

            string name = "Valid Name";
            DateTime targetDate = DateTime.Now.AddDays(10);
            int respId = 3;
            int referId = 5;
            ContractType? createdContractType = ContractType.permanent;
            int? futureDepartmentId = 1;

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() =>
                procedureTemplate.CreateProcedure(name, targetDate, respId, referId, createdContractType, futureDepartmentId));

            procedureTemplate.OrderedTaskTemplateIds = "11,14"; // 14 does not exist in TaskTemplates
            Assert.Throws<InvalidOperationException>(() =>
                procedureTemplate.CreateProcedure(name, targetDate, respId, referId, createdContractType, futureDepartmentId));
        }
}
