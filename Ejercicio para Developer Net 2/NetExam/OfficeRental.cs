namespace NetExam
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using NetExam.Abstractions;
    using NetExam.Dto;
    using NetExam.Entities;

    public class OfficeRental : IOfficeRental
    {
        public List<Location> _locations { get; set; }
        public List<Office> _offices { get; set; }
        public List<Booking> _booking { get; set; }

        public OfficeRental()
        {
            _locations = new List<Location>();
            _offices = new List<Office>();
            _booking = new List<Booking>();
        }

        #region Interface Implemented Methods

        public void AddLocation(LocationSpecs locationSpecs)
        {
            try
            {
                if (locationSpecs != null)
                {
                    if (!ValidateIfLocationNameExist(locationSpecs.Name))
                    {
                        Location location = (Location)locationSpecs;
                        _locations.Add(location);
                    }
                    else
                    {
                        throw new ArgumentException(nameof(locationSpecs.Name));
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
                    if (ValidateIfLocationNameExist(officeSpecs.LocationName))
                    {
                        Office office = (Office)officeSpecs;

                        Location location = GetLocationByName(officeSpecs.LocationName);
                        location.Offices.Add(office);
                        office.Location = location;

                        _offices.Add(office);
                    }
                    else
                    {
                        throw new ArgumentException(nameof(officeSpecs.Name));
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

                        _booking.Add(booking);
                        Office office = _offices.FirstOrDefault(x => x.Name.Contains(bookingRequest.OfficeName));

                        office.Bookings.Add(booking);
                    }
                    else
                    {
                        throw new ArgumentException("Office not available");
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

                foreach (var loc in _locations)
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

        private bool ValidateIfLocationNameExist(string locationName)
        {
            bool exist = _locations.Any(x => x.Name.Contains(locationName));

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
            return _locations.FirstOrDefault(x => x.Name.Contains(LocationName));
        }

        private IEnumerable<Office> GetOfficesByLocationName(string locationName)
        {
            return _offices.Where(x => x.LocationName.Contains(locationName));
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