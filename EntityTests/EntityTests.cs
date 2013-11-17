﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Neon.Entities.Tests {
    [TestClass]
    public class EntityTest {
        [TestMethod]
        public void EntityCreation() {
            IEntity entity = new Entity();
        }

        [TestMethod]
        public void EntityUniqueId() {
            HashSet<int> ids = new HashSet<int>();

            for (int i = 0; i < 200; ++i) {
                IEntity entity = new Entity();
                Assert.IsTrue(ids.Add(entity.UniqueId));
            }
        }

        [TestMethod]
        [ExpectedException(typeof(AlreadyAddedDataException))]

        public void CannotAddMultipleDataInstances() {
            Entity entity = new Entity();
            entity.AddData<TestData0>();
            entity.AddData<TestData0>();
            entity.DataStateChangeUpdate();
        }

        [TestMethod]
        [ExpectedException(typeof(AlreadyAddedDataException))]
        public void CannotAddMultipleDataInstancesAfterStateUpdates() {
            Entity entity = new Entity();
            entity.AddData<TestData0>();
            entity.DataStateChangeUpdate();
            entity.AddData<TestData0>();
            entity.DataStateChangeUpdate();
        }

        [TestMethod]
        public void InitializingReturnsOneConstantInstance() {
            IEntity entity = new Entity();
            TestData0 data0 = entity.AddData<TestData0>();
            TestData0 data1 = entity.AddOrModify<TestData0>();
            TestData0 data2 = entity.Modify<TestData0>();

            Assert.AreEqual(data0, data1);
            Assert.AreEqual(data0, data2);
        }

        [TestMethod]
        [ExpectedException(typeof(NoSuchDataException))]
        public void AddedDataIsNotContained() {
            IEntity entity = new Entity();
            entity.AddData<TestData0>();

            Assert.IsFalse(entity.ContainsData<TestData0>());
            entity.Current<TestData0>();
        }

        /*
        [TestMethod]
        public void RemovedDataIsAccessibleThroughCurrent() {
            IEntity entity = new Entity();
            entity.AddData<TestData0>();

            ((Entity)entity).DataStateChangeUpdate();
            Assert.IsTrue(entity.ContainsData<TestData0>());

            entity.RemoveData<TestData0>();
            ((Entity)entity).DataStateChangeUpdate();

            entity.Current<TestData0>(); // ensure we can access the current data
            Assert.IsFalse(entity.ContainsData<TestData0>());
            try {
                entity.Previous<TestData0>();
                Assert.Fail();
            }
            catch (NoSuchDataException) { }
            try {
                entity.Modify<TestData0>();
                Assert.Fail();
            }
            catch (NoSuchDataException) { }

        }
        */
    }
}