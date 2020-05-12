using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;

namespace Crumbs.EventualConsistency.Extensions
{
    public static class ObservableExtensions
    {
        public static IObservable<T> HotConcat<T>(this IObservable<T> first, IObservable<T> second)
        {
            return Observable.Create<T>(observer =>
            {
                var queue = new Queue<Notification<T>>();
                var secondSubscription = SubscribeAndEnque(second, queue);
                var secondReplay = Observable.Create<T>(secondObserver =>
                {
                    while (true)
                    {
                        Notification<T> item;

                        lock (queue)
                        {
                            if (queue.Count > 0)
                            {
                                item = queue.Dequeue();
                            }
                            else
                            {
                                secondObserver.OnCompleted();
                                secondSubscription.Dispose();
                                queue = null;
                                break;
                            }
                        }

                        if (item != null)
                            item.Accept(secondObserver);
                    }

                    return secondSubscription;
                });

                return first.Concat(secondReplay).Concat(second).Subscribe(observer);
            });
        }

        private static IDisposable SubscribeAndEnque<T>(IObservable<T> second, Queue<Notification<T>> queue)
        {
            return second.Materialize()
                         .Subscribe(item =>
                                    {
                                        if (queue == null)
                                            return;

                                        lock (queue)
                                        {
                                            queue.Enqueue(item);
                                        }
                                    });
        }
    }
}