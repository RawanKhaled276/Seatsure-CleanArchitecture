using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seatsure.Domain
{
    public class Event
    {
        [Key]
        public Guid id { get; set ; } = new Guid();

        // forigen key though onModelCreating in DbContext,
        // Forigen key annotation
        public Guid OrganizerId { get; set; }

        public User Organizer { get; set; }

        [MaxLength(50)]
        [Required]
        public string Title { get; set; }

        [MaxLength(500)]
        [Required]
        public string Description { get; set; } 

        public string VenueName { get; set; }

        public DateTime StartsAtutc { get; set; }

        // even status

        public EventStatus Status { get; set; } = EventStatus.Draft;
        public DateTime CreatedAutUtc { get; set; }

        // navigational property, one to many relationship, one event can have many tickets
        public List<TicketType> Tickets { get; set; } = new List<TicketType>(); 

    }
}
