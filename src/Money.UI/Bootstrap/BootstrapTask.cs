﻿using Money.Data;
using Money.Services;
using Money.Services.Models.Builders;
using Neptuo;
using Neptuo.Activators;
using Neptuo.Data;
using Neptuo.Events;
using Neptuo.Formatters;
using Neptuo.Formatters.Converters;
using Neptuo.Formatters.Metadata;
using Neptuo.Models.Keys;
using Neptuo.Models.Repositories;
using Neptuo.Models.Snapshots;
using Neptuo.Queries;
using Neptuo.ReadModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI;

namespace Money.Bootstrap
{
    public class BootstrapTask
    {
        private PersistentEventDispatcher eventDispatcher;

        public IFactory<Price, decimal> PriceFactory { get; private set; }
        public IRepository<Outcome, IKey> OutcomeRepository { get; private set; }
        public IRepository<Category, IKey> CategoryRepository { get; private set; }
        public IDomainFacade DomainFacade { get; private set; }

        public ContainerEventStore EventStore { get; private set; }

        public IEventDispatcher EventDispatcher
        {
            get { return eventDispatcher; }
        }

        public IFormatter EventFormatter { get; private set; }

        public IQueryDispatcher QueryDispatcher { get; private set; }

        public void Initialize()
        {
            Domain();
            ReadModels();

            DomainFacade = new DefaultDomainFacade(
                OutcomeRepository,
                CategoryRepository,
                PriceFactory,
                QueryDispatcher
            );

            Migrate();
        }

        private void Domain()
        {
            Converts.Repository
                .AddJsonEnumSearchHandler()
                .AddJsonPrimitivesSearchHandler()
                .AddJsonObjectSearchHandler()
                .AddJsonKey()
                .AddJsonTimeSpan();

            ApplicationDataContainer root = ApplicationData.Current.LocalSettings;
            ApplicationDataContainer eventStoreContainer = root
                .CreateContainer("EventStore", ApplicationDataCreateDisposition.Always);

            EventStore = new ContainerEventStore(eventStoreContainer);
            eventDispatcher = new PersistentEventDispatcher(new EmptyEventStore());

            IFactory<ICompositeStorage> compositeStorageFactory = Factory.Default<JsonCompositeStorage>();

            ICompositeTypeProvider typeProvider = new ReflectionCompositeTypeProvider(
                new ReflectionCompositeDelegateFactory(),
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public
            );

            EventFormatter = new CompositeEventFormatter(typeProvider, compositeStorageFactory);

            OutcomeRepository = new AggregateRootRepository<Outcome>(
                EventStore,
                EventFormatter,
                new ReflectionAggregateRootFactory<Outcome>(),
                EventDispatcher,
                new NoSnapshotProvider(),
                new EmptySnapshotStore()
            );

            CategoryRepository = new AggregateRootRepository<Category>(
                EventStore,
                EventFormatter,
                new ReflectionAggregateRootFactory<Category>(),
                EventDispatcher,
                new NoSnapshotProvider(),
                new EmptySnapshotStore()
            );

            PriceFactory = new PriceFactory("CZK");
        }

        private void ReadModels()
        {
            ApplicationDataContainer root = ApplicationData.Current.LocalSettings;
            ApplicationDataContainer readModelsContainer;
            if (!root.Containers.TryGetValue("ReadModels", out readModelsContainer))
                readModelsContainer = root.CreateContainer("ReadModels", ApplicationDataCreateDisposition.Always);

            DefaultQueryDispatcher queryDispatcher = new DefaultQueryDispatcher();
            QueryDispatcher = queryDispatcher;

            CategoryBuilder categoryBuilder = new CategoryBuilder(readModelsContainer);
            queryDispatcher.AddAll(categoryBuilder);
            eventDispatcher.Handlers.Add(categoryBuilder);
        }

        public const int Version = 4;

        private void Migrate()
        {
            int currentVersion = 0;
            ApplicationDataContainer root = ApplicationData.Current.LocalSettings;

            ApplicationDataContainer migrationContainer;
            if (root.Containers.TryGetValue("Migration", out migrationContainer))
                currentVersion = (int?)migrationContainer.Values["Version"] ?? currentVersion;
            else
                migrationContainer = root.CreateContainer("Migration", ApplicationDataCreateDisposition.Always);

            if (currentVersion < 1)
                MigrateVersion1();

            if (currentVersion < 4)
                MigrateVersion4();

            migrationContainer.Values["Version"] = Version;
        }

        private void MigrateVersion1()
        {
            ApplicationDataContainer root = ApplicationData.Current.LocalSettings;

            ApplicationDataContainer eventStoreContainer;
            if (root.Containers.TryGetValue("EventStore", out eventStoreContainer))
            {
                List<EventModel> result = new List<EventModel>();
                foreach (ApplicationDataContainer aggregateContainer in eventStoreContainer.Containers.Values)
                {
                    foreach (ApplicationDataContainer eventContainer in aggregateContainer.Containers.Values)
                        eventContainer.Values["AggregateType"] = typeof(Outcome).AssemblyQualifiedName;
                }
            }
        }

        private void MigrateVersion4()
        {
            ApplicationDataContainer root = ApplicationData.Current.LocalSettings;
            ApplicationDataContainer readModelsContainer;
            if (root.Containers.TryGetValue("ReadModels", out readModelsContainer))
            {
                foreach (string name in readModelsContainer.Containers.Keys.ToList())
                    readModelsContainer.DeleteContainer(name);
            }

            DomainFacade.CreateCategoryAsync("Home", Colors.SandyBrown);
            DomainFacade.CreateCategoryAsync("Food", Colors.OrangeRed);
            DomainFacade.CreateCategoryAsync("Eating Out", Colors.DarkRed);
        }

        //private void MigrateVersion3()
        //{
        //    Rebuilder rebuilder = new Rebuilder(EventStore, EventFormatter);
        //    rebuilder.AddAll();
        //}
    }
}
