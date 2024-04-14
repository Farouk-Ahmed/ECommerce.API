using ECommerce.API.Data;
using ECommerce.API.Interface;
using ECommerce.API.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ECommerce.API.Repository
{
    public class ShoppingRepository : IShoppingRepository
    {
        #region Fields
        private readonly ApplicationDBContext _context;
        #endregion

        #region Contractors
        public ShoppingRepository(ApplicationDBContext context)
        {
            _context = context;
        }


        #endregion

        #region Handler Function

        #region ProductCategories
        public List<ProductCategory> GetProductCategories()
        {
            return _context.ProductCategories.ToList();
        }

        public ProductCategory GetProductCategory(int id)
        {
            return _context.ProductCategories.FirstOrDefault(x => x.Id == id);
        }
        #endregion

        #region Review
        public List<Review> GetProductReviews(int productId)
        {
            return _context.Reviews
                    .Include(r => r.Product)
                    .Where(r => r.Product.Id == productId)
                    .ToList();
        }

        public void InsertReview(Review review)
        {
            var newReview = new Review
            {
                User = review.User,
                Product = review.Product,
                Value = review.Value,
                CreatedAt = review.CreatedAt
            };

            _context.Reviews.Add(newReview);
            _context.SaveChanges();
        }
        #endregion


        #region Offers
        public Offer GetOffer(int id)
        {
            var result = _context.Offers.FirstOrDefault(o => o.Id == id);
            return result ?? new Offer();
        } 
        #endregion

        #region Products
        public List<Product> GetProducts(string category, string subcategory, int count)
        {
            var products = _context.Products
                .Include(p => p.ProductCategory)
                .Include(p => p.Offer)
                .Where(p => p.ProductCategory.Category == category && p.ProductCategory.SubCategory == subcategory)
                .OrderBy(p => Guid.NewGuid())
                .Take(count)
                .ToList();

            return products;
        }
        public Product GetProduct(int id)
        {
            var product = _context.Products
                .Include(p => p.ProductCategory)
                .Include(p => p.Offer)
                .FirstOrDefault(p => p.Id == id);

            return product;
        }
        #endregion

        #region Users

        public User GetUser(string id)
        {
            var user = _context.Users.Find(id);
            return user ?? new User();
        }

        //public async Task<bool> InsertUser(User user)
        //{
        //    var existingUser = _context.Users.FirstOrDefault(u => u.Email == user.Email);
        //    if (existingUser != null)
        //    {
        //        return false;
        //    }
        //    var result = await _userManager.CreateAsync(user, user.Password);
        //    if (!result.Succeeded)
        //    {
        //        return false;
        //    }
        //    return true;
        //}

        public string IsUserPresent(string email, string password)
        {
            var user = _context.Users
                .FirstOrDefault(u => u.Email == email && u.Password == password);

            if (user == null)
            {
                return ""; // User not found
            }


            var signinKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("this is my custom Secret key for authentication"));
            var credentials = new SigningCredentials(signinKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
                        {
                            new Claim("id", user.Id.ToString()),
                            new Claim("firstName", user.FirstName),
                            new Claim("lastName", user.LastName),
                            new Claim("username", user.UserName),
                            new Claim("address", user.Address),
                            new Claim("mobile", user.Mobile),
                            new Claim("email", user.Email),
                            new Claim("createdAt", user.CreatedAt),
                            new Claim("modifiedAt", user.ModifiedAt)
                        };

            var jwtToken = new JwtSecurityToken(
                issuer: "localhost",
                audience: "localhost",
                claims: claims,
                expires: DateTime.Now.AddMinutes(int.Parse("60")),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(jwtToken);
        }

        #endregion

        #region Carts
        public bool InsertCartItem(int userId, int productId)
        {
            var cart = _context.Carts
                .FirstOrDefault(c => c.Id == userId && !c.Ordered);

            if (cart == null)
            {
                cart = new Cart
                {
                    Id = userId,
                    Ordered = false
                };

                _context.Carts.Add(cart);
                _context.SaveChanges();
            }

            int cartId = cart.Id;

            var cartItem = new CartItem
            {
                Id = cartId,
                ProductId = productId
            };

            _context.CartItems.Add(cartItem);
            _context.SaveChanges();

            return true;
        }

        public Cart GetActiveCartOfUser(int userId)
        {
            var cart = _context.Carts
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                .FirstOrDefault(c => c.Id == userId && !c.Ordered);

            if (cart == null)
            {
                return new Cart(); // Return an empty cart if no active cart is found
            }

            return cart;
        }

        public Cart GetCart(int cartId)
        {
            var cart = _context.Carts
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                .FirstOrDefault(c => c.Id == cartId);

            if (cart == null)
            {
                return new Cart(); // Return an empty cart if no cart is found
            }

            return cart;
        }

        public List<Cart> GetAllPreviousCartsOfUser(int userId)
        {
            var carts = _context.Carts
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                .Where(c => c.Id == userId && c.Ordered)
                .ToList();

            return carts;
        }

        #endregion

        #region PaymentMethods
        public List<PaymentMethod> GetPaymentMethods()
        {
            var paymentMethods = _context.PaymentMethods.ToList();
            return paymentMethods;
        }

        public int InsertPayment(Payment payment)
        {
            _context.Payments.Add(payment);
            _context.SaveChanges();
            return payment.Id;
        }


        #endregion


        #region Insert Order
        public int InsertOrder(Order order)
        {
            _context.Orders.Add(order);
            _context.SaveChanges();

            var cartEntity = _context.Carts.FirstOrDefault(c => c.Id == order.Cart.Id);
            if (cartEntity != null)
            {
                cartEntity.Ordered = true;
                cartEntity.OrderedOn = DateTime.Now.ToString();
                _context.SaveChanges();
            }

            return order.Id;
        } 
        #endregion



        #endregion

    }
}
