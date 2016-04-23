// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Microsoft.EntityFrameworkCore.Metadata.Internal
{
    [DebuggerDisplay("{DeclaringEntityType.Name,nq}.{Name,nq} ({ClrType?.Name,nq})")]
    public class Property
        : ConventionalAnnotatable,
            IMutableProperty,
            IPropertyBaseAccessors,
            IPropertyPropertyInfoAccessor,
            IPropertyIndexesAccessor,
            IPropertyKeyMetadata,
            IPropertyIndexMetadata
    {
        // Warning: Never access these fields directly as access needs to be thread-safe
        private IClrPropertyGetter _getter;
        private IClrPropertySetter _setter;
        private PropertyAccessors _accessors;
        private PropertyIndexes _indexes;

        private int _flags;

        private ConfigurationSource _configurationSource;
        private ConfigurationSource? _isReadOnlyAfterSaveConfigurationSource;
        private ConfigurationSource? _isReadOnlyBeforeSaveConfigurationSource;
        private ConfigurationSource? _isNullableConfigurationSource;
        private ConfigurationSource? _isConcurrencyTokenConfigurationSource;
        private ConfigurationSource? _isStoreGeneratedAlwaysConfigurationSource;
        private ConfigurationSource? _requiresValueGeneratorConfigurationSource;
        private ConfigurationSource? _valueGeneratedConfigurationSource;

        public Property(
            [NotNull] string name,
            [NotNull] Type clrType,
            [NotNull] EntityType declaringEntityType,
            ConfigurationSource configurationSource)
        {
            Check.NotEmpty(name, nameof(name));
            Check.NotNull(clrType, nameof(clrType));
            Check.NotNull(declaringEntityType, nameof(declaringEntityType));

            Name = name;
            ClrType = clrType;
            DeclaringEntityType = declaringEntityType;
            _configurationSource = configurationSource;

            Builder = new InternalPropertyBuilder(this, declaringEntityType.Model.Builder);
        }

        public Property(
            [NotNull] PropertyInfo propertyInfo,
            [NotNull] EntityType declaringEntityType,
            ConfigurationSource configurationSource)
            : this(Check.NotNull(propertyInfo, nameof(propertyInfo)).Name,
                propertyInfo.PropertyType,
                Check.NotNull(declaringEntityType, nameof(declaringEntityType)),
                configurationSource)
        {
            PropertyInfo = propertyInfo;
        }

        public virtual string Name { get; }
        public virtual EntityType DeclaringEntityType { get; }
        public virtual InternalPropertyBuilder Builder { get; [param: CanBeNull] set; }

        public virtual ConfigurationSource GetConfigurationSource() => _configurationSource;

        public virtual void UpdateConfigurationSource(ConfigurationSource configurationSource)
            => _configurationSource = _configurationSource.Max(configurationSource);

        // Needed for a workaround before reference counting is implemented
        // Issue #214
        public virtual void SetConfigurationSource(ConfigurationSource configurationSource)
            => _configurationSource = configurationSource;

        public virtual Type ClrType { get; }

        public virtual bool IsNullable
        {
            get
            {
                bool value;
                return TryGetFlag(PropertyFlags.IsNullable, out value) ? value : DefaultIsNullable;
            }
            set { SetIsNullable(value, ConfigurationSource.Explicit); }
        }

        public virtual void SetIsNullable(bool nullable, ConfigurationSource configurationSource)
        {
            if (nullable)
            {
                if (!ClrType.IsNullableType())
                {
                    throw new InvalidOperationException(CoreStrings.CannotBeNullable(Name, DeclaringEntityType.DisplayName(), ClrType.Name));
                }

                if (Keys != null)
                {
                    throw new InvalidOperationException(CoreStrings.CannotBeNullablePK(Name, DeclaringEntityType.DisplayName()));
                }
            }

            UpdateIsNullableConfigurationSource(configurationSource);

            var isChanging = IsNullable != nullable;
            SetFlag(nullable, PropertyFlags.IsNullable);
            if (isChanging)
            {
                DeclaringEntityType.Model.ConventionDispatcher.OnPropertyNullableChanged(Builder);
            }
        }

        private bool DefaultIsNullable => ClrType.IsNullableType();

        public virtual ConfigurationSource? GetIsNullableConfigurationSource() => _isNullableConfigurationSource;

        private void UpdateIsNullableConfigurationSource(ConfigurationSource configurationSource)
            => _isNullableConfigurationSource = configurationSource.Max(_isNullableConfigurationSource);

        public virtual ValueGenerated ValueGenerated
        {
            get
            {
                var value = _flags & (int)PropertyFlags.ValueGenerated;

                return value == 0 ? DefaultValueGenerated : (ValueGenerated)((value >> 8) - 1);
            }
            set { SetValueGenerated(value, ConfigurationSource.Explicit); }
        }

        public virtual void SetValueGenerated(ValueGenerated? valueGenerated, ConfigurationSource configurationSource)
        {
            _flags &= ~(int)PropertyFlags.ValueGenerated;

            if (valueGenerated == null)
            {
                _valueGeneratedConfigurationSource = null;
            }
            else
            {
                _flags |= ((int)valueGenerated + 1) << 8;
                UpdateValueGeneratedConfigurationSource(configurationSource);
            }
        }

        private static ValueGenerated DefaultValueGenerated => ValueGenerated.Never;
        public virtual ConfigurationSource? GetValueGeneratedConfigurationSource() => _valueGeneratedConfigurationSource;

        private void UpdateValueGeneratedConfigurationSource(ConfigurationSource configurationSource)
            => _valueGeneratedConfigurationSource = configurationSource.Max(_valueGeneratedConfigurationSource);

        public virtual bool IsReadOnlyBeforeSave
        {
            get
            {
                bool value;
                return TryGetFlag(PropertyFlags.IsReadOnlyBeforeSave, out value) ? value : DefaultIsReadOnlyBeforeSave;
            }
            set { SetIsReadOnlyBeforeSave(value, ConfigurationSource.Explicit); }
        }

        public virtual void SetIsReadOnlyBeforeSave(bool readOnlyBeforeSave, ConfigurationSource configurationSource)
        {
            SetFlag(readOnlyBeforeSave, PropertyFlags.IsReadOnlyBeforeSave);
            UpdateIsReadOnlyBeforeSaveConfigurationSource(configurationSource);
        }

        private bool DefaultIsReadOnlyBeforeSave
            => (ValueGenerated == ValueGenerated.OnAddOrUpdate)
               && !IsStoreGeneratedAlways;

        public virtual ConfigurationSource? GetIsReadOnlyBeforeSaveConfigurationSource() => _isReadOnlyBeforeSaveConfigurationSource;

        private void UpdateIsReadOnlyBeforeSaveConfigurationSource(ConfigurationSource configurationSource)
            => _isReadOnlyBeforeSaveConfigurationSource = configurationSource.Max(_isReadOnlyBeforeSaveConfigurationSource);

        public virtual bool IsReadOnlyAfterSave
        {
            get
            {
                bool value;
                return TryGetFlag(PropertyFlags.IsReadOnlyAfterSave, out value) ? value : DefaultIsReadOnlyAfterSave;
            }
            set { SetIsReadOnlyAfterSave(value, ConfigurationSource.Explicit); }
        }

        public virtual void SetIsReadOnlyAfterSave(bool readOnlyAfterSave, ConfigurationSource configurationSource)
        {
            if (!readOnlyAfterSave
                && Keys != null)
            {
                throw new InvalidOperationException(CoreStrings.KeyPropertyMustBeReadOnly(Name, DeclaringEntityType.Name));
            }
            SetFlag(readOnlyAfterSave, PropertyFlags.IsReadOnlyAfterSave);
            UpdateIsReadOnlyAfterSaveConfigurationSource(configurationSource);
        }

        private bool DefaultIsReadOnlyAfterSave
            => ((ValueGenerated == ValueGenerated.OnAddOrUpdate)
                && !IsStoreGeneratedAlways)
               || Keys != null;

        public virtual ConfigurationSource? GetIsReadOnlyAfterSaveConfigurationSource() => _isReadOnlyAfterSaveConfigurationSource;

        private void UpdateIsReadOnlyAfterSaveConfigurationSource(ConfigurationSource configurationSource)
            => _isReadOnlyAfterSaveConfigurationSource = configurationSource.Max(_isReadOnlyAfterSaveConfigurationSource);

        public virtual bool RequiresValueGenerator
        {
            get
            {
                bool value;
                return TryGetFlag(PropertyFlags.RequiresValueGenerator, out value) ? value : DefaultRequiresValueGenerator;
            }
            set { SetRequiresValueGenerator(value, ConfigurationSource.Explicit); }
        }

        public virtual void SetRequiresValueGenerator(bool requiresValueGenerator, ConfigurationSource configurationSource)
        {
            SetFlag(requiresValueGenerator, PropertyFlags.RequiresValueGenerator);
            UpdateRequiresValueGeneratorConfigurationSource(configurationSource);
        }

        private static bool DefaultRequiresValueGenerator => false;
        public virtual ConfigurationSource? GetRequiresValueGeneratorConfigurationSource() => _requiresValueGeneratorConfigurationSource;

        private void UpdateRequiresValueGeneratorConfigurationSource(ConfigurationSource configurationSource)
            => _requiresValueGeneratorConfigurationSource = configurationSource.Max(_requiresValueGeneratorConfigurationSource);

        public virtual bool IsShadowProperty => PropertyInfo == null;

        public virtual bool IsConcurrencyToken
        {
            get
            {
                bool value;
                return TryGetFlag(PropertyFlags.IsConcurrencyToken, out value) ? value : DefaultIsConcurrencyToken;
            }
            set { SetIsConcurrencyToken(value, ConfigurationSource.Explicit); }
        }

        public virtual void SetIsConcurrencyToken(bool concurrencyToken, ConfigurationSource configurationSource)
        {
            if (IsConcurrencyToken != concurrencyToken)
            {
                SetFlag(concurrencyToken, PropertyFlags.IsConcurrencyToken);

                DeclaringEntityType.PropertyMetadataChanged();
            }
            UpdateIsConcurrencyTokenConfigurationSource(configurationSource);
        }

        private static bool DefaultIsConcurrencyToken => false;
        public virtual ConfigurationSource? GetIsConcurrencyTokenConfigurationSource() => _isConcurrencyTokenConfigurationSource;

        private void UpdateIsConcurrencyTokenConfigurationSource(ConfigurationSource configurationSource)
            => _isConcurrencyTokenConfigurationSource = configurationSource.Max(_isConcurrencyTokenConfigurationSource);

        public virtual bool IsStoreGeneratedAlways
        {
            get
            {
                bool value;
                return TryGetFlag(PropertyFlags.StoreGeneratedAlways, out value) ? value : DefaultStoreGeneratedAlways;
            }
            set { SetIsStoreGeneratedAlways(value, ConfigurationSource.Explicit); }
        }

        public virtual void SetIsStoreGeneratedAlways(bool storeGeneratedAlways, ConfigurationSource configurationSource)
        {
            if (IsStoreGeneratedAlways != storeGeneratedAlways)
            {
                SetFlag(storeGeneratedAlways, PropertyFlags.StoreGeneratedAlways);

                DeclaringEntityType.PropertyMetadataChanged();
            }
            UpdateIsStoreGeneratedAlwaysConfigurationSource(configurationSource);
        }

        private bool DefaultStoreGeneratedAlways => (ValueGenerated == ValueGenerated.OnAddOrUpdate) && IsConcurrencyToken;
        public virtual ConfigurationSource? GetIsStoreGeneratedAlwaysConfigurationSource() => _isStoreGeneratedAlwaysConfigurationSource;

        private void UpdateIsStoreGeneratedAlwaysConfigurationSource(ConfigurationSource configurationSource)
            => _isStoreGeneratedAlwaysConfigurationSource = configurationSource.Max(_isStoreGeneratedAlwaysConfigurationSource);

        public virtual IEnumerable<ForeignKey> FindContainingForeignKeys()
            => ((IProperty)this).FindContainingForeignKeys().Cast<ForeignKey>();

        public virtual IEnumerable<Key> FindContainingKeys()
            => ((IProperty)this).FindContainingKeys().Cast<Key>();

        public virtual IEnumerable<Index> FindContainingIndexes()
            => ((IProperty)this).FindContainingIndexes().Cast<Index>();

        private bool TryGetFlag(PropertyFlags flag, out bool value)
        {
            var coded = _flags & (int)flag;
            value = coded == (int)flag;
            return coded != 0;
        }

        private void SetFlag(bool value, PropertyFlags flag)
        {
            if (value)
            {
                _flags |= (int)flag;
            }
            else
            {
                var falseValue = ((int)flag << 1) & (int)flag;
                _flags = (_flags & ~(int)flag) | falseValue;
            }
        }

        internal static string Format(IEnumerable<IProperty> properties)
            => "{" + string.Join(", ", properties.Select(p => "'" + p.Name + "'")) + "}";

        IEntityType IPropertyBase.DeclaringEntityType => DeclaringEntityType;
        IMutableEntityType IMutableProperty.DeclaringEntityType => DeclaringEntityType;

        private enum PropertyFlags
        {
            IsConcurrencyToken = 3 << 0,
            IsNullable = 3 << 2,
            IsReadOnlyBeforeSave = 3 << 4,
            IsReadOnlyAfterSave = 3 << 6,
            ValueGenerated = 7 << 8,
            RequiresValueGenerator = 3 << 11,
            StoreGeneratedAlways = 3 << 13
        }

        public static bool AreCompatible([NotNull] IReadOnlyList<Property> properties, [NotNull] EntityType entityType)
        {
            Check.NotNull(properties, nameof(properties));
            Check.NotNull(entityType, nameof(entityType));

            return properties.All(property =>
                property.IsShadowProperty
                || (entityType.HasClrType()
                    && (entityType.ClrType.GetRuntimeProperties().FirstOrDefault(p => p.Name == property.Name) != null)));
        }

        public virtual IClrPropertyGetter Getter
            => NonCapturingLazyInitializer.EnsureInitialized(ref _getter, PropertyInfo, p => new ClrPropertyGetterFactory().Create(p));

        public virtual IClrPropertySetter Setter
            => NonCapturingLazyInitializer.EnsureInitialized(ref _setter, PropertyInfo, p => new ClrPropertySetterFactory().Create(p));

        public virtual PropertyAccessors Accessors
            => NonCapturingLazyInitializer.EnsureInitialized(ref _accessors, this, p => new PropertyAccessorsFactory().Create(p));

        public virtual PropertyIndexes PropertyIndexes
        {
            get
            {
                return NonCapturingLazyInitializer.EnsureInitialized(ref _indexes, this,
                    property => property.DeclaringEntityType.CalculateIndexes(property));
            }

            set
            {
                if (value == null)
                {
                    // This path should only kick in when the model is still mutable and therefore access does not need
                    // to be thread-safe.
                    _indexes = null;
                }
                else
                {
                    NonCapturingLazyInitializer.EnsureInitialized(ref _indexes, value);
                }
            }
        }

        public virtual IKey PrimaryKey { get; [param: CanBeNull] set; }
        public virtual IReadOnlyList<IKey> Keys { get; [param: CanBeNull] set; }
        public virtual IReadOnlyList<IForeignKey> ForeignKeys { get; [param: CanBeNull] set; }
        public virtual IReadOnlyList<IIndex> Indexes { get; [param: CanBeNull] set; }
        public virtual PropertyInfo PropertyInfo { get; }
    }
}