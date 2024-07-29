using System;
using System.Collections.Generic;
using SoPro24Team03.Models;

namespace SoPro24Team03.ViewModels
{
    public class ArchivedProcedureViewModel
    {
         //made by Felix
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime TargetDate { get; set; }
        public bool IsArchived { get; set; }

        public User Resp { get; set; }
        public int RespId { get; set; }
        public List<TaskInst> TaskInsts { get; set; }
    }
}
