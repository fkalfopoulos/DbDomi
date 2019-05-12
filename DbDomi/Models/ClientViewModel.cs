using Microsoft.AspNetCore.Http;
using PagedList.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DbDomi.Models
{
    public class ClientViewModel
    {

        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }       
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string UserAvatar { get; set; }
        public DateTime RegisterDate { get; set; }
        public string Comment { get; set; }
        public PagedList<User>Clients { get; set; }
    }
}
