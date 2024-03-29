// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Storage;

namespace Microsoft.EntityFrameworkCore.ChangeTracking.Internal
{
    // This is lower-level change tracking services used by the ChangeTracker and other parts of the system
    public class StateManager : IStateManager
    {
        private readonly Dictionary<object, InternalEntityEntry> _entityReferenceMap
            = new Dictionary<object, InternalEntityEntry>(ReferenceEqualityComparer.Instance);

        private readonly LazyRef<IDictionary<object, IList<Tuple<INavigation, InternalEntityEntry>>>> _referencedUntrackedEntities
            = new LazyRef<IDictionary<object, IList<Tuple<INavigation, InternalEntityEntry>>>>(
                () => new Dictionary<object, IList<Tuple<INavigation, InternalEntityEntry>>>());

        private IIdentityMap _identityMap0;
        private IIdentityMap _identityMap1;
        private Dictionary<IKey, IIdentityMap> _identityMaps;

        private readonly IInternalEntityEntryFactory _factory;
        private readonly IInternalEntityEntrySubscriber _subscriber;
        private readonly IModel _model;
        private readonly IDatabase _database;
        private readonly IConcurrencyDetector _concurrencyDetector;

        public StateManager(
            [NotNull] IInternalEntityEntryFactory factory,
            [NotNull] IInternalEntityEntrySubscriber subscriber,
            [NotNull] IInternalEntityEntryNotifier notifier,
            [NotNull] IValueGenerationManager valueGeneration,
            [NotNull] IModel model,
            [NotNull] IDatabase database,
            [NotNull] IConcurrencyDetector concurrencyDetector,
            [NotNull] ICurrentDbContext currentContext)
        {
            _factory = factory;
            _subscriber = subscriber;
            Notify = notifier;
            ValueGeneration = valueGeneration;
            _model = model;
            _database = database;
            _concurrencyDetector = concurrencyDetector;
            Context = currentContext.Context;
        }

        public virtual bool? SingleQueryMode { get; set; }

        public virtual IInternalEntityEntryNotifier Notify { get; }

        public virtual IValueGenerationManager ValueGeneration { get; }

        public virtual InternalEntityEntry GetOrCreateEntry(object entity)
        {
            var entry = TryGetEntry(entity);
            if (entry == null)
            {
                SingleQueryMode = false;

                var entityType = _model.FindEntityType(entity.GetType());

                if (entityType == null)
                {
                    throw new InvalidOperationException(CoreStrings.EntityTypeNotFound(entity.GetType().DisplayName(false)));
                }

                entry = _factory.Create(this, entityType, entity);

                _entityReferenceMap[entity] = entry;
            }
            return entry;
        }

        public virtual void BeginTrackingQuery() => SingleQueryMode = SingleQueryMode == null;

        public virtual InternalEntityEntry StartTrackingFromQuery(
            IEntityType baseEntityType,
            object entity,
            ValueBuffer valueBuffer)
        {
            var existingEntry = TryGetEntry(entity);
            if (existingEntry != null)
            {
                return existingEntry;
            }

            var clrType = entity.GetType();

            var newEntry = _factory.Create(this,
                baseEntityType.ClrType == clrType
                    ? baseEntityType
                    : _model.FindEntityType(clrType),
                entity, valueBuffer);

            foreach (var key in baseEntityType.GetKeys())
            {
                GetOrCreateIdentityMap(key).AddOrUpdate(newEntry);
            }

            _entityReferenceMap[entity] = newEntry;

            newEntry.MarkUnchangedFromQuery();

            _subscriber.SnapshotAndSubscribe(newEntry);

            return newEntry;
        }

        public virtual InternalEntityEntry TryGetEntry(IKey key, ValueBuffer valueBuffer, bool throwOnNullKey)
            => GetOrCreateIdentityMap(key).TryGetEntry(valueBuffer, throwOnNullKey);

        public virtual InternalEntityEntry TryGetEntry(object entity)
        {
            InternalEntityEntry entry;
            return !_entityReferenceMap.TryGetValue(entity, out entry) ? null : entry;
        }

        private IIdentityMap GetOrCreateIdentityMap(IKey key)
        {
            if (_identityMap0 == null)
            {
                _identityMap0 = key.GetIdentityMapFactory()();
                return _identityMap0;
            }

            if (_identityMap0.Key == key)
            {
                return _identityMap0;
            }

            if (_identityMap1 == null)
            {
                _identityMap1 = key.GetIdentityMapFactory()();
                return _identityMap1;
            }

            if (_identityMap1.Key == key)
            {
                return _identityMap1;
            }

            if (_identityMaps == null)
            {
                _identityMaps = new Dictionary<IKey, IIdentityMap>();
            }

            IIdentityMap identityMap;
            if (!_identityMaps.TryGetValue(key, out identityMap))
            {
                identityMap = key.GetIdentityMapFactory()();
                _identityMaps[key] = identityMap;
            }
            return identityMap;
        }

        private IIdentityMap FindIdentityMap(IKey key)
        {
            if (_identityMap0 == null)
            {
                return null;
            }

            if (_identityMap0.Key == key)
            {
                return _identityMap0;
            }

            if (_identityMap1 == null)
            {
                return null;
            }

            if (_identityMap1.Key == key)
            {
                return _identityMap1;
            }

            IIdentityMap identityMap;
            if (_identityMaps == null
                || !_identityMaps.TryGetValue(key, out identityMap))
            {
                return null;
            }
            return identityMap;
        }

        public virtual IEnumerable<InternalEntityEntry> Entries => _entityReferenceMap.Values;

        public virtual InternalEntityEntry StartTracking(InternalEntityEntry entry)
        {
            var entityType = entry.EntityType;

            if (entry.StateManager != this)
            {
                throw new InvalidOperationException(CoreStrings.WrongStateManager(entityType.Name));
            }

            var mapKey = entry.Entity ?? entry;
            var existingEntry = TryGetEntry(mapKey);

            if (existingEntry == null
                || existingEntry == entry)
            {
                _entityReferenceMap[mapKey] = entry;
            }
            else
            {
                throw new InvalidOperationException(CoreStrings.MultipleEntries(entityType.Name));
            }

            foreach (var key in entityType.GetKeys())
            {
                GetOrCreateIdentityMap(key).Add(entry);
            }

            _subscriber.SnapshotAndSubscribe(entry);

            return entry;
        }

        public virtual void StopTracking(InternalEntityEntry entry)
        {
            var mapKey = entry.Entity ?? entry;
            _entityReferenceMap.Remove(mapKey);

            foreach (var key in entry.EntityType.GetKeys())
            {
                FindIdentityMap(key)?.Remove(entry);
            }

            if (_referencedUntrackedEntities.HasValue)
            {
                var navigations = entry.EntityType.GetNavigations().ToList();

                foreach (var keyValuePair in _referencedUntrackedEntities.Value.ToList())
                {
                    var entityType = _model.FindEntityType(keyValuePair.Key.GetType());
                    if (navigations.Any(n => n.GetTargetType().IsAssignableFrom(entityType))
                        || entityType.GetNavigations().Any(n => n.GetTargetType().IsAssignableFrom(entry.EntityType)))
                    {
                        _referencedUntrackedEntities.Value.Remove(keyValuePair.Key);

                        var newList = keyValuePair.Value.Where(tuple => tuple.Item2 != entry).ToList();

                        if (newList.Any())
                        {
                            _referencedUntrackedEntities.Value.Add(keyValuePair.Key, newList);
                        }
                    }
                }
            }
        }

        public virtual void RecordReferencedUntrackedEntity(
            object referencedEntity, INavigation navigation, InternalEntityEntry referencedFromEntry)
        {
            IList<Tuple<INavigation, InternalEntityEntry>> danglers;
            if (!_referencedUntrackedEntities.Value.TryGetValue(referencedEntity, out danglers))
            {
                danglers = new List<Tuple<INavigation, InternalEntityEntry>>();
                _referencedUntrackedEntities.Value.Add(referencedEntity, danglers);
            }
            danglers.Add(Tuple.Create(navigation, referencedFromEntry));
        }

        public virtual IEnumerable<Tuple<INavigation, InternalEntityEntry>> GetRecordedReferers(object referencedEntity)
        {
            IList<Tuple<INavigation, InternalEntityEntry>> danglers;
            if (_referencedUntrackedEntities.HasValue
                && _referencedUntrackedEntities.Value.TryGetValue(referencedEntity, out danglers))
            {
                _referencedUntrackedEntities.Value.Remove(referencedEntity);
                return danglers;
            }

            return Enumerable.Empty<Tuple<INavigation, InternalEntityEntry>>();
        }

        public virtual InternalEntityEntry GetPrincipal(InternalEntityEntry dependentEntry, IForeignKey foreignKey)
            => FindIdentityMap(foreignKey.PrincipalKey)?.TryGetEntry(foreignKey, dependentEntry);

        public virtual InternalEntityEntry GetPrincipalUsingRelationshipSnapshot(InternalEntityEntry dependentEntry, IForeignKey foreignKey)
            => FindIdentityMap(foreignKey.PrincipalKey)?.TryGetEntryUsingRelationshipSnapshot(foreignKey, dependentEntry);

        public virtual void UpdateIdentityMap(InternalEntityEntry entry, IKey key)
        {
            if (entry.EntityState == EntityState.Detached)
            {
                return;
            }

            var identityMap = FindIdentityMap(key);
            if (identityMap == null)
            {
                return;
            }

            identityMap.RemoveUsingRelationshipSnapshot(entry);
            identityMap.Add(entry);
        }

        public virtual void UpdateDependentMap(InternalEntityEntry entry, IForeignKey foreignKey)
        {
            if (entry.EntityState == EntityState.Detached)
            {
                return;
            }

            FindIdentityMap(foreignKey.DeclaringEntityType.FindPrimaryKey())
                ?.FindDependentsMap(foreignKey)
                ?.Update(entry);
        }

        public virtual IEnumerable<InternalEntityEntry> GetDependents(
            InternalEntityEntry principalEntry, IForeignKey foreignKey)
        {
            var dependentIdentityMap = FindIdentityMap(foreignKey.DeclaringEntityType.FindPrimaryKey());
            return dependentIdentityMap != null
                ? dependentIdentityMap.GetDependentsMap(foreignKey).GetDependents(principalEntry)
                : Enumerable.Empty<InternalEntityEntry>();
        }

        public virtual IEnumerable<InternalEntityEntry> GetDependentsUsingRelationshipSnapshot(
            InternalEntityEntry principalEntry, IForeignKey foreignKey)
        {
            var dependentIdentityMap = FindIdentityMap(foreignKey.DeclaringEntityType.FindPrimaryKey());
            return dependentIdentityMap != null
                ? dependentIdentityMap.GetDependentsMap(foreignKey).GetDependentsUsingRelationshipSnapshot(principalEntry)
                : Enumerable.Empty<InternalEntityEntry>();
        }

        public virtual IEnumerable<InternalEntityEntry> GetDependentsFromNavigation(InternalEntityEntry principalEntry, IForeignKey foreignKey)
        {
            var navigation = foreignKey.PrincipalToDependent;
            if (navigation == null)
            {
                return null;
            }

            var navigationValue = principalEntry[navigation];
            if (navigationValue == null)
            {
                return Enumerable.Empty<InternalEntityEntry>();
            }

            if (foreignKey.IsUnique)
            {
                var dependentEntry = TryGetEntry(navigationValue);

                return dependentEntry != null
                    ? new[] { dependentEntry }
                    : Enumerable.Empty<InternalEntityEntry>();
            }

            return ((IEnumerable<object>)navigationValue).Select(TryGetEntry).Where(e => e != null);
        }

        public virtual int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            var entriesToSave = GetEntriesToSave();
            if (!entriesToSave.Any())
            {
                return 0;
            }

            try
            {
                var result = SaveChanges(entriesToSave);

                if (acceptAllChangesOnSuccess)
                {
                    AcceptAllChanges(entriesToSave);
                }

                return result;
            }
            catch
            {
                foreach (var entry in entriesToSave)
                {
                    entry.DiscardStoreGeneratedValues();
                }
                throw;
            }
        }

        private List<InternalEntityEntry> GetEntriesToSave()
        {
            foreach (var entry in Entries.Where(
                e => e.EntityState != EntityState.Detached
                     && e.HasConceptualNull).ToList())
            {
                entry.HandleConceptualNulls();
            }

            foreach (var entry in Entries.Where(e => e.EntityState == EntityState.Deleted).ToList())
            {
                entry.CascadeDelete();
            }

            return Entries
                .Where(e => e.EntityState == EntityState.Added
                            || e.EntityState == EntityState.Modified
                            || e.EntityState == EntityState.Deleted)
                .Select(e => e.PrepareToSave())
                .ToList();
        }

        public virtual async Task<int> SaveChangesAsync(
            bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            var entriesToSave = GetEntriesToSave();
            if (!entriesToSave.Any())
            {
                return 0;
            }

            try
            {
                var result = await SaveChangesAsync(entriesToSave, cancellationToken);

                if (acceptAllChangesOnSuccess)
                {
                    AcceptAllChanges(entriesToSave);
                }

                return result;
            }
            catch
            {
                foreach (var entry in entriesToSave)
                {
                    entry.DiscardStoreGeneratedValues();
                }
                throw;
            }
        }

        protected virtual int SaveChanges(
            [NotNull] IReadOnlyList<InternalEntityEntry> entriesToSave)
        {
            using (_concurrencyDetector.EnterCriticalSection())
            {
                return _database.SaveChanges(entriesToSave);
            }
        }

        protected virtual async Task<int> SaveChangesAsync(
            [NotNull] IReadOnlyList<InternalEntityEntry> entriesToSave,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            using (_concurrencyDetector.EnterCriticalSection())
            {
                return await _database.SaveChangesAsync(entriesToSave, cancellationToken);
            }
        }

        public virtual void AcceptAllChanges()
        {
            var changedEntries = Entries
                .Where(e => e.EntityState == EntityState.Added
                            || e.EntityState == EntityState.Modified
                            || e.EntityState == EntityState.Deleted)
                .ToList();

            AcceptAllChanges(changedEntries);
        }

        private static void AcceptAllChanges(IEnumerable<InternalEntityEntry> changedEntries)
        {
            foreach (var entry in changedEntries)
            {
                entry.AcceptChanges();
            }
        }

        public virtual DbContext Context { get; }
    }
}
