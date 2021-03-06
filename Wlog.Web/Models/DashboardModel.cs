﻿//******************************************************************************
// <copyright file="license.md" company="Wlog project  (https://github.com/arduosoft/wlog)">
// Copyright (c) 2016 Wlog project  (https://github.com/arduosoft/wlog)
// Wlog project is released under LGPL terms, see license.md file.
// </copyright>
// <author>Daniele Fontani, Emanuele Bucaelli</author>
// <autogenerated>true</autogenerated>
//******************************************************************************
using System.Collections.Generic;
using Wlog.BLL.Classes;

namespace Wlog.Web.Models
{
    /// <summary>
    /// A simple model to show statistic into home page
    /// </summary>
    public class DashboardModel
    {
        public long ErrorCount { get; set; }

        public long LogCount { get; set; }

        public long WarnCount { get; set; }

        public long InfoCount { get; set; }

        public List<QueueLoad> QueueLoad { get; set; }

        public  List<LogMessage> LastTen { get; set; }

        public List<MessagesListModel> AppLastTen { get; set; }
    }
}