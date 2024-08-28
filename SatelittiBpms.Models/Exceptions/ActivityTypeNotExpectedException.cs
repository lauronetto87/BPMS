using System;

namespace SatelittiBpms.Models.Exceptions
{
    public class ActivityTypeNotExpectedException : Exception
    {
        public string ActivityTypeName { get; private set; }

        public ActivityTypeNotExpectedException(string activityTypeName)
        {
            ActivityTypeName = activityTypeName;
        }

    }
}
