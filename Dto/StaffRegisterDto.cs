namespace backend.Dto
{
    public class StaffRegisterDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public string Role { get; set; } // STAFF or ADMIN
    }
}
