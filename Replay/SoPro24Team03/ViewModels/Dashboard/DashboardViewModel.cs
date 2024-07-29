using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SoPro24Team03.Models;

namespace SoPro24Team03.ViewModels;

public class DashboardViewModel
{
    public List<Procedure> Procedures { get; set; } = null!;

    public List<TaskInst> TaskInsts { get; set; } = null!;

    public String? message { get; set; } = null;
}
