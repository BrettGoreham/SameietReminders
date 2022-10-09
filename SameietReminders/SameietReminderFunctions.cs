using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace SameietReminders
{
    public static class SameietReminderFunctions
    {
        // 0 */5 * * * *
        //0 0 10 * * Tue 
        [FunctionName("GarbargeReminder")]
        public static async Task Run(
            [TimerTrigger("0 0 * * * *")] TimerInfo myTimer, ILogger log)
        {
            var weekNumber = ISOWeek.GetWeekOfYear(DateTime.Now);

            log.LogInformation($"it is {weekNumber}");

            var variables = Environment.GetEnvironmentVariables().Keys.Cast<string>();
            var users = variables.Where(s => s.EndsWith("_user"));

            foreach (var user in users)
            {
                var weekString = Environment.GetEnvironmentVariable($"{user.Substring(5)}_weeks");


                var responsibleWeeks = weekString.Split(",").Select(s => int.Parse(s));

                if (responsibleWeeks.Contains(weekNumber))
                {
                    await EmailSender.SendEmailToAddresses(Environment.GetEnvironmentVariable(user).Split(",").ToList());
                }
            } 
        }
    }
}
