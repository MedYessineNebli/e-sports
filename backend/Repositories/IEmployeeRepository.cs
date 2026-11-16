using API.Models;

namespace API.Repositories
{
    public interface IEmployeeRepository
    {
        Task<IEnumerable<Employee>> GetEmployees();

        Task<Employee?> GetEmployeeById(int id);

        Task<Employee> AddEmployee(Employee employee);

        Task<bool> UpdateEmployee(Employee employee);

        Task<bool> DeleteEmployee(int id);
    }
}