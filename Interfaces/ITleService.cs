using MyConsoleApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyConsoleApp.Interfaces
{
    public interface ITleService
    {
        Task<List<Tle>> GetAllTlesAsync();
        void SaveTlesToFile(List<Tle> tles, string filePath);
        List<Tle> GetTlesFromFile(string filePath);
        bool CheckIfSatelliteExistsInFile(string filePath, long satelliteId);
    }
}
