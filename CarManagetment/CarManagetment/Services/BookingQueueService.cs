using CarManagetment.DTOs;
using Hangfire;

namespace CarManagetment.Services
{
    public class BookingQueueService
    {

        private readonly IEmailBookingService _emailService;
        
        public BookingQueueService(IEmailBookingService emailService)
        {
            _emailService = emailService;
        }

        public async Task SendImmediateNotificationsAsync(BookingDTO booking)
        {
            await _emailService.SendBookingConfirmationAsync(
                toEmail: booking.Email,
                customerId: booking.CustomerId,
                customerName: booking.FullName,
                bookingDate: booking.BookingDate,
                licensePlate: booking.LiensePlate,
                technicianName: booking.TechnicianName,
                body: "Your booking has been confirmed."
            );
        }
        public async Task SendEmailDealerAsync(BookingDTO booking)
        {
            await _emailService.SendBookingNotificationToDealerAsync(
                dealerEmail: booking.dealerEmail,
                garageName: booking.FullName,
                customerId: booking.CustomerId,
                customerName: booking.FullName,
                customerEmail: booking.Email,
                bookingDate: booking.BookingDate,
                licensePlate: booking.LiensePlate,
                serviceName: booking.ServiceName
            );
        }

        public void ScheduleReminderEmail(BookingDTO booking)
        {
            var reminderTime = booking.BookingDate.AddMinutes(-5);
            if (reminderTime > DateTime.Now)
            {
                BackgroundJob.Schedule(
                    () => _emailService.SendNotificationReminderAsync(
                        booking.Email,
                        booking.FullName,
                        booking.BookingDate,
                        booking.LiensePlate
                    ),
                    reminderTime - DateTime.Now
                );
            }
        }
    }
}
