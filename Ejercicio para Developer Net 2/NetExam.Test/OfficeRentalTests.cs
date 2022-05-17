namespace NetExam.Test
{
    using System;
    using System.Linq;
    using NetExam.Abstractions;
    using NetExam.Dto;
    using Xunit;

    public class OfficeRentalTests
    {
        public const string LocationName = "Urbetrack";
        public const string Neighborhood = "Almagro";
        public const string OfficeName = "Sala Azul";
        public const int OfficeMaxCapacity = 8;
        public const string UserName = "ceo@urbetrack.com";
        public const int BookingHours = 2;
        public static DateTime BookingDate => new DateTime(2021, 1, 1, 9, 0, 0);

        public LocationSpecs DefaultLocationSpecs => new LocationSpecs(LocationName, Neighborhood);
        public OfficeSpecs DefaultOfficeSpecs => new OfficeSpecs(LocationName, OfficeName, OfficeMaxCapacity);
        public BookingRequest DefaultBookingRequest => new BookingRequest(LocationName, OfficeName, BookingDate, BookingHours, UserName);

        protected IOfficeRental OfficeRental = new OfficeRental();

        //done
        [Fact]
        public void ShouldAddALocation()
        {
            OfficeRental.AddLocation(DefaultLocationSpecs);

            Assert.Equal(LocationName, OfficeRental.GetLocations().Single().Name);
        }

        //done
        [Fact]
        public void ShouldThrowWhenLocationNameAlreadyExists()
        {
            OfficeRental.AddLocation(DefaultLocationSpecs);

            Assert.ThrowsAny<Exception>(() =>
            {
                OfficeRental.AddLocation(DefaultLocationSpecs);
            });
        }

        //done
        [Fact]
        public void ShouldAddOffice()
        {
            OfficeRental.AddLocation(DefaultLocationSpecs);

            OfficeRental.AddOffice(DefaultOfficeSpecs);

            Assert.Equal(OfficeName, OfficeRental.GetOffices(LocationName).Single().Name);
        }

        //done
        [Fact]
        public void ShouldThrowWhenAddingOfficeToUnexistingLocation()
        {
            Assert.ThrowsAny<Exception>(() =>
            {
                OfficeRental.AddOffice(DefaultOfficeSpecs);
            });
        }

        //done
        [Fact]
        public void ShouldBookAnOffice()
        {
            OfficeRental.AddLocation(DefaultLocationSpecs);
            OfficeRental.AddOffice(DefaultOfficeSpecs);

            OfficeRental.BookOffice(DefaultBookingRequest);

            Assert.Equal(BookingDate, OfficeRental.GetBookings(LocationName, OfficeName).Single().DateTime);
        }

        //done
        [Fact]
        public void ShouldThrowWhenBookingAnAlreadyTakenOffice()
        {
            OfficeRental.AddLocation(DefaultLocationSpecs);
            OfficeRental.AddOffice(DefaultOfficeSpecs);
            OfficeRental.BookOffice(DefaultBookingRequest);

            Assert.ThrowsAny<Exception>(() =>
            {
                OfficeRental.BookOffice(new BookingRequest(LocationName, OfficeName, BookingDate.AddHours(1), BookingHours, UserName));
            });
        }

        //done
        [Theory]
        [InlineData(18, "Palermo", null, "Centro 2", "1")] //3ro
        [InlineData(6, "Centro", new[] { "wi-fi", "tv" }, "Centro 2", "3")] //4to
        [InlineData(2, null, null, "Centro 1", "4")] //2do
        [InlineData(2, null, new[] { "proyector", "catering" }, "Centro 2", "1")] //1ro
        [InlineData(30, null, null, null, null)] //5to
        public void ShouldGiveOfficeSuggestions(int capacityNeeded, string preferedNeigborHood, string[] resourcesNeeded, string expectedLocation, string expectedOffice)
        {
            OfficeRental.AddLocation(new LocationSpecs("Centro 1", "Centro"));
            OfficeRental.AddOffice(new OfficeSpecs("Centro 1", "1", 12, new[] { "wi-fi", "proyector", "cafe" }));
            OfficeRental.AddOffice(new OfficeSpecs("Centro 1", "2", 8, new[] { "wi-fi", "tv", "cafe" }));
            OfficeRental.AddOffice(new OfficeSpecs("Centro 1", "3", 8, new[] { "wi-fi" }));
            OfficeRental.AddOffice(new OfficeSpecs("Centro 1", "4", 4, new[] { "tv" }));

            OfficeRental.AddLocation(new LocationSpecs("Centro 2", "Centro"));
            OfficeRental.AddOffice(new OfficeSpecs("Centro 2", "1", 20, new[] { "wi-fi", "proyector", "cafe", "catering" }));
            OfficeRental.AddOffice(new OfficeSpecs("Centro 2", "2", 6, new[] { "wi-fi", "tv", "cafe" }));
            OfficeRental.AddOffice(new OfficeSpecs("Centro 2", "3", 6, new[] { "wi-fi", "tv" }));

            OfficeRental.AddLocation(new LocationSpecs("Palermo", "Palermo"));
            OfficeRental.AddOffice(new OfficeSpecs("Palermo", "1", 10, new[] { "wi-fi", "tv" }));
            OfficeRental.AddOffice(new OfficeSpecs("Palermo", "2", 8, new[] { "wi-fi", "tv" }));

            var suggestions = OfficeRental.GetOfficeSuggestion(new SuggestionRequest(capacityNeeded, preferedNeigborHood, resourcesNeeded));
            Assert.Equal(expectedLocation, suggestions.FirstOrDefault()?.LocationName);
            Assert.Equal(expectedOffice, suggestions.FirstOrDefault()?.Name);
        }
    }
}