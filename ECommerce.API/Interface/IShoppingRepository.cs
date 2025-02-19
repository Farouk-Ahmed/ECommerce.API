﻿using ECommerce.API.Models;

namespace ECommerce.API.Interface
{
    public interface IShoppingRepository
    {
        List<ProductCategory> GetProductCategories();
        ProductCategory GetProductCategory(int id);


        List<Review> GetProductReviews(int productId);


        Offer GetOffer(int id);
        List<Product> GetProducts(string category, string subcategory, int count);
        Product GetProduct(int id);
        //Task<bool> InsertUser(User user);
        string IsUserPresent(string email, string password);
        void InsertReview(Review review);
        User GetUser(string id);
        bool InsertCartItem(int userId, int productId);
        Cart GetActiveCartOfUser(int userid);
        Cart GetCart(int cartid);
        List<Cart> GetAllPreviousCartsOfUser(int userid);
        List<PaymentMethod> GetPaymentMethods();
        int InsertPayment(Payment payment);
        int InsertOrder(Order order);
    }
}
