using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Numerics;

namespace GameEngineUI
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
       
        TimeSpan lastRender;
        bool lastVisible;
        Point oldMousePosition;

        public MainWindow()
        {
            this.InitializeComponent();
            this.host.Loaded += new RoutedEventHandler(this.Host_Loaded);
            this.host.SizeChanged += new SizeChangedEventHandler(this.Host_SizeChanged);
        }

        private static bool Init()
        {

            bool initSucceeded = NativeMethods.InvokeWithDllProtection(() => NativeMethods.Init()) >= 0;

            if (!initSucceeded)
            {
                MessageBox.Show("Failed to initialize.", "WPF D3D Interop", MessageBoxButton.OK, MessageBoxImage.Error);

                if (Application.Current != null)
                {
                    Application.Current.Shutdown();
                }
            }

            return initSucceeded;
        }

        private static void Cleanup()
        {
            NativeMethods.InvokeWithDllProtection(NativeMethods.Cleanup);
        }

        private static int Render(IntPtr resourcePointer, bool isNewSurface)
        {
            return NativeMethods.InvokeWithDllProtection(() => NativeMethods.Render(resourcePointer, isNewSurface));
        }


        #region Callbacks
        private void Host_Loaded(object sender, RoutedEventArgs e)
        {
            Init();
            this.InitializeRendering();

        }

        private void Host_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double dpiScale = 1.0; // default value for 96 dpi

            // determine DPI
            // (as of .NET 4.6.1, this returns the DPI of the primary monitor, if you have several different DPIs)
            var hwndTarget = PresentationSource.FromVisual(this).CompositionTarget as HwndTarget;
            if (hwndTarget != null)
            {
                dpiScale = hwndTarget.TransformToDevice.M11;
            }

            int surfWidth = (int)(host.ActualWidth < 0 ? 0 : Math.Ceiling(host.ActualWidth * dpiScale));
            int surfHeight = (int)(host.ActualHeight < 0 ? 0 : Math.Ceiling(host.ActualHeight * dpiScale));

            // Notify the D3D11Image of the pixel size desired for the DirectX rendering.
            // The D3DRendering component will determine the size of the new surface it is given, at that point.
            InteropImage.SetPixelSize(surfWidth, surfHeight);

            // Stop rendering if the D3DImage isn't visible - currently just if width or height is 0
            // TODO: more optimizations possible (scrolled off screen, etc...)
            bool isVisible = (surfWidth != 0 && surfHeight != 0);
            if (lastVisible != isVisible)
            {
                lastVisible = isVisible;
                if (lastVisible)
                {
                    CompositionTarget.Rendering += CompositionTarget_Rendering;
                }
                else
                {
                    CompositionTarget.Rendering -= CompositionTarget_Rendering;
                }
            }
        }
        #endregion Callbacks

        #region Helpers
        private void InitializeRendering()
        {
            InteropImage.WindowOwner = (new System.Windows.Interop.WindowInteropHelper(this)).Handle;
            InteropImage.OnRender = this.DoRender;

            // Start rendering now!
            InteropImage.RequestRender();
        }

        void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            RenderingEventArgs args = (RenderingEventArgs)e;

            // It's possible for Rendering to call back twice in the same frame 
            // so only render when we haven't already rendered in this frame.
            if (this.lastRender != args.RenderingTime)
            {
                InteropImage.RequestRender();
                this.lastRender = args.RenderingTime;
            }
        }

        private void UninitializeRendering()
        {
            Cleanup();

            CompositionTarget.Rendering -= this.CompositionTarget_Rendering;
        }
        #endregion Helpers

        private void DoRender(IntPtr surface, bool isNewSurface)
        {
            Render(surface, isNewSurface);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.UninitializeRendering();

            
        }

        private static class NativeMethods
        {
            /// <summary>
            /// Variable used to track whether the missing dependency dialog has been displayed,
            /// used to prevent multiple notifications of the same failure.
            /// </summary>
            private static bool errorHasDisplayed;

            [DllImport("GameEngineDLL.dll", CallingConvention = CallingConvention.Cdecl)]
            public static extern int Init();

            [DllImport("GameEngineDLL.dll", CallingConvention = CallingConvention.Cdecl)]
            public static extern void Cleanup();

            [DllImport("GameEngineDLL.dll", CallingConvention = CallingConvention.Cdecl)]
            public static extern int Render(IntPtr resourcePointer, bool isNewSurface);

            [DllImport("GameEngineDLL.dll", CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetObjectPosition(string ObjectName, Vector3 Position);

            [DllImport("GameEngineDLL.dll", CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetObjectRotation(string ObjectName, Vector3 Rotaion);

            [DllImport("GameEngineDLL.dll", CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetObjectScale(string ObjectName, Vector3 Scale);

            [DllImport("GameEngineDLL.dll", CallingConvention = CallingConvention.Cdecl)]
            public static extern Vector3 GetObjectPosition(string ObjectName);

            [DllImport("GameEngineDLL.dll", CallingConvention = CallingConvention.Cdecl)]
            public static extern Vector3 GetObjectRotation(string ObjectName);

            [DllImport("GameEngineDLL.dll", CallingConvention = CallingConvention.Cdecl)]
            public static extern Vector3 GetObjectScale(string ObjectName);

            /// <summary>
            /// Method used to invoke an Action that will catch DllNotFoundExceptions and display a warning dialog.
            /// </summary>
            /// <param name="action">The Action to invoke.</param>
            public static void InvokeWithDllProtection(Action action)
            {
                InvokeWithDllProtection(
                    () =>
                    {
                        action.Invoke();
                        return 0;
                    });
            }

            /// <summary>
            /// Method used to invoke A Func that will catch DllNotFoundExceptions and display a warning dialog.
            /// </summary>
            /// <param name="func">The Func to invoke.</param>
            /// <returns>The return value of func, or default(T) if a DllNotFoundException was caught.</returns>
            /// <typeparam name="T">The return type of the func.</typeparam>
            public static T InvokeWithDllProtection<T>(Func<T> func)
            {
                try
                {
                    return func.Invoke();
                }
                catch (DllNotFoundException e)
                {
                    if (!errorHasDisplayed)
                    {
                        MessageBox.Show("This sample requires:\nManual build of the D3DVisualization project, which requires installation of Windows 10 SDK or DirectX SDK.\n" +
                                        "Installation of the DirectX runtime on non-build machines.\n\n" +
                                        "Detailed exception message: " + e.Message, "WPF D3D11 Interop",
                                        MessageBoxButton.OK, MessageBoxImage.Error);
                        errorHasDisplayed = true;

                        if (Application.Current != null)
                        {
                            Application.Current.Shutdown();
                        }
                    }
                }

                return default(T);
            }
        }

        private void Host_MouseMove(object sender, MouseEventArgs e)
        {
            Point mousePosition = e.GetPosition(host);
            Vector3 cameraPosition = NativeMethods.InvokeWithDllProtection(() => NativeMethods.GetObjectPosition("Camera"));
            Vector3 cameraRotation = NativeMethods.InvokeWithDllProtection(() => NativeMethods.GetObjectRotation("Camera"));

            if(e.RightButton == MouseButtonState.Pressed)
            {
                cameraRotation.Y += (float)(mousePosition.X - oldMousePosition.X) * 0.003f;
                cameraRotation.X += (float)(mousePosition.Y - oldMousePosition.Y) * 0.003f;
                NativeMethods.InvokeWithDllProtection(() => NativeMethods.SetObjectRotation("Camera", cameraRotation));

            }

            if(e.LeftButton == MouseButtonState.Pressed)
            {
                float dx = (float)(mousePosition.X - oldMousePosition.X) * 0.01f;
                float dy = (float)(mousePosition.Y - oldMousePosition.Y) * 0.01f;

                cameraPosition.X -= (float)Math.Cos(cameraRotation.Y) * dx
                    - (float)Math.Sin(cameraRotation.Y) * (float)Math.Sin(cameraRotation.X) * dy;
                cameraPosition.Z += (float)Math.Sin(cameraRotation.Y) * dx
                    + (float)Math.Cos(cameraRotation.Y) * (float)Math.Sin(cameraRotation.X) * dy;
                cameraPosition.Y += (float)Math.Cos(cameraRotation.X) * dy;

                NativeMethods.InvokeWithDllProtection(() => NativeMethods.SetObjectPosition("Camera", cameraPosition));

            }

            oldMousePosition = mousePosition;
        }
    }
}
