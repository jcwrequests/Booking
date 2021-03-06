﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.MicroKernel.SubSystems.Configuration;
using Ploeh.Samples.Booking.WebModel;
using System.Web.Mvc;
using Ploeh.Samples.Booking.DomainModel;
using Ploeh.Samples.Booking.Persistence.FileSystem;
using System.IO;
using System.Web.Hosting;
using Ploeh.Samples.Booking.JsonAntiCorruption;
using Ploeh.Samples.Booking.PersistenceModel;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;

namespace Ploeh.Samples.Booking.WebUI.Windsor
{
    public class WebWindsorInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Classes
                .FromAssemblyContaining<HomeController>()
                .BasedOn<IController>()
                .LifestylePerWebRequest());

            container.Register(Classes
                .FromAssemblyInDirectory(new AssemblyFilter("bin").FilterByName(an => an.Name.StartsWith("Ploeh.Samples.Booking")))
                .Pick()
                .WithServiceAllInterfaces());

            container.Kernel.Resolver.AddSubResolver(new ExtensionConvention());
            container.Kernel.Resolver.AddSubResolver(new DirectoryConvention(container.Kernel));
            container.Kernel.Resolver.AddSubResolver(new CollectionResolver(container.Kernel));

            #region Manual configuration that requires maintenance
            container.Register(Component
                .For<DirectoryInfo>()
                .UsingFactoryMethod(() =>
                    new DirectoryInfo(HostingEnvironment.MapPath("~/Queue")).CreateIfAbsent())
                .Named("queueDirectory"));
            container.Register(Component
                .For<DirectoryInfo>()
                .UsingFactoryMethod(() =>
                    new DirectoryInfo(HostingEnvironment.MapPath("~/SSoT")).CreateIfAbsent())
                .Named("ssotDirectory"));
            container.Register(Component
                .For<DirectoryInfo>()
                .UsingFactoryMethod(() =>
                    new DirectoryInfo(HostingEnvironment.MapPath("~/ViewStore")).CreateIfAbsent())
                .Named("viewStoreDirectory"));            
            #endregion
        }
    }
}