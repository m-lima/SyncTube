using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SyncTube.Models;

namespace SyncTube.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class Room
    {
        [Key]
        public long Id { get; set; }
        public ApplicationUser CreatedBy { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string VideoId { get; set; }
        public int Duration { get; set; }
    }
}
