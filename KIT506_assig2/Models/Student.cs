﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAP.Models
{
    public class Student : Researcher
    {
        public string Degree { get; set; }
        public Staff Supervisor
        {
            get;
            set;
        }
    }
}
