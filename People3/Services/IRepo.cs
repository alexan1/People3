using People3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace People3.Services
{
    public interface IRepo
    {
        Task<int> AddAsync(Person item);
        Task<IEnumerable<Person>> GetAllAsync();
        Task<PersonViewModel> DetailAsync(int key);        
        Task<IEnumerable<PersonViewModel>> FindAsync(string name);
        Task<int> RateAsync(Person item, int rate);

    }
}
