namespace NetExam.Entities
{
    using NetExam.Abstractions;
    using NetExam.Dto;
    using System.Collections.Generic;

    public class Location : ILocation
    {
        public string Name { get; set; }
        public string Neighborhood { get; set; }
        public List<Office> Offices { get; set; }

        public static explicit operator Location(LocationSpecs locationSpecs)
        {
            Location location = new Location()
            {
                Name = locationSpecs.Name,
                    Neighborhood = locationSpecs.Neighborhood,
                        Offices = new List<Office>()
            };

            return location;
        }
    }
}
