using System;
using System.Reactive.Subjects;
using EcsRx.Entities;
using EcsRx.Executor.Handlers;
using EcsRx.Groups;
using EcsRx.Groups.Accessors;
using EcsRx.Pools;
using EcsRx.Systems;
using NSubstitute;
using Xunit;

namespace EcsRx.Tests
{
    public class TeardownSystemHandlerTests
    {
        [Fact]
        public void should_correctly_handle_systems()
        {
            var mockPoolManager = Substitute.For<IPoolManager>();
            var teardownSystemHandler = new TeardownSystemHandler(mockPoolManager);
            
            var fakeMatchingSystem = Substitute.For<ITeardownSystem>();
            var fakeNonMatchingSystem1 = Substitute.For<IReactToEntitySystem>();
            var fakeNonMatchingSystem2 = Substitute.For<ISystem>();
            
            Assert.True(teardownSystemHandler.CanHandleSystem(fakeMatchingSystem));
            Assert.False(teardownSystemHandler.CanHandleSystem(fakeNonMatchingSystem1));
            Assert.False(teardownSystemHandler.CanHandleSystem(fakeNonMatchingSystem2));
        }
        
        [Fact]
        public void should_teardown_entity_when_removed()
        {
            var fakeEntity1 = Substitute.For<IEntity>();
            fakeEntity1.Id.Returns(Guid.NewGuid());
            var fakeEntities = new IEntity[] {};

            var removeSubject = new Subject<IEntity>();
            var mockObservableGroup = Substitute.For<IObservableGroup>();
            mockObservableGroup.OnEntityAdded.Returns(new Subject<IEntity>());
            mockObservableGroup.OnEntityRemoved.Returns(removeSubject);
            mockObservableGroup.Entities.Returns(fakeEntities);
            
            var mockPoolManager = Substitute.For<IPoolManager>();

            var fakeGroup = Substitute.For<IGroup>();
            fakeGroup.MatchesComponents.Returns(new Type[0]);
            mockPoolManager.CreateObservableGroup(Arg.Is(fakeGroup)).Returns(mockObservableGroup);
            
            var mockSystem = Substitute.For<ITeardownSystem>();
            mockSystem.TargetGroup.Returns(fakeGroup);

            var systemHandler = new TeardownSystemHandler(mockPoolManager);
            systemHandler.SetupSystem(mockSystem);
            
            removeSubject.OnNext(fakeEntity1);
            
            mockSystem.Received(1).Teardown(Arg.Is(fakeEntity1));
            Assert.Equal(1, systemHandler._systemSubscriptions.Count);
            Assert.NotNull(systemHandler._systemSubscriptions[mockSystem]);
        }
    }
    
    public class ManualSystemHandlerTests
    {
        [Fact]
        public void should_correctly_handle_systems()
        {
            var mockPoolManager = Substitute.For<IPoolManager>();
            var teardownSystemHandler = new ManualSystemHandler(mockPoolManager);
            
            var fakeMatchingSystem = Substitute.For<IManualSystem>();
            var fakeNonMatchingSystem1 = Substitute.For<IReactToEntitySystem>();
            var fakeNonMatchingSystem2 = Substitute.For<ISystem>();
            
            Assert.True(teardownSystemHandler.CanHandleSystem(fakeMatchingSystem));
            Assert.False(teardownSystemHandler.CanHandleSystem(fakeNonMatchingSystem1));
            Assert.False(teardownSystemHandler.CanHandleSystem(fakeNonMatchingSystem2));
        }
        
        [Fact]
        public void should_start_system_when_added_to_handler()
        {
            var mockObservableGroup = Substitute.For<IObservableGroup>();
            var mockPoolManager = Substitute.For<IPoolManager>();

            mockPoolManager.CreateObservableGroup(Arg.Any<IGroup>()).Returns(mockObservableGroup);
            var mockSystem = Substitute.For<IManualSystem>();

            var systemHandler = new ManualSystemHandler(mockPoolManager);
            systemHandler.SetupSystem(mockSystem);
            
            mockSystem.Received(1).StartSystem(Arg.Is(mockObservableGroup));
        }
        
        [Fact]
        public void should_stop_system_when_added_to_handler()
        {
            var mockObservableGroup = Substitute.For<IObservableGroup>();
            var mockPoolManager = Substitute.For<IPoolManager>();

            mockPoolManager.CreateObservableGroup(Arg.Any<IGroup>()).Returns(mockObservableGroup);
            var mockSystem = Substitute.For<IManualSystem>();

            var systemHandler = new ManualSystemHandler(mockPoolManager);
            systemHandler.DestroySystem(mockSystem);
            
            mockSystem.Received(1).StopSystem(Arg.Is(mockObservableGroup));
        }
    }
}