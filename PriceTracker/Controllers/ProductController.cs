using Microsoft.AspNetCore.Mvc;
using PriceTracker.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Data;

namespace PriceTracker.Controllers
{
    public class ProductController : Controller
    {

        private readonly string  _connectionString= "Data Source=.;Initial Catalog=PriceTracker;Integrated Security=True";
        private readonly SqlConnection _connection;

        public ProductController()
        {
            _connection = new SqlConnection(_connectionString);
        }
        public async  Task<IActionResult> Index()
        {
            try
            {
                List<Product> products = new List<Product>();
                string selectQuery = "Select * from Product";
                SqlCommand cmd = new SqlCommand(selectQuery, _connection);


                await _connection.OpenAsync();
                SqlDataReader reader = await cmd.ExecuteReaderAsync();

                while(await reader.ReadAsync())
                {
                    Product product = new Product();
                    product.ProductId = Convert.ToInt32(reader["ProductID"].ToString());
                    product.Productname = reader["ProductName"].ToString();
                    product.ProductLink = reader["ProductLink"].ToString();
                    products.Add(product);
                }

                return View(products);
            }
            catch(Exception ex)
            {
                throw ex;
            }
            finally
            {
                await _connection.CloseAsync();
            }
        }

        [HttpGet]
        public IActionResult NewProduct()
        {
            return View();
        }

        [HttpGet]
        public IActionResult NewProducts()
        {
            return View();
        }

        public async Task<IActionResult> NewProduct(Product product)
        {
            try 
            { 
               if(ModelState.IsValid)
                {
                    string insertQuery = "insert into Product(ProductName,ProductLink)Values(@ProductName,@ProductLink)";
                    SqlCommand cmd = new SqlCommand(insertQuery, _connection);
                    cmd.CommandType = CommandType.Text;
                    await _connection.OpenAsync();
                    cmd.Parameters.AddWithValue("@ProductName", product.Productname);
                    cmd.Parameters.AddWithValue("@ProductLink", product.ProductLink);
                    await cmd.ExecuteNonQueryAsync();
                    return RedirectToAction(nameof(Index));
                }
                return View(product);            
            }
            catch(Exception ex)
            {
                throw ex;
            }
            finally
            {
                await _connection.CloseAsync();
            }
        }

        [HttpGet]

        public async Task<IActionResult> UpdateProduct(int ProductId)
        {
            try
            {
                Product product = new Product();
                string selectQuery = String.Format("select * from Product where ProductID = {0}", ProductId);
                SqlCommand cmd = new SqlCommand(selectQuery, _connection);

                await _connection.OpenAsync();
                SqlDataReader reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    product.ProductId = Convert.ToInt32(reader["ProductID"].ToString());
                    product.Productname = reader["ProductName"].ToString();
                    product.ProductLink = reader["ProductLink"].ToString();
                }
                return View(product);
            }
            catch(Exception ex)
            {
                throw ex;
            }
            finally
            {
                await _connection.CloseAsync();
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProduct(int productId,Product product)
        {
            try
            {
                if (productId != product.ProductId)
                    return NotFound();

                if(ModelState.IsValid)
                {
                    string UpdateQuery = "Update Product set ProductName = @ProductName,ProductLink = @ProductLink where ProductID = @ProductID";
                    SqlCommand cmd = new SqlCommand(UpdateQuery, _connection);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@ProductId", product.ProductId);
                    cmd.Parameters.AddWithValue("@ProductName", product.Productname);
                    cmd.Parameters.AddWithValue("@ProductLink", product.ProductLink);

                    await _connection.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                    return RedirectToAction(nameof(Index));
                }

                return View(product);
            }
            catch(Exception ex)
            {
                throw ex;
            }
            finally
            {
                await _connection.CloseAsync();
            }
        }


        [HttpPost]
        public async Task<IActionResult> DeleteProduct(int ProductId)
        {
            try
            {
                string deleteQuery = "Delete from Product where ProductId = @ProductId";
                SqlCommand cmd = new SqlCommand(deleteQuery, _connection);

                cmd.Parameters.AddWithValue("@ProductId", ProductId);

                await _connection.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
                return RedirectToAction(nameof(Index));                    
           }
            catch(Exception ex)
            {
                throw ex;
            }
            finally
            {
                await _connection.CloseAsync();
            }
        }


        public async Task<IActionResult> DetailProduct(int productId)
        {
            try
            {
                ViewData["ProductId"] = productId;
                List<ProductionInformation> productionInformation = new List<ProductionInformation>();
                string selectQuery = String.Format("Select Value,Day,Month,Year from ProductInformationTbl where ProductId = {0}", productId);
                SqlCommand cmd = new SqlCommand(selectQuery, _connection);
                await _connection.OpenAsync();
                SqlDataReader reader = await cmd.ExecuteReaderAsync();

                while(await reader.ReadAsync())
                {
                    ProductionInformation production = new ProductionInformation();
                    production.value = Convert.ToDouble(reader["Value"].ToString());
                    production.Day = Convert.ToInt32(reader["Day"].ToString());
                    production.Month = Convert.ToInt32(reader["Month"].ToString());
                    production.Year = Convert.ToInt32(reader["Year"].ToString());

                    productionInformation.Add(production);
                }
                return View(productionInformation); 
            }
            catch(Exception ex)
            {
                throw ex;
            }
            finally
            {
                await _connection.CloseAsync();
            }
        }


        public async Task<JsonResult> GraphData(int productId, int Year, int Month,int startDay,int endDay)
        {
            try
            {
                List<ProductionInformation> productionInformation = new List<ProductionInformation>();
                string selectQuery = string.Format("Select Value,Day,Month,Year " +
                    "From ProductInformationTbl" +
                    "where ProductId = {0} and Year = {1} and Month = {2} and (Day >= {3} and Day <= {4})", productId, Year, Month, startDay, endDay);
                SqlCommand cmd = new SqlCommand(selectQuery, _connection);

                await _connection.OpenAsync();
                SqlDataReader reader = await cmd.ExecuteReaderAsync();

                while(await reader.ReadAsync())
                {
                    ProductionInformation productions = new ProductionInformation();
                    productions.value = Convert.ToDouble(reader["Value"]);
                    productions.Day = Convert.ToInt32(reader["Day"]);
                    productions.Month = Convert.ToInt32(reader["Month"]);
                    productions.value = Convert.ToDouble(reader["Year"]);

                    productionInformation.Add(productions);
                }

                return Json(productionInformation);
            }
            catch(Exception ex)
            {
                throw ex;
            }
            finally
            {
                await _connection.CloseAsync();
            }
        }
    }
}
