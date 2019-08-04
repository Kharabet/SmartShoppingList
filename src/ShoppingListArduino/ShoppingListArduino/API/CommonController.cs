﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using ShoppingListArduino.Data;
using ShoppingListArduino.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ShoppingListArduino.API
{
    [Route("api/")]
    public class CommonController : Controller
    {
        private ApplicationDbContext _context;

        public CommonController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/<controller>
        [HttpGet]
        [Route("get-product")]
        public JObject GetProduct(string code)
        {
            var product = _context.Products.Where(x => x.Barcode == code).FirstOrDefault();

            if (product == null)
            {
                return JObject.FromObject(new { success = false });
            }
            else
            {
                return JObject.FromObject(new
                {
                    success = true,
                    product = new
                    {
                        id = product.Id,
                        title = product.Title,
                        description = product.Description
                    }
                });
            }
           
        }

        [HttpGet]
        [Route("get-product-by-name")]
        public JObject GetProductByName(string name)
        {
            var product = _context.Products.FirstOrDefault(x => x.Title.ToLower() == name || x.Description.ToLower() == name);

            if (product == null)
            {
                return JObject.FromObject(new { success = false });
            }
            else
            {
                return JObject.FromObject(new
                {
                    success = true,
                    product = new
                    {
                        id = product.Id,
                        title = product.Title,
                        description = product.Description,
                        barcode = product.Barcode
                    }
                });
            }

        }


        [HttpPost]
        [Route("add-user-product")]
        public JObject AddUserProduct(string userId, int productId, int quantity)
        {
            var userProduct = _context.UserProduct.Find(userId, productId);
            if (userProduct == null)
            {
                _context.UserProduct.Add(new UserProduct()
                {
                    UserId = userId,
                    ProductId = productId,
                    Quantity = quantity
                });

            }
            else
            {
                userProduct.Quantity += quantity;
            }

            try
            {
                _context.SaveChanges();
                return JObject.FromObject(new { success = true });

            }
            catch (Exception ex)
            {
                return JObject.FromObject(new { success = false });

            }

        }

        [HttpPost]
        [Route("user-product-to-bin")]
        public JObject UserProductToBin(string userId, int productId, int quantity)
        {
            var userProduct = _context.UserProduct.Find(userId, productId);
            if (userProduct == null)
            {
                return JObject.FromObject(new { success = false, message = "У Вас дома уже не числится такой продукт!" });
            }
                userProduct.Quantity -= quantity;
            if (userProduct.Quantity <= 0)
            {
                _context.UserProduct.Remove(userProduct);

            }
            try
            {
                _context.SaveChanges();
                return JObject.FromObject(new { success = true });

            }
            catch (Exception ex)
            {
                return JObject.FromObject(new { success = false });

            }

        }
        [HttpPost]
        [Route("user-product-to-bin-by-barcode")]
        public JObject UserProductToBinByBarcode(string userId, string barcode)
        {
            var product = _context.Products.FirstOrDefault(x => x.Barcode == barcode);
            if (product == null)
            {
                return JObject.FromObject(new { success = false });
            }

            var userProduct = _context.UserProduct.Find(userId, product.Id);

            if (userProduct == null)
            {
                return JObject.FromObject(new { success = true });
            }

            userProduct.Quantity -= 1;

            if (userProduct.Quantity <= 0)
            {
                _context.UserProduct.Remove(userProduct);
            }

            try
            {
                _context.SaveChanges();
                return JObject.FromObject(new { success = true });
            }
            catch (Exception ex)
            {
                return JObject.FromObject(new { success = false });
            }

        }

        // GET api/<controller>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        [HttpPost]
        public string Post([FromBody]string value)
        {
            return "post success";
        }

        // PUT api/<controller>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
