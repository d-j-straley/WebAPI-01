using AngularServer01.Classes;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System.Data;
using System.Data.SqlClient;
using RobotServer.Classes;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AngularServer01.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private string _sConnectionString = @"Server=(localdb)\MSSQLLocalDB ;Database=RobotShop;Trusted_Connection=True;";

        // DJS: this is the default GET method for the CartController; in reality, this should never be called
        // GET: api/<CartController>
        [HttpGet]
        public string Get()
        {
            DataTable dt = new DataTable();
            using (SqlConnection connection = new SqlConnection(_sConnectionString))
            {
                //connection.Open();
                using (SqlCommand command = new SqlCommand("CartItemSelectAllByUserID", connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    /*
                     * DJS: this is a hack for now.  We need to get the UserID from the session
                     * at the Client and pass it to the server.  For now, we are hardcoding the
                     * UserID at the server just to make it work.
                     */
                    command.Parameters.AddWithValue("@UserID", 1);

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

        // We're going to pull the CartItems for the specified UserID
        // GET api/<CartController>/5
        [HttpGet("{nUserID}")]
        public string Get(int nUserID)
        {
            DataTable dt = new DataTable();

            // Do a sanity check on input variable
            if (nUserID < 1)
            {
                return ("Invalid UserID");
            }
            using (SqlConnection connection = new SqlConnection(_sConnectionString))
            {
                //connection.Open();
                using (SqlCommand command = new SqlCommand("CartItemSelectAllByUserID", connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@UserID", nUserID);

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


        // We're going to pull out the Products with all of the
        // relevant details which are associated with the CartItems
        // (i.e. the items in the cart) for the specified UserID
        // GET api/<CartController>/5
        [HttpGet("ProductsInCart/{nUserID}")]
        public string GetProductsInCart(int nUserID)
        {
            DataTable dt = new DataTable();

            // Do a sanity check on input variable
            if (nUserID < 1)
            {
                return ("Invalid UserID");
            }
            using (SqlConnection connection = new SqlConnection(_sConnectionString))
            {
                //connection.Open();
                using (SqlCommand command = new SqlCommand("ProductsInCartSelectAllByUserID", connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@UserID", nUserID);

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


        [HttpPost]
        // Create a version of POST which receives DTO variablees through the Body
        public int CartItemInsert([FromBody] CartItemInsert incoming)
        {
            int nRetVal = this.CartItemInsert(incoming.UserID, incoming.ProductID);
            return nRetVal;
        }

       
        // We're going to add a new item into the Cart
        // Create a version of POST which receives incoming variables through the URL
        // POST api/<CartController>/1/1
        [HttpPost("{nUserID}/{nProductID}")]
        public int CartItemInsert(int nUserID, int nProductID )
        {
            int nNewCartItemID = 0;
            using (SqlConnection connection = new SqlConnection(_sConnectionString))
            {
                using (SqlCommand command = new SqlCommand("CartItemInsert", connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@UserID", nUserID);
                    command.Parameters.AddWithValue("@ProductID", nProductID);

                    // retrieve the new CartItemID as an output parameter.  The Client
                    // probably doesn't even need this value, but we'll return it anyway.
                    SqlParameter param = new SqlParameter("@NewCartItemID", SqlDbType.Int);
                    param.Direction = ParameterDirection.Output;
                    command.Parameters.Add(param);

                    connection.Open();
                    command.ExecuteNonQuery();
                    nNewCartItemID = (int)param.Value;
                }
            }
            return nNewCartItemID;
        }

        // PUT api/<CartController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // We are removing one specific item from the cart here
        // DELETE api/<CartController>/1/1
        [HttpDelete("{nUserID}/{nProductID}")]
        public void CartItemDelete(int nUserID, int nProductID)
        {
            using (SqlConnection connection = new SqlConnection(_sConnectionString))
            {
                using (SqlCommand command = new SqlCommand("CartItemDeleteByUserID-ProductID", connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@UserID", nUserID);
                    command.Parameters.AddWithValue("@ProductID", nProductID);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
