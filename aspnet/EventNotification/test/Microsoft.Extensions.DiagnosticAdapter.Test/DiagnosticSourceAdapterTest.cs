// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNetCore.Testing.xunit;
using Xunit;

namespace Microsoft.Extensions.DiagnosticAdapter
{
    public class DiagnosticSourceAdapterTest
    {
        public class OneTarget
        {
            public int OneCallCount { get; private set; }

            [DiagnosticName("One")]
            public void One()
            {
                ++OneCallCount;
            }
        }

        [Fact]
        public void IsEnabled_TrueForEnlistedEvent()
        {
            // Arrange
            var adapter = CreateAdapter(new OneTarget());

            // Act & Assert
            Assert.True(adapter.IsEnabled("One"));
        }

        [Fact]
        public void IsEnabled_True_PredicateCalledForIsEnabled()
        {
            // Arrange
            var callCount = 0;
            Func<string, bool> isEnabled = (name) =>
            {
                Assert.Equal("One", name);
                callCount++;
                return true;
            };
            
            var adapter = CreateAdapter(new OneTarget(), isEnabled);

            // Act & Assert
            Assert.True(adapter.IsEnabled("One"));
            Assert.Equal(1, callCount);
        }

        [Fact]
        public void IsEnabled_False_PredicateCalledForIsEnabled()
        {
            // Arrange
            var callCount = 0;
            Func<string, bool> isEnabled = (name) =>
            {
                Assert.Equal("One", name);
                callCount++;
                return false;
            };

            var adapter = CreateAdapter(new OneTarget(), isEnabled);

            // Act & Assert
            Assert.False(adapter.IsEnabled("One"));
            Assert.Equal(1, callCount);
        }

        [Fact]
        public void IsEnabled_FalseForNonenlistedEvent()
        {
            // Arrange
            var adapter = CreateAdapter(new OneTarget());

            // Act & Assert
            Assert.False(adapter.IsEnabled("Two"));
        }

        [Fact]
        public void CallingWriteWillInvokeMethod()
        {
            // Arrange
            var target = new OneTarget();
            var adapter = CreateAdapter(target);

            // Act & Assert
            Assert.Equal(0, target.OneCallCount);
            adapter.Write("One", new { });
            Assert.Equal(1, target.OneCallCount);
        }

        [Fact]
        public void CallingWriteForNonEnlistedNameIsHarmless()
        {
            // Arrange
            var target = new OneTarget();
            var adapter = CreateAdapter(target);

            // Act & Assert
            Assert.Equal(0, target.OneCallCount);
            adapter.Write("Two", new { });
            Assert.Equal(0, target.OneCallCount);
        }

        [Fact]
        public void Write_True_CallsIsEnabled()
        {
            // Arrange
            var callCount = 0;
            Func<string, bool> isEnabled = (name) =>
            {
                Assert.Equal("One", name);
                callCount++;
                return true;
            };

            var target = new OneTarget();
            var adapter = CreateAdapter(target, isEnabled);

            // Act
            adapter.Write("One", new { });

            // Assert
            Assert.Equal(1, callCount);
            Assert.Equal(1, target.OneCallCount);
        }

        [Fact]
        public void Write_False_CallsIsEnabled()
        {
            // Arrange
            var callCount = 0;
            Func<string, bool> isEnabled = (name) =>
            {
                Assert.Equal("One", name);
                callCount++;
                return false;
            };

            var target = new OneTarget();
            var adapter = CreateAdapter(target, isEnabled);

            // Act
            adapter.Write("One", new { });

            // Assert
            Assert.Equal(1, callCount);
            Assert.Equal(0, target.OneCallCount);
        }

        private class TwoTarget
        {
            public string Alpha { get; private set; }
            public string Beta { get; private set; }
            public int Delta { get; private set; }

            [DiagnosticName("Two")]
            public void Two(string alpha, string beta, int delta)
            {
                Alpha = alpha;
                Beta = beta;
                Delta = delta;
            }
        }

        [Fact]
        public void ParametersWillSplatFromObjectByName()
        {
            // Arrange
            var target = new TwoTarget();
            var adapter = CreateAdapter(target);

            // Act
            adapter.Write("Two", new { alpha = "ALPHA", beta = "BETA", delta = -1 });

            // Assert
            Assert.Equal("ALPHA", target.Alpha);
            Assert.Equal("BETA", target.Beta);
            Assert.Equal(-1, target.Delta);
        }

        [Fact]
        public void ExtraParametersAreHarmless()
        {
            // Arrange
            var target = new TwoTarget();
            var adapter = CreateAdapter(target);

            // Act
            adapter.Write("Two", new { alpha = "ALPHA", beta = "BETA", delta = -1, extra = this });

            // Assert
            Assert.Equal("ALPHA", target.Alpha);
            Assert.Equal("BETA", target.Beta);
            Assert.Equal(-1, target.Delta);
        }

        [Fact]
        public void MissingParametersArriveAsNull()
        {
            // Arrange
            var target = new TwoTarget();
            var adapter = CreateAdapter(target);

            // Act
            adapter.Write("Two", new { alpha = "ALPHA", delta = -1 });

            // Assert
            Assert.Equal("ALPHA", target.Alpha);
            Assert.Null(target.Beta);
            Assert.Equal(-1, target.Delta);
        }

        [Fact]
        public void Write_CanDuckType()
        {
            // Arrange
            var target = new ThreeTarget();
            var adapter = CreateAdapter(target);

            // Act
            adapter.Write("Three", new
            {
                person = new Person
                {
                    FirstName = "Alpha",
                    Address = new Address
                    {
                        City = "Beta",
                        State = "Gamma",
                        Zip = 98028
                    }
                }
            });

            // Assert
            Assert.Equal("Alpha", target.Person.FirstName);
            Assert.Equal("Beta", target.Person.Address.City);
            Assert.Equal("Gamma", target.Person.Address.State);
            Assert.Equal(98028, target.Person.Address.Zip);
        }

        [Fact]
        public void Write_CanDuckType_RuntimeType()
        {
            // Arrange
            var target = new FourTarget();
            var adapter = CreateAdapter(target);

            // Act
            adapter.Write("Four", new
            {
                person = (Person)new CoolPerson
                {
                    FirstName = "Alpha",
                    Address = new Address
                    {
                        City = "Beta",
                        State = "Gamma",
                        Zip = 98028
                    },
                    Coolness = 5.7m,
                }
            });

            // Assert
            Assert.Equal("Alpha", target.Person.FirstName);
            Assert.Equal("Beta", target.Person.Address.City);
            Assert.Equal("Gamma", target.Person.Address.State);
            Assert.Equal(98028, target.Person.Address.Zip);
            Assert.Equal(5.7m, target.Person.Coolness);
        }

        [Fact]
        public void Write_CanDuckType_Null()
        {
            // Arrange
            var target = new ThreeTarget();
            var adapter = CreateAdapter(target);

            // Act
            adapter.Write("Three", new
            {
                person = (Person)null,
            });

            // Assert
            Assert.Null(target.Person);
        }

        [Fact]
        public void Write_NominialType()
        {
            // Arrange
            var target = new ThreeTarget();
            var adapter = CreateAdapter(target);

            // Act
            adapter.Write("Three", new NominalType()
            {
                Person = new Person
                {
                    FirstName = "Alpha",
                    Address = new Address
                    {
                        City = "Beta",
                        State = "Gamma",
                        Zip = 98028
                    }
                }
            });

            // Assert
            Assert.Equal("Alpha", target.Person.FirstName);
            Assert.Equal("Beta", target.Person.Address.City);
            Assert.Equal("Gamma", target.Person.Address.State);
            Assert.Equal(98028, target.Person.Address.Zip);
        }

        public class ThreeTarget
        {
            public IPerson Person { get; private set; }

            [DiagnosticName("Three")]
            public void Three(IPerson person)
            {
                Person = person;
            }
        }

        public class FourTarget
        {
            public ICoolPerson Person { get; private set; }

            [DiagnosticName("Four")]
            public void Three(ICoolPerson person)
            {
                Person = person;
            }
        }

        public interface IPerson
        {
            string FirstName { get; }
            string LastName { get; }
            IAddress Address { get; }
        }

        public interface IAddress
        {
            string City { get; }
            string State { get; }
            int Zip { get; }
        }

        public interface ICoolPerson : IPerson
        {
            decimal Coolness { get; }
        }

        public class Person
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public Address Address { get; set; }
        }

        public class CoolPerson : Person
        {
            public decimal Coolness { get; set; }
        }

        public class NominalType
        {
            public Person Person { get; set; }
        }

        public class DerivedPerson : Person
        {
            public double CoolnessFactor { get; set; }
        }

        public class Address
        {
            public string City { get; set; }
            public string State { get; set; }
            public int Zip { get; set; }
        }

        public class SomeValueType
        {
            public SomeValueType(int value)
            {
                Value = value;
            }

            public int Value { get; private set; }
        }

        private static DiagnosticSourceAdapter CreateAdapter(object target, Func<string, bool> isEnabled = null)
        {
            return new DiagnosticSourceAdapter(target, isEnabled, new ProxyDiagnosticSourceMethodAdapter());
        }
    }
}
