using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seatsure.Domain
{
    public class Reservation
    {
        public Guid id { get; set; }

        public Guid TicketTypeId { get; set; }

        public Guid UserId { get; set; }

        public TicketType ticket { get; set; }

        public User user { get; set; }

        // ticket considered as a physical 
        // ticket, id, manyquantity, 
        // not different ticeketType
        // user within the reservation can boook from one to many tickets of the same type 

        public int Quantity { get; set; }

        public DateTime HoldExpiresAtUTc { get; set; }

        public DateTime CreatedAtUtc { get; set; }

        public DateTime? ConfirmedAtUtc { get; set; }
    }
}
