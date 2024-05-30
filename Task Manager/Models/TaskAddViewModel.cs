using System.ComponentModel.DataAnnotations;

namespace Task_Manager.Models
{
    public class TaskAddViewModel
    {
        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; }
    }
}
