using ECommerce.API.Interface;
using ECommerce.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShoppingController : ControllerBase
    {
        readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public ShoppingController(IUnitOfWork unitOfWork, UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet("GetCategoryList")]
        public IActionResult GetCategoryList()
        {
            var result = _unitOfWork.shoppingRepository.GetProductCategories();
            return Ok(result);
        }

        [HttpGet("GetProducts")]
        public IActionResult GetProducts(string category, string subcategory, int count)
        {
            var result = _unitOfWork.shoppingRepository.GetProducts(category, subcategory, count);
            return Ok(result);
        }

        [HttpGet("GetProduct/{id}")]
        public IActionResult GetProduct(int id)
        {
            var result = _unitOfWork.shoppingRepository.GetProduct(id);
            return Ok(result);
        }

        [HttpPost("RegisterUser")]
        public async Task<IActionResult> RegisterUser([FromBody] User user)
        {
            user.CreatedAt = DateTime.Now.ToString();
            user.ModifiedAt = DateTime.Now.ToString();

            //var existingUser = _unitOfWork.shoppingRepository.GetUser(user.Id);
            //if (existingUser != null)
            //{
            //    return BadRequest();
            //}
            var result = await _userManager.CreateAsync(user, user.Password);
            if (!result.Succeeded)
            {
                return BadRequest();
            }

            string? message;
            if (result.Succeeded) message = "inserted";
            else message = "email not available";
            return Ok(message);
        }

        [HttpPost("LoginUser")]
        public IActionResult LoginUser([FromBody] User user)
        {
            var token = _unitOfWork.shoppingRepository.IsUserPresent(user.Email, user.Password);
            if (token == "") token = "invalid";
            return Ok(token);
        }

        [HttpPost("InsertReview")]
        public IActionResult InsertReview([FromBody] Review review)
        {
            review.CreatedAt = DateTime.Now.ToString();
            _unitOfWork.shoppingRepository.InsertReview(review);
            return Ok("inserted");
        }

        [HttpGet("GetProductReviews/{productId}")]
        public IActionResult GetProductReviews(int productId)
        {
            var result = _unitOfWork.shoppingRepository.GetProductReviews(productId);
            return Ok(result);
        }

        [HttpPost("InsertCartItem/{userid}/{productid}")]
        public IActionResult InsertCartItem(int userid, int productid)
        {
            var result = _unitOfWork.shoppingRepository.InsertCartItem(userid, productid);
            return Ok(result ? "inserted" : "not inserted");
        }

        [HttpGet("GetActiveCartOfUser/{id}")]
        public IActionResult GetActiveCartOfUser(int id)
        {
            var result = _unitOfWork.shoppingRepository.GetActiveCartOfUser(id);
            return Ok(result);
        }

        [HttpGet("GetAllPreviousCartsOfUser/{id}")]
        public IActionResult GetAllPreviousCartsOfUser(int id)
        {
            var result = _unitOfWork.shoppingRepository.GetAllPreviousCartsOfUser(id);
            return Ok(result);
        }

        [HttpGet("GetPaymentMethods")]
        public IActionResult GetPaymentMethods()
        {
            var result = _unitOfWork.shoppingRepository.GetPaymentMethods();
            return Ok(result);
        }

        [HttpPost("InsertPayment")]
        public IActionResult InsertPayment(Payment payment)
        {
            payment.CreatedAt = DateTime.Now.ToString();
            var id = _unitOfWork.shoppingRepository.InsertPayment(payment);
            return Ok(id.ToString());
        }

        [HttpPost("InsertOrder")]
        public IActionResult InsertOrder(Order order)
        {
            order.CreatedAt = DateTime.Now.ToString();
            var id = _unitOfWork.shoppingRepository.InsertOrder(order);
            return Ok(id.ToString());
        }
    }

}
