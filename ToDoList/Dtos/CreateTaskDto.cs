using System.ComponentModel.DataAnnotations;

namespace ToDoList.Dtos
{
    public class CreateTaskDto : IValidatableObject
    {
        [Required]
        public string Title { get; set; }

        public string Description { get; set; }
        [Required]
        public string Status { get; set; }
        [Required]
        public DateTime? DueDate { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (DueDate.HasValue && DueDate.Value.Date < DateTime.Now.Date)
            {
                yield return new ValidationResult(
                    "Due date cannot be in the past.",
                    new[] { nameof(DueDate) });
            }
        }
    }
}
