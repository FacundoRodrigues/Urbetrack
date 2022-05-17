namespace NetExam
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using NetExam.Abstractions;
    using NetExam.Dto;
    using NetExam.Entities;
    using NetExam.Repository;

    public class OfficeRental : IOfficeRental
    {
        public Repository<Location> LocationRepository;
        public Repository<Office> OfficeRepository;
        public Repository<Booking> BookingRepository;

        public OfficeRental()
        {
            LocationRepository = new Repository<Location>();
            OfficeRepository = new Repository<Office>();
            BookingRepository = new Repository<Booking>();
        }

        #region Interface Implemented Methods

        public void AddLocation(LocationSpecs locationSpecs)
        {
            try
            {
                if (locationSpecs != null)
                {
                    if (!ValidateLocationNameAlreadyExists(locationSpecs.Name))
                    {
                        Location location = (Location)locationSpecs;
                        LocationRepository.Add(location);
                    }
                    else
                    {
                        throw new ArgumentException("LocationName already exists");
                    }
                }
            }
            catch (ArgumentException)
            {

                throw;
            }
            catch (Exception)
            {

                throw;
            }

        }

        public void AddOffice(OfficeSpecs officeSpecs)
        {
            try
            {
                if (officeSpecs != null)
                {
                    if (ValidateLocationNameAlreadyExists(officeSpecs.LocationName))
                    {
                        Office office = (Office)officeSpecs;

                        Location location = GetLocationByName(officeSpecs.LocationName);
                        location.Offices.Add(office);
                        office.Location = location;

                        OfficeRepository.Add(office);
                    }
                    else
                    {
                        throw new ArgumentException("Adding Office to unexisting Location");
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void BookOffice(BookingRequest bookingRequest)
        {
            try
            {
                if (bookingRequest != null)
                {
                    if (!CheckAvailabilityOffice(bookingRequest))
                    {
                        Booking booking = (Booking)bookingRequest;

                        BookingRepository.Add(booking);
                        Office office = OfficeRepository.GetAll().FirstOrDefault(x => x.Name.Contains(bookingRequest.OfficeName));

                        office.Bookings.Add(booking);
                    }
                    else
                    {
                        throw new ArgumentException("Booking an already taken Office");
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public IEnumerable<IBooking> GetBookings(string locationName, string officeName)
        {
            try
            {
                List<IBooking> bookings = new List<IBooking>();

                Office office = GetOfficeByName(locationName, officeName);

                foreach (var booking in office.Bookings)
                {
                    bookings.Add(booking);
                }

                return bookings;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public IEnumerable<ILocation> GetLocations()
        {
            try
            {
                List<ILocation> locations = new List<ILocation>();

                foreach (var loc in LocationRepository.GetAll())
                {
                    locations.Add(loc);
                }

                return locations;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public IEnumerable<IOffice> GetOffices(string locationName)
        {
            try
            {
                List<IOffice> offices = new List<IOffice>();

                Location location = GetLocationByName(locationName);

                foreach (var offi in location.Offices)
                {
                    offices.Add(offi);
                }

                return offices;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public IEnumerable<IOffice> GetOfficeSuggestion(SuggestionRequest suggestionRequest)
        {
            List<IOffice> offices = new List<IOffice>();
            List<Office> filteredOffices = FilterOffices(suggestionRequest)?.ToList();

            foreach (var office in filteredOffices)
            {
                int countResourcesNeeded = 0;

                foreach (var resources in suggestionRequest.ResourcesNeeded)
                {
                    if (office.AvailableResources.Contains(resources))
                    {
                        countResourcesNeeded++;
                        continue;
                    }
                    else
                    {
                        break;
                    }
                }

                if (suggestionRequest.ResourcesNeeded.Count() == countResourcesNeeded)
                {
                    offices.Add(office);
                }
                else
                {
                    continue;
                }
            }

            return offices;
        }

        #endregion

        #region Private Methods

        private bool ValidateLocationNameAlreadyExists(string locationName)
        {
            bool exist = LocationRepository.GetAll().Any(x => x.Name.Contains(locationName));

            if (exist)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool CheckAvailabilityOffice(BookingRequest bookingRequest)
        {
            bool isAlreadyTaken = false;
            Office office = GetOfficeByName(bookingRequest.LocationName, bookingRequest.OfficeName);

            if (office.Bookings.Count() > 0)
            {
                foreach (var booking in office.Bookings)
                {
                    DateTime startDate = booking.DateTime;
                    DateTime endDate = booking.DateTime.AddHours(bookingRequest.Hours);
                    DateTime solicitedHour = bookingRequest.DateTime;

                    if (IsBetweenStartDateAndEndDate(startDate, endDate, solicitedHour))
                    {
                        isAlreadyTaken = true;
                        break;
                    }
                    else
                    {
                        isAlreadyTaken = false;
                        continue;
                    }
                }
            }
            else
            {
                return false;
            }

            return isAlreadyTaken;
        }

        private bool IsBetweenStartDateAndEndDate(DateTime startDate, DateTime endDate, DateTime solicitedHour)
        {
            bool isBetween = false;

            if (solicitedHour >= startDate && solicitedHour < endDate)
            {
                isBetween = true;
            }
            else
            {
                isBetween = false;
            }

            return isBetween;
        }

        private IEnumerable<Office> FilterOffices(SuggestionRequest suggestionRequest)
        {
            IEnumerable<Office> offices = new List<Office>();
            var _offices = OfficeRepository.GetAll();

            if (suggestionRequest.PreferedNeigborHood != null)
            {
                bool hasMatchWithAnyNeighborhood = _offices.Any(x => x.Location.Neighborhood.Contains(suggestionRequest.PreferedNeigborHood));

                if (hasMatchWithAnyNeighborhood)
                {
                    offices = _offices.Where(x => x.Location.Neighborhood.Contains(suggestionRequest.PreferedNeigborHood) &&
                                                x.MaxCapacity >= suggestionRequest.CapacityNeeded);
                }

                if (offices.Count() == 0)
                {
                    offices = _offices.Where(x => x.Location.Neighborhood.Contains("") &&
                                                x.MaxCapacity >= suggestionRequest.CapacityNeeded);
                }
            }
            else
            {
                offices = _offices.Where(x => x.MaxCapacity >= suggestionRequest.CapacityNeeded);
            }

            return offices.OrderBy(x => x.MaxCapacity).ThenBy(x => x.AvailableResources?.Count());
        }

        private Location GetLocationByName(string LocationName)
        {
            return LocationRepository.GetAll().FirstOrDefault(x => x.Name.Contains(LocationName));
        }

        private IEnumerable<Office> GetOfficesByLocationName(string locationName)
        {
            return OfficeRepository.GetAll().Where(x => x.LocationName.Contains(locationName));
        }

        private Office GetOfficeByName(string locationName, string officeName)
        {
            List<Office> officesByLocation = GetOfficesByLocationName(locationName).ToList();

            Office office = officesByLocation.FirstOrDefault(x => x.Name.Contains(officeName));

            return office;
        }

        #endregion
    }
}