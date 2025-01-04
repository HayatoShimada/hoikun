using hoikun.Data;
using Microsoft.EntityFrameworkCore;

namespace hoikun.Services
{
    public class AppointmentService
    {
        private readonly ApplicationDbContext _context;

        public AppointmentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Appointment>> GetAppointmentsAsync(DateTime startDate, DateTime endDate)
        {
            List<Appointment> appointments = await _context.Appointments
                .Where(a => a.StartDate >= startDate && a.EndDate <= endDate)
                .ToListAsync();

            List<Appointment> tests = await _context.Appointments.ToListAsync();

            return appointments;
        }


        public async Task AddAppointmentAsync(Appointment appointment)
        {
            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAppointmentAsync(Appointment appointment)
        {
            Appointment? existingAppointment = await _context.Appointments.FirstOrDefaultAsync(a => a.AppointmentId == appointment.AppointmentId);
            if (existingAppointment != null)
            {
                existingAppointment.StartDate = appointment.StartDate;
                existingAppointment.EndDate = appointment.EndDate;
                existingAppointment.Caption = appointment.Caption;
                existingAppointment.Label = appointment.Label;
                existingAppointment.Status = appointment.Status;
                existingAppointment.AppointmentType = appointment.AppointmentType;
                await _context.SaveChangesAsync();
            }
        }
    }
}
