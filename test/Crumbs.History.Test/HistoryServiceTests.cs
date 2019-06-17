using Crumbs.Core.Event;
using FakeItEasy;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Crumbs.History.Test
{
    public class HistoryServiceTests
    {
        public static Guid AggregateId = Guid.Parse("4111d3d8-1310-d110-d1d3-d43d52bd11d5");
        public static Guid UserId = Guid.Parse("1c11c2d8-d110-1470-d1d3-d13121bd11d5");
        public static string User = "Darth Vader";

        public static DateTimeOffset Event1Timestamp = DateTimeOffset.FromUnixTimeSeconds(10000000);
        public static DateTimeOffset Event2Timestamp = DateTimeOffset.FromUnixTimeSeconds(20000000);

        public class When_getting_history_entries
        {
            private IEventStore _eventStore;
            private IUserDescriptor _userDescriptor;

            public When_getting_history_entries()
            {
                _eventStore = A.Fake<IEventStore>();
                _userDescriptor = A.Fake<IUserDescriptor>();

                A.CallTo(() => _eventStore.Get(AggregateId))
                    .Returns(Task.FromResult(CreateTestEvents()));
                A.CallTo(() => _userDescriptor.GetUserDescription(UserId))
                    .Returns(User);
            }

            [Fact]
            public void Should_return_history_entries_for_each_event()
            {
                new HistoryService(_eventStore, _userDescriptor).Get(AggregateId).Result
                    .Should().HaveCount(2);
            }

            [Fact]
            public void Should_set_triggered_by_user()
            {
                new HistoryService(_eventStore, _userDescriptor).Get(AggregateId).Result
                    .First().TriggeredByUser.Should().Be(User);
            }

            [Fact]
            public void Should_set_unknow_user_if_user_does_not_exist()
            {
                var userDescriptor = A.Fake<IUserDescriptor>();
                A.CallTo(() => userDescriptor.GetUserDescription(A<Guid>.Ignored))
                    .Returns(Task.FromResult<string>(null));

                new HistoryService(_eventStore, userDescriptor).Get(AggregateId).Result.ToList()
                    .ForEach(x => x.TriggeredByUser.Should().Be("Unknown"));
            }

            [Fact]
            public void Should_set_timestamp()
            {
                var historyEntries = new HistoryService(_eventStore, _userDescriptor).Get(AggregateId).Result;

                historyEntries.First().Timestamp.Should().Be(Event1Timestamp);
                historyEntries.Last().Timestamp.Should().Be(Event2Timestamp);
            }

            [Fact]
            public void Should_split_event_name_and_remove_postfix()
            {
                var historyEntries = new HistoryService(_eventStore, _userDescriptor).Get(AggregateId).Result;

                historyEntries.First().Name.Should().Be("Test Operation Completed One");
                historyEntries.Last().Name.Should().Be("Test Operation Completed Two");
            }

            [Fact]
            public void Should_only_include_details_for_properties_with_history_entry_attributes()
            {
                var historyEntries = new HistoryService(_eventStore, _userDescriptor).Get(AggregateId).Result;

                historyEntries.Any(x => x.Details.Contains(nameof(TestOperationCompletedEvent.DoNotIncludeInDetails)))
                    .Should().Be(false);

                historyEntries.First().Details.Contains(nameof(TestOperationCompletedOneEvent.IncludeInDetailsWithLabel))
                    .Should().Be(true);
                historyEntries.Last().Details.Contains(nameof(TestOperationCompletedTwoEvent.IncludeInDetailsWithPropertyName))
                    .Should().Be(true);
            }

            [Fact]
            public void Should_use_attribute_label_in_details_if_set()
            {
                var historyEntries = new HistoryService(_eventStore, _userDescriptor).Get(AggregateId).Result;

                historyEntries.First().Details
                    .Should().Be($"MyLabel: {nameof(TestOperationCompletedOneEvent.IncludeInDetailsWithLabel)}");
            }

            [Fact]
            public void Should_default_to_property_name_in_details_if_attribute_label_is_not_set()
            {
                var historyEntries = new HistoryService(_eventStore, _userDescriptor).Get(AggregateId).Result;

                historyEntries.Last().Details
                    .Should().Be($"{nameof(TestOperationCompletedTwoEvent.IncludeInDetailsWithPropertyName)}:" +
                    $" {nameof(TestOperationCompletedTwoEvent.IncludeInDetailsWithPropertyName)}");
            }
        }

        public class TestOperationCompletedEvent : DomainEvent
        {
            public string DoNotIncludeInDetails { get; set; }
        }

        public class TestOperationCompletedOneEvent : TestOperationCompletedEvent
        {
            [HistoryEntry("MyLabel")]
            public string IncludeInDetailsWithLabel { get; set; }
        }

        public class TestOperationCompletedTwoEvent : TestOperationCompletedEvent
        {
            [HistoryEntry]
            public string IncludeInDetailsWithPropertyName { get; set; }
        }
        public static IReadOnlyCollection<IDomainEvent> CreateTestEvents()
        {
            return new List<IDomainEvent>
            {
                new TestOperationCompletedOneEvent
                {
                    AppliedByUserId = UserId,
                    Timestamp = Event1Timestamp,
                    Version = 1,
                    DoNotIncludeInDetails = nameof(TestOperationCompletedEvent.DoNotIncludeInDetails),
                    IncludeInDetailsWithLabel = nameof(TestOperationCompletedOneEvent.IncludeInDetailsWithLabel),
                },
                new TestOperationCompletedTwoEvent
                {
                    AppliedByUserId = UserId,
                    Timestamp = Event2Timestamp,
                    Version = 2,
                    DoNotIncludeInDetails = nameof(TestOperationCompletedEvent.DoNotIncludeInDetails),
                    IncludeInDetailsWithPropertyName = nameof(TestOperationCompletedTwoEvent.IncludeInDetailsWithPropertyName),
                }
            };
        }
    }
}
