using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HamadenMonitor.Models;

namespace HamadenMonitor.BL.Interfaces
{
    public interface ILocalizationService
    {
        mst_language GetStringResource(string resourceKey, string langcode);
        List<mst_language> GetAllLanguageResources(string langcode);
    }
}
