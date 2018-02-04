using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using DoINeedToWork.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DoINeedToWork.Api.Controllers
{
    [Route("api/[controller]")]
    public class HolidaysController
    {
        public IActionResult GetHolidayForToday()
        {
            //SystemTime.LocalNow = () => new DateTime(2018, 2, 19);

            var html = @"https://www.alberta.ca/alberta-general-holidays.aspx";
            HtmlWeb web = new HtmlWeb();
            var htmlDoc = web.Load(html);

            var statRows = htmlDoc.DocumentNode.SelectNodes("//*[@id='table1']/table")[0].Descendants("tr");
            var optionalRows = htmlDoc.DocumentNode.SelectNodes("//*[@id='table1']/table")[1].Descendants("tr");
            var statDateArray = statRows.First().Descendants("th").ToArray();
            var index = Array.FindIndex(statDateArray, EqualCurrentYear);

            var statHolidays = GetHolidaysFromTableRows(statRows, HolidayType.Statutory, index);

            var optionalHolidays = GetHolidaysFromTableRows(optionalRows, HolidayType.Optional, index);

            var holidayForToday = statHolidays.Concat(optionalHolidays).SingleOrDefault(h => h.Date == SystemTime.LocalNow().Date);

            if (holidayForToday != null)
            {
                return new ObjectResult(holidayForToday);
            }
            return new NotFoundObjectResult("Today is not a holiday.");
        }

        private IEnumerable<Holiday> GetHolidaysFromTableRows(IEnumerable<HtmlNode> rows, HolidayType type, int index)
        {
            return rows.Skip(1).Select(r => new Holiday
            {
                Name = r.Descendants("td").ToArray()[0].InnerText.Replace("\n", "").Replace("\t", ""),
                Date = DateTime.Parse(r.Descendants("td").ToArray()[index].InnerText.Replace("&nbsp;", "")),
                Location = Province.Alberta.ToString(),
                Type = type.ToString()
            });
        }

        private bool EqualCurrentYear(HtmlNode n)
        {
            return n.InnerText == DateTime.Today.Year.ToString() ? true : false;
        }
    }
}
