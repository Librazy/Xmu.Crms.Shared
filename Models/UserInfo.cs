namespace Xmu.Crms.Shared.Models
{
    public class UserInfo
    {
        public int Id { get; set; }
        public string Phone { get; set; }
        public string Avatar { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public School School { get; set; }
        public int Gender { get; set; }
        public string Number { get; set; }
        public int Education { get; set; }
        public int Title { get; set; }
        public string Email { get; set; }
    }

    public class Teacher : UserInfo
    {
    }

    public class Student : UserInfo
    {
    }
}