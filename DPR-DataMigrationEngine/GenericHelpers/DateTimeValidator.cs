using System;

namespace DPR_DataMigrationEngine.GenericHelpers
{
    public class DateTimeValidator
    {
        public bool IsValidTime(string s, out string error)
        {
          
            if (!string.IsNullOrEmpty(s))
            {
                TimeSpan outTime;
                var result = TimeSpan.TryParse(s, out outTime);
                if (!result)
                {
                    error = "Invalid Time supplied";
                    return false;
                }
           
                error = "";
                return true;
            }
         
            error = "Empty Time input";
            return false;
            
        }

        public bool IsValidDate(string s, out string error)
        {
            if (!string.IsNullOrEmpty(s))
            {
                DateTime outTime;
                var result = DateTime.TryParse(s, out outTime);
                if (!result)
                {
                    error = "Invalid Date supplied";
                    return false;
                }

                error = "";
                return true;
            }

            error = "Empty Date input";
            return false;

        }
    }
    
}