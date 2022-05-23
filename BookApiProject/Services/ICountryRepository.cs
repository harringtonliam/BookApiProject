using BookApiProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookApiProject.Services
{
    public interface ICountryRepository
    {
        ICollection<Country> GetCountries();

        Country GetCountry(int CountryId);

        Country GetCountryOfAnAuthor(int authorId);

        ICollection<Author> GetAuthorsFromACountry(int countryId);
        bool CountryExists(int countryId);

        bool IsDuplicateCountryName(int countryId, string countryName);

        bool CreateCounty(Country country);
        bool UpdateCountry(Country country);
        bool DeleteCountry(Country country);
        bool Save();

    }
}
