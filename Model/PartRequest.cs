using System.ComponentModel.DataAnnotations;
namespace backend.Model
{
    public class PartRequest
    {
        [Key]
        public int RequestId { get; set; }
        public int UserId { get; set; }

        public int? PartId { get; set; }
        public string PartName { get; set; }

        public string Status { get; set; }
        public DateTime RequestDate { get; set; }

        public User User { get; set; }
        public Part Part { get; set; }
    }
}