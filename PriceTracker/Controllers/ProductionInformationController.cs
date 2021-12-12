using Microsoft.AspNetCore.Mvc;
using PriceTracker.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace PriceTracker.Controllers
{
    public class ProductionInformationController : Controller
    {
        private readonly string _connectionString = "Data Source=.;Initial Catalog=PriceTracker;Integrated Security=True";
        private readonly SqlConnection _connection;

        public ProductionInformationController()
        {
            _connection = new SqlConnection(_connectionString);
        }

        [HttpGet]
        public IActionResult NewInformation(int ProductId)
        {
            ProductionInformation info = new ProductionInformation();
            {
                ProductId = info.ProductId;
            };
            
            return View(info);
        }

        [HttpPost]
        public async Task<IActionResult> NewInformation(ProductionInformation information)
        {
            try
            {
                if(ModelState.IsValid)
                {
                    string insertQuery = "Insert into ProductInformationTbl(ProductId,Value,Day,Month,Year)Values(@ProductId,@Value,@Day,@Month,@Year) ";
                    SqlCommand cmd = new SqlCommand(insertQuery, _connection);
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Parameters.AddWithValue("@ProductId", information.ProductId);
                    cmd.Parameters.AddWithValue("@Value", information.value);
                    cmd.Parameters.AddWithValue("@Day", information.Day);
                    cmd.Parameters.AddWithValue("@Month", information.Month);
                    cmd.Parameters.AddWithValue("@Year", information.Year);
                    await _connection.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                    return RedirectToAction("DetailProduct", "Product",new { productid = information.ProductId });


                }
                return View(information); 
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
