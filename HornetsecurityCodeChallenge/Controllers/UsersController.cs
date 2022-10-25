using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BCrypt.Net;
using System.Web.Http.Cors;
using System.Web.Http.Description;
using HornetsecurityCodeChallenge.Data;
using HornetsecurityCodeChallenge.Models;
using RestSharp;
using System.Security.Cryptography;

namespace HornetsecurityCodeChallenge.Controllers
{

    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class UsersController : ApiController
    {
        
        private HornetsecurityCodeChallengeContext db = new HornetsecurityCodeChallengeContext();

        // GET: api/Users
        public IQueryable<User> GetUsers()
        {
            return db.Users;
        }

        // POST: api/Register
        [Route("api/Register")]
        [HttpPost]
        public IHttpActionResult PostRegister(User registerDetails)
        {
            if (registerDetails == null)
            {
                return BadRequest();
            }
            registerDetails.Password = BCrypt.Net.BCrypt.HashPassword(registerDetails.Password);
            db.Users.Add(registerDetails);

            db.SaveChanges();

            return Ok("Registered");

        }

        [Route("api/Login")]
        [HttpPost]
        [ResponseType(typeof(User))]
        public IHttpActionResult PostLogin(User loginDetails)
        {
            if(loginDetails == null)
            {
                return BadRequest("Please enter Email and Password");
            }

            User user = db.Users.Where(x => x.Email == loginDetails.Email).FirstOrDefault();

            if(user == null)
            {
                return BadRequest("Please enter Email and Password");
            }

            if(BCrypt.Net.BCrypt.Verify(loginDetails.Password, user.Password))
            {
                user.Password = null;
                return Ok(user);
            }
            else
            {
                return BadRequest("Incorrect Email or Password");
            }
        }

        // GET: api/Weather/Gostivar
        [Route("api/Weather")]
        [HttpGet]
        public IHttpActionResult GetWeather(string city, string units)
        {
            var client = new RestClient("https://api.openweathermap.org/data/2.5/forecast?q="+city+"&units="+units+"&appid=d7baa2a4209fe531b51fbbf7acdbfe70");
            var request = new RestRequest("https://api.openweathermap.org/data/2.5/forecast?q="+city+"&units="+units+"&appid=d7baa2a4209fe531b51fbbf7acdbfe70", Method.Get);
            RestResponse response = client.Execute(request);
            return Ok(response.Content);
        }
    }
}
