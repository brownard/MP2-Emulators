using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SharpRetro.LibRetro
{
  public enum RETRO_DEVICE
  {
    NONE = 0,
    JOYPAD = 1,
    MOUSE = 2,
    KEYBOARD = 3,
    LIGHTGUN = 4,
    ANALOG = 5,
    POINTER = 6,
    SENSOR_ACCELEROMETER = 7
  };

  public enum RETRO_DEVICE_ID_JOYPAD
  {
    B = 0,
    Y = 1,
    SELECT = 2,
    START = 3,
    UP = 4,
    DOWN = 5,
    LEFT = 6,
    RIGHT = 7,
    A = 8,
    X = 9,
    L = 10,
    R = 11,
    L2 = 12,
    R2 = 13,
    L3 = 14,
    R3 = 15
  };

  public enum RETRO_LOG_LEVEL : int //exact type size is unclear
  {
    DEBUG = 0,
    INFO,
    WARN,
    ERROR,

    DUMMY = Int32.MaxValue
  };

  public enum RETRO_DEVICE_INDEX_ANALOG
  {
    LEFT = 0,
    RIGHT = 1
  };

  public enum RETRO_DEVICE_ID_ANALOG
  {
    X = 0,
    Y = 1
  };

  public enum RETRO_DEVICE_ID_MOUSE
  {
    X = 0,
    Y = 1,
    LEFT = 2,
    RIGHT = 3
  };

  public enum RETRO_DEVICE_ID_LIGHTGUN
  {
    X = 0,
    Y = 1,
    TRIGGER = 2,
    CURSOR = 3,
    TURBO = 4,
    PAUSE = 5,
    START = 6
  };

  public enum RETRO_DEVICE_ID_POINTER
  {
    X = 0,
    Y = 1,
    PRESSED = 2
  };

  public enum RETRO_DEVICE_ID_SENSOR_ACCELEROMETER
  {
    X = 0,
    Y = 1,
    Z = 2
  };

  public enum RETRO_REGION
  {
    NTSC = 0,
    PAL = 1
  };

  public enum RETRO_MEMORY
  {
    SAVE_RAM = 0,
    RTC = 1,
    SYSTEM_RAM = 2,
    VIDEO_RAM = 3,
  };

  public enum RETRO_KEY
  {
    UNKNOWN = 0,
    FIRST = 0,
    BACKSPACE = 8,
    TAB = 9,
    CLEAR = 12,
    RETURN = 13,
    PAUSE = 19,
    ESCAPE = 27,
    SPACE = 32,
    EXCLAIM = 33,
    QUOTEDBL = 34,
    HASH = 35,
    DOLLAR = 36,
    AMPERSAND = 38,
    QUOTE = 39,
    LEFTPAREN = 40,
    RIGHTPAREN = 41,
    ASTERISK = 42,
    PLUS = 43,
    COMMA = 44,
    MINUS = 45,
    PERIOD = 46,
    SLASH = 47,
    _0 = 48,
    _1 = 49,
    _2 = 50,
    _3 = 51,
    _4 = 52,
    _5 = 53,
    _6 = 54,
    _7 = 55,
    _8 = 56,
    _9 = 57,
    COLON = 58,
    SEMICOLON = 59,
    LESS = 60,
    EQUALS = 61,
    GREATER = 62,
    QUESTION = 63,
    AT = 64,
    LEFTBRACKET = 91,
    BACKSLASH = 92,
    RIGHTBRACKET = 93,
    CARET = 94,
    UNDERSCORE = 95,
    BACKQUOTE = 96,
    a = 97,
    b = 98,
    c = 99,
    d = 100,
    e = 101,
    f = 102,
    g = 103,
    h = 104,
    i = 105,
    j = 106,
    k = 107,
    l = 108,
    m = 109,
    n = 110,
    o = 111,
    p = 112,
    q = 113,
    r = 114,
    s = 115,
    t = 116,
    u = 117,
    v = 118,
    w = 119,
    x = 120,
    y = 121,
    z = 122,
    DELETE = 127,

    KP0 = 256,
    KP1 = 257,
    KP2 = 258,
    KP3 = 259,
    KP4 = 260,
    KP5 = 261,
    KP6 = 262,
    KP7 = 263,
    KP8 = 264,
    KP9 = 265,
    KP_PERIOD = 266,
    KP_DIVIDE = 267,
    KP_MULTIPLY = 268,
    KP_MINUS = 269,
    KP_PLUS = 270,
    KP_ENTER = 271,
    KP_EQUALS = 272,

    UP = 273,
    DOWN = 274,
    RIGHT = 275,
    LEFT = 276,
    INSERT = 277,
    HOME = 278,
    END = 279,
    PAGEUP = 280,
    PAGEDOWN = 281,

    F1 = 282,
    F2 = 283,
    F3 = 284,
    F4 = 285,
    F5 = 286,
    F6 = 287,
    F7 = 288,
    F8 = 289,
    F9 = 290,
    F10 = 291,
    F11 = 292,
    F12 = 293,
    F13 = 294,
    F14 = 295,
    F15 = 296,

    NUMLOCK = 300,
    CAPSLOCK = 301,
    SCROLLOCK = 302,
    RSHIFT = 303,
    LSHIFT = 304,
    RCTRL = 305,
    LCTRL = 306,
    RALT = 307,
    LALT = 308,
    RMETA = 309,
    LMETA = 310,
    LSUPER = 311,
    RSUPER = 312,
    MODE = 313,
    COMPOSE = 314,

    HELP = 315,
    PRINT = 316,
    SYSREQ = 317,
    BREAK = 318,
    MENU = 319,
    POWER = 320,
    EURO = 321,
    UNDO = 322,

    LAST
  };

  [Flags]
  public enum RETRO_MOD
  {
    NONE = 0,
    SHIFT = 1,
    CTRL = 2,
    ALT = 4,
    META = 8,
    NUMLOCK = 16,
    CAPSLOCK = 32,
    SCROLLLOCK = 64
  };

  [Flags]
  public enum RETRO_SIMD
  {
    SSE = (1 << 0),
    SSE2 = (1 << 1),
    VMX = (1 << 2),
    VMX128 = (1 << 3),
    AVX = (1 << 4),
    NEON = (1 << 5),
    SSE3 = (1 << 6),
    SSSE3 = (1 << 7),
    MMX = (1 << 8),
    MMXEXT = (1 << 9),
    SSE4 = (1 << 10),
    SSE42 = (1 << 11),
    AVX2 = (1 << 12),
    VFPU = (1 << 13),
    PS = (1 << 14),
    AES = (1 << 15),
    VFPV3 = (1 << 16),
    VFPV4 = (1 << 17),
  }

  public enum RETRO_ENVIRONMENT
  {
    /// <summary>
    /// const unsigned * --<br/>
    /// Sets screen rotation of graphics.<br/>
    /// Valid values are 0, 1, 2, 3, which rotates screen by 0, 90, 180,<br/>
    /// 270 degrees counter-clockwise respectively.<br/>
    /// </summary>
    SET_ROTATION = 1,

    /// <summary>
    /// bool * --<br/>
    /// NOTE: As of 2019 this callback is considered deprecated in favor of<br/>
    /// using core options to manage overscan in a more nuanced, core-specific way.<br/>
    /// </summary>
    GET_OVERSCAN = 2,

    /// <summary>
    /// bool * --<br/>
    /// Boolean value whether or not frontend supports frame duping,<br/>
    /// passing NULL to video frame callback.<br/>
    /// </summary>
    GET_CAN_DUPE = 3,

    /// <summary>
    /// const struct retro_message * --<br/>
    /// Sets a message to be displayed in implementation-specific manner<br/>
    /// for a certain amount of 'frames'.<br/>
    /// Should not be used for trivial messages, which should simply be<br/>
    /// logged via RETRO_ENVIRONMENT_GET_LOG_INTERFACE (or as a<br/>
    /// fallback, stderr).<br/>
    /// </summary>
    SET_MESSAGE = 6,

    /// <summary>
    /// N<br/>
    /// Requests the frontend to shutdown.<br/>
    /// Should only be used if game has a specific<br/>
    /// way to shutdown the game from a menu item or similar.<br/>
    /// </summary>
    SHUTDOWN = 7,

    /// <summary>
    /// const unsigned * --<br/>
    /// Gives a hint to the frontend how demanding this implementation<br/>
    /// is on a system. E.g. reporting a level of 2 means<br/>
    /// this implementation should run decently on all frontends<br/>
    /// of level 2 and up.<br/>
    /// </summary>
    SET_PERFORMANCE_LEVEL = 8,

    /// <summary>
    /// const char ** --<br/>
    /// Returns the "system" directory of the frontend.<br/>
    /// This directory can be used to store system specific<br/>
    /// content such as BIOSes, configuration data, etc.<br/>
    /// The returned value can be NULL.<br/>
    /// If so, no such directory is defined,<br/>
    /// and it's up to the implementation to find a suitable directory.<br/>
    /// </summary>
    GET_SYSTEM_DIRECTORY = 9,

    /// <summary>
    /// const enum retro_pixel_format * --<br/>
    /// Sets the internal pixel format used by the implementation.<br/>
    /// The default pixel format is RETRO_PIXEL_FORMAT_0RGB1555.<br/>
    /// This pixel format however, is deprecated (see enum retro_pixel_format).<br/>
    /// If the call returns false, the frontend does not support this pixel<br/>
    /// format.<br/>
    /// </summary>
    SET_PIXEL_FORMAT = 10,

    /// <summary>
    /// const struct retro_input_descriptor * --<br/>
    /// Sets an array of retro_input_descriptors.<br/>
    /// It is up to the frontend to present this in a usable way.<br/>
    /// The array is terminated by retro_input_descriptor::description<br/>
    /// being set to NULL.<br/>
    /// This function can be called at any time, but it is recommended<br/>
    /// to call it as early as possible.<br/>
    /// </summary>
    SET_INPUT_DESCRIPTORS = 11,

    /// <summary>
    /// const struct retro_keyboard_callback * --<br/>
    /// Sets a callback function used to notify core about keyboard events.<br/>
    /// </summary>
    SET_KEYBOARD_CALLBACK = 12,

    /// <summary>
    /// const struct retro_disk_control_callback * --<br/>
    /// Sets an interface which frontend can use to eject and insert<br/>
    /// disk images.<br/>
    /// This is used for games which consist of multiple images and<br/>
    /// must be manually swapped out by the user (e.g. PSX).<br/>
    /// </summary>
    SET_DISK_CONTROL_INTERFACE = 13,

    /// <summary>
    /// struct retro_hw_render_callback * --<br/>
    /// Sets an interface to let a libretro core render with<br/>
    /// hardware acceleration.<br/>
    /// Should be called in retro_load_game().<br/>
    /// If successful, libretro cores will be able to render to a<br/>
    /// frontend-provided framebuffer.<br/>
    /// The size of this framebuffer will be at least as large as<br/>
    /// max_width<br/>
    /// If HW rendering is used, pass only RETRO_HW_FRAME_BUFFER_VALID or<br/>
    /// NULL to retro_video_refresh_t.<br/>
    /// </summary>
    SET_HW_RENDER = 14,

    /// <summary>
    /// struct retro_variable * --<br/>
    /// Interface to acquire user-defined information from environment<br/>
    /// that cannot feasibly be supported in a multi-system way.<br/>
    /// 'key' should be set to a key which has already been set by<br/>
    /// SET_VARIABLES.<br/>
    /// 'data' will be set to a value or NULL.<br/>
    /// </summary>
    GET_VARIABLE = 15,

    /// <summary>
    /// const struct retro_variable * --<br/>
    /// Allows an implementation to signal the environment<br/>
    /// which variables it might want to check for later using<br/>
    /// GET_VARIABLE.<br/>
    /// This allows the frontend to present these variables to<br/>
    /// a user dynamically.<br/>
    /// This should be called the first time as early as<br/>
    /// possible (ideally in retro_set_environment).<br/>
    /// Afterward it may be called again for the core to communicate<br/>
    /// updated options to the frontend, but the number of core<br/>
    /// options must not change from the number in the initial call.<br/>
    /// </summary>
    SET_VARIABLES = 16,

    /// <summary>
    /// bool * --<br/>
    /// Result is set to true if some variables are updated by<br/>
    /// frontend since last call to RETRO_ENVIRONMENT_GET_VARIABLE.<br/>
    /// Variables should be queried with GET_VARIABLE.<br/>
    /// </summary>
    GET_VARIABLE_UPDATE = 17,

    /// <summary>
    /// const bool * --<br/>
    /// If true, the libretro implementation supports calls to<br/>
    /// retro_load_game() with NULL as argument.<br/>
    /// Used by cores which can run without particular game data.<br/>
    /// This should be called within retro_set_environment() only.<br/>
    /// </summary>
    SET_SUPPORT_NO_GAME = 18,

    /// <summary>
    /// const char ** --<br/>
    /// Retrieves the absolute path from where this libretro<br/>
    /// implementation was loaded.<br/>
    /// NULL is returned if the libretro was loaded statically<br/>
    /// (i.e. linked statically to frontend), or if the path cannot be<br/>
    /// determined.<br/>
    /// Mostly useful in cooperation with SET_SUPPORT_NO_GAME as assets can<br/>
    /// be loaded without ugly hacks.<br/>
    /// </summary>
    GET_LIBRETRO_PATH = 19,

    /// <summary>
    /// const struct retro_frame_time_callback * --<br/>
    /// Lets the core know how much time has passed since last<br/>
    /// invocation of retro_run().<br/>
    /// The frontend can tamper with the timing to fake fast-forward,<br/>
    /// slow-motion, frame stepping, etc.<br/>
    /// In this case the delta time will use the reference value<br/>
    /// in frame_time_callback..<br/>
    /// </summary>
    SET_FRAME_TIME_CALLBACK = 21,

    /// <summary>
    /// const struct retro_audio_callback * --<br/>
    /// Sets an interface which is used to notify a libretro core about audio<br/>
    /// being available for writing.<br/>
    /// The callback can be called from any thread, so a core using this must<br/>
    /// have a thread safe audio implementation.<br/>
    /// It is intended for games where audio and video are completely<br/>
    /// asynchronous and audio can be generated on the fly.<br/>
    /// This interface is not recommended for use with emulators which have<br/>
    /// highly synchronous audio.<br/>
    /// </summary>
    SET_AUDIO_CALLBACK = 22,

    /// <summary>
    /// struct retro_rumble_interface * --<br/>
    /// Gets an interface which is used by a libretro core to set<br/>
    /// state of rumble motors in controllers.<br/>
    /// A strong and weak motor is supported, and they can be<br/>
    /// controlled indepedently.<br/>
    /// Should be called from either retro_init() or retro_load_game().<br/>
    /// Should not be called from retro_set_environment().<br/>
    /// Returns false if rumble functionality is unavailable.<br/>
    /// </summary>
    GET_RUMBLE_INTERFACE = 23,

    /// <summary>
    /// uint64_t * --<br/>
    /// Gets a bitmask telling which device type are expected to be<br/>
    /// handled properly in a call to retro_input_state_t.<br/>
    /// Devices which are not handled or recognized always return<br/>
    /// 0 in retro_input_state_t.<br/>
    /// Example bitmask: caps = (1 << RETRO_DEVICE_JOYPAD) | (1 << RETRO_DEVICE_ANALOG).<br/>
    /// Should only be called in retro_run().<br/>
    /// </summary>
    GET_INPUT_DEVICE_CAPABILITIES = 24,

    /// <summary>
    /// struct retro_sensor_interface * --<br/>
    /// Gets access to the sensor interface.<br/>
    /// The purpose of this interface is to allow<br/>
    /// setting state related to sensors such as polling rate,<br/>
    /// enabling<br/>
    /// Reading sensor state is done via the normal<br/>
    /// input_state_callback API.<br/>
    /// </summary>
    GET_SENSOR_INTERFACE = 25 | RETRO_ENVIRONMENT_EXPERIMENTAL,

    /// <summary>
    /// struct retro_camera_callback * --<br/>
    /// Gets an interface to a video camera driver.<br/>
    /// A libretro core can use this interface to get access to a<br/>
    /// video camera.<br/>
    /// New video frames are delivered in a callback in same<br/>
    /// thread as retro_run().<br/>
    /// </summary>
    GET_CAMERA_INTERFACE = 26 | RETRO_ENVIRONMENT_EXPERIMENTAL,

    /// <summary>
    /// struct retro_log_callback * --<br/>
    /// Gets an interface for logging. This is useful for<br/>
    /// logging in a cross-platform way<br/>
    /// as certain platforms cannot use stderr for logging.<br/>
    /// It also allows the frontend to<br/>
    /// show logging information in a more suitable way.<br/>
    /// If this interface is not used, libretro cores should<br/>
    /// log to stderr as desired.<br/>
    /// </summary>
    GET_LOG_INTERFACE = 27,

    /// <summary>
    /// struct retro_perf_callback * --<br/>
    /// Gets an interface for performance counters. This is useful<br/>
    /// for performance logging in a cross-platform way and for detecting<br/>
    /// architecture-specific features, such as SIMD support.<br/>
    /// </summary>
    GET_PERF_INTERFACE = 28,

    /// <summary>
    /// struct retro_location_callback * --<br/>
    /// Gets access to the location interface.<br/>
    /// The purpose of this interface is to be able to retrieve<br/>
    /// location-based information from the host device,<br/>
    /// such as current latitude<br/>
    /// </summary>
    GET_LOCATION_INTERFACE = 29,

    /// <summary>
    /// Old name, kept for compatibility. *<br/>
    /// </summary>
    GET_CONTENT_DIRECTORY = 30,

    /// <summary>
    /// const char ** --<br/>
    /// Returns the "core assets" directory of the frontend.<br/>
    /// This directory can be used to store specific assets that the<br/>
    /// core relies upon, such as art assets,<br/>
    /// input data, etc etc.<br/>
    /// The returned value can be NULL.<br/>
    /// If so, no such directory is defined,<br/>
    /// and it's up to the implementation to find a suitable directory.<br/>
    /// </summary>
    GET_CORE_ASSETS_DIRECTORY = 30,

    /// <summary>
    /// const char ** --<br/>
    /// Returns the "save" directory of the frontend, unless there is no<br/>
    /// save directory available. The save directory should be used to<br/>
    /// store SRAM, memory cards, high scores, etc, if the libretro core<br/>
    /// cannot use the regular memory interface (retro_get_memory_data()).<br/>
    /// </summary>
    GET_SAVE_DIRECTORY = 31,

    /// <summary>
    /// const struct retro_system_av_info * --<br/>
    /// Sets a new av_info structure. This can only be called from<br/>
    /// within retro_run().<br/>
    /// This should *only* be used if the core is completely altering the<br/>
    /// internal resolutions, aspect ratios, timings, sampling rate, etc.<br/>
    /// Calling this can require a full reinitialization of video<br/>
    /// drivers in the frontend,<br/>
    /// </summary>
    SET_SYSTEM_AV_INFO = 32,

    /// <summary>
    /// const struct retro_get_proc_address_interface * --<br/>
    /// Allows a libretro core to announce support for the<br/>
    /// get_proc_address() interface.<br/>
    /// This interface allows for a standard way to extend libretro where<br/>
    /// use of environment calls are too indirect,<br/>
    /// e.g. for cases where the frontend wants to call directly into the core.<br/>
    /// </summary>
    SET_PROC_ADDRESS_CALLBACK = 33,

    /// <summary>
    /// const struct retro_subsystem_info * --<br/>
    /// This environment call introduces the concept of libretro "subsystems".<br/>
    /// A subsystem is a variant of a libretro core which supports<br/>
    /// different kinds of games.<br/>
    /// The purpose of this is to support e.g. emulators which might<br/>
    /// have special needs, e.g. Super Nintendo's Super GameBoy, Sufami Turbo.<br/>
    /// It can also be used to pick among subsystems in an explicit way<br/>
    /// if the libretro implementation is a multi-system emulator itself.<br/>
    /// </summary>
    SET_SUBSYSTEM_INFO = 34,

    /// <summary>
    /// const struct retro_controller_info * --<br/>
    /// This environment call lets a libretro core tell the frontend<br/>
    /// which controller subclasses are recognized in calls to<br/>
    /// retro_set_controller_port_device().<br/>
    /// </summary>
    SET_CONTROLLER_INFO = 35,

    /// <summary>
    /// const struct retro_memory_map * --<br/>
    /// This environment call lets a libretro core tell the frontend<br/>
    /// about the memory maps this core emulates.<br/>
    /// This can be used to implement, for example, cheats in a core-agnostic way.<br/>
    /// </summary>
    SET_MEMORY_MAPS = 36 | RETRO_ENVIRONMENT_EXPERIMENTAL,

    /// <summary>
    /// const struct retro_game_geometry * --<br/>
    /// This environment call is similar to SET_SYSTEM_AV_INFO for changing<br/>
    /// video parameters, but provides a guarantee that drivers will not be<br/>
    /// reinitialized.<br/>
    /// This can only be called from within retro_run().<br/>
    /// </summary>
    SET_GEOMETRY = 37,

    /// <summary>
    /// const char **<br/>
    /// Returns the specified username of the frontend, if specified by the user.<br/>
    /// This username can be used as a nickname for a core that has online facilities<br/>
    /// or any other mode where personalization of the user is desirable.<br/>
    /// The returned value can be NULL.<br/>
    /// If this environ callback is used by a core that requires a valid username,<br/>
    /// a default username should be specified by the core.<br/>
    /// </summary>
    GET_USERNAME = 38,

    /// <summary>
    /// unsigned * --<br/>
    /// Returns the specified language of the frontend, if specified by the user.<br/>
    /// It can be used by the core for localization purposes.<br/>
    /// </summary>
    GET_LANGUAGE = 39,

    /// <summary>
    /// struct retro_framebuffer * --<br/>
    /// Returns a preallocated framebuffer which the core can use for rendering<br/>
    /// the frame into when not using SET_HW_RENDER.<br/>
    /// The framebuffer returned from this call must not be used<br/>
    /// after the current call to retro_run() returns.<br/>
    /// </summary>
    GET_CURRENT_SOFTWARE_FRAMEBUFFER = 40 | RETRO_ENVIRONMENT_EXPERIMENTAL,

    /// <summary>
    /// const struct retro_hw_render_interface ** --<br/>
    /// Returns an API specific rendering interface for accessing API specific data.<br/>
    /// Not all HW rendering APIs support or need this.<br/>
    /// The contents of the returned pointer is specific to the rendering API<br/>
    /// being used. See the various headers like libretro_vulkan.h, etc.<br/>
    /// </summary>
    GET_HW_RENDER_INTERFACE = 41 | RETRO_ENVIRONMENT_EXPERIMENTAL,

    /// <summary>
    /// const bool * --<br/>
    /// If true, the libretro implementation supports achievements<br/>
    /// either via memory descriptors set with RETRO_ENVIRONMENT_SET_MEMORY_MAPS<br/>
    /// or via retro_get_memory_data<br/>
    /// </summary>
    SET_SUPPORT_ACHIEVEMENTS = 42 | RETRO_ENVIRONMENT_EXPERIMENTAL,

    /// <summary>
    /// const struct retro_hw_render_context_negotiation_interface * --<br/>
    /// Sets an interface which lets the libretro core negotiate with frontend how a context is created.<br/>
    /// The semantics of this interface depends on which API is used in SET_HW_RENDER earlier.<br/>
    /// This interface will be used when the frontend is trying to create a HW rendering context,<br/>
    /// so it will be used after SET_HW_RENDER, but before the context_reset callback.<br/>
    /// </summary>
    SET_HW_RENDER_CONTEXT_NEGOTIATION_INTERFACE = 43 | RETRO_ENVIRONMENT_EXPERIMENTAL,

    /// <summary>
    /// uint64_t * --<br/>
    /// Sets quirk flags associated with serialization. The frontend will zero any flags it doesn't<br/>
    /// recognize or support. Should be set in either retro_init or retro_load_game, but not both.<br/>
    /// </summary>
    SET_SERIALIZATION_QUIRKS = 44,

    /// <summary>
    /// N<br/>
    /// The frontend will try to use a 'shared' hardware context (mostly applicable<br/>
    /// to OpenGL) when a hardware context is being set up.<br/>
    /// </summary>
    SET_HW_SHARED_CONTEXT = 44 | RETRO_ENVIRONMENT_EXPERIMENTAL,

    /// <summary>
    /// struct retro_vfs_interface_info * --<br/>
    /// Gets access to the VFS interface.<br/>
    /// VFS presence needs to be queried prior to load_game or any<br/>
    /// get_system<br/>
    /// core supports VFS before it starts handing out paths.<br/>
    /// It is recomended to do so in retro_set_environment<br/>
    /// </summary>
    GET_VFS_INTERFACE = 45 | RETRO_ENVIRONMENT_EXPERIMENTAL,

    /// <summary>
    /// struct retro_led_interface * --<br/>
    /// Gets an interface which is used by a libretro core to set<br/>
    /// state of LEDs.<br/>
    /// </summary>
    GET_LED_INTERFACE = 46 | RETRO_ENVIRONMENT_EXPERIMENTAL,

    /// <summary>
    /// int * --<br/>
    /// Tells the core if the frontend wants audio or video.<br/>
    /// If disabled, the frontend will discard the audio or video,<br/>
    /// so the core may decide to skip generating a frame or generating audio.<br/>
    /// This is mainly used for increasing performance.<br/>
    /// Bit 0 (value 1): Enable Video<br/>
    /// Bit 1 (value 2): Enable Audio<br/>
    /// Bit 2 (value 4): Use Fast Savestates.<br/>
    /// Bit 3 (value 8): Hard Disable Audio<br/>
    /// Other bits are reserved for future use and will default to zero.<br/>
    /// If video is disabled:<br/>
    /// * The frontend wants the core to not generate any video,<br/>
    /// including presenting frames via hardware acceleration.<br/>
    /// * The frontend's video frame callback will do nothing.<br/>
    /// * After running the frame, the video output of the next frame should be<br/>
    /// no different than if video was enabled, and saving and loading state<br/>
    /// should have no issues.<br/>
    /// If audio is disabled:<br/>
    /// * The frontend wants the core to not generate any audio.<br/>
    /// * The frontend's audio callbacks will do nothing.<br/>
    /// * After running the frame, the audio output of the next frame should be<br/>
    /// no different than if audio was enabled, and saving and loading state<br/>
    /// should have no issues.<br/>
    /// Fast Savestates:<br/>
    /// * Guaranteed to be created by the same binary that will load them.<br/>
    /// * Will not be written to or read from the disk.<br/>
    /// * Suggest that the core assumes loading state will succeed.<br/>
    /// * Suggest that the core updates its memory buffers in-place if possible.<br/>
    /// * Suggest that the core skips clearing memory.<br/>
    /// * Suggest that the core skips resetting the system.<br/>
    /// * Suggest that the core may skip validation steps.<br/>
    /// Hard Disable Audio:<br/>
    /// * Used for a secondary core when running ahead.<br/>
    /// * Indicates that the frontend will never need audio from the core.<br/>
    /// * Suggests that the core may stop synthesizing audio, but this should not<br/>
    /// compromise emulation accuracy.<br/>
    /// * Audio output for the next frame does not matter, and the frontend will<br/>
    /// never need an accurate audio state in the future.<br/>
    /// * State will never be saved when using Hard Disable Audio.<br/>
    /// </summary>
    GET_AUDIO_VIDEO_ENABLE = 47 | RETRO_ENVIRONMENT_EXPERIMENTAL,

    /// <summary>
    /// struct retro_midi_interface ** --<br/>
    /// Returns a MIDI interface that can be used for raw data I<br/>
    /// </summary>
    GET_MIDI_INTERFACE = 48 | RETRO_ENVIRONMENT_EXPERIMENTAL,

    /// <summary>
    /// bool * --<br/>
    /// Boolean value that indicates whether or not the frontend is in<br/>
    /// fastforwarding mode.<br/>
    /// </summary>
    GET_FASTFORWARDING = 49 | RETRO_ENVIRONMENT_EXPERIMENTAL,

    /// <summary>
    /// float * --<br/>
    /// Float value that lets us know what target refresh rate<br/>
    /// is curently in use by the frontend.<br/>
    /// </summary>
    GET_TARGET_REFRESH_RATE = 50 | RETRO_ENVIRONMENT_EXPERIMENTAL,

    /// <summary>
    /// bool * --<br/>
    /// Boolean value that indicates whether or not the frontend supports<br/>
    /// input bitmasks being returned by retro_input_state_t. The advantage<br/>
    /// of this is that retro_input_state_t has to be only called once to<br/>
    /// grab all button states instead of multiple times.<br/>
    /// </summary>
    GET_INPUT_BITMASKS = 51 | RETRO_ENVIRONMENT_EXPERIMENTAL,

    /// <summary>
    /// unsigned * --<br/>
    /// Unsigned value is the API version number of the core options<br/>
    /// interface supported by the frontend. If callback return false,<br/>
    /// API version is assumed to be 0.<br/>
    /// </summary>
    GET_CORE_OPTIONS_VERSION = 52,

    /// <summary>
    /// const struct retro_core_option_definition ** --<br/>
    /// Allows an implementation to signal the environment<br/>
    /// which variables it might want to check for later using<br/>
    /// GET_VARIABLE.<br/>
    /// This allows the frontend to present these variables to<br/>
    /// a user dynamically.<br/>
    /// This should only be called if RETRO_ENVIRONMENT_GET_CORE_OPTIONS_VERSION<br/>
    /// returns an API version of >= 1.<br/>
    /// This should be called instead of RETRO_ENVIRONMENT_SET_VARIABLES.<br/>
    /// This should be called the first time as early as<br/>
    /// possible (ideally in retro_set_environment).<br/>
    /// Afterwards it may be called again for the core to communicate<br/>
    /// updated options to the frontend, but the number of core<br/>
    /// options must not change from the number in the initial call.<br/>
    /// </summary>
    SET_CORE_OPTIONS = 53,

    /// <summary>
    /// const struct retro_core_options_intl * --<br/>
    /// Allows an implementation to signal the environment<br/>
    /// which variables it might want to check for later using<br/>
    /// GET_VARIABLE.<br/>
    /// This allows the frontend to present these variables to<br/>
    /// a user dynamically.<br/>
    /// This should only be called if RETRO_ENVIRONMENT_GET_CORE_OPTIONS_VERSION<br/>
    /// returns an API version of >= 1.<br/>
    /// This should be called instead of RETRO_ENVIRONMENT_SET_VARIABLES.<br/>
    /// This should be called instead of RETRO_ENVIRONMENT_SET_CORE_OPTIONS.<br/>
    /// This should be called the first time as early as<br/>
    /// possible (ideally in retro_set_environment).<br/>
    /// Afterwards it may be called again for the core to communicate<br/>
    /// updated options to the frontend, but the number of core<br/>
    /// options must not change from the number in the initial call.<br/>
    /// </summary>
    SET_CORE_OPTIONS_INTL = 54,

    /// <summary>
    /// struct retro_core_option_display * --<br/>
    /// </summary>
    SET_CORE_OPTIONS_DISPLAY = 55,

    /// <summary>
    /// unsigned * --<br/>
    /// </summary>
    GET_PREFERRED_HW_RENDER = 56,

    /// <summary>
    /// unsigned * --<br/>
    /// Unsigned value is the API version number of the disk control<br/>
    /// interface supported by the frontend. If callback return false,<br/>
    /// API version is assumed to be 0.<br/>
    /// </summary>
    GET_DISK_CONTROL_INTERFACE_VERSION = 57,

    /// <summary>
    /// const struct retro_disk_control_ext_callback * --<br/>
    /// Sets an interface which frontend can use to eject and insert<br/>
    /// disk images, and also obtain information about individual<br/>
    /// disk image files registered by the core.<br/>
    /// This is used for games which consist of multiple images and<br/>
    /// must be manually swapped out by the user (e.g. PSX, floppy disk<br/>
    /// based systems).<br/>
    /// </summary>
    SET_DISK_CONTROL_EXT_INTERFACE = 58,

    /// <summary>
    /// unsigned * --<br/>
    /// Unsigned value is the API version number of the message<br/>
    /// interface supported by the frontend. If callback returns<br/>
    /// false, API version is assumed to be 0.<br/>
    /// </summary>
    GET_MESSAGE_INTERFACE_VERSION = 59,

    /// <summary>
    /// const struct retro_message_ext * --<br/>
    /// Sets a message to be displayed in an implementation-specific<br/>
    /// manner for a certain amount of 'frames'. Additionally allows<br/>
    /// the core to specify message logging level, priority and<br/>
    /// destination (OSD, logging interface or both).<br/>
    /// Should not be used for trivial messages, which should simply be<br/>
    /// logged via RETRO_ENVIRONMENT_GET_LOG_INTERFACE (or as a<br/>
    /// fallback, stderr).<br/>
    /// </summary>
    SET_MESSAGE_EXT = 60,

    /// <summary>
    /// unsigned * --<br/>
    /// Unsigned value is the number of active input devices<br/>
    /// provided by the frontend. This may change between<br/>
    /// frames, but will remain constant for the duration<br/>
    /// of each frame.<br/>
    /// If callback returns true, a core need not poll any<br/>
    /// input device with an index greater than or equal to<br/>
    /// the number of active devices.<br/>
    /// If callback returns false, the number of active input<br/>
    /// devices is unknown. In this case, all input devices<br/>
    /// should be considered active.<br/>
    /// </summary>
    GET_INPUT_MAX_USERS = 61,

    /// <summary>
    /// const struct retro_audio_buffer_status_callback * --<br/>
    /// Lets the core know the occupancy level of the frontend<br/>
    /// audio buffer. Can be used by a core to attempt frame<br/>
    /// skipping in order to avoid buffer under-runs.<br/>
    /// A core may pass NULL to disable buffer status reporting<br/>
    /// in the frontend.<br/>
    /// </summary>
    SET_AUDIO_BUFFER_STATUS_CALLBACK = 62,

    /// <summary>
    /// const unsigned * --<br/>
    /// Sets minimum frontend audio latency in milliseconds.<br/>
    /// Resultant audio latency may be larger than set value,<br/>
    /// or smaller if a hardware limit is encountered. A frontend<br/>
    /// is expected to honour requests up to 512 ms.<br/>
    /// </summary>
    SET_MINIMUM_AUDIO_LATENCY = 63,

    /// <summary>
    /// const struct retro_fastforwarding_override * --<br/>
    /// Used by a libretro core to override the current<br/>
    /// fastforwarding mode of the frontend.<br/>
    /// If NULL is passed to this function, the frontend<br/>
    /// will return true if fastforwarding override<br/>
    /// functionality is supported (no change in<br/>
    /// fastforwarding state will occur in this case).<br/>
    /// </summary>
    SET_FASTFORWARDING_OVERRIDE = 64,

    /// <summary>
    /// const struct retro_system_content_info_override * --<br/>
    /// Allows an implementation to override 'global' content<br/>
    /// info parameters reported by retro_get_system_info().<br/>
    /// Overrides also affect subsystem content info parameters<br/>
    /// set via RETRO_ENVIRONMENT_SET_SUBSYSTEM_INFO.<br/>
    /// This function must be called inside retro_set_environment().<br/>
    /// If callback returns false, content info overrides<br/>
    /// are unsupported by the frontend, and will be ignored.<br/>
    /// If callback returns true, extended game info may be<br/>
    /// retrieved by calling RETRO_ENVIRONMENT_GET_GAME_INFO_EXT<br/>
    /// in retro_load_game() or retro_load_game_special().<br/>
    /// </summary>
    SET_CONTENT_INFO_OVERRIDE = 65,

    /// <summary>
    /// const struct retro_game_info_ext ** --<br/>
    /// Allows an implementation to fetch extended game<br/>
    /// information, providing additional content path<br/>
    /// and memory buffer status details.<br/>
    /// This function may only be called inside<br/>
    /// retro_load_game() or retro_load_game_special().<br/>
    /// If callback returns false, extended game information<br/>
    /// is unsupported by the frontend. In this case, only<br/>
    /// regular retro_game_info will be available.<br/>
    /// RETRO_ENVIRONMENT_GET_GAME_INFO_EXT is guaranteed<br/>
    /// to return true if RETRO_ENVIRONMENT_SET_CONTENT_INFO_OVERRIDE<br/>
    /// returns true.<br/>
    /// </summary>
    GET_GAME_INFO_EXT = 66,

    /// <summary>
    /// const struct retro_core_options_v2 * --<br/>
    /// Allows an implementation to signal the environment<br/>
    /// which variables it might want to check for later using<br/>
    /// GET_VARIABLE.<br/>
    /// This allows the frontend to present these variables to<br/>
    /// a user dynamically.<br/>
    /// This should only be called if RETRO_ENVIRONMENT_GET_CORE_OPTIONS_VERSION<br/>
    /// returns an API version of >= 2.<br/>
    /// This should be called instead of RETRO_ENVIRONMENT_SET_VARIABLES.<br/>
    /// This should be called instead of RETRO_ENVIRONMENT_SET_CORE_OPTIONS.<br/>
    /// This should be called the first time as early as<br/>
    /// possible (ideally in retro_set_environment).<br/>
    /// Afterwards it may be called again for the core to communicate<br/>
    /// updated options to the frontend, but the number of core<br/>
    /// options must not change from the number in the initial call.<br/>
    /// If RETRO_ENVIRONMENT_GET_CORE_OPTIONS_VERSION returns an API<br/>
    /// version of >= 2, this callback is guaranteed to succeed<br/>
    /// (i.e. callback return value does not indicate success)<br/>
    /// If callback returns true, frontend has core option category<br/>
    /// support.<br/>
    /// If callback returns false, frontend does not have core option<br/>
    /// category support.<br/>
    /// </summary>
    SET_CORE_OPTIONS_V2 = 67,

    /// <summary>
    /// const struct retro_core_options_v2_intl * --<br/>
    /// Allows an implementation to signal the environment<br/>
    /// which variables it might want to check for later using<br/>
    /// GET_VARIABLE.<br/>
    /// This allows the frontend to present these variables to<br/>
    /// a user dynamically.<br/>
    /// This should only be called if RETRO_ENVIRONMENT_GET_CORE_OPTIONS_VERSION<br/>
    /// returns an API version of >= 2.<br/>
    /// This should be called instead of RETRO_ENVIRONMENT_SET_VARIABLES.<br/>
    /// This should be called instead of RETRO_ENVIRONMENT_SET_CORE_OPTIONS.<br/>
    /// This should be called instead of RETRO_ENVIRONMENT_SET_CORE_OPTIONS_INTL.<br/>
    /// This should be called instead of RETRO_ENVIRONMENT_SET_CORE_OPTIONS_V2.<br/>
    /// This should be called the first time as early as<br/>
    /// possible (ideally in retro_set_environment).<br/>
    /// Afterwards it may be called again for the core to communicate<br/>
    /// updated options to the frontend, but the number of core<br/>
    /// options must not change from the number in the initial call.<br/>
    /// If RETRO_ENVIRONMENT_GET_CORE_OPTIONS_VERSION returns an API<br/>
    /// version of >= 2, this callback is guaranteed to succeed<br/>
    /// (i.e. callback return value does not indicate success)<br/>
    /// If callback returns true, frontend has core option category<br/>
    /// support.<br/>
    /// If callback returns false, frontend does not have core option<br/>
    /// category support.<br/>
    /// </summary>
    SET_CORE_OPTIONS_V2_INTL = 68,

    /// <summary>
    /// const struct retro_core_options_update_display_callback * --<br/>
    /// Allows a frontend to signal that a core must update<br/>
    /// the visibility of any dynamically hidden core options,<br/>
    /// and enables the frontend to detect visibility changes.<br/>
    /// Used by the frontend to update the menu display status<br/>
    /// of core options without requiring a call of retro_run().<br/>
    /// Must be called in retro_set_environment().<br/>
    /// </summary>
    SET_CORE_OPTIONS_UPDATE_DISPLAY_CALLBACK = 69,

    /// <summary>
    /// const struct retro_variable * --<br/>
    /// Allows an implementation to notify the frontend<br/>
    /// that a core option value has changed.<br/>
    /// </summary>
    SET_VARIABLE = 70,

    /// <summary>
    /// struct retro_throttle_state * --<br/>
    /// Allows an implementation to get details on the actual rate<br/>
    /// the frontend is attempting to call retro_run().<br/>
    /// </summary>
    GET_THROTTLE_STATE = 71 | RETRO_ENVIRONMENT_EXPERIMENTAL,

    /// <summary>
    /// int * --<br/>
    /// Tells the core about the context the frontend is asking for savestate.<br/>
    /// (see enum retro_savestate_context)<br/>
    /// </summary>
    GET_SAVESTATE_CONTEXT = 72 | RETRO_ENVIRONMENT_EXPERIMENTAL,

    /// <summary>
    /// struct retro_hw_render_context_negotiation_interface * --<br/>
    /// Before calling SET_HW_RNEDER_CONTEXT_NEGOTIATION_INTERFACE, a core can query<br/>
    /// which version of the interface is supported.<br/>
    /// </summary>
    GET_HW_RENDER_CONTEXT_NEGOTIATION_INTERFACE_SUPPORT = 73 | RETRO_ENVIRONMENT_EXPERIMENTAL,

    /// <summary>
    /// bool * --<br/>
    /// Result is set to true if the frontend has already verified JIT can be<br/>
    /// used, mainly for use iOS<br/>
    /// </summary>
    GET_JIT_CAPABLE = 74,

    RETRO_ENVIRONMENT_EXPERIMENTAL = 0x10000
  };

  [Flags]
  public enum AUDIO_VIDEO_ENABLE
  {
    ENABLE_VIDEO = 1,
    ENABLE_AUDIO = 2,
    USE_FAST_SAVESTATES = 4,
    HARD_DISABLE_AUDIO = 8
  }

  public enum retro_hw_context_type
  {
    RETRO_HW_CONTEXT_NONE = 0,
    RETRO_HW_CONTEXT_OPENGL = 1,
    RETRO_HW_CONTEXT_OPENGLES2 = 2,
    RETRO_HW_CONTEXT_OPENGL_CORE = 3,
    RETRO_HW_CONTEXT_OPENGLES3 = 4,
    RETRO_HW_CONTEXT_OPENGLES_VERSION = 5,

    RETRO_HW_CONTEXT_DUMMY = Int32.MaxValue
  };

  [Flags]
  public enum RETRO_SERIALIZATION_QUIRK
  {
    /// <summary>
    /// Serialized state is incomplete in some way. Set if serialization is
    /// usable in typical end-user cases but should not be relied upon to
    /// implement frame-sensitive frontend features such as netplay or
    /// rerecording. 
    ///</summary>
    INCOMPLETE = (1 << 0),

    /// <summary>
    /// The core must spend some time initializing before serialization is
    /// supported. retro_serialize() will initially fail; retro_unserialize()
    /// and retro_serialize_size() may or may not work correctly either.
    /// </summary>
    MUST_INITIALIZE = (1 << 1),

    /// <summary>
    /// Serialization size may change within a session.
    /// </summary>
    CORE_VARIABLE_SIZE = (1 << 2),

    /// <summary>
    /// Set by the frontend to acknowledge that it supports variable-sized
    /// states.
    /// </summary>
    FRONT_VARIABLE_SIZE = (1 << 3),

    /// <summary>
    /// Serialized state can only be loaded during the same session.
    /// </summary>
    SINGLE_SESSION = (1 << 4),

    /// <summary>
    /// Serialized state cannot be loaded on an architecture with a different
    /// endianness from the one it was saved on.
    /// </summary>
    ENDIAN_DEPENDENT = (1 << 5),
    
    /// <summary>
    /// Serialized state cannot be loaded on a different platform from the one it
    /// was saved on for reasons other than endianness, such as word size
    /// dependence
    /// </summary>
    PLATFORM_DEPENDENT = (1 << 6),
  }

  [StructLayout(LayoutKind.Sequential)]
  public struct retro_hw_render_callback
  {
    public const int RETRO_HW_FRAME_BUFFER_VALID = -1;

    public uint context_type; //retro_hw_context_type
    public IntPtr context_reset; //retro_hw_context_reset_t
    public IntPtr get_current_framebuffer; //retro_hw_get_current_framebuffer_t
    public IntPtr get_proc_address; //retro_hw_get_proc_address_t
    [MarshalAs(UnmanagedType.U1)]
    public bool depth;
    [MarshalAs(UnmanagedType.U1)]
    public bool stencil;
    [MarshalAs(UnmanagedType.U1)]
    public bool bottom_left_origin;
    public uint version_major;
    public uint version_minor;
    [MarshalAs(UnmanagedType.U1)]
    public bool cache_context;
    public IntPtr context_destroy; //retro_hw_context_reset_t
    [MarshalAs(UnmanagedType.U1)]
    public bool debug_context;
  };

  [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
  public delegate void retro_hw_context_reset_t();

  [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
  public delegate uint retro_hw_get_current_framebuffer_t();

  //not used
  [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
  public delegate void retro_proc_address_t();

  [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
  public delegate IntPtr retro_hw_get_proc_address_t(IntPtr sym);

  [StructLayout(LayoutKind.Sequential)]
  struct retro_memory_map
  {
    public IntPtr descriptors; //retro_memory_descriptor *
    public uint num_descriptors;
  };

  [StructLayout(LayoutKind.Sequential)]
  struct retro_memory_descriptor
  {
    ulong flags;
    IntPtr ptr;
    IntPtr offset; //size_t
    IntPtr start; //size_t
    IntPtr select; //size_t
    IntPtr disconnect; //size_t
    IntPtr len; //size_t
    IntPtr addrspace;
  };

  public enum RETRO_PIXEL_FORMAT
  {
    XRGB1555 = 0,
    XRGB8888 = 1,
    RGB565 = 2
  };

  [StructLayout(LayoutKind.Sequential)]
  public struct retro_message
  {
    public string msg;
    public uint frames;
  }

  [StructLayout(LayoutKind.Sequential)]
  public struct retro_input_descriptor
  {
    public uint port;
    public uint device;
    public uint index;
    public uint id;
  }

  [StructLayout(LayoutKind.Sequential)]
  public struct retro_system_info
  {
    public IntPtr library_name;
    public IntPtr library_version;
    public IntPtr valid_extensions;
    [MarshalAs(UnmanagedType.U1)]
    public bool need_fullpath;
    [MarshalAs(UnmanagedType.U1)]
    public bool block_extract;
  }

  [StructLayout(LayoutKind.Sequential)]
  public struct retro_game_geometry
  {
    public uint base_width;
    public uint base_height;
    public uint max_width;
    public uint max_height;
    public float aspect_ratio;
  }

  [StructLayout(LayoutKind.Sequential)]
  public struct retro_system_timing
  {
    public double fps;
    public double sample_rate;
  }

  [StructLayout(LayoutKind.Sequential)]
  public struct retro_system_av_info
  {
    public retro_game_geometry geometry;
    public retro_system_timing timing;
  }

  [StructLayout(LayoutKind.Sequential)]
  public struct retro_variable
  {
    public string key;
    public string value;
  }

  [StructLayout(LayoutKind.Sequential)]
  public struct retro_game_info
  {
    public string path;
    public IntPtr data;
    public uint size;
    public string meta;
  }

  //untested
  [StructLayout(LayoutKind.Sequential)]
  public struct retro_perf_counter
  {
    public string ident;
    public ulong start;
    public ulong total;
    public ulong call_cnt;

    [MarshalAs(UnmanagedType.U1)]
    public bool registered;
  };

  //perf callbacks
  [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
  public delegate long retro_perf_get_time_usec_t();
  [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
  public delegate long retro_perf_get_counter_t();
  [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
  public delegate ulong retro_get_cpu_features_t();
  [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
  public delegate void retro_perf_log_t();
  [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
  public delegate void retro_perf_register_t(ref retro_perf_counter counter);
  [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
  public delegate void retro_perf_start_t(ref retro_perf_counter counter);
  [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
  public delegate void retro_perf_stop_t(ref retro_perf_counter counter);

  //for GET_PERF_INTERFACE
  [StructLayout(LayoutKind.Sequential)]
  public struct retro_perf_callback
  {
    public retro_perf_get_time_usec_t get_time_usec;
    public retro_get_cpu_features_t get_cpu_features;
    public retro_perf_get_counter_t get_perf_counter;
    public retro_perf_register_t perf_register;
    public retro_perf_start_t perf_start;
    public retro_perf_stop_t perf_stop;
    public retro_perf_log_t perf_log;
  }

  //Rumble interface
  public enum retro_rumble_effect
  {
    RETRO_RUMBLE_STRONG = 0,
    RETRO_RUMBLE_WEAK = 1,

    RETRO_RUMBLE_DUMMY = int.MaxValue
  };

  [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
  [return: MarshalAs(UnmanagedType.U1)]
  public delegate bool retro_set_rumble_state_t(uint port, retro_rumble_effect effect, ushort strength);

  [StructLayout(LayoutKind.Sequential)]
  public struct retro_rumble_interface
  {
    public retro_set_rumble_state_t set_rumble_state;
  }

  [StructLayout(LayoutKind.Sequential)]
  public struct retro_subsystem_memory_info
  {
    /* The extension associated with a memory type, e.g. "psram". */
    public string extension;

    /* The memory type for retro_get_memory(). This should be at 
     * least 0x100 to avoid conflict with standardized 
     * libretro memory types. */
    uint type;
  };

  [StructLayout(LayoutKind.Sequential)]
  public struct retro_subsystem_rom_info
  {
    /* Describes what the content is (SGB BIOS, GB ROM, etc). */
    public string desc; //const char *

    /* Same definition as retro_get_system_info(). */
    public string valid_extensions; //const char *

    /* Same definition as retro_get_system_info(). */
    [MarshalAs(UnmanagedType.U1)]
    public bool need_fullpath;

    /* Same definition as retro_get_system_info(). */
    [MarshalAs(UnmanagedType.U1)]
    public bool block_extract;

    /* This is set if the content is required to load a game. 
     * If this is set to false, a zeroed-out retro_game_info can be passed. */
    [MarshalAs(UnmanagedType.U1)]
    public bool required;

    /* Content can have multiple associated persistent 
     * memory types (retro_get_memory()). */
    public IntPtr memory; //retro_subsystem_memory_info *
    public uint num_memory;
  };

  [StructLayout(LayoutKind.Sequential)]
  public struct retro_subsystem_info
  {
    /* Human-readable string of the subsystem type, e.g. "Super GameBoy" */
    public string desc;

    /* A computer friendly short string identifier for the subsystem type.
     * This name must be [a-z].
     * E.g. if desc is "Super GameBoy", this can be "sgb".
     * This identifier can be used for command-line interfaces, etc.
     */
    public string ident;

    /* Infos for each content file. The first entry is assumed to be the 
     * "most significant" content for frontend purposes.
     * E.g. with Super GameBoy, the first content should be the GameBoy ROM, 
     * as it is the most "significant" content to a user.
     * If a frontend creates new file paths based on the content used 
     * (e.g. savestates), it should use the path for the first ROM to do so. */
    public IntPtr roms; //retro_subsystem_rom_info*

    /* Number of content files associated with a subsystem. */
    public uint num_roms;

    /* The type passed to retro_load_game_special(). */
    public uint id;
  };

  #region callback prototypes
  [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
  public delegate void retro_log_printf_t(RETRO_LOG_LEVEL level, string fmt, IntPtr args);
  [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
  [return: MarshalAs(UnmanagedType.U1)]
  public delegate bool retro_environment_t(RETRO_ENVIRONMENT cmd, IntPtr data);
  [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
  public delegate void retro_video_refresh_t(IntPtr data, uint width, uint height, uint pitch);
  [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
  public delegate void retro_audio_sample_t(short left, short right);
  [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
  public delegate uint retro_audio_sample_batch_t(IntPtr data, uint frames);
  [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
  public delegate void retro_input_poll_t();
  [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
  public delegate short retro_input_state_t(uint port, uint device, uint index, uint id);
  #endregion

  #region entry point prototypes
  [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
  public delegate void epretro_set_environment(retro_environment_t cb);
  [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
  public delegate void epretro_set_video_refresh(retro_video_refresh_t cb);
  [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
  public delegate void epretro_set_audio_sample(retro_audio_sample_t cb);
  [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
  public delegate void epretro_set_audio_sample_batch(retro_audio_sample_batch_t cb);
  [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
  public delegate void epretro_set_input_poll(retro_input_poll_t cb);
  [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
  public delegate void epretro_set_input_state(retro_input_state_t cb);
  [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
  public delegate void epretro_init();
  [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
  public delegate void epretro_deinit();
  [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
  public delegate uint epretro_api_version();
  [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
  public delegate void epretro_get_system_info(ref retro_system_info info);
  [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
  public delegate void epretro_get_system_av_info(ref retro_system_av_info info);
  [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
  public delegate void epretro_set_controller_port_device(uint port, uint device);
  [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
  public delegate void epretro_reset();
  [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
  public delegate void epretro_run();
  [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
  public delegate uint epretro_serialize_size();
  [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
  [return: MarshalAs(UnmanagedType.U1)]
  public delegate bool epretro_serialize(IntPtr data, uint size);
  [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
  [return: MarshalAs(UnmanagedType.U1)]
  public delegate bool epretro_unserialize(IntPtr data, uint size);
  [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
  public delegate void epretro_cheat_reset();
  [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
  public delegate void epretro_cheat_set(uint index, [MarshalAs(UnmanagedType.U1)]bool enabled, string code);
  [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
  [return: MarshalAs(UnmanagedType.U1)]
  public delegate bool epretro_load_game(ref retro_game_info game);
  [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
  [return: MarshalAs(UnmanagedType.U1)]
  public delegate bool epretro_load_game_special(uint game_type, ref retro_game_info info, uint num_info);
  [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
  public delegate void epretro_unload_game();
  [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
  public delegate uint epretro_get_region();
  [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
  public delegate IntPtr epretro_get_memory_data(RETRO_MEMORY id);
  [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
  public delegate uint epretro_get_memory_size(RETRO_MEMORY id);
  #endregion
}
