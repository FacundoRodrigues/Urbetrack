namespace NetExam.Entities
{
    using NetExam.Abstractions;
    using NetExam.Dto;
    using System;

    public class Booking : IBooking
    {
        public DateTime DateTime { get; set; }
        public int Hours { get; set; }
        public string UserName { get; set; }
        public Location Location { get; set; }
        public Office Office { get; set; }

        public static explicit operator Booking(BookingRequest bookingRequest)
        {
            Location location = new Location() 
            {
                Name = bookingRequest.LocationName
            };

            Office office = new Office()
            {
                Name=bookingRequest.OfficeName,
                LocationName =bookingRequest.LocationName
            };

            Booking booking = new Booking() 
            {
                Location = location,
                    Office = office,
                        UserName = bookingRequest.UserName,
                            DateTime = bookingRequest.DateTime,
                            Hours = bookingRequest.Hours
            };

            return booking;
        }
    }
}
