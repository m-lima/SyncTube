using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SyncTube.ViewModels.Room
{
    public class CreateRoomViewModel
    {
        [Required]
        [Display(Name = "Name:")]
        public string Name { get; set; }

        [Display(Name = "Description:")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "YouTube video ID:")]
        public string VideoId { get; set; }

        [Required]
        [Display(Name = "Duration:")]
        public int Duration { get; set; }
    }
}
