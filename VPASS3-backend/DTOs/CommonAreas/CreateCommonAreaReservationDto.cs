﻿using System.ComponentModel.DataAnnotations;

namespace VPASS3_backend.DTOs.CommonAreas
{
    public class CreateCommonAreaReservationDto
    {
        [Required] public int IdCommonArea { get; set; }
        public DateTime? ReservationStart { get; set; }
        public TimeSpan? ReservationTime { get; set; }
        [Required] public int IdPersonReservedBy { get; set; }

        //public List<int>? GuestIds { get; set; } = new();
        public int? GuestsNumber { get; set; } = 0;

    }
}
