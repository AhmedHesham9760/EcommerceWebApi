using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using EcommerceWebApi.Models;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

using System.Security.Claims;
using System.Text;

namespace EcommerceWebApi.Controllers
{
    public class UsersController : ApiController
    {
        private EcommerceDbContext db = new EcommerceDbContext();

        public IQueryable<User> GetUsers()
        {
            return db.Users;
        }

        [HttpPost]
        [Route("api/users/login/{email}/{password}")]
        [ResponseType(typeof(User))]
        public IHttpActionResult logIn(string email,string password)
        {
            User user = db.Users.Where(u=>u.Email == email && u.Password == password).FirstOrDefault();
            if (user == null)
            {
                return NotFound();
            }
            return Ok(createToken(user));
        }

        [ResponseType(typeof(void))]
        public IHttpActionResult PutUser(int id, User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != user.UserID)
            {
                return BadRequest();
            }

            db.Entry(user).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        [ResponseType(typeof(User))]
        public IHttpActionResult PostUser(User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            db.Users.Add(user);
            db.SaveChanges();
            return CreatedAtRoute("DefaultApi", null, createToken(user));
        }

        [ResponseType(typeof(User))]
        public IHttpActionResult DeleteUser(int id)
        {
            User user = db.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }

            db.Users.Remove(user);
            db.SaveChanges();

            return Ok(user);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool UserExists(int id)
        {
            return db.Users.Count(e => e.UserID == id) > 0;
        }
        private string createToken(User user) {
            string key = "webapi-desc-key123456";
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim("userId", user.UserID.ToString()));
            claims.Add(new Claim("isAdmin", user.IsAdmin.ToString()));
            claims.Add(new Claim("name", user.fullname));

            var token = new JwtSecurityToken(
                expires: DateTime.Now.AddDays(10),
                signingCredentials: credentials,
                claims: claims
                );
            var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);
            return jwtToken;
        }
    }
}