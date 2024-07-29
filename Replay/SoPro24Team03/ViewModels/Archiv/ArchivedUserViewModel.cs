using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SoPro24Team03.Models;

namespace SoPro24Team03.ViewModels;

    public class ArchivedUserViewModel
    {
         //made by Felix
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }

        public bool isArchived { get; set; }
    }

