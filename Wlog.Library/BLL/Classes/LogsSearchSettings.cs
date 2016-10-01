﻿//******************************************************************************
// <copyright file="license.md" company="Wlog project  (https://github.com/arduosoft/wlog)">
// Copyright (c) 2016 Wlog project  (https://github.com/arduosoft/wlog)
// Wlog project is released under LGPL terms, see license.md file.
// </copyright>
// <author>Daniele Fontani, Emanuele Bucaelli</author>
// <autogenerated>true</autogenerated>
//******************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wlog.Library.BLL.Enums;

namespace Wlog.Library.BLL.Classes
{
    public class LogsSearchSettings:SearchSettingsBase
    {
        public string SerchMessage { get; set; }
        public List<Guid> Applications { get; set; }
        public string OrderBy { get; set; }
        public SortDirection SortDirection { get; set; }
        public string FullTextQuery { get; set; }

        public LogsSearchSettings()
        {
            this.Applications = new List<Guid>();
        }

    }
}
