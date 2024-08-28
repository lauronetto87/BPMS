
namespace SatelittiBpms.Models.Integration.Signer
{
    /// <summary> 
    /// Reminder frequency: 
    /// - 1: Do not send 
    /// - 2: Every single day 
    /// - 3: Every 2 days 
    /// - 4: Every 4 days 
    /// - 5: Every 7 days 
    /// </summary> 
    public enum ReminderFrequencyEnum
    {
        DO_NOT_SEND = 1,
        EVERY_DAY = 2,
        EVERY_2_DAYS = 3,
        EVERY_4_DAYS = 4,
        EVERY_7_DAYS = 5
    }
}
