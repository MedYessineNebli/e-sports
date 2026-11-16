using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly ApplicationContext context;

        public EmployeeRepository(ApplicationContext context)
        {
            this.context = context;
        }
        public async Task<Employee> AddEmployee(Employee employee)
        {
            context.Employees.Add(employee);
            await context.SaveChangesAsync();
            return employee;
        }

        public async Task<bool> DeleteEmployee(int id)
        {
            var employee = await context.Employees.FindAsync(id);
            if (employee == null)
            {
                return false;
            }
            context.Employees.Remove(employee);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<Employee?> GetEmployeeById(int id)
        {
            return await context.Employees.FindAsync(id);
        }

        public async Task<IEnumerable<Employee>> GetEmployees()
        {
            return await context.Employees.ToListAsync();
        }

        public async Task<bool> UpdateEmployee(Employee employee)
        {
            context.Employees.Update(employee);
            await context.SaveChangesAsync();
            return true;
        }
    }
}