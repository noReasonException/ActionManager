﻿using ApiProject.DBClasses;
using System.Web.Http;
using ApiProject.DBClasses.DB_EFContext;
using System.Web.Http.Results;
using ApiProject.DBClasses;
using ApiProject.DBClasses.DB_EFContext;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;
using ApiProject.Utills;

namespace ApiProject.Controllers
{
    
    public class ActivityController : ApiController
    {
        ///TODO:Code Duplication -> FixIT! One Common Static con and JsonSettings for both Controllers!
        static Context con = new Context();
        static Newtonsoft.Json.JsonSerializerSettings JsonSettings;

        static ActivityController()
        {
            con = new Context();
            JsonSettings = new Newtonsoft.Json.JsonSerializerSettings();
            JsonSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;

        }
        /// <summary>
        /// Gets the Customer ID , Return every Activity Ascosiated with him
        /// </summary>
        /// <param name="id"> The <see cref="Customer.CustomerID"/> </param>
        ///<returns>
        ///     <list type="bullet">
        ///         <item>
        ///             <term>200(OK)</term>
        ///             <description>Everything fine!</description>
        ///         </item>
        ///         <item>
        ///             <term>404(Not Found)</term>
        ///             <description>id not found</description>
        ///         </item>
        ///          <item>
        ///             <term>204(No Context)</term>
        ///             <description>id found , but has no ascosiated Activities with it!</description>
        ///         </item>
        ///         <item>
        ///             <term>400(Bad GateWay)</term>
        ///             <description>Fatal , Unkown error happened</description>
        ///         </item>
        ///     </list>
        /// 
        /// 
        /// </returns>
        [Route("api/Activity/{id}")]
        [HttpGet]
        public JsonResult<Activity[]> Get(int id)
        {
            Debug.WriteLine("api/Activity/id Get Controller:    Incoming Request GET_ALL_ACTIVITIES with ID{0} from {1}", id, Request.Headers.From);

            Activity[] ActivitiesFromDB;
            try
            {
                ActivitiesFromDB = con.ActivityContainer.Where<Activity>(o => o.Customer.CustomerID == id).ToArray();
            }catch(Exception e)
            {
                Debug.WriteLine("api/Activity/id Get Controller ,[From :{0}] Encountered a fatal error {1} ",Request.Headers.From,e.Message);
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadGateway)
                {
                    ReasonPhrase = "FATAL ERROR"
                });
            }
            if (ActivitiesFromDB.Length == 0)
            {
                if (con.CustomerContainer.Find(id) == null)
                {
                    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound));
                }
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NoContent));
            }
            Debug.WriteLine("api/Activity/id Get Controller:     Handles Appropiate Request , Returns 200! [From {0}]",Request.Headers.From);
            return Json(ActivitiesFromDB,JsonSettings);
        }
        /// <summary>Posts a new Activity for the <see cref="Customer"/> Customer with Id As Specified in <paramref name="id"/></summary>
        /// <param name="id">The <see cref="Customer"/> ID </param>
        /// <param name="ActivityFromForm"> The <see cref="Activity"/> Object To Add</param>
        /// <returns>
        ///      <list type="bullet">
        ///         <item>
        ///             <term>200(OK)</term>
        ///             <description>Everything fine!</description>
        ///         </item>
        ///         <item>
        ///             <term>404(Not Found)</term>
        ///             <description>id not found</description>
        ///         </item>
        ///         <item>
        ///             <term>400(Bad GateWay)</term>
        ///             <description>Fatal , Unkown error happened</description>
        ///         </item>
        ///     </list>
        /// </returns>
        [Route("api/Activity/{id}")]
        [HttpPost]
        public IHttpActionResult PostNewActivity(int id,Activity ActivityFromForm)
        {
            Debug.WriteLine("api/Activity/id Post Controller:Incoming Request POST_NEW_ACTIVITY with Activity", Utills.Utills.IFNULL(ActivityFromForm));

            Customer CustomerFromDB = con.CustomerContainer.Find(id);
            if (CustomerFromDB == null) return NotFound();
            //if (!ModelState.IsValid || ActivityFromForm == null) return BadRequest();
            ActivityFromForm.Customer = CustomerFromDB;
            con.ActivityContainer.Add(ActivityFromForm);
            try
            {
                con.SaveChanges();
            }
            catch (Exception e)//Todo:Catch Specific Exceptions
            {
                Debug.WriteLine("api/Activity/id Post Controller:   Faces Fatal Error{0} , Returns 400(Bad)! [From {1}]",e.Message, Request.Headers.From);

                return BadRequest();
            }
            Debug.WriteLine("api/Activity/id Post Controller:   Handles Appropiate Request , Returns 200! [From {0}]", Request.Headers.From);
            return Ok();
        }
        [Route("api/Activity/{ActivityId}")]
        [HttpDelete]
        public IHttpActionResult DeleteActivityByID(int ActivityId)
        {
            Activity ActivityFromDB_ToAttach = con.ActivityContainer.Find(ActivityId);
            if (ActivityFromDB_ToAttach == null) return NotFound();
            Activity Act =con.ActivityContainer.Attach(ActivityFromDB_ToAttach);
            con.ActivityContainer.Remove(Act);
            try
            {
                con.SaveChanges();
            }
            catch (Exception e)//Todo:Catch Specific Exceptions
            {
                Debug.WriteLine("api/Activity/id Delete Controller:   Faces Fatal Error{0} , Returns 400(Bad)! [From {1}]", e.Message, Request.Headers.From);

                return BadRequest();
            }
            return Ok();
        }


    }
    
}
