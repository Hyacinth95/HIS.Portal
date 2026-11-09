#pragma warning disable 108
// ReSharper disable RedundantUsingDirective
// ReSharper disable JoinDeclarationAndInitializer
// ReSharper disable ArrangeTypeMemberModifiers
// ReSharper disable UnusedType.Local
// ReSharper disable InconsistentNaming
// ReSharper disable RedundantNameQualifier
// ReSharper disable RedundantCast
// ReSharper disable IdentifierTypo
// ReSharper disable PartialTypeWithSinglePart
// ReSharper disable RedundantUnsafeContext
// ReSharper disable RedundantBaseQualifier
// ReSharper disable EmptyStatement
// ReSharper disable RedundantAttributeParentheses
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable FieldCanBeMadeReadOnly.Global
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using MicroCom.Runtime;

namespace AvaloniaFluentUI.Controls.Interop.WinRT
{
    internal enum TrustLevel
    {
        BaseTrust,
        PartialTrust,
        FullTrust
    }

    internal enum HandPreference
    {
        LeftHanded = 0,
        RightHanded = 1
    }

    internal enum UIColorType
    {
        Background = 0,
        Foreground = 1,
        AccentDark3 = 2,
        AccentDark2 = 3,
        AccentDark1 = 4,
        Accent = 5,
        AccentLight1 = 6,
        AccentLight2 = 7,
        AccentLight3 = 8,
        Complement = 9
    }

    internal enum UIElementType
    {
        ActiveCaption = 0,
        Background = 1,
        ButtonFace = 2,
        ButtonText = 3,
        CaptionText = 4,
        GrayText = 5,
        Highlight = 6,
        HighlightText = 7,
        Hotlight = 8,
        InactiveCaption = 9,
        InactiveCaptionText = 10,
        Window = 11,
        WindowText = 12,
        AccentColor = 1000,
        TextHigh = 1001,
        TextMedium = 1002,
        TextLow = 1003,
        TextContrastWithHigh = 1004,
        NonTextHigh = 1005,
        NonTextMediumHigh = 1006,
        NonTextMedium = 1007,
        NonTextMediumLow = 1008,
        NonTextLow = 1009,
        PageBackground = 1010,
        PopupBackground = 1011,
        OverlayOutsidePopup = 1012
    }

    internal unsafe partial interface IInspectable : global::MicroCom.Runtime.IUnknown
    {
        void GetIids(ulong* iidCount, Guid** iids);
        IntPtr RuntimeClassName { get; }

        TrustLevel TrustLevel { get; }
    }

    internal unsafe partial interface IActivationFactory : IInspectable
    {
        IntPtr ActivateInstance();
    }

    internal unsafe partial interface IUISettings : IInspectable
    {
        HandPreference HandPreference { get; }

        AvaloniaFluentUI.Controls.Interop.WinRT.WinRTSize CursorSize { get; }

        AvaloniaFluentUI.Controls.Interop.WinRT.WinRTSize ScrollBarSize { get; }

        AvaloniaFluentUI.Controls.Interop.WinRT.WinRTSize ScrollBarArrowSize { get; }

        AvaloniaFluentUI.Controls.Interop.WinRT.WinRTSize ScrollBarThumbBoxSize { get; }

        uint MessageDuration { get; }

        int AnimationsEnabled { get; }

        int CaretBrowsingEnabled { get; }

        uint CaretBlinkRate { get; }

        uint CaretWidth { get; }

        uint DoubleClickTime { get; }

        uint MouseHoverTime { get; }

        AvaloniaFluentUI.Controls.Interop.WinRT.WinRTColor UIElementColor(UIElementType desiredElement);
    }

    internal unsafe partial interface IUISettings2 : IInspectable
    {
        double TextScaleFactor { get; }
    }

    internal unsafe partial interface IUISettings3 : IInspectable
    {
        AvaloniaFluentUI.Controls.Interop.WinRT.WinRTColor GetColorValue(UIColorType desiredColor);
    }

    internal unsafe partial interface IUISettings4 : IInspectable
    {
        int AdvancedEffectsEnabled { get; }
    }

    internal unsafe partial interface IAccessibilitySettings : IInspectable
    {
        int HighContrast { get; }

        IntPtr HighContrastScheme { get; }
    }

    internal unsafe partial interface ITaskbarList : global::MicroCom.Runtime.IUnknown
    {
        void HrInit();
        void AddTab(IntPtr hwnd);
        void DeleteTab(IntPtr hwnd);
        void ActivateTab(IntPtr hwnd);
        void SetActiveAlt(IntPtr hwnd);
    }

    internal unsafe partial interface ITaskbarList2 : ITaskbarList
    {
        void MarkFullscreenWindow(IntPtr hwnd, int fFullscreen);
    }

    internal unsafe partial interface ITaskbarList3 : ITaskbarList2
    {
        void SetProgressValue(IntPtr hwnd, ulong ullCompleted, ulong ullTotal);
        void SetProgressState(IntPtr hwnd, int tbpFlags);
        void RegisterTab(IntPtr hwndTab, IntPtr hwndMDI);
        void UnregisterTab(IntPtr hwndTab);
        void SetTabOrder(IntPtr hwndTab, IntPtr hwndInsertBefore);
        void SetTabActive(IntPtr hwndTab, IntPtr hwndMDI, int dwReserved);
        void ThumbBarAddButtons(IntPtr hwnd, uint cButtons, int pButton);
        void ThumbBarUpdateButtons(IntPtr hwnd, uint cButtons, int pButton);
        void ThumbBarSetImageList(IntPtr hwnd, IntPtr himl);
        void SetOverlayIcon(IntPtr hwnd, void* hIcon, ushort* pszDescription);
        void SetThumbnailTooltip(IntPtr hwnd, ushort* pszTip);
        void SetThumbnailClip(IntPtr hwnd, AvaloniaFluentUI.Controls.Interop.Win32.RECT* prcClip);
    }
}

namespace AvaloniaFluentUI.Controls.Interop.WinRT.Impl
{
    internal unsafe partial class __MicroComIInspectableProxy : global::MicroCom.Runtime.MicroComProxyBase, IInspectable
    {
        public void GetIids(ulong* iidCount, Guid** iids)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, void*, int>)(*PPV)[base.VTableSize + 0])(PPV, iidCount, iids);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("GetIids failed", __result);
        }

        public IntPtr RuntimeClassName
        {
            get
            {
                int __result;
                IntPtr className = default;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 1])(PPV, &className);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetRuntimeClassName failed", __result);
                return className;
            }
        }

        public TrustLevel TrustLevel
        {
            get
            {
                int __result;
                TrustLevel trustLevel = default;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 2])(PPV, &trustLevel);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetTrustLevel failed", __result);
                return trustLevel;
            }
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit()
        {
            global::MicroCom.Runtime.MicroComRuntime.Register(typeof(IInspectable), new Guid("AF86E2E0-B12D-4c6a-9C5A-D7AA65101E90"), (p, owns) => new __MicroComIInspectableProxy(p, owns));
        }

        protected __MicroComIInspectableProxy(IntPtr nativePointer, bool ownsHandle) : base(nativePointer, ownsHandle)
        {
        }

        protected override int VTableSize => base.VTableSize + 3;
    }

    unsafe class __MicroComIInspectableVTable : global::MicroCom.Runtime.MicroComVtblBase
    {
        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetIidsDelegate(void* @this, ulong* iidCount, Guid** iids);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetIids(void* @this, ulong* iidCount, Guid** iids)
        {
            IInspectable __target = null;
            try
            {
                {
                    __target = (IInspectable)global::MicroCom.Runtime.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.GetIids(iidCount, iids);
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::MicroCom.Runtime.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetRuntimeClassNameDelegate(void* @this, IntPtr* className);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetRuntimeClassName(void* @this, IntPtr* className)
        {
            IInspectable __target = null;
            try
            {
                {
                    __target = (IInspectable)global::MicroCom.Runtime.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.RuntimeClassName;
                        *className = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::MicroCom.Runtime.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetTrustLevelDelegate(void* @this, TrustLevel* trustLevel);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetTrustLevel(void* @this, TrustLevel* trustLevel)
        {
            IInspectable __target = null;
            try
            {
                {
                    __target = (IInspectable)global::MicroCom.Runtime.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.TrustLevel;
                        *trustLevel = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::MicroCom.Runtime.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        protected __MicroComIInspectableVTable()
        {
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, ulong*, Guid**, int>)&GetIids); 
#else
            base.AddMethod((GetIidsDelegate)GetIids); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, IntPtr*, int>)&GetRuntimeClassName); 
#else
            base.AddMethod((GetRuntimeClassNameDelegate)GetRuntimeClassName); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, TrustLevel*, int>)&GetTrustLevel); 
#else
            base.AddMethod((GetTrustLevelDelegate)GetTrustLevel); 
#endif
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit() => global::MicroCom.Runtime.MicroComRuntime.RegisterVTable(typeof(IInspectable), new __MicroComIInspectableVTable().CreateVTable());
    }

    internal unsafe partial class __MicroComIActivationFactoryProxy : __MicroComIInspectableProxy, IActivationFactory
    {
        public IntPtr ActivateInstance()
        {
            int __result;
            IntPtr instance = default;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 0])(PPV, &instance);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("ActivateInstance failed", __result);
            return instance;
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit()
        {
            global::MicroCom.Runtime.MicroComRuntime.Register(typeof(IActivationFactory), new Guid("00000035-0000-0000-C000-000000000046"), (p, owns) => new __MicroComIActivationFactoryProxy(p, owns));
        }

        protected __MicroComIActivationFactoryProxy(IntPtr nativePointer, bool ownsHandle) : base(nativePointer, ownsHandle)
        {
        }

        protected override int VTableSize => base.VTableSize + 1;
    }

    unsafe class __MicroComIActivationFactoryVTable : __MicroComIInspectableVTable
    {
        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int ActivateInstanceDelegate(void* @this, IntPtr* instance);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int ActivateInstance(void* @this, IntPtr* instance)
        {
            IActivationFactory __target = null;
            try
            {
                {
                    __target = (IActivationFactory)global::MicroCom.Runtime.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.ActivateInstance();
                        *instance = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::MicroCom.Runtime.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        protected __MicroComIActivationFactoryVTable()
        {
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, IntPtr*, int>)&ActivateInstance); 
#else
            base.AddMethod((ActivateInstanceDelegate)ActivateInstance); 
#endif
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit() => global::MicroCom.Runtime.MicroComRuntime.RegisterVTable(typeof(IActivationFactory), new __MicroComIActivationFactoryVTable().CreateVTable());
    }

    internal unsafe partial class __MicroComIUISettingsProxy : __MicroComIInspectableProxy, IUISettings
    {
        public HandPreference HandPreference
        {
            get
            {
                int __result;
                HandPreference value = default;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 0])(PPV, &value);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetHandPreference failed", __result);
                return value;
            }
        }

        public AvaloniaFluentUI.Controls.Interop.WinRT.WinRTSize CursorSize
        {
            get
            {
                int __result;
                AvaloniaFluentUI.Controls.Interop.WinRT.WinRTSize value = default;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 1])(PPV, &value);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetCursorSize failed", __result);
                return value;
            }
        }

        public AvaloniaFluentUI.Controls.Interop.WinRT.WinRTSize ScrollBarSize
        {
            get
            {
                int __result;
                AvaloniaFluentUI.Controls.Interop.WinRT.WinRTSize value = default;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 2])(PPV, &value);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetScrollBarSize failed", __result);
                return value;
            }
        }

        public AvaloniaFluentUI.Controls.Interop.WinRT.WinRTSize ScrollBarArrowSize
        {
            get
            {
                int __result;
                AvaloniaFluentUI.Controls.Interop.WinRT.WinRTSize value = default;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 3])(PPV, &value);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetScrollBarArrowSize failed", __result);
                return value;
            }
        }

        public AvaloniaFluentUI.Controls.Interop.WinRT.WinRTSize ScrollBarThumbBoxSize
        {
            get
            {
                int __result;
                AvaloniaFluentUI.Controls.Interop.WinRT.WinRTSize value = default;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 4])(PPV, &value);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetScrollBarThumbBoxSize failed", __result);
                return value;
            }
        }

        public uint MessageDuration
        {
            get
            {
                int __result;
                uint value = default;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 5])(PPV, &value);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetMessageDuration failed", __result);
                return value;
            }
        }

        public int AnimationsEnabled
        {
            get
            {
                int __result;
                int value = default;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 6])(PPV, &value);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetAnimationsEnabled failed", __result);
                return value;
            }
        }

        public int CaretBrowsingEnabled
        {
            get
            {
                int __result;
                int value = default;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 7])(PPV, &value);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetCaretBrowsingEnabled failed", __result);
                return value;
            }
        }

        public uint CaretBlinkRate
        {
            get
            {
                int __result;
                uint value = default;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 8])(PPV, &value);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetCaretBlinkRate failed", __result);
                return value;
            }
        }

        public uint CaretWidth
        {
            get
            {
                int __result;
                uint value = default;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 9])(PPV, &value);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetCaretWidth failed", __result);
                return value;
            }
        }

        public uint DoubleClickTime
        {
            get
            {
                int __result;
                uint value = default;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 10])(PPV, &value);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetDoubleClickTime failed", __result);
                return value;
            }
        }

        public uint MouseHoverTime
        {
            get
            {
                int __result;
                uint value = default;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 11])(PPV, &value);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetMouseHoverTime failed", __result);
                return value;
            }
        }

        public AvaloniaFluentUI.Controls.Interop.WinRT.WinRTColor UIElementColor(UIElementType desiredElement)
        {
            int __result;
            AvaloniaFluentUI.Controls.Interop.WinRT.WinRTColor value = default;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, UIElementType, void*, int>)(*PPV)[base.VTableSize + 12])(PPV, desiredElement, &value);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("UIElementColor failed", __result);
            return value;
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit()
        {
            global::MicroCom.Runtime.MicroComRuntime.Register(typeof(IUISettings), new Guid("85361600-1C63-4627-BCB1-3A89E0BC9C55"), (p, owns) => new __MicroComIUISettingsProxy(p, owns));
        }

        protected __MicroComIUISettingsProxy(IntPtr nativePointer, bool ownsHandle) : base(nativePointer, ownsHandle)
        {
        }

        protected override int VTableSize => base.VTableSize + 13;
    }

    unsafe class __MicroComIUISettingsVTable : __MicroComIInspectableVTable
    {
        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetHandPreferenceDelegate(void* @this, HandPreference* value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetHandPreference(void* @this, HandPreference* value)
        {
            IUISettings __target = null;
            try
            {
                {
                    __target = (IUISettings)global::MicroCom.Runtime.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.HandPreference;
                        *value = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::MicroCom.Runtime.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetCursorSizeDelegate(void* @this, AvaloniaFluentUI.Controls.Interop.WinRT.WinRTSize* value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetCursorSize(void* @this, AvaloniaFluentUI.Controls.Interop.WinRT.WinRTSize* value)
        {
            IUISettings __target = null;
            try
            {
                {
                    __target = (IUISettings)global::MicroCom.Runtime.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.CursorSize;
                        *value = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::MicroCom.Runtime.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetScrollBarSizeDelegate(void* @this, AvaloniaFluentUI.Controls.Interop.WinRT.WinRTSize* value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetScrollBarSize(void* @this, AvaloniaFluentUI.Controls.Interop.WinRT.WinRTSize* value)
        {
            IUISettings __target = null;
            try
            {
                {
                    __target = (IUISettings)global::MicroCom.Runtime.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.ScrollBarSize;
                        *value = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::MicroCom.Runtime.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetScrollBarArrowSizeDelegate(void* @this, AvaloniaFluentUI.Controls.Interop.WinRT.WinRTSize* value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetScrollBarArrowSize(void* @this, AvaloniaFluentUI.Controls.Interop.WinRT.WinRTSize* value)
        {
            IUISettings __target = null;
            try
            {
                {
                    __target = (IUISettings)global::MicroCom.Runtime.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.ScrollBarArrowSize;
                        *value = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::MicroCom.Runtime.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetScrollBarThumbBoxSizeDelegate(void* @this, AvaloniaFluentUI.Controls.Interop.WinRT.WinRTSize* value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetScrollBarThumbBoxSize(void* @this, AvaloniaFluentUI.Controls.Interop.WinRT.WinRTSize* value)
        {
            IUISettings __target = null;
            try
            {
                {
                    __target = (IUISettings)global::MicroCom.Runtime.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.ScrollBarThumbBoxSize;
                        *value = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::MicroCom.Runtime.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetMessageDurationDelegate(void* @this, uint* value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetMessageDuration(void* @this, uint* value)
        {
            IUISettings __target = null;
            try
            {
                {
                    __target = (IUISettings)global::MicroCom.Runtime.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.MessageDuration;
                        *value = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::MicroCom.Runtime.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetAnimationsEnabledDelegate(void* @this, int* value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetAnimationsEnabled(void* @this, int* value)
        {
            IUISettings __target = null;
            try
            {
                {
                    __target = (IUISettings)global::MicroCom.Runtime.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.AnimationsEnabled;
                        *value = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::MicroCom.Runtime.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetCaretBrowsingEnabledDelegate(void* @this, int* value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetCaretBrowsingEnabled(void* @this, int* value)
        {
            IUISettings __target = null;
            try
            {
                {
                    __target = (IUISettings)global::MicroCom.Runtime.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.CaretBrowsingEnabled;
                        *value = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::MicroCom.Runtime.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetCaretBlinkRateDelegate(void* @this, uint* value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetCaretBlinkRate(void* @this, uint* value)
        {
            IUISettings __target = null;
            try
            {
                {
                    __target = (IUISettings)global::MicroCom.Runtime.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.CaretBlinkRate;
                        *value = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::MicroCom.Runtime.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetCaretWidthDelegate(void* @this, uint* value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetCaretWidth(void* @this, uint* value)
        {
            IUISettings __target = null;
            try
            {
                {
                    __target = (IUISettings)global::MicroCom.Runtime.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.CaretWidth;
                        *value = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::MicroCom.Runtime.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetDoubleClickTimeDelegate(void* @this, uint* value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetDoubleClickTime(void* @this, uint* value)
        {
            IUISettings __target = null;
            try
            {
                {
                    __target = (IUISettings)global::MicroCom.Runtime.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.DoubleClickTime;
                        *value = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::MicroCom.Runtime.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetMouseHoverTimeDelegate(void* @this, uint* value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetMouseHoverTime(void* @this, uint* value)
        {
            IUISettings __target = null;
            try
            {
                {
                    __target = (IUISettings)global::MicroCom.Runtime.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.MouseHoverTime;
                        *value = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::MicroCom.Runtime.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int UIElementColorDelegate(void* @this, UIElementType desiredElement, AvaloniaFluentUI.Controls.Interop.WinRT.WinRTColor* value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int UIElementColor(void* @this, UIElementType desiredElement, AvaloniaFluentUI.Controls.Interop.WinRT.WinRTColor* value)
        {
            IUISettings __target = null;
            try
            {
                {
                    __target = (IUISettings)global::MicroCom.Runtime.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.UIElementColor(desiredElement);
                        *value = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::MicroCom.Runtime.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        protected __MicroComIUISettingsVTable()
        {
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, HandPreference*, int>)&GetHandPreference); 
#else
            base.AddMethod((GetHandPreferenceDelegate)GetHandPreference); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, AvaloniaFluentUI.Controls.Interop.WinRT.WinRTSize*, int>)&GetCursorSize); 
#else
            base.AddMethod((GetCursorSizeDelegate)GetCursorSize); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, AvaloniaFluentUI.Controls.Interop.WinRT.WinRTSize*, int>)&GetScrollBarSize); 
#else
            base.AddMethod((GetScrollBarSizeDelegate)GetScrollBarSize); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, AvaloniaFluentUI.Controls.Interop.WinRT.WinRTSize*, int>)&GetScrollBarArrowSize); 
#else
            base.AddMethod((GetScrollBarArrowSizeDelegate)GetScrollBarArrowSize); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, AvaloniaFluentUI.Controls.Interop.WinRT.WinRTSize*, int>)&GetScrollBarThumbBoxSize); 
#else
            base.AddMethod((GetScrollBarThumbBoxSizeDelegate)GetScrollBarThumbBoxSize); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, uint*, int>)&GetMessageDuration); 
#else
            base.AddMethod((GetMessageDurationDelegate)GetMessageDuration); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, int*, int>)&GetAnimationsEnabled); 
#else
            base.AddMethod((GetAnimationsEnabledDelegate)GetAnimationsEnabled); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, int*, int>)&GetCaretBrowsingEnabled); 
#else
            base.AddMethod((GetCaretBrowsingEnabledDelegate)GetCaretBrowsingEnabled); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, uint*, int>)&GetCaretBlinkRate); 
#else
            base.AddMethod((GetCaretBlinkRateDelegate)GetCaretBlinkRate); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, uint*, int>)&GetCaretWidth); 
#else
            base.AddMethod((GetCaretWidthDelegate)GetCaretWidth); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, uint*, int>)&GetDoubleClickTime); 
#else
            base.AddMethod((GetDoubleClickTimeDelegate)GetDoubleClickTime); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, uint*, int>)&GetMouseHoverTime); 
#else
            base.AddMethod((GetMouseHoverTimeDelegate)GetMouseHoverTime); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, UIElementType, AvaloniaFluentUI.Controls.Interop.WinRT.WinRTColor*, int>)&UIElementColor); 
#else
            base.AddMethod((UIElementColorDelegate)UIElementColor); 
#endif
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit() => global::MicroCom.Runtime.MicroComRuntime.RegisterVTable(typeof(IUISettings), new __MicroComIUISettingsVTable().CreateVTable());
    }

    internal unsafe partial class __MicroComIUISettings2Proxy : __MicroComIInspectableProxy, IUISettings2
    {
        public double TextScaleFactor
        {
            get
            {
                int __result;
                double value = default;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 0])(PPV, &value);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetTextScaleFactor failed", __result);
                return value;
            }
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit()
        {
            global::MicroCom.Runtime.MicroComRuntime.Register(typeof(IUISettings2), new Guid("BAD82401-2721-44F9-BB91-2BB228BE442F"), (p, owns) => new __MicroComIUISettings2Proxy(p, owns));
        }

        protected __MicroComIUISettings2Proxy(IntPtr nativePointer, bool ownsHandle) : base(nativePointer, ownsHandle)
        {
        }

        protected override int VTableSize => base.VTableSize + 1;
    }

    unsafe class __MicroComIUISettings2VTable : __MicroComIInspectableVTable
    {
        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetTextScaleFactorDelegate(void* @this, double* value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetTextScaleFactor(void* @this, double* value)
        {
            IUISettings2 __target = null;
            try
            {
                {
                    __target = (IUISettings2)global::MicroCom.Runtime.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.TextScaleFactor;
                        *value = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::MicroCom.Runtime.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        protected __MicroComIUISettings2VTable()
        {
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, double*, int>)&GetTextScaleFactor); 
#else
            base.AddMethod((GetTextScaleFactorDelegate)GetTextScaleFactor); 
#endif
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit() => global::MicroCom.Runtime.MicroComRuntime.RegisterVTable(typeof(IUISettings2), new __MicroComIUISettings2VTable().CreateVTable());
    }

    internal unsafe partial class __MicroComIUISettings3Proxy : __MicroComIInspectableProxy, IUISettings3
    {
        public AvaloniaFluentUI.Controls.Interop.WinRT.WinRTColor GetColorValue(UIColorType desiredColor)
        {
            int __result;
            AvaloniaFluentUI.Controls.Interop.WinRT.WinRTColor value = default;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, UIColorType, void*, int>)(*PPV)[base.VTableSize + 0])(PPV, desiredColor, &value);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("GetColorValue failed", __result);
            return value;
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit()
        {
            global::MicroCom.Runtime.MicroComRuntime.Register(typeof(IUISettings3), new Guid("03021BE4-5254-4781-8194-5168F7D06D7B"), (p, owns) => new __MicroComIUISettings3Proxy(p, owns));
        }

        protected __MicroComIUISettings3Proxy(IntPtr nativePointer, bool ownsHandle) : base(nativePointer, ownsHandle)
        {
        }

        protected override int VTableSize => base.VTableSize + 1;
    }

    unsafe class __MicroComIUISettings3VTable : __MicroComIInspectableVTable
    {
        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetColorValueDelegate(void* @this, UIColorType desiredColor, AvaloniaFluentUI.Controls.Interop.WinRT.WinRTColor* value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetColorValue(void* @this, UIColorType desiredColor, AvaloniaFluentUI.Controls.Interop.WinRT.WinRTColor* value)
        {
            IUISettings3 __target = null;
            try
            {
                {
                    __target = (IUISettings3)global::MicroCom.Runtime.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.GetColorValue(desiredColor);
                        *value = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::MicroCom.Runtime.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        protected __MicroComIUISettings3VTable()
        {
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, UIColorType, AvaloniaFluentUI.Controls.Interop.WinRT.WinRTColor*, int>)&GetColorValue); 
#else
            base.AddMethod((GetColorValueDelegate)GetColorValue); 
#endif
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit() => global::MicroCom.Runtime.MicroComRuntime.RegisterVTable(typeof(IUISettings3), new __MicroComIUISettings3VTable().CreateVTable());
    }

    internal unsafe partial class __MicroComIUISettings4Proxy : __MicroComIInspectableProxy, IUISettings4
    {
        public int AdvancedEffectsEnabled
        {
            get
            {
                int __result;
                int value = default;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 0])(PPV, &value);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetAdvancedEffectsEnabled failed", __result);
                return value;
            }
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit()
        {
            global::MicroCom.Runtime.MicroComRuntime.Register(typeof(IUISettings4), new Guid("52BB3002-919B-4D6B-9B78-8DD66FF4B93B"), (p, owns) => new __MicroComIUISettings4Proxy(p, owns));
        }

        protected __MicroComIUISettings4Proxy(IntPtr nativePointer, bool ownsHandle) : base(nativePointer, ownsHandle)
        {
        }

        protected override int VTableSize => base.VTableSize + 1;
    }

    unsafe class __MicroComIUISettings4VTable : __MicroComIInspectableVTable
    {
        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetAdvancedEffectsEnabledDelegate(void* @this, int* value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetAdvancedEffectsEnabled(void* @this, int* value)
        {
            IUISettings4 __target = null;
            try
            {
                {
                    __target = (IUISettings4)global::MicroCom.Runtime.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.AdvancedEffectsEnabled;
                        *value = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::MicroCom.Runtime.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        protected __MicroComIUISettings4VTable()
        {
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, int*, int>)&GetAdvancedEffectsEnabled); 
#else
            base.AddMethod((GetAdvancedEffectsEnabledDelegate)GetAdvancedEffectsEnabled); 
#endif
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit() => global::MicroCom.Runtime.MicroComRuntime.RegisterVTable(typeof(IUISettings4), new __MicroComIUISettings4VTable().CreateVTable());
    }

    internal unsafe partial class __MicroComIAccessibilitySettingsProxy : __MicroComIInspectableProxy, IAccessibilitySettings
    {
        public int HighContrast
        {
            get
            {
                int __result;
                int value = default;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 0])(PPV, &value);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetHighContrast failed", __result);
                return value;
            }
        }

        public IntPtr HighContrastScheme
        {
            get
            {
                int __result;
                IntPtr value = default;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 1])(PPV, &value);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetHighContrastScheme failed", __result);
                return value;
            }
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit()
        {
            global::MicroCom.Runtime.MicroComRuntime.Register(typeof(IAccessibilitySettings), new Guid("FE0E8147-C4C0-4562-B962-1327B52AD5B9"), (p, owns) => new __MicroComIAccessibilitySettingsProxy(p, owns));
        }

        protected __MicroComIAccessibilitySettingsProxy(IntPtr nativePointer, bool ownsHandle) : base(nativePointer, ownsHandle)
        {
        }

        protected override int VTableSize => base.VTableSize + 2;
    }

    unsafe class __MicroComIAccessibilitySettingsVTable : __MicroComIInspectableVTable
    {
        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetHighContrastDelegate(void* @this, int* value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetHighContrast(void* @this, int* value)
        {
            IAccessibilitySettings __target = null;
            try
            {
                {
                    __target = (IAccessibilitySettings)global::MicroCom.Runtime.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.HighContrast;
                        *value = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::MicroCom.Runtime.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetHighContrastSchemeDelegate(void* @this, IntPtr* value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetHighContrastScheme(void* @this, IntPtr* value)
        {
            IAccessibilitySettings __target = null;
            try
            {
                {
                    __target = (IAccessibilitySettings)global::MicroCom.Runtime.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.HighContrastScheme;
                        *value = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::MicroCom.Runtime.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        protected __MicroComIAccessibilitySettingsVTable()
        {
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, int*, int>)&GetHighContrast); 
#else
            base.AddMethod((GetHighContrastDelegate)GetHighContrast); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, IntPtr*, int>)&GetHighContrastScheme); 
#else
            base.AddMethod((GetHighContrastSchemeDelegate)GetHighContrastScheme); 
#endif
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit() => global::MicroCom.Runtime.MicroComRuntime.RegisterVTable(typeof(IAccessibilitySettings), new __MicroComIAccessibilitySettingsVTable().CreateVTable());
    }

    internal unsafe partial class __MicroComITaskbarListProxy : global::MicroCom.Runtime.MicroComProxyBase, ITaskbarList
    {
        public void HrInit()
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, int>)(*PPV)[base.VTableSize + 0])(PPV);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("HrInit failed", __result);
        }

        public void AddTab(IntPtr hwnd)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, IntPtr, int>)(*PPV)[base.VTableSize + 1])(PPV, hwnd);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("AddTab failed", __result);
        }

        public void DeleteTab(IntPtr hwnd)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, IntPtr, int>)(*PPV)[base.VTableSize + 2])(PPV, hwnd);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("DeleteTab failed", __result);
        }

        public void ActivateTab(IntPtr hwnd)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, IntPtr, int>)(*PPV)[base.VTableSize + 3])(PPV, hwnd);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("ActivateTab failed", __result);
        }

        public void SetActiveAlt(IntPtr hwnd)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, IntPtr, int>)(*PPV)[base.VTableSize + 4])(PPV, hwnd);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("SetActiveAlt failed", __result);
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit()
        {
            global::MicroCom.Runtime.MicroComRuntime.Register(typeof(ITaskbarList), new Guid("56FDF342-FD6D-11d0-958A-006097C9A090"), (p, owns) => new __MicroComITaskbarListProxy(p, owns));
        }

        protected __MicroComITaskbarListProxy(IntPtr nativePointer, bool ownsHandle) : base(nativePointer, ownsHandle)
        {
        }

        protected override int VTableSize => base.VTableSize + 5;
    }

    unsafe class __MicroComITaskbarListVTable : global::MicroCom.Runtime.MicroComVtblBase
    {
        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int HrInitDelegate(void* @this);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int HrInit(void* @this)
        {
            ITaskbarList __target = null;
            try
            {
                {
                    __target = (ITaskbarList)global::MicroCom.Runtime.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.HrInit();
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::MicroCom.Runtime.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int AddTabDelegate(void* @this, IntPtr hwnd);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int AddTab(void* @this, IntPtr hwnd)
        {
            ITaskbarList __target = null;
            try
            {
                {
                    __target = (ITaskbarList)global::MicroCom.Runtime.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.AddTab(hwnd);
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::MicroCom.Runtime.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int DeleteTabDelegate(void* @this, IntPtr hwnd);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int DeleteTab(void* @this, IntPtr hwnd)
        {
            ITaskbarList __target = null;
            try
            {
                {
                    __target = (ITaskbarList)global::MicroCom.Runtime.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.DeleteTab(hwnd);
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::MicroCom.Runtime.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int ActivateTabDelegate(void* @this, IntPtr hwnd);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int ActivateTab(void* @this, IntPtr hwnd)
        {
            ITaskbarList __target = null;
            try
            {
                {
                    __target = (ITaskbarList)global::MicroCom.Runtime.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.ActivateTab(hwnd);
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::MicroCom.Runtime.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int SetActiveAltDelegate(void* @this, IntPtr hwnd);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int SetActiveAlt(void* @this, IntPtr hwnd)
        {
            ITaskbarList __target = null;
            try
            {
                {
                    __target = (ITaskbarList)global::MicroCom.Runtime.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.SetActiveAlt(hwnd);
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::MicroCom.Runtime.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        protected __MicroComITaskbarListVTable()
        {
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, int>)&HrInit); 
#else
            base.AddMethod((HrInitDelegate)HrInit); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, IntPtr, int>)&AddTab); 
#else
            base.AddMethod((AddTabDelegate)AddTab); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, IntPtr, int>)&DeleteTab); 
#else
            base.AddMethod((DeleteTabDelegate)DeleteTab); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, IntPtr, int>)&ActivateTab); 
#else
            base.AddMethod((ActivateTabDelegate)ActivateTab); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, IntPtr, int>)&SetActiveAlt); 
#else
            base.AddMethod((SetActiveAltDelegate)SetActiveAlt); 
#endif
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit() => global::MicroCom.Runtime.MicroComRuntime.RegisterVTable(typeof(ITaskbarList), new __MicroComITaskbarListVTable().CreateVTable());
    }

    internal unsafe partial class __MicroComITaskbarList2Proxy : __MicroComITaskbarListProxy, ITaskbarList2
    {
        public void MarkFullscreenWindow(IntPtr hwnd, int fFullscreen)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, IntPtr, int, int>)(*PPV)[base.VTableSize + 0])(PPV, hwnd, fFullscreen);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("MarkFullscreenWindow failed", __result);
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit()
        {
            global::MicroCom.Runtime.MicroComRuntime.Register(typeof(ITaskbarList2), new Guid("602D4995-B13A-429b-A66E-1935E44F4317"), (p, owns) => new __MicroComITaskbarList2Proxy(p, owns));
        }

        protected __MicroComITaskbarList2Proxy(IntPtr nativePointer, bool ownsHandle) : base(nativePointer, ownsHandle)
        {
        }

        protected override int VTableSize => base.VTableSize + 1;
    }

    unsafe class __MicroComITaskbarList2VTable : __MicroComITaskbarListVTable
    {
        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int MarkFullscreenWindowDelegate(void* @this, IntPtr hwnd, int fFullscreen);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int MarkFullscreenWindow(void* @this, IntPtr hwnd, int fFullscreen)
        {
            ITaskbarList2 __target = null;
            try
            {
                {
                    __target = (ITaskbarList2)global::MicroCom.Runtime.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.MarkFullscreenWindow(hwnd, fFullscreen);
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::MicroCom.Runtime.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        protected __MicroComITaskbarList2VTable()
        {
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, IntPtr, int, int>)&MarkFullscreenWindow); 
#else
            base.AddMethod((MarkFullscreenWindowDelegate)MarkFullscreenWindow); 
#endif
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit() => global::MicroCom.Runtime.MicroComRuntime.RegisterVTable(typeof(ITaskbarList2), new __MicroComITaskbarList2VTable().CreateVTable());
    }

    internal unsafe partial class __MicroComITaskbarList3Proxy : __MicroComITaskbarList2Proxy, ITaskbarList3
    {
        public void SetProgressValue(IntPtr hwnd, ulong ullCompleted, ulong ullTotal)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, IntPtr, ulong, ulong, int>)(*PPV)[base.VTableSize + 0])(PPV, hwnd, ullCompleted, ullTotal);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("SetProgressValue failed", __result);
        }

        public void SetProgressState(IntPtr hwnd, int tbpFlags)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, IntPtr, int, int>)(*PPV)[base.VTableSize + 1])(PPV, hwnd, tbpFlags);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("SetProgressState failed", __result);
        }

        public void RegisterTab(IntPtr hwndTab, IntPtr hwndMDI)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, IntPtr, IntPtr, int>)(*PPV)[base.VTableSize + 2])(PPV, hwndTab, hwndMDI);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("RegisterTab failed", __result);
        }

        public void UnregisterTab(IntPtr hwndTab)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, IntPtr, int>)(*PPV)[base.VTableSize + 3])(PPV, hwndTab);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("UnregisterTab failed", __result);
        }

        public void SetTabOrder(IntPtr hwndTab, IntPtr hwndInsertBefore)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, IntPtr, IntPtr, int>)(*PPV)[base.VTableSize + 4])(PPV, hwndTab, hwndInsertBefore);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("SetTabOrder failed", __result);
        }

        public void SetTabActive(IntPtr hwndTab, IntPtr hwndMDI, int dwReserved)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, IntPtr, IntPtr, int, int>)(*PPV)[base.VTableSize + 5])(PPV, hwndTab, hwndMDI, dwReserved);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("SetTabActive failed", __result);
        }

        public void ThumbBarAddButtons(IntPtr hwnd, uint cButtons, int pButton)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, IntPtr, uint, int, int>)(*PPV)[base.VTableSize + 6])(PPV, hwnd, cButtons, pButton);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("ThumbBarAddButtons failed", __result);
        }

        public void ThumbBarUpdateButtons(IntPtr hwnd, uint cButtons, int pButton)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, IntPtr, uint, int, int>)(*PPV)[base.VTableSize + 7])(PPV, hwnd, cButtons, pButton);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("ThumbBarUpdateButtons failed", __result);
        }

        public void ThumbBarSetImageList(IntPtr hwnd, IntPtr himl)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, IntPtr, IntPtr, int>)(*PPV)[base.VTableSize + 8])(PPV, hwnd, himl);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("ThumbBarSetImageList failed", __result);
        }

        public void SetOverlayIcon(IntPtr hwnd, void* hIcon, ushort* pszDescription)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, IntPtr, void*, void*, int>)(*PPV)[base.VTableSize + 9])(PPV, hwnd, hIcon, pszDescription);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("SetOverlayIcon failed", __result);
        }

        public void SetThumbnailTooltip(IntPtr hwnd, ushort* pszTip)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, IntPtr, void*, int>)(*PPV)[base.VTableSize + 10])(PPV, hwnd, pszTip);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("SetThumbnailTooltip failed", __result);
        }

        public void SetThumbnailClip(IntPtr hwnd, AvaloniaFluentUI.Controls.Interop.Win32.RECT* prcClip)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, IntPtr, void*, int>)(*PPV)[base.VTableSize + 11])(PPV, hwnd, prcClip);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("SetThumbnailClip failed", __result);
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit()
        {
            global::MicroCom.Runtime.MicroComRuntime.Register(typeof(ITaskbarList3), new Guid("ea1afb91-9e28-4b86-90e9-9e9f8a5eefaf"), (p, owns) => new __MicroComITaskbarList3Proxy(p, owns));
        }

        protected __MicroComITaskbarList3Proxy(IntPtr nativePointer, bool ownsHandle) : base(nativePointer, ownsHandle)
        {
        }

        protected override int VTableSize => base.VTableSize + 12;
    }

    unsafe class __MicroComITaskbarList3VTable : __MicroComITaskbarList2VTable
    {
        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int SetProgressValueDelegate(void* @this, IntPtr hwnd, ulong ullCompleted, ulong ullTotal);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int SetProgressValue(void* @this, IntPtr hwnd, ulong ullCompleted, ulong ullTotal)
        {
            ITaskbarList3 __target = null;
            try
            {
                {
                    __target = (ITaskbarList3)global::MicroCom.Runtime.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.SetProgressValue(hwnd, ullCompleted, ullTotal);
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::MicroCom.Runtime.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int SetProgressStateDelegate(void* @this, IntPtr hwnd, int tbpFlags);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int SetProgressState(void* @this, IntPtr hwnd, int tbpFlags)
        {
            ITaskbarList3 __target = null;
            try
            {
                {
                    __target = (ITaskbarList3)global::MicroCom.Runtime.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.SetProgressState(hwnd, tbpFlags);
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::MicroCom.Runtime.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int RegisterTabDelegate(void* @this, IntPtr hwndTab, IntPtr hwndMDI);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int RegisterTab(void* @this, IntPtr hwndTab, IntPtr hwndMDI)
        {
            ITaskbarList3 __target = null;
            try
            {
                {
                    __target = (ITaskbarList3)global::MicroCom.Runtime.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.RegisterTab(hwndTab, hwndMDI);
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::MicroCom.Runtime.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int UnregisterTabDelegate(void* @this, IntPtr hwndTab);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int UnregisterTab(void* @this, IntPtr hwndTab)
        {
            ITaskbarList3 __target = null;
            try
            {
                {
                    __target = (ITaskbarList3)global::MicroCom.Runtime.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.UnregisterTab(hwndTab);
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::MicroCom.Runtime.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int SetTabOrderDelegate(void* @this, IntPtr hwndTab, IntPtr hwndInsertBefore);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int SetTabOrder(void* @this, IntPtr hwndTab, IntPtr hwndInsertBefore)
        {
            ITaskbarList3 __target = null;
            try
            {
                {
                    __target = (ITaskbarList3)global::MicroCom.Runtime.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.SetTabOrder(hwndTab, hwndInsertBefore);
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::MicroCom.Runtime.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int SetTabActiveDelegate(void* @this, IntPtr hwndTab, IntPtr hwndMDI, int dwReserved);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int SetTabActive(void* @this, IntPtr hwndTab, IntPtr hwndMDI, int dwReserved)
        {
            ITaskbarList3 __target = null;
            try
            {
                {
                    __target = (ITaskbarList3)global::MicroCom.Runtime.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.SetTabActive(hwndTab, hwndMDI, dwReserved);
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::MicroCom.Runtime.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int ThumbBarAddButtonsDelegate(void* @this, IntPtr hwnd, uint cButtons, int pButton);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int ThumbBarAddButtons(void* @this, IntPtr hwnd, uint cButtons, int pButton)
        {
            ITaskbarList3 __target = null;
            try
            {
                {
                    __target = (ITaskbarList3)global::MicroCom.Runtime.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.ThumbBarAddButtons(hwnd, cButtons, pButton);
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::MicroCom.Runtime.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int ThumbBarUpdateButtonsDelegate(void* @this, IntPtr hwnd, uint cButtons, int pButton);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int ThumbBarUpdateButtons(void* @this, IntPtr hwnd, uint cButtons, int pButton)
        {
            ITaskbarList3 __target = null;
            try
            {
                {
                    __target = (ITaskbarList3)global::MicroCom.Runtime.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.ThumbBarUpdateButtons(hwnd, cButtons, pButton);
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::MicroCom.Runtime.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int ThumbBarSetImageListDelegate(void* @this, IntPtr hwnd, IntPtr himl);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int ThumbBarSetImageList(void* @this, IntPtr hwnd, IntPtr himl)
        {
            ITaskbarList3 __target = null;
            try
            {
                {
                    __target = (ITaskbarList3)global::MicroCom.Runtime.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.ThumbBarSetImageList(hwnd, himl);
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::MicroCom.Runtime.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int SetOverlayIconDelegate(void* @this, IntPtr hwnd, void* hIcon, ushort* pszDescription);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int SetOverlayIcon(void* @this, IntPtr hwnd, void* hIcon, ushort* pszDescription)
        {
            ITaskbarList3 __target = null;
            try
            {
                {
                    __target = (ITaskbarList3)global::MicroCom.Runtime.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.SetOverlayIcon(hwnd, hIcon, pszDescription);
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::MicroCom.Runtime.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int SetThumbnailTooltipDelegate(void* @this, IntPtr hwnd, ushort* pszTip);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int SetThumbnailTooltip(void* @this, IntPtr hwnd, ushort* pszTip)
        {
            ITaskbarList3 __target = null;
            try
            {
                {
                    __target = (ITaskbarList3)global::MicroCom.Runtime.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.SetThumbnailTooltip(hwnd, pszTip);
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::MicroCom.Runtime.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int SetThumbnailClipDelegate(void* @this, IntPtr hwnd, AvaloniaFluentUI.Controls.Interop.Win32.RECT* prcClip);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int SetThumbnailClip(void* @this, IntPtr hwnd, AvaloniaFluentUI.Controls.Interop.Win32.RECT* prcClip)
        {
            ITaskbarList3 __target = null;
            try
            {
                {
                    __target = (ITaskbarList3)global::MicroCom.Runtime.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.SetThumbnailClip(hwnd, prcClip);
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::MicroCom.Runtime.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        protected __MicroComITaskbarList3VTable()
        {
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, IntPtr, ulong, ulong, int>)&SetProgressValue); 
#else
            base.AddMethod((SetProgressValueDelegate)SetProgressValue); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, IntPtr, int, int>)&SetProgressState); 
#else
            base.AddMethod((SetProgressStateDelegate)SetProgressState); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, IntPtr, IntPtr, int>)&RegisterTab); 
#else
            base.AddMethod((RegisterTabDelegate)RegisterTab); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, IntPtr, int>)&UnregisterTab); 
#else
            base.AddMethod((UnregisterTabDelegate)UnregisterTab); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, IntPtr, IntPtr, int>)&SetTabOrder); 
#else
            base.AddMethod((SetTabOrderDelegate)SetTabOrder); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, IntPtr, IntPtr, int, int>)&SetTabActive); 
#else
            base.AddMethod((SetTabActiveDelegate)SetTabActive); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, IntPtr, uint, int, int>)&ThumbBarAddButtons); 
#else
            base.AddMethod((ThumbBarAddButtonsDelegate)ThumbBarAddButtons); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, IntPtr, uint, int, int>)&ThumbBarUpdateButtons); 
#else
            base.AddMethod((ThumbBarUpdateButtonsDelegate)ThumbBarUpdateButtons); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, IntPtr, IntPtr, int>)&ThumbBarSetImageList); 
#else
            base.AddMethod((ThumbBarSetImageListDelegate)ThumbBarSetImageList); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, IntPtr, void*, ushort*, int>)&SetOverlayIcon); 
#else
            base.AddMethod((SetOverlayIconDelegate)SetOverlayIcon); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, IntPtr, ushort*, int>)&SetThumbnailTooltip); 
#else
            base.AddMethod((SetThumbnailTooltipDelegate)SetThumbnailTooltip); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, IntPtr, AvaloniaFluentUI.Controls.Interop.Win32.RECT*, int>)&SetThumbnailClip); 
#else
            base.AddMethod((SetThumbnailClipDelegate)SetThumbnailClip); 
#endif
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit() => global::MicroCom.Runtime.MicroComRuntime.RegisterVTable(typeof(ITaskbarList3), new __MicroComITaskbarList3VTable().CreateVTable());
    }
}