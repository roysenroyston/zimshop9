using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ShopMate.Models
{
    [TrackChanges]
    public class Bank
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int AccountNumber { get; set; }
    }
}