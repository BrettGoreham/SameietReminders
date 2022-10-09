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
            [TimerTrigger("0 0 10 * * Tue")] TimerInfo myTimer, ILogger log)
        {
            var weekNumber = ISOWeek.GetWeekOfYear(DateTime.Now);

            var variables = Environment.GetEnvironmentVariables().Keys.Cast<string>();

            var users = variables.Where(s => s.EndsWith("_user") && !s.StartsWith("APPSETTING_")).ToList();
            log.LogInformation($"got {users.Count} users ");

            foreach (var user in users)
            {
                var weekString = Environment.GetEnvironmentVariable($"{user.Substring(0, user.Length - 5)}_weeks");
                log.LogInformation($"{user} weeks = {weekString}");

                var responsibleWeeks = weekString.Split(",").Select(s => int.Parse(s));

                if (responsibleWeeks.Contains(weekNumber))
                {
                    log.LogInformation($"sending email to {user}");
                    await EmailSender.SendEmailToAddresses(Environment.GetEnvironmentVariable(user).Split(",").ToList());
                }
            } 
        }
    }
}
