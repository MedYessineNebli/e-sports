using API.DTOs;
using API.Models;
using API.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeRepository repository;
        private readonly IMapper mapper;

        public EmployeesController(IEmployeeRepository repository
            ,IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await repository.GetEmployees());
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByID(int id)
        {
            var emp= await repository.GetEmployeeById(id);
            if(emp == null) return NotFound();
            return Ok(emp);
        }
        [HttpPost]
        public async Task<IActionResult> Create(EmployeeDTO dto)
        {
            var employee = mapper.Map<Employee>(dto); 
            var emps = await repository.GetEmployees();
             if(emps.Where(p => string.Equals(p.Email, employee.Email, StringComparison.OrdinalIgnoreCase))
                        .Count() >0 )
            {
                return BadRequest("Employee Existe");
            }
            var emp = await repository.AddEmployee(employee);
            if (emp != null)
            {
                return CreatedAtAction(nameof(GetByID)
                    ,new {id = emp.EmployeeID},emp
                    );
            }
            return BadRequest();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result= await repository.DeleteEmployee(id);
            if (result) return NoContent();
            return NotFound();
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Employee employee)
        {
            var result= await repository.UpdateEmployee(employee);
            if (result) return NoContent();
            return NotFound();
        }
    }
}