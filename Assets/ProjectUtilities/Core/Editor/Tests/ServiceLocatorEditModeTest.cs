using System.Reflection;
using NUnit.Framework;
using ProjectUtilities.Core;
using UnityEngine;
using UnityEngine.TestTools;

namespace ProjectUtilities.Core.Editor
{
    /// <summary>
    /// EditMode tests for ServiceLocator: Register, Get, Unregister, and singleton behavior.
    /// </summary>
    public class ServiceLocatorEditModeTest
    {
        private GameObject _locatorGo;

        [SetUp]
        public void SetUp()
        {
            ResetServiceLocatorInstance();
            _locatorGo = new GameObject("ServiceLocator");
            _locatorGo.AddComponent<ServiceLocator>();
        }

        [TearDown]
        public void TearDown()
        {
            if (_locatorGo != null)
            {
                Object.DestroyImmediate(_locatorGo);
            }

            ResetServiceLocatorInstance();
        }

        [Test]
        public void Register_ThenGet_ReturnsSameInstance()
        {
            var service = new TestService();
            ServiceLocator.Instance.Register(service);

            var retrieved = ServiceLocator.Instance.Get<TestService>();

            Assert.That(retrieved, Is.SameAs(service));
        }

        [Test]
        public void Get_BeforeRegister_ReturnsNull()
        {
            var retrieved = ServiceLocator.Instance.Get<TestService>();

            Assert.That(retrieved, Is.Null);
        }

        [Test]
        public void Unregister_ThenGet_ReturnsNull()
        {
            var service = new TestService();
            ServiceLocator.Instance.Register(service);
            ServiceLocator.Instance.Unregister<TestService>();

            var retrieved = ServiceLocator.Instance.Get<TestService>();

            Assert.That(retrieved, Is.Null);
        }

        [Test]
        public void Register_AfterUnregister_Works()
        {
            var first = new TestService();
            ServiceLocator.Instance.Register(first);
            ServiceLocator.Instance.Unregister<TestService>();

            var second = new TestService();
            ServiceLocator.Instance.Register(second);

            var retrieved = ServiceLocator.Instance.Get<TestService>();
            Assert.That(retrieved, Is.SameAs(second));
        }

        [Test]
        public void Register_Null_DoesNotStore_GetReturnsNull()
        {
            LogAssert.Expect(LogType.Error, "Cannot register null service.");
            ServiceLocator.Instance.Register<TestService>(null);

            var retrieved = ServiceLocator.Instance.Get<TestService>();

            Assert.That(retrieved, Is.Null);
        }

        [Test]
        public void Register_Null_DoesNotOverwriteExistingRegistration()
        {
            var service = new TestService();
            ServiceLocator.Instance.Register(service);
            LogAssert.Expect(LogType.Error, "Cannot register null service.");
            ServiceLocator.Instance.Register<TestService>(null);

            var retrieved = ServiceLocator.Instance.Get<TestService>();

            Assert.That(retrieved, Is.SameAs(service));
        }

        [Test]
        public void Singleton_SecondInstance_InstanceRemainsFirst()
        {
            var first = ServiceLocator.Instance;
            var secondGo = new GameObject("ServiceLocatorDuplicate");
            secondGo.AddComponent<ServiceLocator>();

            // Second Awake should not replace Instance (it destroys itself); Instance must still be the first
            Assert.That(ServiceLocator.Instance, Is.SameAs(first), "Instance should remain the first ServiceLocator.");

            if (secondGo != null)
            {
                Object.DestroyImmediate(secondGo);
            }
        }

        private static void ResetServiceLocatorInstance()
        {
            var field = typeof(ServiceLocator).GetField("_instance",
                BindingFlags.Static | BindingFlags.NonPublic);
            field?.SetValue(null, null);
        }

        private class TestService { }
    }
}
