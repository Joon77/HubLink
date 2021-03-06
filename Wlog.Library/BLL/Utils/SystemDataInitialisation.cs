﻿using NHibernate.Linq;
using NLog;
using System;
using System.Collections.Generic;
using System.Reflection;
using Wlog.BLL.Classes;
using Wlog.BLL.Entities;
using Wlog.DAL.NHibernate.Helpers;
using Wlog.Library.BLL.DataBase;
using Wlog.Library.BLL.Interfaces;
using Wlog.Library.BLL.Reporitories;

namespace Wlog.Library.BLL.Utils
{
    public class SystemDataInitialisation
    {
        private static volatile SystemDataInitialisation _instance;
        private static object lockObject = new object();
        private Logger _logger = LogManager.GetCurrentClassLogger();

        private SystemDataInitialisation()
        {
        }

        public static SystemDataInitialisation Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (lockObject)
                    {
                        _instance = new SystemDataInitialisation();
                    }
                }

                return _instance;
            }
        }


        /// <summary>
        /// Apply schema changes
        /// </summary>
        public void ApplySchemaChanges()
        {
            //TODO:if the entity change structure....in mongo what happen??
            _logger.Debug("[repo] entering ApplySchemaChanges");
            var uof = new UnitFactory();
            var unit = uof.GetUnit(RepositoryContext.Current.Applications);

            if (unit is UnitOfMongo)
            {
                Assembly entityAssembly = Assembly.GetAssembly(typeof(LogEntity));
                Type[] classes = Helpers.ReflectionHelper.GetTypesInNamespace(entityAssembly, "Wlog.BLL.Entities");

                foreach (Type enityClass in classes)
                {
                    try
                    {
                        object o = Activator.CreateInstance(enityClass);
                        if (o is IEntityBase)
                        {
                            string collectionName = o.GetType().Name;
                            //Create collection
                            MongoContext.Current.CreateCollectionIfNotExists(collectionName);
                        }
                    }
                    catch (Exception err)
                    {
                        _logger.Warn(err, "Unable to create collection for repo" + enityClass.FullName);
                    }
                }
            }
            else if (unit is UnitOfNhibernate)
            {
                NHibernateContext.ApplySchemaChanges();
            }
            else
            {
                throw new Exception("Unkwnow unit type");
            }
        }

        public void EnsureSampleData()
        {
            _logger.Debug("[SystemDataHelper]: EnsureSampleData");

            List<UserEntity> userList = RepositoryContext.Current.Users.GetAll();

            if (userList == null || userList.Count == 0)
            {
                ApplicationEntity app = new ApplicationEntity();
                app.ApplicationName = "SampleApp";
                app.IsActive = true;
                app.StartDate = DateTime.Now;
                app.EndDate = DateTime.Now.AddYears(1);
                app.PublicKey = new Guid("{8C075ED0-45A7-495A-8E09-3A98FD6E8248}");
                RepositoryContext.Current.Applications.Save(app);

                // Insert admin user
                UserEntity user = new UserEntity();
                user.CreationDate = DateTime.Now;
                user.Email = "mymail@weblog-logger.it";
                user.IsAdmin = true;
                user.IsApproved = true;
                user.IsLockedOut = false;
                user.Password = PasswordManagement.EncodePassword("12345678");
                user.ProfileId = RepositoryContext.Current.Profiles.GetProfileByName(Constants.Profiles.Admin).Id;
                user.Username = "admin";

                RepositoryContext.Current.Users.Save(user);

                RolesEntity role = RepositoryContext.Current.Roles.GetRoleByName(Constants.Roles.Admin);
                RepositoryContext.Current.Applications.AssignRoleToUser(app, user, role);
            }
        }

        public void InsertMissingDictionary()
        {
            var apps=RepositoryContext.Current.Applications.Find(null, 0, int.MaxValue, null, Enums.SortDirection.ASC);
            foreach(var app in apps)
            {
                var dict=RepositoryContext.Current.KeyPairRepository.GetDictionaries(app.Id,Constants.DictionaryNames.Main,0,0);
                if (dict == null || dict.Count == 0)
                {
                    DictionaryEntity d = new DictionaryEntity()
                    {
                        ApplicationId = app.Id,
                        Name = Constants.DictionaryNames.Main
                    };

                    RepositoryContext.Current.KeyPairRepository.CreateDictionary(d);
                }
            }
        }

        private RolesEntity InsertRoleIfNotExists(string rolename, bool global, bool application)
        {
            _logger.Debug("[SystemDataHelper]: InsertRoleIfNotExists");

            RolesEntity role = RepositoryContext.Current.Roles.GetRoleByName(rolename);

            if (role == null)
            {
                role = new RolesEntity() { RoleName = rolename, GlobalScope = global, ApplicationScope = application };
                RepositoryContext.Current.Roles.Save(role);
            }

            return role;
        }

        private ProfilesEntity InsertProfileIfNotExists(string profileName)
        {
            _logger.Debug("[SystemDataHelper]: InsertProfileIfNotExists");

            ProfilesEntity profile = RepositoryContext.Current.Profiles.GetProfileByName(profileName);

            if (profile == null)
            {
                profile = new ProfilesEntity() { ProfileName = profileName };
                RepositoryContext.Current.Profiles.Save(profile);
            }

            return profile;
        }

        public void InsertJobsDefinitions()
        {
            _logger.Debug("[SystemDataHelper]: InsertJobsDefinitions");

            try
            {
                // insert empty bin job
                var jobDefinition = InsertJobDefinition("EmptyBinJob", "This job physically deletes entries",
                   typeof(Wlog.Library.Scheduler.Jobs.EmptyBinJob).FullName);

                if (jobDefinition != null)
                {
                    InsertJobInstance(jobDefinition, "*/1 * * * *");
                }

                // insert move to bin job
                jobDefinition = InsertJobDefinition("MoveToBinJob", "This job moves enties to bin table",
                   typeof(Wlog.Library.Scheduler.Jobs.MoveToBinJob).FullName);

                if (jobDefinition != null)
                {
                    InsertJobInstance(jobDefinition, "*/1 * * * *");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        private JobDefinitionEntity InsertJobDefinition(string name, string description, string fullClassName)
        {
            var job = RepositoryContext.Current.JobDefinition.GetJobDefinitionByName(name);

            if (job == null)
            {
                JobDefinitionEntity jobDefinition = new JobDefinitionEntity()
                {
                    Name = name,
                    Description = description,
                    FullClassname = fullClassName,
                    Instantiable = true,
                    System = true
                };
                RepositoryContext.Current.JobDefinition.Save(jobDefinition);

                return jobDefinition;
            }

            return null;
        }

        private static JobInstanceEntity InsertJobInstance(JobDefinitionEntity jobDefinition, string cron)
        {
            var job = RepositoryContext.Current.JobInstance.GetJobInstanceByDefinitionAndCron(cron, jobDefinition);

            if (job == null)
            {
                JobInstanceEntity jobInstance = new JobInstanceEntity()
                {
                    Active = true,
                    ActivationDate = DateTime.UtcNow,
                    JobDefinitionID = jobDefinition.Id,
                    CronExpression = cron
                };

                RepositoryContext.Current.JobInstance.Save(jobInstance);

                return jobInstance;
            }

            return null;
        }

        public void InsertRolesAndProfiles()
        {
            _logger.Debug("[SystemDataHelper]: InsertRolesAndProfiles");

            // create roles
            var adminRole = InsertRoleIfNotExists(Constants.Roles.Admin, true, false);
            var appWriter = InsertRoleIfNotExists(Constants.Roles.AppWriter, true, false);
            var createApp = InsertRoleIfNotExists(Constants.Roles.CreateApp, true, false);
            var login = InsertRoleIfNotExists(Constants.Roles.Login, true, false);

            var readLog = InsertRoleIfNotExists(Constants.Roles.ReadLog, false, true);
            var writeLog = InsertRoleIfNotExists(Constants.Roles.WriteLog, false, true);

            // create profiles
            var adminProfile = InsertProfileIfNotExists(Constants.Profiles.Admin);
            var apiUser = InsertProfileIfNotExists(Constants.Profiles.ApiUser);
            var reader = InsertProfileIfNotExists(Constants.Profiles.Reader);
            var standardUser = InsertProfileIfNotExists(Constants.Profiles.StandardUser);

            // assign profiles to roles
            RepositoryContext.Current.Profiles.AssignRolesToProfile(adminProfile, new List<RolesEntity>() { adminRole });
            RepositoryContext.Current.Profiles.AssignRolesToProfile(apiUser, new List<RolesEntity>() { writeLog });
            RepositoryContext.Current.Profiles.AssignRolesToProfile(reader, new List<RolesEntity>() { login, readLog });
            RepositoryContext.Current.Profiles.AssignRolesToProfile(standardUser, new List<RolesEntity>() { login, readLog, createApp, appWriter });
        }
    }
}
