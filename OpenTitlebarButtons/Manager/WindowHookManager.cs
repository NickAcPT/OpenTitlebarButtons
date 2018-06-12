using System;
using System.Diagnostics;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;

namespace OpenTitlebarButtons.Manager
{
    //Class created by Iain Ballard (https://github.com/i-e-b)
    //Taken from WindowsJedi. All credits go to Iain Ballard.
    /// <summary>
    ///     Provides a set of events that hook into the Win32 window manager
    /// </summary>
    public class WindowHookManager : CriticalFinalizerObject, IDisposable
    {
        public delegate void WinEventDelegate(IntPtr hWinEventHook,
            uint eventType, IntPtr hwnd, int idObject,
            int idChild, uint dwEventThread, uint dwmsEventTime);

        private readonly int _ownProcessId;

        private readonly IntPtr _windowsEventsHook;
        private GCHandle _focusedChangedEventPin, _hookDelegatePin;

        /// <summary>
        ///     Create a window hook manager and start listening for events
        /// </summary>
        public WindowHookManager()
        {
            WinEventDelegate hookDelegate = WinEventProc;

            _focusedChangedEventPin = GCHandle.Alloc(WindowFocusChanged);
            _hookDelegatePin = GCHandle.Alloc(hookDelegate);

            _ownProcessId = Process.GetCurrentProcess().Id;

            _windowsEventsHook = SetWinEventHook(
                EventMin, EventMax, // give me all the events. This may cause slow-down...
                IntPtr.Zero,
                hookDelegate,
                0, 0, // all processes and threads
                WineventOutofcontext | WineventSkipownprocess); // only other processes
        }

        /// <summary>
        ///     Dispose of hooks and GC pins.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public event EventHandler<WindowHandleEventArgs> WindowFocusChanged;
        public event EventHandler<WindowHandleEventArgs> WindowCreated;
        public event EventHandler<WindowHandleEventArgs> WindowDestroyed;
        public event EventHandler<WindowHandleEventArgs> WindowChanged;

        [DllImport("user32.dll")]
        public static extern bool UnhookWinEvent(IntPtr hWinEventHook);

        [DllImport("user32.dll")]
        public static extern IntPtr SetWinEventHook(uint eventMin,
            uint eventMax, IntPtr hmodWinEventProc,
            WinEventDelegate lpfnWinEventProc, uint idProcess,
            uint idThread, uint dwFlags);

        ~WindowHookManager()
        {
            UnhookWinEvent(_windowsEventsHook);
            if (_focusedChangedEventPin.IsAllocated) _focusedChangedEventPin.Free();
            if (_hookDelegatePin.IsAllocated) _hookDelegatePin.Free();
        }

        protected void Dispose(bool disposing)
        {
            if (!disposing) return;

            UnhookWinEvent(_windowsEventsHook);
            if (_focusedChangedEventPin.IsAllocated) _focusedChangedEventPin.Free();
            if (_hookDelegatePin.IsAllocated) _hookDelegatePin.Free();
        }

        /// <summary>
        ///     Bridge from Win32 callback to .Net event
        /// </summary>
        private void WinEventProc(IntPtr hWinEventHook, uint eventType,
            IntPtr hwnd, int idObject, int idChild,
            uint dwEventThread, uint dwmsEventTime)
        {
            if (OwnProcessThread(hwnd)) return;

            switch (eventType)
            {
                case EventSystemForeground:
                    InvokeWindowFocusChanged(hwnd);
                    return;

                case EventObjectCreate:
                    DispatchObjectCreation(hwnd, idObject, idChild);
                    return;

                case EventObjectDestroy:
                    DispatchObjectDestruction(hwnd, idObject, idChild);
                    return;

                case EventObjectLocationchange:
                    DispatchObjectLocationChange(hwnd, idObject, idChild);
                    return;

                default: // an event we don't have a C# handler for
                    return;
            }
        }

        private bool OwnProcessThread(IntPtr hwnd)
        {
            var procid = WindowProcess(hwnd);
            return procid == _ownProcessId || procid == 0;
        }


        [DllImport("user32.dll")]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        public static uint WindowProcess(IntPtr hWnd)
        {
            GetWindowThreadProcessId(hWnd, out var processid);
            return processid;
        }

        
        private void DispatchObjectLocationChange(IntPtr hwnd, int idObject, int idChild)
        {
            // Only handle created windows for now
            if (idObject != ObjidWindow) return;
            if (idChild != 0) return;

            InvokeWindowChanged(hwnd);
        }


        private void DispatchObjectDestruction(IntPtr hwnd, int idObject, int idChild)
        {
            // Only handle created windows for now
            if (idObject != ObjidWindow) return;
            if (idChild != 0) return;

            InvokeWindowCreated(hwnd);
        }

        private void DispatchObjectCreation(IntPtr hwnd, int idObject, int idChild)
        {
            // Only handle created windows for now
            if (idObject != ObjidWindow) return;
            if (idChild != 0) return;
            if (OwnProcessThread(hwnd)) return;

            InvokeWindowDestroyed(hwnd);
        }

        public class WindowHandleEventArgs : EventArgs
        {
            public WindowHandleEventArgs(IntPtr windowHandle)
            {
                WindowHandle = windowHandle;
            }

            public IntPtr WindowHandle { get; set; }
        }

        #region Event Invocation

        /// <summary>
        ///     Trigger the WindowFocusChanged event for a given window handle
        /// </summary>
        public void InvokeWindowFocusChanged(IntPtr windowHandle)
        {
            var handler = WindowFocusChanged;
            handler?.Invoke(this, new WindowHandleEventArgs(windowHandle));
        }

        /// <summary>
        ///     Trigger the WindowFocusChanged event for a given window handle
        /// </summary>
        public void InvokeWindowCreated(IntPtr windowHandle)
        {
            var handler = WindowCreated;
            handler?.Invoke(this, new WindowHandleEventArgs(windowHandle));
        }

        /// <summary>
        ///     Trigger the WindowFocusChanged event for a given window handle
        /// </summary>
        public void InvokeWindowDestroyed(IntPtr windowHandle)
        {
            var handler = WindowDestroyed;
            handler?.Invoke(this, new WindowHandleEventArgs(windowHandle));
        }

        protected virtual void InvokeWindowChanged(IntPtr windowHandle)
        {
            WindowChanged?.Invoke(this, new WindowHandleEventArgs(windowHandle));
        }

        #endregion

        #region Windows Events Flag

        public const uint WineventOutofcontext = 0;

        public const uint WineventSkipownprocess = 2;

        public const uint EventSystemForeground = 3;

        public const uint EventObjectCreate = 0x8000;

        public const uint EventObjectDestroy = 0x8001;

        public const uint EventObjectLocationchange = 0x800B;

        public const uint EventMin = 0x00000001;

        public const uint EventMax = 0x7FFFFFFF;

        public const uint ObjidWindow = 0x00000000;

        #endregion
    }
}