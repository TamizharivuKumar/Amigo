using Amigo.Entities;
namespace Amigo.Models
{
    public class Admin
    {
        public int Adminid {get;set;}
        public string? Adminname {get;set;}
        public string? Adminemail {get;set;}
        public string? Adminpassword {get;set;}
        public Role Role {get;set;}
    }
}