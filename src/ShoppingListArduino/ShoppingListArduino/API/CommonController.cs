using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
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
        private IMemoryCache _cache;

        public CommonController(ApplicationDbContext context, IMemoryCache memoryCache)
        {
            _context = context;
            _cache = memoryCache;
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
            var userProduct = _context.UserProducts.FirstOrDefault(p => p.UserId == userId && p.ProductId == productId);
            if (userProduct == null)
            {
                _context.UserProducts.Add(new UserProduct()
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
        [Route("add-unassigned-rfid")]
        public JObject AddUnassignedRfid(string rfid, string userId)
        {
            if(_context.UserProductRfids.Any(x => x.Rfid == rfid))
                return JObject.FromObject(new { success = false, message = "Продукт с такой меткой уже есть в базе." });
            try
            {
                var result = _cache.Set(userId, rfid, TimeSpan.FromMinutes(20.0));
                return JObject.FromObject(new { success = true });

            }
            catch (Exception ex)
            {
                return JObject.FromObject(new { success = false });

            }
        }

        [HttpPost]
        [Route("bind-unassigned-rfid-to-user-product")]
        public JObject BindUnassignedRfidToUserProduct(int userProductId)
        {
            var userProduct = _context.UserProducts.Find(userProductId);

            if (_context.UserProductRfids.Count(x=>x.UserProductId==userProductId) == userProduct.Quantity)
            {
                return JObject.FromObject(new { success = false, message = "Все товары данного типа уже привязаны к RFID метке" });
            }
            string unassignedRfid;
            if (_cache.TryGetValue(userProduct.UserId, out unassignedRfid))
            {
                _context.Add(new UserProductRfid
                {
                    UserProductId = userProductId,
                    Rfid = unassignedRfid
                });
                _cache.Remove(userProduct.UserId);

                try
                {
                    _context.SaveChanges();
                    return JObject.FromObject(new { success = true, message = "RFID успешно привязан к товару!" });

                }
                catch (Exception ex)
                {
                    return JObject.FromObject(new { success = false, message = "Ошибка при сохранении в базу!" });

                }
            }

            return JObject.FromObject(new { success = false, message = "Свободного RFID не найдено. Просканируйте новый RFID!" });

        }



        [HttpPost]
        [Route("user-product-to-bin")]
        public JObject UserProductToBin(string userId, int productId, int quantity)
        {
            var userProduct = _context.UserProducts.
                Include(x => x.UserProductRfids)
                .FirstOrDefault(p => p.UserId == userId && p.ProductId == productId);
            if (userProduct == null)
            {
                return JObject.FromObject(new { success = false, message = "У Вас дома уже не числится такой продукт!" });
            }
                userProduct.Quantity -= quantity;
            if (userProduct.Quantity <= 0)
            {
                if (userProduct.UserProductRfids.Count > 0)
                {
                    _context.UserProductRfids.RemoveRange(userProduct.UserProductRfids);
                }
                _context.UserProducts.Remove(userProduct);
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

            var userProduct = _context.UserProducts.FirstOrDefault(p => p.UserId == userId && p.ProductId == product.Id);

            if (userProduct == null)
            {
                return JObject.FromObject(new { success = true });
            }

            userProduct.Quantity -= 1;

            if (userProduct.Quantity <= 0)
            {
                _context.UserProducts.Remove(userProduct);
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
        [Route("user-product-to-bin-by-rfid")]
        public JObject UserProductToBinByRfid(string rfid)
        {
            var userProductRfid = _context.UserProductRfids
                .Include(x => x.UserProduct)
                .FirstOrDefault(x => x.Rfid == rfid);
            if (userProductRfid == null)
            {
                return JObject.FromObject(new { success = false });
            }

            var userProduct = userProductRfid.UserProduct;

            if (userProduct == null)
            {
                return JObject.FromObject(new { success = false });
            }

            userProduct.Quantity -= 1;

            _context.UserProductRfids.Remove(userProductRfid);


            if (userProduct.Quantity <= 0)
            {
                _context.UserProductRfids.RemoveRange(userProduct.UserProductRfids);
                _context.UserProducts.Remove(userProduct);
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
