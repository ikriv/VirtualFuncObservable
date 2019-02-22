using System;
using System.Collections.Generic;
using System.Reactive.Subjects;

namespace VirtualFuncObservable
{
    // Base class, provides virtual HandleItem() method called when new item is observed
    class BaseHandler : IDisposable
    {
        private readonly IDisposable _subscription;

        public BaseHandler(IObservable<int> items)
        {
            _subscription = items.Subscribe(HandleItem);
        }

        public void Dispose()

        {
            _subscription.Dispose();
        }

        // default handler for observed item prints the value
        protected virtual void HandleItem(int n)
        {
            Console.WriteLine(n);
        }
    }

    // Derived class handles observed items by adding them to a list
    class DerivedHandler : BaseHandler
    {
        private readonly List<int> _items; // not initialized here on purpose

        public DerivedHandler(IObservable<int> items)
            :
            base(items)
        {
            _items = new List<int>();
        }

        protected override void HandleItem(int n)
        {
            Console.WriteLine("Adding item: {0}", n);
            _items.Add(n); // this causes NullReferenceExceptino if called from the base's constructor
        }
    }

    class Program
    {
        static void Main()
        {
            using (var subject = new ReplaySubject<int>())
            {
                // Publish an item before constructing any handler objects; this will cause an exception in constructor
                // Comment this out to prevent the exception
                subject.OnNext(42); 

                using (new DerivedHandler(subject))
                {
                    // publish a new item to process
                    subject.OnNext(43);
                    Console.ReadLine();
                }
            }
        }
    }
}
