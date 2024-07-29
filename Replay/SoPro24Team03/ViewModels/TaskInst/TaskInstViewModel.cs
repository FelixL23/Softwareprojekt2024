using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using SoPro24Team03.Models;

namespace SoPro24Team03.ViewModels
{
    public class TaskInstViewModel
    {
        public List<TaskInst> Tasks = new List<TaskInst>();

        public List<SelectListItem>? MyProcedures {get; set;}

        public Boolean? isAdmin {get; set;}
        

        [Display(Name = "Name")]
        public String? TaskName;

        [Display(Name = "Vorgang")]
        public int? ProcedureId;

        [Display(Name = "nur persönlich zuständig")]
        public int? level;
    }
}
