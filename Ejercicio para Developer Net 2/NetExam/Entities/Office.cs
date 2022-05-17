namespace NetExam.Entities
{
    using NetExam.Abstractions;
    using NetExam.Dto;
    using System;
    using System.Collections.Generic;

    public class Office : IOffice
    {
        public string LocationName { get; set; }
        public string Name { get; set; }
        public int MaxCapacity { get; set; }
        public IEnumerable<string> AvailableResources { get; set; }
        public Location Location { get; set; } //debería tener composicion acá?
        public List<Booking> Bookings { get; set; }

        public static explicit operator Office(OfficeSpecs officeSpecs)
        {
            Office office = new Office()
            {
                Name = officeSpecs.Name,
                LocationName = officeSpecs.LocationName,
                AvailableResources = officeSpecs.AvailableResources,
                MaxCapacity = officeSpecs.MaxCapacity,
                Location = new Location(),
                Bookings = new List<Booking>()
            };

            return office;
        }
    }
}
