using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.WindowsAzure.Storage.Table;

namespace AzureBlobApp.Model
{

    public class Student :TableEntity
    {
       
        public string Name { get; set; }
        public string Address { get; set; }
        public int Age { get; set; }
        public bool IsActive { get; set; }

        public Student()
        {
            this.IsActive = true;
        }
    }
}
