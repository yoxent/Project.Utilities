using System.Collections.Generic;
using NUnit.Framework;
using ProjectUtilities.Pooling.Core;
using UnityEngine;

namespace ProjectUtilities.Pooling.Editor
{
    /// <summary>
    /// EditMode tests for ObjectPool: Rent/Return lifecycle, reuse, and no growth when within capacity.
    /// </summary>
    public class ObjectPoolEditModeTest
    {
        private Transform _parent;
        private GameObject _prefabGo;
        private TestPoolableBehaviour _prefab;
        private ObjectPool<TestPoolableBehaviour> _pool;

        [SetUp]
        public void SetUp()
        {
            _parent = new GameObject("PoolRoot").transform;
            _prefabGo = new GameObject("Prefab");
            _prefab = _prefabGo.AddComponent<TestPoolableBehaviour>();

            _pool = new ObjectPool<TestPoolableBehaviour>(
                _prefab,
                initialSize: 0,
                maxSize: 5,
                autoExpand: true,
                _parent);
        }

        [TearDown]
        public void TearDown()
        {
            if (_parent != null)
                Object.DestroyImmediate(_parent.gameObject);
            if (_prefabGo != null)
                Object.DestroyImmediate(_prefabGo);
        }

        [Test]
        public void Rent_ThenReturn_ReusesSameInstance()
        {
            var a = _pool.Rent();
            Assert.That(a, Is.Not.Null);

            _pool.Return(a);

            var b = _pool.Rent();
            Assert.That(b, Is.SameAs(a), "Returned instance should be reused on next Rent.");
        }

        [Test]
        public void RentReturnLoop_TotalCountDoesNotGrow_WhenWithinMaxSize()
        {
            // Rent 5 (pool grows from 2 to 5)
            var first = new List<TestPoolableBehaviour>();
            for (int i = 0; i < 5; i++)
            {
                var instance = _pool.Rent();
                Assert.That(instance, Is.Not.Null);
                first.Add(instance);
            }

            int totalAfterFirstRent = _pool.ActiveCount + _pool.InactiveCount;
            Assert.That(totalAfterFirstRent, Is.EqualTo(5), "Pool should have exactly 5 instances.");

            // Return all 5
            foreach (var instance in first)
                _pool.Return(instance);

            Assert.That(_pool.ActiveCount, Is.EqualTo(0));
            Assert.That(_pool.InactiveCount, Is.EqualTo(5));

            // Rent 5 again — should reuse, no new allocations
            var second = new List<TestPoolableBehaviour>();
            for (int i = 0; i < 5; i++)
                second.Add(_pool.Rent());

            int totalAfterSecondRent = _pool.ActiveCount + _pool.InactiveCount;
            Assert.That(totalAfterSecondRent, Is.EqualTo(5), "Total instances should still be 5 (no growth).");

            // Same objects reused
            foreach (var instance in second)
                Assert.That(first, Does.Contain(instance), "All second Rent() calls should return previously created instances.");
        }

        [Test]
        public void MaxSize_NoExpand_RentBeyondMaxReturnsNull()
        {
            var noExpandPool = new ObjectPool<TestPoolableBehaviour>(_prefab, initialSize: 2, maxSize: 5, autoExpand: false, _parent);
            for (int i = 0; i < 5; i++)
                Assert.That(noExpandPool.Rent(), Is.Not.Null, "First 5 Rents should succeed.");

            var sixth = noExpandPool.Rent();
            Assert.That(sixth, Is.Null, "6th Rent with maxSize=5 and autoExpand=false should return null.");
        }

        [Test]
        public void Return_ThenRent_InstanceAvailableAgain()
        {
            var a = _pool.Rent();
            _pool.Return(a);
            var b = _pool.Rent();
            Assert.That(b, Is.SameAs(a));
            Assert.That(_pool.ActiveCount, Is.EqualTo(1));
            Assert.That(_pool.InactiveCount, Is.EqualTo(0), "With initialSize 0, only one instance exists; after Rent it is active.");
        }

        private class TestPoolableBehaviour : MonoBehaviour, IPoolable
        {
            public void OnSpawned() { }
            public void OnDespawned() { }
        }
    }
}
