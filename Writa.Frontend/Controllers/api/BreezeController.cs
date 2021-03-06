﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Breeze;
using Breeze.WebApi2;
using Writa.Models;
namespace Writa.Frontend.Controllers.api
{
    /*
     * This controller handles any breeze.js requests for data. 
     */
     

    [BreezeController]
    public class BreezeController : ApiController
    {
        public IDataHelper _datahelper;

        public BreezeController(IDataHelper d)
        {
            _datahelper = d;
        }

        [Authorize]
        [Queryable(EnableConstantParameterization = false)] // fix found here http://www.kacode.com/v973233-.html
        public IQueryable<WritaPost> GetPosts()
        {
            return _datahelper.GetAllPosts();
        }

        [Authorize]
        public WritaPost GetPost(string postid)
        {
            return _datahelper.GetPostFromId(postid);
        }

    }
}