namespace CashFlowDashboard.Models.Enums;

// Defines the lifecycle status of an alert.
public enum AlertStatus
{
    // Alert has not been viewed by the user.
    Unread = 0,

    // Alert has been viewed but not acted upon.
    Read = 1,

    // Alert has been dismissed by the user (soft delete).
    Dismissed = 2,

    // Alert has been resolved (underlying issue addressed).
    Resolved = 3
}
