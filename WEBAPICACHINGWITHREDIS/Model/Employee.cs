﻿using System.ComponentModel.DataAnnotations;

namespace WEBAPICACHINGWITHREDIS.Model
{
    public class Employee
    {
        [Key]
        public int EmployeeId { get; set; }
        [Required]
        public string EmployeeCode { get; set; }
          
        public string Address { get; set; }
        [Required]
        public string Designation { get; set; }
        
    }
}