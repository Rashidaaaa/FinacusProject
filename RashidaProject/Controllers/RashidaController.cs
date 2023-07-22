using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace RashidaProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RashidaController : ControllerBase
    {
        SqlConnection con;
        public RashidaController(IConfiguration configuration)
        {
            con = new SqlConnection();
            con.ConnectionString = configuration.GetConnectionString("ConStr");
        }
        [HttpGet("products")]
        public ActionResult<IEnumerable<Product>> get()
        {
            SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM Product_RA", con);
            con.Open();
            DataSet ds = new DataSet();
            adapter.Fill(ds);
            List<Product> products = new List<Product>();
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    Product product = new Product()
                    {
                        ProductId = Convert.ToInt32(row["ProductId"]),
                        Name = row["Name"].ToString(),
                        Description = row["Description"].ToString(),
                        Price = float.Parse(row["Price"].ToString()),
                        Category = row["Category"].ToString(),
                    };
                    products.Add(product);
                }
            }
            else
            {
                return NotFound("No Products found");
            }
           
            
            con.Close();
            return products;

        }
        [HttpGet("products/{id}")]
        public ActionResult<Product> get(int id)
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM Product_RA where ProductId=@P1", con);
            cmd.Parameters.Add("p1", SqlDbType.Int);
            cmd.Parameters["p1"].Value = id;
            con.Open();
            SqlDataReader dr = cmd.ExecuteReader();
            Product product = null;
            if (dr.Read())
            {
                product = new Product()
                {
                    ProductId = Convert.ToInt32(dr["ProductId"]),
                    Name = dr["Name"].ToString(),
                    Description = dr["Description"].ToString(),
                    Price = float.Parse(dr["Price"].ToString()),
                    Category = dr["Category"].ToString(),
                };
            }
            else
            {
                return NotFound();
            }
            con.Close();
            return product;

        }

        [HttpPost("products")]
        public ActionResult post([FromBody] Product product)
        {
            SqlCommand cmd = new SqlCommand("insert into Product_RA values(@p1,@p2,@p3,@p4,@p5)", con);
            cmd.Parameters.Add("p1", SqlDbType.Int);
            cmd.Parameters.Add("p2", SqlDbType.VarChar, 250);
            cmd.Parameters.Add("p3", SqlDbType.VarChar, 250);
            cmd.Parameters.Add("p4", SqlDbType.Float);
            cmd.Parameters.Add("p5", SqlDbType.VarChar, 15);

            cmd.Parameters["p1"].Value = product.ProductId;
            cmd.Parameters["p2"].Value = product.Name;
            cmd.Parameters["p3"].Value = product.Description;
            cmd.Parameters["p4"].Value = product.Price;
            cmd.Parameters["p5"].Value = product.Category;

            con.Open();
            int result = cmd.ExecuteNonQuery();
            if (result == 0)
            {
                return Problem("Failed to Create");
            }
            con.Close();
            return Ok("Created Successfully");
        }

        [HttpDelete("products/{id}")]
        public ActionResult delete(int id)
        {
            SqlCommand cmd = new SqlCommand("DELETE FROM Product_RA where ProductId=@p1", con);
            cmd.Parameters.Add("p1", SqlDbType.Int);
            cmd.Parameters["p1"].Value = id;
            con.Open();
            int result = cmd.ExecuteNonQuery();
            if (result == 0)
            {
                return Problem("Delete Failed");
            }
            con.Close();
            return Ok("Deleted Successfully");
        }

        [HttpPut("products/{id}")]
        public ActionResult update(int id, [FromBody] Product product)
        {
            SqlCommand cmd = new SqlCommand("update Product_RA set Name=@p2 , Description=@p3 , Price=@p4 , Category=@p5 where ProductId=@p1", con);
            cmd.Parameters.Add("p1", SqlDbType.Int);
            cmd.Parameters.Add("p2", SqlDbType.VarChar, 250);
            cmd.Parameters.Add("p3", SqlDbType.VarChar, 250);
            cmd.Parameters.Add("p4", SqlDbType.Float);
            cmd.Parameters.Add("p5", SqlDbType.VarChar, 15);

            cmd.Parameters["p1"].Value = id;
            cmd.Parameters["p2"].Value = product.Name;
            cmd.Parameters["p3"].Value = product.Description;
            cmd.Parameters["p4"].Value = product.Price;
            cmd.Parameters["p5"].Value = product.Category;

            con.Open();
            int result = cmd.ExecuteNonQuery();
            if (result == 0)
            {
                return Problem("Update Failed");
            }
            con.Close();
            return Ok("Updated Successfully");
        }


    }
}
