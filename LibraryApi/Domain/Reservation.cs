﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryApi.Domain
{
    public enum ReservationStatus { Pending, Approved, Cancelled }

    public class Reservation
    {
        public int Id { get; set; }
        public string For { get; set; }
        public DateTime ReservactionCreated { get; set; }
        public string Books { get; set; } //"1,2,3,4"

        public ReservationStatus Status { get; set; }


    }
}
