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
    public class ApplicationSearchSettings : SearchSettingsBase
    {
        public ApplicationFields Orderby { get; set; }
        public string SerchFilter { get; set; }
        public bool IsAdmin { get; set; }
        public List<Guid> ApplicationsForUser { get; set; }
    }
}
