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
    SET_ROTATION = 1,
    GET_OVERSCAN = 2,
    GET_CAN_DUPE = 3,
    SET_MESSAGE = 6,
    SHUTDOWN = 7,
    SET_PERFORMANCE_LEVEL = 8,
    GET_SYSTEM_DIRECTORY = 9,
    SET_PIXEL_FORMAT = 10,
    SET_INPUT_DESCRIPTORS = 11,
    SET_KEYBOARD_CALLBACK = 12,
    SET_DISK_CONTROL_INTERFACE = 13,
    SET_HW_RENDER = 14,
    GET_VARIABLE = 15,
    SET_VARIABLES = 16,
    GET_VARIABLE_UPDATE = 17,
    SET_SUPPORT_NO_GAME = 18,
    GET_LIBRETRO_PATH = 19,
    SET_AUDIO_CALLBACK = 22,
    SET_FRAME_TIME_CALLBACK = 21,
    GET_RUMBLE_INTERFACE = 23,
    GET_INPUT_DEVICE_CAPABILITIES = 24,
    //25,26 are experimental
    GET_LOG_INTERFACE = 27,
    GET_PERF_INTERFACE = 28,
    GET_LOCATION_INTERFACE = 29,
    GET_CORE_ASSETS_DIRECTORY = 30,
    GET_SAVE_DIRECTORY = 31,
    SET_SYSTEM_AV_INFO = 32,
    SET_PROC_ADDRESS_CALLBACK = 33,
    SET_SUBSYSTEM_INFO = 34,
    SET_CONTROLLER_INFO = 35,
    SET_MEMORY_MAPS = 36 | RETRO_ENVIRONMENT_EXPERIMENTAL,
    SET_GEOMETRY = 37,
    GET_USERNAME = 38,
    GET_LANGUAGE = 39,

    /// <summary>
    /// uint64_t * --
    /// Sets quirk flags associated with serialization. The frontend will zero any flags it doesn't
    /// recognize or support. Should be set in either retro_init or retro_load_game, but not both.
    /// </summary>
    SET_SERIALIZATION_QUIRKS = 44,

    /// <summary>
    /// * N/A (null) * --
    /// The frontend will try to use a 'shared' hardware context (mostly applicable
    /// to OpenGL) when a hardware context is being set up.
    ///
    /// Returns true if the frontend supports shared hardware contexts and false
    /// if the frontend does not support shared hardware contexts.
    ///
    /// This will do nothing on its own until SET_HW_RENDER env callbacks are
    /// being used.
    /// </summary>
    SET_HW_SHARED_CONTEXT = 44 | RETRO_ENVIRONMENT_EXPERIMENTAL,

    /// <summary>
    /// int * --
    /// Tells the core if the frontend wants audio or video.
    /// If disabled, the frontend will discard the audio or video,
    /// so the core may decide to skip generating a frame or generating audio.
    /// This is mainly used for increasing performance.
    /// Bit 0 (value 1): Enable Video
    /// Bit 1 (value 2): Enable Audio
    /// Bit 2 (value 4): Use Fast Savestates.
    /// Bit 3 (value 8): Hard Disable Audio
    /// Other bits are reserved for future use and will default to zero.
    /// If video is disabled:
    /// * The frontend wants the core to not generate any video,
    ///   including presenting frames via hardware acceleration.
    /// * The frontend's video frame callback will do nothing.
    /// * After running the frame, the video output of the next frame should be
    ///   no different than if video was enabled, and saving and loading state
    ///   should have no issues.
    /// If audio is disabled:
    /// * The frontend wants the core to not generate any audio.
    /// * The frontend's audio callbacks will do nothing.
    /// * After running the frame, the audio output of the next frame should be
    ///   no different than if audio was enabled, and saving and loading state
    ///   should have no issues.
    /// Fast Savestates:
    /// * Guaranteed to be created by the same binary that will load them.
    /// * Will not be written to or read from the disk.
    /// * Suggest that the core assumes loading state will succeed.
    /// * Suggest that the core updates its memory buffers in-place if possible.
    /// * Suggest that the core skips clearing memory.
    /// * Suggest that the core skips resetting the system.
    /// * Suggest that the core may skip validation steps.
    /// Hard Disable Audio:
    /// * Used for a secondary core when running ahead.
    /// * Indicates that the frontend will never need audio from the core.
    /// * Suggests that the core may stop synthesizing audio, but this should not
    ///   compromise emulation accuracy.
    /// * Audio output for the next frame does not matter, and the frontend will
    ///   never need an accurate audio state in the future.
    /// * State will never be saved when using Hard Disable Audio.
    /// </summary>
    RETRO_ENVIRONMENT_GET_AUDIO_VIDEO_ENABLE = 47 | RETRO_ENVIRONMENT_EXPERIMENTAL,

    /// <summary>
    /// Unsigned value is the API version number of the core options
    /// interface supported by the frontend.If callback return false,
    /// API version is assumed to be 0.
    ///
    /// In legacy code, core options are set by passing an array of
    /// retro_variable structs to RETRO_ENVIRONMENT_SET_VARIABLES.
    /// This may be still be done regardless of the core options
    /// interface version.
    ///
    /// If version is >= 1 however, core options may instead be set by
    /// passing an array of retro_core_option_definition structs to
    /// RETRO_ENVIRONMENT_SET_CORE_OPTIONS, or a 2D array of
    /// retro_core_option_definition structs to RETRO_ENVIRONMENT_SET_CORE_OPTIONS_INTL.
    /// This allows the core to additionally set option sublabel information
    /// and/or provide localisation support.
    ///
    /// If version is >= 2, core options may instead be set by passing
    /// a retro_core_options_v2 struct to RETRO_ENVIRONMENT_SET_CORE_OPTIONS_V2,
    /// or an array of retro_core_options_v2 structs to
    /// RETRO_ENVIRONMENT_SET_CORE_OPTIONS_V2_INTL.This allows the core
    /// to additionally set optional core option category information
    /// for frontends with core option category support.
    /// </summary>
    RETRO_ENVIRONMENT_GET_CORE_OPTIONS_VERSION = 52,

    ///<summary>
    /// const struct retro_core_option_definition ** --
    /// Allows an implementation to signal the environment
    /// which variables it might want to check for later using
    /// GET_VARIABLE.
    /// This allows the frontend to present these variables to
    /// a user dynamically.
    /// This should only be called if RETRO_ENVIRONMENT_GET_CORE_OPTIONS_VERSION
    /// returns an API version of >= 1.
    /// This should be called instead of RETRO_ENVIRONMENT_SET_VARIABLES.
    /// This should be called the first time as early as
    /// possible (ideally in retro_set_environment).
    /// Afterwards it may be called again for the core to communicate
    /// updated options to the frontend, but the number of core
    /// options must not change from the number in the initial call.
    ///
    /// 'data' points to an array of retro_core_option_definition structs
    /// terminated by a { NULL, NULL, NULL, {{0}}, NULL } element.
    /// retro_core_option_definition::key should be namespaced to not collide
    /// with other implementations' keys. e.g. A core called
    /// 'foo' should use keys named as 'foo_option'.
    /// retro_core_option_definition::desc should contain a human readable
    /// description of the key.
    /// retro_core_option_definition::info should contain any additional human
    /// readable information text that a typical user may need to
    /// understand the functionality of the option.
    /// retro_core_option_definition::values is an array of retro_core_option_value
    /// structs terminated by a { NULL, NULL } element.
    /// > retro_core_option_definition::values[index].value is an expected option
    ///   value.
    /// > retro_core_option_definition::values[index].label is a human readable
    ///   label used when displaying the value on screen. If NULL,
    ///   the value itself is used.
    /// retro_core_option_definition::default_value is the default core option
    /// setting. It must match one of the expected option values in the
    /// retro_core_option_definition::values array. If it does not, or the
    /// default value is NULL, the first entry in the
    /// retro_core_option_definition::values array is treated as the default.
    ///
    /// The number of possible option values should be very limited,
    /// and must be less than RETRO_NUM_CORE_OPTION_VALUES_MAX.
    /// i.e. it should be feasible to cycle through options
    /// without a keyboard.
    ///
    /// Example entry:
    /// {
    ///     "foo_option",
    ///     "Speed hack coprocessor X",
    ///     "Provides increased performance at the expense of reduced accuracy",
    /// 	  {
    ///         { "false",    NULL },
    ///         { "true",     NULL },
    ///         { "unstable", "Turbo (Unstable)" },
    ///         { NULL, NULL },
    ///     },
    ///     "false"
    /// }
    ///
    /// Only strings are operated on. The possible values will
    /// generally be displayed and stored as-is by the frontend.
    /// </summary>
    RETRO_ENVIRONMENT_SET_CORE_OPTIONS = 53,

    /// <summary>
    /// const struct retro_core_options_intl * --
    /// Allows an implementation to signal the environment
    /// which variables it might want to check for later using
    /// GET_VARIABLE.
    /// This allows the frontend to present these variables to
    /// a user dynamically.
    /// This should only be called if RETRO_ENVIRONMENT_GET_CORE_OPTIONS_VERSION
    /// returns an API version of >= 1.
    /// This should be called instead of RETRO_ENVIRONMENT_SET_VARIABLES.
    /// This should be called instead of RETRO_ENVIRONMENT_SET_CORE_OPTIONS.
    /// This should be called the first time as early as
    /// possible (ideally in retro_set_environment).
    /// Afterwards it may be called again for the core to communicate
    /// updated options to the frontend, but the number of core
    /// options must not change from the number in the initial call.
    ///
    /// This is fundamentally the same as RETRO_ENVIRONMENT_SET_CORE_OPTIONS,
    /// with the addition of localisation support. The description of the
    /// RETRO_ENVIRONMENT_SET_CORE_OPTIONS callback should be consulted
    /// for further details.
    ///
    /// 'data' points to a retro_core_options_intl struct.
    ///
    /// retro_core_options_intl::us is a pointer to an array of
    /// retro_core_option_definition structs defining the US English
    /// core options implementation. It must point to a valid array.
    ///
    /// retro_core_options_intl::local is a pointer to an array of
    /// retro_core_option_definition structs defining core options for
    /// the current frontend language. It may be NULL (in which case
    /// retro_core_options_intl::us is used by the frontend). Any items
    /// missing from this array will be read from retro_core_options_intl::us
    /// instead.
    ///
    /// NOTE: Default core option values are always taken from the
    /// retro_core_options_intl::us array. Any default values in
    /// retro_core_options_intl::local array will be ignored.
    /// </summary>
    RETRO_ENVIRONMENT_SET_CORE_OPTIONS_INTL = 54,

    /// <summary>
    /// struct retro_core_option_display * --
    /// Allows an implementation to signal the environment to show
    /// or hide a variable when displaying core options. This is
    /// considered a *suggestion*. The frontend is free to ignore
    /// this callback, and its implementation not considered mandatory.
    ///
    /// 'data' points to a retro_core_option_display struct
    ///
    /// retro_core_option_display::key is a variable identifier
    /// which has already been set by SET_VARIABLES/SET_CORE_OPTIONS.
    ///
    /// retro_core_option_display::visible is a boolean, specifying
    /// whether variable should be displayed
    ///
    /// Note that all core option variables will be set visible by
    /// default when calling SET_VARIABLES/SET_CORE_OPTIONS.
    /// </summary>
    RETRO_ENVIRONMENT_SET_CORE_OPTIONS_DISPLAY = 55,

    /// <summary>
    /// const struct retro_core_options_v2 * --
    /// Allows an implementation to signal the environment
    /// which variables it might want to check for later using
    /// GET_VARIABLE.
    /// This allows the frontend to present these variables to
    /// a user dynamically.
    /// This should only be called if RETRO_ENVIRONMENT_GET_CORE_OPTIONS_VERSION
    /// returns an API version of >= 2.
    /// This should be called instead of RETRO_ENVIRONMENT_SET_VARIABLES.
    /// This should be called instead of RETRO_ENVIRONMENT_SET_CORE_OPTIONS.
    /// This should be called the first time as early as
    /// possible (ideally in retro_set_environment).
    /// Afterwards it may be called again for the core to communicate
    /// updated options to the frontend, but the number of core
    /// options must not change from the number in the initial call.
    /// If RETRO_ENVIRONMENT_GET_CORE_OPTIONS_VERSION returns an API
    /// version of >= 2, this callback is guaranteed to succeed
    /// (i.e. callback return value does not indicate success)
    /// If callback returns true, frontend has core option category
    /// support.
    /// If callback returns false, frontend does not have core option
    /// category support.
    ///
    /// 'data' points to a retro_core_options_v2 struct, containing
    /// of two pointers:
    /// - retro_core_options_v2::categories is an array of
    ///   retro_core_option_v2_category structs terminated by a
    ///   { NULL, NULL, NULL } element. If retro_core_options_v2::categories
    ///   is NULL, all core options will have no category and will be shown
    ///   at the top level of the frontend core option interface. If frontend
    ///   does not have core option category support, categories array will
    ///   be ignored.
    /// - retro_core_options_v2::definitions is an array of
    ///   retro_core_option_v2_definition structs terminated by a
    ///   { NULL, NULL, NULL, NULL, NULL, NULL, {{0}}, NULL }
    ///   element.
    ///
    /// >> retro_core_option_v2_category notes:
    ///
    /// - retro_core_option_v2_category::key should contain string
    ///   that uniquely identifies the core option category. Valid
    ///   key characters are [a-z, A-Z, 0-9, _, -]
    ///   Namespace collisions with other implementations' category
    ///   keys are permitted.
    /// - retro_core_option_v2_category::desc should contain a human
    ///   readable description of the category key.
    /// - retro_core_option_v2_category::info should contain any
    ///   additional human readable information text that a typical
    ///   user may need to understand the nature of the core option
    ///   category.
    ///
    /// Example entry:
    /// {
    ///     "advanced_settings",
    ///     "Advanced",
    ///     "Options affecting low-level emulation performance and accuracy."
    /// }
    ///
    /// >> retro_core_option_v2_definition notes:
    ///
    /// - retro_core_option_v2_definition::key should be namespaced to not
    ///   collide with other implementations' keys. e.g. A core called
    ///   'foo' should use keys named as 'foo_option'. Valid key characters
    ///   are [a-z, A-Z, 0-9, _, -].
    /// - retro_core_option_v2_definition::desc should contain a human readable
    ///   description of the key. Will be used when the frontend does not
    ///   have core option category support. Examples: "Aspect Ratio" or
    ///   "Video > Aspect Ratio".
    /// - retro_core_option_v2_definition::desc_categorized should contain a
    ///   human readable description of the key, which will be used when
    ///   frontend has core option category support. Example: "Aspect Ratio",
    ///   where associated retro_core_option_v2_category::desc is "Video".
    ///   If empty or NULL, the string specified by
    ///   retro_core_option_v2_definition::desc will be used instead.
    ///   retro_core_option_v2_definition::desc_categorized will be ignored
    ///   if retro_core_option_v2_definition::category_key is empty or NULL.
    /// - retro_core_option_v2_definition::info should contain any additional
    ///   human readable information text that a typical user may need to
    ///   understand the functionality of the option.
    /// - retro_core_option_v2_definition::info_categorized should contain
    ///   any additional human readable information text that a typical user
    ///   may need to understand the functionality of the option, and will be
    ///   used when frontend has core option category support. This is provided
    ///   to accommodate the case where info text references an option by
    ///   name/desc, and the desc/desc_categorized text for that option differ.
    ///   If empty or NULL, the string specified by
    ///   retro_core_option_v2_definition::info will be used instead.
    ///   retro_core_option_v2_definition::info_categorized will be ignored
    ///   if retro_core_option_v2_definition::category_key is empty or NULL.
    /// - retro_core_option_v2_definition::category_key should contain a
    ///   category identifier (e.g. "video" or "audio") that will be
    ///   assigned to the core option if frontend has core option category
    ///   support. A categorized option will be shown in a subsection/
    ///   submenu of the frontend core option interface. If key is empty
    ///   or NULL, or if key does not match one of the
    ///   retro_core_option_v2_category::key values in the associated
    ///   retro_core_option_v2_category array, option will have no category
    ///   and will be shown at the top level of the frontend core option
    ///   interface.
    /// - retro_core_option_v2_definition::values is an array of
    ///   retro_core_option_value structs terminated by a { NULL, NULL }
    ///   element.
    /// --> retro_core_option_v2_definition::values[index].value is an
    ///     expected option value.
    /// --> retro_core_option_v2_definition::values[index].label is a
    ///     human readable label used when displaying the value on screen.
    ///     If NULL, the value itself is used.
    /// - retro_core_option_v2_definition::default_value is the default
    ///   core option setting. It must match one of the expected option
    ///   values in the retro_core_option_v2_definition::values array. If
    ///   it does not, or the default value is NULL, the first entry in the
    ///   retro_core_option_v2_definition::values array is treated as the
    ///   default.
    ///
    /// The number of possible option values should be very limited,
    /// and must be less than RETRO_NUM_CORE_OPTION_VALUES_MAX.
    /// i.e. it should be feasible to cycle through options
    /// without a keyboard.
    ///
    /// Example entries:
    ///
    /// - Uncategorized:
    ///
    /// {
    ///     "foo_option",
    ///     "Speed hack coprocessor X",
    ///     NULL,
    ///     "Provides increased performance at the expense of reduced accuracy.",
    ///     NULL,
    ///     NULL,
    /// 	  {
    ///         { "false",    NULL },
    ///         { "true",     NULL },
    ///         { "unstable", "Turbo (Unstable)" },
    ///         { NULL, NULL },
    ///     },
    ///     "false"
    /// }
    ///
    /// - Categorized:
    ///
    /// {
    ///     "foo_option",
    ///     "Advanced > Speed hack coprocessor X",
    ///     "Speed hack coprocessor X",
    ///     "Setting 'Advanced > Speed hack coprocessor X' to 'true' or 'Turbo' provides increased performance at the expense of reduced accuracy",
    ///     "Setting 'Speed hack coprocessor X' to 'true' or 'Turbo' provides increased performance at the expense of reduced accuracy",
    ///     "advanced_settings",
    /// 	  {
    ///         { "false",    NULL },
    ///         { "true",     NULL },
    ///         { "unstable", "Turbo (Unstable)" },
    ///         { NULL, NULL },
    ///     },
    ///     "false"
    /// }
    ///
    /// Only strings are operated on. The possible values will
    /// generally be displayed and stored as-is by the frontend.
    /// </summary>
    RETRO_ENVIRONMENT_SET_CORE_OPTIONS_V2 = 67,

    /// <summary>
    /// const struct retro_core_options_v2_intl * --
    /// Allows an implementation to signal the environment
    /// which variables it might want to check for later using
    /// GET_VARIABLE.
    /// This allows the frontend to present these variables to
    /// a user dynamically.
    /// This should only be called if RETRO_ENVIRONMENT_GET_CORE_OPTIONS_VERSION
    /// returns an API version of >= 2.
    /// This should be called instead of RETRO_ENVIRONMENT_SET_VARIABLES.
    /// This should be called instead of RETRO_ENVIRONMENT_SET_CORE_OPTIONS.
    /// This should be called instead of RETRO_ENVIRONMENT_SET_CORE_OPTIONS_INTL.
    /// This should be called instead of RETRO_ENVIRONMENT_SET_CORE_OPTIONS_V2.
    /// This should be called the first time as early as
    /// possible (ideally in retro_set_environment).
    /// Afterwards it may be called again for the core to communicate
    /// updated options to the frontend, but the number of core
    /// options must not change from the number in the initial call.
    /// If RETRO_ENVIRONMENT_GET_CORE_OPTIONS_VERSION returns an API
    /// version of >= 2, this callback is guaranteed to succeed
    /// (i.e. callback return value does not indicate success)
    /// If callback returns true, frontend has core option category
    /// support.
    /// If callback returns false, frontend does not have core option
    /// category support.
    ///
    /// This is fundamentally the same as RETRO_ENVIRONMENT_SET_CORE_OPTIONS_V2,
    /// with the addition of localisation support. The description of the
    /// RETRO_ENVIRONMENT_SET_CORE_OPTIONS_V2 callback should be consulted
    /// for further details.
    ///
    /// 'data' points to a retro_core_options_v2_intl struct.
    ///
    /// - retro_core_options_v2_intl::us is a pointer to a
    ///   retro_core_options_v2 struct defining the US English
    ///   core options implementation. It must point to a valid struct.
    ///
    /// - retro_core_options_v2_intl::local is a pointer to a
    ///   retro_core_options_v2 struct defining core options for
    ///   the current frontend language. It may be NULL (in which case
    ///   retro_core_options_v2_intl::us is used by the frontend). Any items
    ///   missing from this struct will be read from
    ///   retro_core_options_v2_intl::us instead.
    ///
    /// NOTE: Default core option values are always taken from the
    /// retro_core_options_v2_intl::us struct. Any default values in
    /// the retro_core_options_v2_intl::local struct will be ignored.
    /// </summary>
    RETRO_ENVIRONMENT_SET_CORE_OPTIONS_V2_INTL = 68,

    /// <summary>
    /// const struct retro_core_options_update_display_callback * --
    /// Allows a frontend to signal that a core must update
    /// the visibility of any dynamically hidden core options,
    /// and enables the frontend to detect visibility changes.
    /// Used by the frontend to update the menu display status
    /// of core options without requiring a call of retro_run().
    /// Must be called in retro_set_environment().
    /// </summary>
    RETRO_ENVIRONMENT_SET_CORE_OPTIONS_UPDATE_DISPLAY_CALLBACK = 69,

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
    /// <summary>
    /// Variable to query in RETRO_ENVIRONMENT_GET_VARIABLE.
    /// If NULL, obtains the complete environment string if more
    /// complex parsing is necessary.
    /// The environment string is formatted as key-value pairs
    /// delimited by semicolons as so:
    /// "key1=value1;key2=value2;..."
    /// </summary>
    public string key;

    /// <summary>
    /// Value to be obtained. If key does not exist, it is set to NULL.
    /// </summary>
    public string value;
  }

  [StructLayout(LayoutKind.Sequential)]
  public struct retro_core_option_display
  {
    /// <summary>
    /// Variable to configure in RETRO_ENVIRONMENT_SET_CORE_OPTIONS_DISPLAY
    /// </summary>
    public string key;

    /// <summary>
    /// Specifies whether variable should be displayed
    /// when presenting core options to the user
    /// </summary>
    [MarshalAs(UnmanagedType.U1)]
    public bool visible;
  }

  public static class OptionsConsts
  {
    /// <summary>
    /// Maximum number of values permitted for a core option
    /// > Note: We have to set a maximum value due the limitations
    ///   of the C language - i.e. it is not possible to create an
    ///   array of structs each containing a variable sized array,
    ///   so the retro_core_option_definition values array must
    ///   have a fixed size. The size limit of 128 is a balancing
    ///   act - it needs to be large enough to support all 'sane'
    ///   core options, but setting it too large may impact low memory
    ///   platforms. In practise, if a core option has more than
    ///   128 values then the implementation is likely flawed.
    ///   To quote the above API reference:
    ///      "The number of possible options should be very limited
    ///       i.e. it should be feasible to cycle through options
    ///       without a keyboard."
    /// </summary>
    public const int RETRO_NUM_CORE_OPTION_VALUES_MAX = 128;
  }

  [StructLayout(LayoutKind.Sequential)]
  public struct retro_core_option_value
  {
    /// <summary>
    /// Expected option value
    /// </summary>
    public string value;

    /// <summary>
    /// Human-readable value label. If NULL, value itself
    /// will be displayed by the frontend
    /// </summary>
    public string label;
  }

  [StructLayout(LayoutKind.Sequential)]
  public struct retro_core_option_definition
  {
    /// <summary>
    /// Variable to query in RETRO_ENVIRONMENT_GET_VARIABLE.
    /// </summary>
    public string key;

    /// <summary>
    /// Human-readable core option description (used as menu label)
    /// </summary>
    public string desc;

    /// <summary>
    /// Human-readable core option information (used as menu sublabel)
    /// </summary>
    public string info;

    /// <summary>
    /// Array of retro_core_option_value structs, terminated by NULL
    /// </summary>
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = OptionsConsts.RETRO_NUM_CORE_OPTION_VALUES_MAX)]
    public retro_core_option_value[] values;

    /// <summary>
    /// Default core option value. Must match one of the values
    /// in the retro_core_option_value array, otherwise will be
    /// ignored
    /// </summary>
    public string default_value;
  }

  [StructLayout(LayoutKind.Sequential)]
  public struct retro_core_options_intl
  {
    /// <summary>
    /// Pointer to an array of retro_core_option_definition structs
    /// - US English implementation
    /// - Must point to a valid array
    /// </summary>
    public IntPtr us;

    /// <summary>
    /// Pointer to an array of retro_core_option_definition structs
    /// - Implementation for current frontend language
    /// - May be NULL
    /// </summary>
    public IntPtr local;
  }

  [StructLayout(LayoutKind.Sequential)]
  public struct retro_core_option_v2_category
  {
    /// <summary>
    /// Variable uniquely identifying the
    /// option category.Valid key characters
    /// are[a - z, A - Z, 0 - 9, _, -]
    /// </summary>
    public string key;

    /// <summary>
    /// Human-readable category description
    /// > Used as category menu label when
    ///   frontend has core option category
    /// support
    /// </summary>
    public string desc;

    /// <summary>
    /// Human-readable category information
    /// > Used as category menu sublabel when
    ///   frontend has core option category
    /// support
    /// > Optional(may be NULL or an empty
    ///   string)
    /// </summary>
    public string info;
  }

  [StructLayout(LayoutKind.Sequential)]
  public struct retro_core_option_v2_definition
  {
    /// <summary>
    /// Variable to query in RETRO_ENVIRONMENT_GET_VARIABLE.
    /// Valid key characters are[a - z, A - Z, 0 - 9, _, -]
    /// </summary>
    public string key;

    /// <summary>
    /// Human-readable core option description
    /// > Used as menu label when frontend does
    /// not have core option category support
    /// e.g. "Video > Aspect Ratio"
    /// </summary>
    public string desc;

    /// <summary>
    /// Human-readable core option description
    /// > Used as menu label when frontend has
    ///   core option category support
    ///   e.g. "Aspect Ratio", where associated
    ///   retro_core_option_v2_category::desc
    ///   is "Video"
    /// > If empty or NULL, the string specified by
    ///   desc will be used as the menu label
    /// > Will be ignored (and may be set to NULL)
    ///   if category_key is empty or NULL
    /// </summary>
    public string desc_categorized;

    /// <summary>
    /// Human-readable core option information
    /// > Used as menu sublabel
    /// </summary>
    public string info;

    /// <summary>
    /// Human-readable core option information
    /// > Used as menu sublabel when frontend
    ///   has core option category support
    ///   (e.g. may be required when info text
    ///   references an option by name/desc,
    ///   and the desc/desc_categorized text
    ///   for that option differ)
    /// > If empty or NULL, the string specified by
    ///   info will be used as the menu sublabel
    /// > Will be ignored (and may be set to NULL)
    ///   if category_key is empty or NULL
    /// </summary>
    public string info_categorized;

    /// <summary>
    /// Variable specifying category (e.g. "video",
    /// "audio") that will be assigned to the option
    /// if frontend has core option category support.
    /// > Categorized options will be displayed in a
    ///   subsection/submenu of the frontend core
    ///   option interface
    /// > Specified string must match one of the
    ///   retro_core_option_v2_category::key values
    ///   in the associated retro_core_option_v2_category
    ///   array; If no match is not found, specified
    ///   string will be considered as NULL
    /// > If specified string is empty or NULL, option will
    ///   have no category and will be shown at the top
    ///   level of the frontend core option interface
    /// </summary>
    public string category_key;

    /// <summary>
    /// Array of retro_core_option_value structs, terminated by NULL
    /// </summary>
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = OptionsConsts.RETRO_NUM_CORE_OPTION_VALUES_MAX)]
    public retro_core_option_value[] values;

    /// <summary>
    /// Default core option value. Must match one of the values
    /// in the retro_core_option_value array, otherwise will be
    /// ignored
    /// </summary>
    public string default_value;
  }

  [StructLayout(LayoutKind.Sequential)]
  public struct retro_core_options_v2
  {
    /// <summary>
    /// Array of retro_core_option_v2_category structs,
    /// terminated by NULL
    /// > If NULL, all entries in definitions array
    ///   will have no category and will be shown at
    ///   the top level of the frontend core option
    ///   interface
    /// > Will be ignored if frontend does not have
    ///   core option category support
    /// </summary>
    public IntPtr categories;

    /// <summary>
    /// Array of retro_core_option_v2_definition structs,
    /// terminated by NULL
    /// </summary>
    public IntPtr definitions;
  }

  [StructLayout(LayoutKind.Sequential)]
  public struct retro_core_options_v2_intl
  {
    /// <summary>
    /// Pointer to a retro_core_options_v2 struct
    /// > US English implementation
    /// > Must point to a valid struct
    /// </summary>
    public IntPtr us;

    /// <summary>
    /// Pointer to a retro_core_options_v2 struct
    /// - Implementation for current frontend language
    /// - May be NULL
    /// </summary>
    public IntPtr local;
  }

  /// <summary>
  /// Used by the frontend to monitor changes in core option
  /// visibility. May be called each time any core option
  /// value is set via the frontend.
  /// - On each invocation, the core must update the visibility
  ///   of any dynamically hidden options using the
  ///   RETRO_ENVIRONMENT_SET_CORE_OPTIONS_DISPLAY environment
  ///   callback.
  /// - On the first invocation, returns 'true' if the visibility
  ///   of any core option has changed since the last call of
  ///   retro_load_game() or retro_load_game_special().
  /// - On each subsequent invocation, returns 'true' if the
  ///   visibility of any core option has changed since the last
  ///   time the function was called.
  /// </summary>
  /// <returns></returns>
  [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
  [return: MarshalAs(UnmanagedType.U1)]
  public delegate bool retro_core_options_update_display_callback_t();

  public struct retro_core_options_update_display_callback
  {
    public retro_core_options_update_display_callback_t callback;
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
  public unsafe delegate void retro_log_printf_t(RETRO_LOG_LEVEL level, string fmt, IntPtr a0, IntPtr a1, IntPtr a2, IntPtr a3, IntPtr a4, IntPtr a5, IntPtr a6, IntPtr a7, IntPtr a8, IntPtr a9, IntPtr a10, IntPtr a11, IntPtr a12, IntPtr a13, IntPtr a14, IntPtr a15);
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
