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
            return await _context.Appointments
                .Where(a => a.StartDate >= startDate && a.EndDate <= endDate)
                .ToListAsync();
        }


        public async Task AddAppointmentAsync(Appointment appointment)
        {
            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();
        }
    }

}
