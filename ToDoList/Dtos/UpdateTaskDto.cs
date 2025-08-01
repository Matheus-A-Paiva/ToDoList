using System.ComponentModel.DataAnnotations;

namespace ToDoList.Dtos
{
    public class UpdateTaskDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string Description { get; set; }
        [Required]
        public string Status{ get; set; }
        [Required]
        public DateTime? DueDate { get; set; }       
        
    }
}
