﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ApiProject.DBClasses;
using System.Diagnostics;

namespace WebApp.Controllers
{
    public class EditCustomerController : Controller
    {
        public IActionResult Index(string id)
        {
            
            if (Request.Method.Equals("POST"))
            {
                ViewBag.Success = PostActivities();
                
                
            }
            if (String.IsNullOrEmpty(id)) return BadRequest();
            try
            {
                Customer cust = WebApp.Utills.Service.WebServiceUtills.JsonToCustomerDecoder(WebApp.Utills.Service.WebService.GetCustomerByID(System.Int32.Parse(id)));
                ViewBag.cust = cust;
                Debug.WriteLine(cust.CustomerID);
                 return View();
            }catch(Exception e)
            {
                //TODO : Fix it properly
            }
            return BadRequest();
            
        }
        public bool PostActivities()
        {
            return true;
        }
    }
}