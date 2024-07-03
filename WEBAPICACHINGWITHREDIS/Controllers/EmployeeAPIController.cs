using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using WEBAPICACHINGWITHREDIS.DATA;
using WEBAPICACHINGWITHREDIS.Model;
using WEBAPICACHINGWITHREDIS.Model.DTO;
using WEBAPICACHINGWITHREDIS.Services;
using static Azure.Core.HttpHeader;

namespace WEBAPICACHINGWITHREDIS.Controllers
{
    [Route("api/employee")]
    [ApiController]
    public class EmployeeAPIController : ControllerBase
    {
        private readonly AppDbContext _db;
        private ResponseDto _response;
        private ICacheService _cacheService;
        public EmployeeAPIController(AppDbContext db, ICacheService cacheService)
        {
            _db = db;
            _cacheService = cacheService;
            _response = new ResponseDto();
        }


        [HttpGet("Employees")]
        public async Task<IActionResult> Get()
        {
            var CacheData = _cacheService.GetData<IEnumerable<Employee>>("Employees");

            if(CacheData!=null && CacheData.Count()>0)
            {
                _response.Result = CacheData;
                _response.IsSuccess = true;
                _response.Message = "success";
                return Ok(CacheData);
            }

            IEnumerable<Employee> Couponlist = new List<Employee>();
            Couponlist = _db.Employees.ToList();

            if (Couponlist.Any())
            {

                //Set Data in redis cache

                var expirytime=DateTimeOffset.Now.AddSeconds(10);
                _cacheService.SetData<IEnumerable<Employee>>("Employees", Couponlist, expirytime);

                _response.Result = Couponlist;
                _response.IsSuccess = true;
                _response.Message = "success";
                return Ok(_response);
            }
            else
            {
                _response.Result = Empty;
                _response.IsSuccess = false;
                _response.Message = "Error while fetching";
                return BadRequest(_response);
            }
        }

        [HttpGet]
        [Route("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            Employee CouponData = new Employee();
            CouponData = _db.Employees.FirstOrDefault(z => z.EmployeeId == id);

            if (CouponData != null)
            {
                _response.Result = CouponData;
                _response.IsSuccess = true;
                _response.Message = "success";
                return Ok(_response);
            }
            else
            {
                _response.Result = CouponData;
                _response.IsSuccess = false;
                _response.Message = "Error while fetching";
                return BadRequest(_response);
            }
        }


        [HttpGet]
        [Route("{code}")]
        public async Task<IActionResult> GetByCode(string code)
        {
            Employee CouponData = new Employee();
            CouponData = _db.Employees.FirstOrDefault(z => z.EmployeeCode == code.ToString());

            if (CouponData.EmployeeCode != null)
            {
                _response.Result = CouponData;
                _response.IsSuccess = true;
                _response.Message = "success";
                return Ok(_response);
            }
            else
            {
                _response.Result = CouponData;
                _response.IsSuccess = false;
                _response.Message = "Error while fetching";
                return BadRequest(_response);
            }
        }


        [HttpPost("AddEmployee")]
        public async Task<IActionResult> AddCoupon([FromBody] Employee coupon)
        {
            try
            {
                _db.Employees.Add(coupon);
                _db.SaveChanges();

                return Ok(_response);
            }
            catch
            {
                return BadRequest(_response);
            }
        }


        [HttpPut("UpdateEmployee")]
        public async Task<IActionResult> UpdateCoupon([FromBody] Employee coupon)
        {
            try
            {
                _db.Employees.Update(coupon);
                _db.SaveChanges();

                return Ok(_response);
            }
            catch
            {
                return BadRequest(_response);
            }
        }



        [HttpDelete("DeleteEmployee")]
        public async Task<IActionResult> DeleteCoupon(string _EmployeeCode)
        {
            try
            {
                var coupondata = _db.Employees.First(u => u.EmployeeCode == _EmployeeCode);
                _db.Employees.Remove(coupondata);
                _db.SaveChanges();

                return Ok(_response);
            }
            catch
            {
                return BadRequest(_response);
            }
        }

    }
}
