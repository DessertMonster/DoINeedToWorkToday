using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using DoINeedToWork.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Amazon.Lambda.Core;
using System.Threading.Tasks;

namespace DoINeedToWork.Api.Controllers
{
    [Route("api/[controller]")]
    public class HolidaysController
    {
        [HttpGet]
        public async Task<IActionResult> GetHolidayForTodayAsync()
        {
            var systemTimeZones = TimeZoneInfo.GetSystemTimeZones();

            LambdaLogger.Log("System time zones:");

            systemTimeZones.ToList().ForEach(tz => LambdaLogger.Log(tz.Id));

            var now = GetDateTimeInTimeZone(TimeZoneInfo.CreateCustomTimeZone("Mountain Standard Time", new TimeSpan(-7, 0, 0), "(UTC-07:00) Mountain Time (US & Canada)", "Mountain Standard Time"));

            LambdaLogger.Log($"It's {now} in Edmonton now");

            var holidayForToday = (await GetGeneralHolidays()).SingleOrDefault(h => h.Date == now.Date);

            if (holidayForToday != null)
            {
                return new ObjectResult(holidayForToday);
            }
            return new NotFoundObjectResult("Today is not a holiday.");
        }

        private async Task<List<Holiday>> GetGeneralHolidays()
        {
            var html = @"https://www.alberta.ca/alberta-general-holidays.aspx";
            HtmlWeb web = new HtmlWeb();
            var htmlDoc = await web.LoadFromWebAsync(html);

            var statRows = htmlDoc.DocumentNode.SelectNodes("//*[@class='goa-table']/table")[0].Descendants("tr");
            var optionalRows = htmlDoc.DocumentNode.SelectNodes("//*[@class='goa-table']/table")[1].Descendants("tr");
            var statDateArray = statRows.First().Descendants("th").ToArray();
            var index = Array.FindIndex(statDateArray, EqualCurrentYear);

            var statHolidays = GetHolidaysFromTableRows(statRows, HolidayType.Statutory, index);
            var optionalHolidays = GetHolidaysFromTableRows(optionalRows, HolidayType.Optional, index);

            return statHolidays.Concat(optionalHolidays).ToList();
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

        private DateTime GetDateTimeInTimeZone(TimeZoneInfo timeZoneInfo)
        {
            var dateTime = TimeZoneInfo.ConvertTime(SystemTime.LocalNow(), TimeZoneInfo.Local, timeZoneInfo);
            return dateTime;
        }
    }
}
