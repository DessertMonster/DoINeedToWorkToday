using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoINeedToWork.Api.Settings
{
    public interface IDataSourceSettings
    {
        string Alberta { get; }
    }

    public class DataSourceSettings : IDataSourceSettings
    {
        private readonly IConfigurationSection _holidayLinksSection;

        public DataSourceSettings(IConfiguration configuration)
        {
            _holidayLinksSection = configuration.GetSection("HolidayLinks");
        }
        public string Alberta => _holidayLinksSection.GetValue<string>(nameof(IDataSourceSettings.Alberta));
    }
}
