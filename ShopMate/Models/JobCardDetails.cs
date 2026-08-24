using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopMate.Models
{

    public class JobCardDetails
    {
        public JobCard JobCard { get; set; }
        public IEnumerable<JobCardMaterials> JobCardMaterials { get; set; }
        public IEnumerable<JobCardServices> JobCardServices { get; set; }
    }
}