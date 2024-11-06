using AngularServer01.Classes;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;    

//using System.Text.Json;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AngularServer01.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RobotCatalogController : ControllerBase
    {
        private string _sConnectionString = @"Server=(localdb)\MSSQLLocalDB ;Database=RobotShop;Trusted_Connection=True;";

        // GET: api/<RobotCatalogController>
        /* Access SQL Server via Stored Procedure
         Use DataTable to store the result
         Return the DataTable as JSON
        */
        [HttpGet]
        public string Get()
        {

            DataTable dt = new DataTable();
            using (SqlConnection connection = new SqlConnection(_sConnectionString))
            {
                using (SqlCommand command = new SqlCommand("ProductSelectAll", connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        connection.Open();
                        adapter.Fill(dt);
                    }
                }
            }
            var jsonSerializerSettings = new JsonSerializerSettings
            {
                // DJS: I have no idea how the following line of code works
                // apparently there are *many* options available.
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };  
            string sJsonOutput = JsonConvert.SerializeObject(dt, jsonSerializerSettings);

            return (sJsonOutput);
        }


        // GET api/<RobotCatalogController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<RobotCatalogController>
        [HttpPost]
        public int Post([FromBody] Product newproduct)
        {
            // insert new product into the database via the CatalogInsert stored procedure
            using (SqlConnection connection = new SqlConnection(_sConnectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("ProductInsert", connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@CategoryID", newproduct.CategoryID);
                    command.Parameters.AddWithValue("@Name", newproduct.Name);
                    command.Parameters.AddWithValue("@Description", newproduct.Description);
                    command.Parameters.AddWithValue("@ImageName", newproduct.ImageName);
                    command.Parameters.AddWithValue("@Price", newproduct.Price);
                    command.Parameters.AddWithValue("@Discount", newproduct.Discount);

                    // retrieve the new CatalogID as an output parameter
                    SqlParameter param = new SqlParameter("@NewProductID", SqlDbType.Int);
                    param.Direction = ParameterDirection.Output;
                    command.Parameters.Add(param);

                    command.ExecuteNonQuery();
                    newproduct.ProductID = (int)param.Value;
                }
            }
            return newproduct.ProductID;
        }

        // PUT api/<RobotCatalogController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }
        // DELETE api/<RobotCatalogController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
