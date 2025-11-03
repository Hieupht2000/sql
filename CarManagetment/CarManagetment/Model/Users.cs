namespace CarManagetment.Model
{
    public class Users
    {
        public int user_id { get; set;}
        public string user_name { get; set; }
        public string email { get; set; }
        public string Password_hash { get; set; }
        public string Role { get; set; }
    }
}
