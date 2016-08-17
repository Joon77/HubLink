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
using System.Diagnostics;
using Wlog.Library.BLL.Classes;

namespace Wlog.Library.BLL.Configuration
{
    public static class InfoPageConfigurator
    {
        private static InfoPageConfiguration current;
        public static InfoPageConfiguration Configuration
        {
            get
            {
                return current ?? (current = new InfoPageConfiguration());
            }
        }

        public static void Configure(Action<InfoPageConfiguration> conf)
        {

            StackFrame frame = new StackFrame(1);
            var method = frame.GetMethod();
            var type = method.DeclaringType;
            

      

            InfoPageConfiguration settings = new InfoPageConfiguration();

            settings.MainAssembly=type.Assembly;
            settings.ApplicationName = settings.MainAssembly.GetName().Name;
            settings.ApplicationSubtitle = "info about this application";
            conf.Invoke(settings);
            

            InfoPageConfigurator.current = settings;
           

        }
    }
}
