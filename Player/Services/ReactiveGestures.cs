using System; 
using System.Reactive.Linq; 
using System.Windows;
using System.Windows.Input;

namespace Player.Services
{
    public static class ReactiveGestures
    {
        public static void DoubleClick(this FrameworkElement element, ICommand action)
        { 
            var mouseDown = Observable.FromEventPattern<MouseButtonEventArgs>(element, "MouseLeftButtonDown");
            mouseDown.BufferWithTimeAndCount(TimeSpan.FromMilliseconds(300.0), 2).Subscribe(a =>
            {
                if (action.CanExecute(null))
                    action.Execute(null);
            });
        }

        private static IObservable<T> BufferWithTimeAndCount<T>(this IObservable<T> src, TimeSpan t, int count)
        {
            return src.Timestamp()
                            .Buffer(count)
                            .Where(s => s[s.Count - 1].Timestamp - s[0].Timestamp <= t)
                            .Select(arr => arr[arr.Count - 1].Value);
        }

        private static IObservable<Vector> RightButtonDrag(this FrameworkElement element)
        {
            var mousedown = from evt in Observable.FromEventPattern<MouseButtonEventArgs>(element, "MouseRightButtonDown")
                            select evt.EventArgs.GetPosition(element);

            var mouseup = from evt in Observable.FromEventPattern<MouseButtonEventArgs>(element, "MouseRightButtonUp")
                          select evt.EventArgs.GetPosition(element);

            var mouseleave = from evt in Observable.FromEventPattern<MouseEventArgs>(element, "MouseLeave")
                             select evt.EventArgs.GetPosition(element);

            var mousemove = from evt in Observable.FromEventPattern<MouseEventArgs>(element, "MouseMove")
                            select evt.EventArgs.GetPosition(element);

            var moveResult = from start in mousedown
                             from end in mousemove
                                 .StartWith(start)
                                 .TakeUntil(mouseleave)
                                 .TakeUntil(mouseup)
                                 .TakeLast(1)
                             select end - start;

            return moveResult;
        }

        public static void RightButtonDragLeft(this FrameworkElement element, ICommand action)
        {
            element.RightButtonDrag().AsObservable().Subscribe(value =>
            {
                if (value.X < 0 && Math.Abs(value.X) > Math.Abs(value.Y))
                    if (action.CanExecute(null))
                        action.Execute(null);
            });
        }

        public static void RightButtonDragRight(this FrameworkElement element, ICommand action)
        {
            element.RightButtonDrag().AsObservable().Subscribe(value =>
            {
                if (value.X > 0 && Math.Abs(value.X) > Math.Abs(value.Y))
                    if (action.CanExecute(null))
                        action.Execute(null);
            });
        }

        public static void RightButtonDragDown(this FrameworkElement element, ICommand action)
        {
            element.RightButtonDrag().AsObservable().Subscribe(value =>
            {
                if (value.Y > 0 && Math.Abs(value.X) <= Math.Abs(value.Y))
                    if (action.CanExecute(null))
                        action.Execute(null);
            });
        }

        private static IObservable<int> Scroll(this FrameworkElement element)
        {
            var scroll = from evt in Observable.FromEventPattern<MouseWheelEventArgs>(element, "MouseWheel")
                         select evt.EventArgs.Delta;
            return scroll;
        }

        public static void ScrollUp(this FrameworkElement element, ICommand action)
        {
            element.Scroll().AsObservable().Subscribe(v =>
            {
                if (v > 0) 
                    if (action.CanExecute(null))
                        action.Execute(null);
            });
        }

        public static void ScrollDown(this FrameworkElement element, ICommand action)
        {
            element.Scroll().AsObservable().Subscribe(v =>
            {
                if (v < 0) 
                    if (action.CanExecute(null))
                        action.Execute(null);
            });
        } 
    } 
}
