﻿using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neon.Entities;
using System.IO;

namespace EntityTests {
    class TestData2 : GameData<TestData2> {
        public int Value;

        public override void DoCopyFrom(TestData2 source) {
            Value = source.Value;
        }

        public override bool SupportsMultipleModifications {
            get { return false; }
        }

        public override int HashCode {
            get { return Value; }
        }
    }

    class AllTriggers : ITriggerLifecycle, ITriggerModified, ITriggerUpdate, ITriggerGlobalPreUpdate, ITriggerGlobalPostUpdate, ITriggerInput, ITriggerGlobalInput {
        public DataAccessor[] ComputeEntityFilter() {
            return new DataAccessor[] { };
        }

        public void OnAdded(IEntity entity) {
        }

        public void OnRemoved(IEntity entity) {
        }

        public void OnPassedFilter(IEntity entity) {
        }

        public void OnModified(IEntity entity) {
        }

        public void OnUpdate(IEntity entity) {
        }

        public void OnGlobalPreUpdate(IEntity singletonEntity) {
        }

        public void OnGlobalPostUpdate(IEntity singletonEntity) {
        }

        public Type IStructuredInputType {
            get { return typeof(int); }
        }

        public void OnInput(IStructuredInput input, IEntity entity) {
        }

        public void OnGlobalInput(IStructuredInput input, IEntity singletonEntity) {
        }
    }

    class CountUpdatesTrigger : ITriggerUpdate, ITriggerGlobalPreUpdate, ITriggerGlobalPostUpdate {
        public int Count = 0;
        public int PreCount = 0;
        public int PostCount = 0;

        public void OnUpdate(IEntity entity) {
            ++Count;
        }

        public DataAccessor[] ComputeEntityFilter() {
            return new DataAccessor[] { };
        }

        public void OnGlobalPreUpdate(IEntity singletonEntity) {
            ++PreCount;
        }

        public void OnGlobalPostUpdate(IEntity singletonEntity) {
            ++PostCount;
        }
    }

    public static class EntityManagerExtensions {
        public static void UpdateWorld(this EntityManager em) {
            IStructuredInput[] commands = new IStructuredInput[0];
            em.UpdateWorld(commands);
        }
    }

    [TestClass]
    public class EntityManagerTests {
        [TestMethod]
        public void Creation() {
            EntityManager em = new EntityManager(EntityFactory.Create());

            Assert.IsNotNull(em.SingletonEntity);
        }

        [TestMethod]
        public void UpdateNumber() {
            EntityManager em = new EntityManager(EntityFactory.Create());

            for (int i = 0; i < 50; ++i) {
                Assert.AreEqual(em.UpdateNumber, i);
                em.UpdateWorld();
            }
        }

        [TestMethod]
        public void AddTrigger() {
            EntityManager em = new EntityManager(EntityFactory.Create());

            CountUpdatesTrigger updates = new CountUpdatesTrigger();
            em.AddSystem(updates);
            em.AddSystem(new AllTriggers());

            em.UpdateWorld();
            Assert.AreEqual(1, updates.PreCount);
            Assert.AreEqual(1, updates.PostCount);
            Assert.AreEqual(0, updates.Count);
            updates.PreCount = 0;
            updates.PostCount = 0;

            int numEntities = 25;
            for (int i = 0; i < numEntities; ++i) {
                IEntity e = EntityFactory.Create(); 
                em.AddEntity(e);
            }

            for (int i = 1; i < 10; ++i) {
                em.UpdateWorld();
                Assert.AreEqual(i, updates.PreCount);
                Assert.AreEqual(i, updates.PostCount);
                Assert.AreEqual(i * numEntities, updates.Count);
            }
        }

        [TestMethod]
        public void AddEntity() {
            EntityManager em = new EntityManager(EntityFactory.Create());
            IEntity e = EntityFactory.Create();
            em.AddEntity(e);
        }

        [TestMethod]
        public void InitializeData() {
            EntityManager em = new EntityManager(EntityFactory.Create());

            IEntity e = EntityFactory.Create();
            em.AddEntity(e);

            TestData2 data = e.AddData<TestData2>();
            data.Value = 33;
            
            em.UpdateWorld();
            Assert.AreEqual(33, e.Current<TestData2>().Value);

            // by using ++ we ensure that the modify value is currently 33
            e.Modify<TestData2>().Value++;
            em.UpdateWorld();
            Assert.AreEqual(33, e.Previous<TestData2>().Value);
            Assert.AreEqual(34, e.Current<TestData2>().Value);
        }
    }
}
