﻿//******************************************************************************
// <copyright file="license.md" company="Wlog project  (https://github.com/arduosoft/wlog)">
// Copyright (c) 2016 Wlog project  (https://github.com/arduosoft/wlog)
// Wlog project is released under LGPL terms, see license.md file.
// </copyright>
// <author>Daniele Fontani, Emanuele Bucaelli</author>
// <autogenerated>true</autogenerated>
//******************************************************************************
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using Wlog.BLL.Entities;
using Wlog.Library.BLL.Classes;
using Wlog.Library.BLL.Reporitories;

namespace Wlog.Web.Code.Helpers
{
    public class LogHelper
    {


        public static IPagedList<LogEntity> GetLogs(Guid applicationId, string sortOrder, string sortBy, string serchMessage, int pageSize, int pageNumber)
        {
           


            List<Guid> alloweApps=UserHelper.GetAppsIdsForUser(Membership.GetUser().UserName);
           
            if (!alloweApps.Contains(applicationId))
            {
                return new StaticPagedList<LogEntity>(new LogEntity[] { }, 0, 0, 0);
            }


            LogsSearchSettings settings = new LogsSearchSettings()
            {
                Applications = alloweApps,
                SerchMessage = serchMessage,
                PageNumber = pageNumber,
                PageSize = pageSize,
                FullTextQuery = serchMessage,
                SortDirection = (Library.BLL.Enums.SortDirection)Enum.Parse(typeof(Library.BLL.Enums.SortDirection), sortOrder.ToUpper()),
                OrderBy=sortBy
            };

            
         


            return RepositoryContext.Current.Logs.SearchLogindex(applicationId,settings);
        }

       

       
    }
}