using System;

namespace AuthWebAPI.Models
{
    public class UserModel
    {
        public Guid Id { get; set; }           // If used as Guid in controller
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }  // Optional if needed
        public string Roles { get; set; }         // Optional
    }
}
//namespace AuthWebAPI.Models
//{
//    public class UserModel
//    {
//        public int Id { get; set; }
//        public string Username { get; set; }
//        public string PasswordHash { get; set; }
//        public string Email { get; set; }
//        public string Roles { get; set; }
//    }
//}
